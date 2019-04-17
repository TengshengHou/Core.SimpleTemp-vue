using Core.SimpleTemp.Repositories.IRepositories.Internal.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.SimpleTemp.Application
{
    public partial interface IBaseAppService<TDto, TEntity, TPrimaryKey>
    {
        Task<List<TDto>> GetAllListAsync(Expression<Func<TEntity, TEntity>> selector = null);

        Task<List<TDto>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> selector = null);
        Task<TDto> GetAsync(TPrimaryKey id);
        Task<TDto> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultEntityAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TDto> InsertAsync(TDto dto, bool autoSave = true);
        Task<TDto> UpdateAsync(TDto dto, bool autoSave = true, List<string> noUpdateProperties = null);
        Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true);


        Task DeleteAsync(TEntity entity, bool autoSave = true);
        Task DeleteAsync(TPrimaryKey id, bool autoSave = true);
        Task DeleteAsync(Expression<Func<TEntity, bool>> where, bool autoSave = true);
        Task DeleteBatchAsync(Guid[] ids, bool autoSave = true);

        Task<IPageModel<TDto>> GetAllPageListAsync(int startPage, int pageSize, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null);
        Task<IPageModel<TDto>> LoadPageOffsetAsync(int offset, int limit, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, Expression<Func<TEntity, TEntity>> selector = null);


        Task<BsonArray> ExecuteQueryAsync(string sql, IEnumerable<DbParameter> paras);
        Task<BsonArray> ExecuteQueryAsync(string sql, params DbParameter[] paras);

        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParameter> paras);

        Task<int> ExecuteNonQueryAsync(string sql, params  DbParameter[] paras);
        Task<BsonArray> PageQueryAsync(string querySql, int offset, int limit, string order, IEnumerable<DbParameter> paras);
    }

    public interface IBaseAppService<TDto, TEntity> : IBaseAppService<TDto, TEntity, Guid>
    {
    }
}
