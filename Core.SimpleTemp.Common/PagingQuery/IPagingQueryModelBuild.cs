using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.SimpleTemp.Common.PagingQuery
{
    public interface IPagingQueryModelBuild<TEntity>
    {
        void MakePagingQueryModel(FilterPagingQueryModel<TEntity> filterPagingQueryModel);


        FilterPagingQueryModel<TEntity> GetPaginQueryModel();
    }
}
