using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Repositories.IRepositories;
using Core.SimpleTemp.Repositories.IRepositories.Internal.Data;
using Core.SimpleTemp.Repository.RepositoryEntityFrameworkCore.Internal.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.SimpleTemp.Repository.RepositoryEntityFrameworkCore.Internal
{
    public partial class BaseRepository<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey> where TDbContext : DbContext
    {

        public virtual async Task<IPageModel<TEntity>> LoadPageOffsetAsync(int offset, int limit, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, Expression<Func<TEntity, TEntity>> selector = null)
        {
            var result = QueryBase(selector);

            if (where != null)
                result = result.Where(where);
            if (order != null)
                result = result.OrderBy(order);
            else
                result = result.OrderBy(m => m.Id);
            int rowCount = await result.CountAsync();
            var pageData = await result.Skip(offset).Take(limit).ToListAsync();

            var PageModel = new PageModel<TEntity>()
            {
                RowCount = rowCount,
                PageData = pageData
            };
            return PageModel;
        }

        /// <summary>
        /// 该方法优化空间很大。以后有时间定要优化
        /// </summary>
        /// <param name="pTargetObjSrc"></param>
        /// <param name="pTargetObjDest"></param>
        /// <param name="noUpdateProperties"></param>
        protected void EntityToEntity(TEntity pTargetObjSrc, TEntity pTargetObjDest, List<string> noUpdateProperties = null)
        {
            foreach (var mItem in typeof(TEntity).GetProperties())
            {
                noUpdateProperties = noUpdateProperties ?? new List<string>();
                if (!noUpdateProperties.Contains(mItem.Name))
                    mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
            }
        }

        /// <summary>
        /// 根据主键构建判断表达式
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }


        #region 原生SQL
        private string pageSqlFormat = @"select * from 
                                    ( 
        	                            select *,ROW_NUMBER() OVER(ORDER BY {3}) AS NB 
        	                            from 
                                        ( 
        	                            {0}
                                       ) a 
                                    ) b 
                                    where NB > {1} and NB < {2}";

        public async Task<BsonArray> ExecuteQueryAsync(string sql, DbParameter[] paras)
        {
            var bsonArray = new BsonArray();
            var conn = _dbContext.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    string query = sql;
                    command.CommandText = query;
                    //添加查询参数
                    if (!Equals(paras, null))
                        command.Parameters.AddRange(paras.ToArray());
                    DbDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new BsonDocument();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {

                                row.Add(reader.GetName(i), reader.GetValue(i).ToString());
                            }

                            bsonArray.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }

            return bsonArray;
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string sql, DbParameter[] paras) where T : new()
        {
            List<T> dataList = new List<T>();
            var conn = _dbContext.Database.GetDbConnection();
            var modelType = typeof(T);
            var properties = modelType.GetProperties();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    string query = sql;
                    command.CommandText = query;
                    //添加查询参数
                    if (!Equals(paras, null))
                        command.Parameters.AddRange(paras.ToArray());
                    DbDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var model = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var propertyInfo = properties.FirstOrDefault(x => string.Equals(reader.GetName(i), x.Name));
                                if (!Equals(propertyInfo, null))
                                    propertyInfo.SetValue(model, reader.GetValue(i));
                            }
                            dataList.Add(model);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return dataList;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, DbParameter[] paras)
        {
            //响应行数
            int affectedRow = 0;
            var conn = _dbContext.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = sql;
                    //添加查询参数
                    if (!Equals(paras, null))
                        command.Parameters.AddRange(paras.ToArray());
                    affectedRow = await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                conn.Close();
            }

            return affectedRow;
        }


        public async Task<BsonArray> PageQueryAsync(string querySql, int offset, int limit, string order, DbParameter[] paras)
        {

            if (!string.IsNullOrEmpty(order))
            {
                order = "id";
            }
            var sql = string.Format(pageSqlFormat, querySql, offset, offset + limit, order);
            return await ExecuteQueryAsync(sql, paras);
        }

        public async Task<List<T>> PageQueryAsync<T>(string querySql, int offset, int limit, string order, DbParameter[] paras)
        {

            if (!string.IsNullOrEmpty(order))
            {
                order = "id";
            }
            var sql = string.Format(pageSqlFormat, querySql, offset, offset + limit, order);
            return await ExecuteQueryAsync<T>(sql, paras);
        }
        #endregion

    }
}
