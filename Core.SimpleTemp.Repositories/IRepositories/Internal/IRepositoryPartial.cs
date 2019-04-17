using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Repositories.IRepositories.Internal.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.SimpleTemp.Repositories.IRepositories
{
    public partial interface IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {

        Task<IPageModel<TEntity>> LoadPageOffsetAsync(int offset, int limit, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, Expression<Func<TEntity, TEntity>> selector = null);
        #region 原生sql
        Task<BsonArray> ExecuteQueryAsync(string sql, DbParameter[] paras);
        Task<int> ExecuteNonQueryAsync(string sql, DbParameter[] paras);

        Task<BsonArray> PageQueryAsync(string querySql, int offset, int limit, string order , DbParameter[] paras);
        #endregion
    }
}
