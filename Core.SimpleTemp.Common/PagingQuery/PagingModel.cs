using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.SimpleTemp.Common.PagingQuery
{
    public class PagingModelBase<TEntity>
    {

        public int Offset { get; set; }
        [Range(1, SysConsts.PAGING_MAXI_LIMIT, ErrorMessage = "非法的Limit")]
        public int Limit { get; set; }
    }

    public class PagingModel<TEntity>:PagingModelBase<TEntity>
    {
        public TEntity Model { get; set; }
    }
}
