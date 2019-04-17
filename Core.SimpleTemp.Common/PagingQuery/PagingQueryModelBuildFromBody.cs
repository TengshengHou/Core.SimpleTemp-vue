using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Core.SimpleTemp.Common.FilterExpression;
using System.IO;

namespace Core.SimpleTemp.Common.PagingQuery
{
    public class PagingQueryModelBuildFromBody<TEntity> : IPagingQueryModelBuild<TEntity>
    {
        #region 常量定义
        const string FILTER_CONDITION_LIST_KEY = "filterConditionList";
        const string OFFSET = "offset";
        const string LIMIT = "limit";
        #endregion

        JsonSerializer jsonSerializer = new JsonSerializer();
        FilterPagingQueryModel<TEntity> pagingQueryModel = new FilterPagingQueryModel<TEntity>();

        HttpContext _httpContext;

        public PagingQueryModelBuildFromBody(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }

        /// <summary>
        /// 根据当前Http请求创建PagingQuery对象
        /// </summary>
        /// <returns></returns>
        public void MakePagingQueryModel(FilterPagingQueryModel<TEntity> filterPagingQueryModel)
        {
            pagingQueryModel = filterPagingQueryModel;
            pagingQueryModel.FilterExpression = ExpressionExtension.GetFilterExpression<TEntity>(pagingQueryModel.FilterConditionList);
        }

        public FilterPagingQueryModel<TEntity> GetPaginQueryModel()
        {
            return pagingQueryModel;
        }
    }
}