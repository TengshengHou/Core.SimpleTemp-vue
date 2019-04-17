using Core.SimpleTemp.Common.FilterExpression;
using Core.SimpleTemp.Common.PagingQuery;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.SimpleTemp.Common
{
    /// <summary>
    /// 分页条件查询Model
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class FilterPagingQueryModel<TEntity>:PagingModelBase<TEntity>
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public List<FilterModel> FilterConditionList { get; set; }
        /// <summary>
        /// 编译生成的过滤Expression表达式
        /// </summary>
        public Expression<Func<TEntity, bool>> FilterExpression { get; set; }

    }
}
