﻿
using AutoMapper;
using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Repositories.IRepositories;
using Core.SimpleTemp.Repositories.IRepositories.Internal.Data;
using Core.SimpleTemp.Repository.RepositoryEntityFrameworkCore.Internal.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.SimpleTemp.Application
{
    public partial class BaseAppService<TDto, TEntity, TRepository> : IBaseAppService<TDto, TEntity> where TEntity : Entity where TRepository : IRepository<TEntity>
    {
        public TRepository _repository;

        public BaseAppService(TRepository repository) => _repository = repository;

        public Task DeleteAsync(TEntity entity, bool autoSave = true) => _repository.DeleteAsync(entity, autoSave);

        public Task DeleteAsync(Guid id, bool autoSave = true) => _repository.DeleteAsync(id, autoSave);

        public Task DeleteAsync(Expression<Func<TEntity, bool>> where, bool autoSave = true) => _repository.DeleteAsync(where, autoSave);

        public Task DeleteBatchAsync(Guid[] ids, bool autoSave = true)
        {
            return this.DeleteAsync(it => ids.Contains(it.Id), autoSave);
        }

        public async Task<TDto> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) => Mapper.Map<TDto>(await _repository.FirstOrDefaultAsync(predicate));
        public async Task<TEntity> FirstOrDefaultEntityAsync(Expression<Func<TEntity, bool>> predicate) => await _repository.FirstOrDefaultAsync(predicate);

        public async Task<List<TDto>> GetAllListAsync(Expression<Func<TEntity, TEntity>> selector = null) => Mapper.Map<List<TDto>>(await _repository.GetAllListAsync(selector));

        public async Task<List<TDto>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> selector = null) => Mapper.Map<List<TDto>>(await _repository.GetAllListAsync(predicate, selector));

        public async Task<TDto> GetAsync(Guid id) => Mapper.Map<TDto>(await _repository.GetAsync(id));
        public async Task<IPageModel<TDto>> GetAllPageListAsync(int startPage, int pageSize, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null)
        {
            if (startPage == 0)
                startPage = 1;
            var pageModelDto = await LoadPageOffsetAsync((startPage - 1) * pageSize, pageSize, where, order);
            return pageModelDto;
        }

        public async Task<IPageModel<TDto>> LoadPageOffsetAsync(int offset, int limit, Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, Expression<Func<TEntity, TEntity>> selector = null)
        {
            var pageModelEntity = await _repository.LoadPageOffsetAsync(offset, limit, where, order,selector);
            IPageModel<TDto> pageModelDto = new PageModel<TDto>()
            {
                PageData = Mapper.Map<List<TDto>>(pageModelEntity.PageData),
                RowCount = pageModelEntity.RowCount
            };
            return pageModelDto;

        }

        public async Task<TDto> InsertAsync(TDto dto, bool autoSave = true)
        {
            var entity = await _repository.InsertAsync(Mapper.Map<TEntity>(dto), autoSave);
            return Mapper.Map<TDto>(entity);
        }


        public async Task<TDto> UpdateAsync(TDto dto, bool autoSave = true, List<string> noUpdateProperties = null)
        {
            var entity = await _repository.UpdateAsync(Mapper.Map<TEntity>(dto), autoSave, noUpdateProperties);
            return Mapper.Map<TDto>(entity);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true)
        {
            return await _repository.UpdateAsync(entity, autoSave);
        }

        #region 原生SQL
        public async Task<BsonArray> ExecuteQueryAsync(string sql, IEnumerable<DbParameter> paras = null)
        {
            return await this.ExecuteQueryAsync(sql, paras.ToArray());
        }
        public async Task<BsonArray> ExecuteQueryAsync(string sql, params DbParameter[] paras)
        {
            return await _repository.ExecuteQueryAsync(sql, paras);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParameter> paras = null)
        {
            return await this.ExecuteNonQueryAsync(sql, paras.ToArray());
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] paras)
        {
            return await _repository.ExecuteNonQueryAsync(sql, paras);
        }

        public async Task<BsonArray> PageQueryAsync(string querySql, int offset, int limit, string order, IEnumerable<DbParameter> paras)
        {
            return await _repository.PageQueryAsync(querySql, offset, limit, order, paras.ToArray());
        }
        #endregion

    }
}
