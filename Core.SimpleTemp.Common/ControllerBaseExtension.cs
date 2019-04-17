using Core.SimpleTemp.Common.ActionResultHelp;
using Core.SimpleTemp.Common.PagingQuery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Core.SimpleTemp.Common
{
    public static class ControllerBaseExtension
    {
        #region JsonResult

        /// <summary>
        /// 返回带有成功标识的JsonModel
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonResult JsonSuccess(this ControllerBase controllerBase, object data = null)
        {

            return JsonResultHelp.JsonSuccess(data);
        }

        /// <summary>
        /// 返回带有失败标识的JsonModel
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonResult JsonFaild(this ControllerBase controllerBase, string message = null)
        {
            return JsonResultHelp.JsonFaild(message);
        }
        #endregion

        [HttpPost(nameof(Upload))]
        public static void Upload(this ControllerBase controllerBase)
        {

        }


        public static FilterPagingQueryModel<T> GetPagingQueryModel<T>(this ControllerBase controllerBase, FilterPagingQueryModel<T> filterPagingQueryModel)
        {
            IPagingQueryModelBuild<T> pagingQueryModelBuild = null;
            pagingQueryModelBuild = controllerBase.ControllerContext.HttpContext.RequestServices.GetService<IPagingQueryModelBuild<T>> ();
            PagingQueryModelDesigner<T> pagingQueryModelDesigner = new PagingQueryModelDesigner<T>();
             pagingQueryModelDesigner.Build(pagingQueryModelBuild, filterPagingQueryModel);
            return pagingQueryModelBuild.GetPaginQueryModel();
        }
    }
}
