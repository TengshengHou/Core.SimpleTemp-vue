using Core.SimpleTemp.Application.RoleApp;
using Core.SimpleTemp.Application.UserApp;
using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Mvc.Api.Internal;
using Core.SimpleTemp.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using Core.SimpleTemp.Repositories.IRepositories.Internal.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using Core.SimpleTemp.Common.PagingQuery;
using System.Linq.Expressions;

namespace Core.SimpleTemp.Mvc.Api.Sys
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [ApiExplorerSettings(GroupName = "Sys")]
    [Route("api/[controller]")]
    [EnableCors("any")]
    [ApiController]

    public class UserController : CoreApiController
    {
        private readonly ISysUserAppService _service;
        private readonly ISysRoleAppService _roleService;
        private readonly ILogger<UserController> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="sysRoleAppService"></param>
        /// <param name="logger"></param>
        public UserController(ISysUserAppService service, ISysRoleAppService sysRoleAppService, ILogger<UserController> logger)
        {
            _service = service;
            _roleService = sysRoleAppService;
            _logger = logger;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagingQueryModel"></param>
        /// <returns></returns>
        [HttpPost("GetList")]

        public async Task<IActionResult> GetList(PagingModel<SysUserDto> pagingQueryModel)
        {
            IPageModel<SysUserDto> result;

            #region Linq动态拼接查询条件Demo
            Expression<Func<SysUser, bool>> where = ExpressionExtension.True<SysUser>();
            //LoginName Like
            if (!string.IsNullOrWhiteSpace(pagingQueryModel.Model.LoginName))
            {
                where = where.And(m => m.LoginName.Contains(pagingQueryModel.Model.LoginName));
            }
            //Sex
            if (pagingQueryModel.Model.Sex != null)
            {
                where = where.And(m => m.Sex == pagingQueryModel.Model.Sex);
            }

            #endregion

            result = await _service.LoadPageOffsetAsync(
             pagingQueryModel.Offset,
             pagingQueryModel.Limit,
             where,
             orderModel => orderModel.LastUpdate,
             u => new SysUser { Id = u.Id, LoginName = u.LoginName, Name = u.Name, LastUpdate = u.LastUpdate, Sex = u.Sex }//查询字段
             );

            return this.JsonSuccess(result);
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("Save")]
        //[PermissionFilter(UserPermission.UserController_Edit)]
        public async Task<IActionResult> SaveAsync(SysUserDto dto)
        {
            //添加
            if (Equals(dto.Id, Guid.Empty))
            {
                await _service.InsertAsync(dto);
                return this.JsonSuccess(string.Empty);
            }
            else
            {
                await _service.UpdateAsync(dto, true, new List<string>() { nameof(SysUser.Password) });
                return this.JsonSuccess(string.Empty);
            }
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("DeleteMuti")]
        //[PermissionFilter(UserPermission.UserController_DeleteMuti)]
        public async Task<IActionResult> DeleteMutiAsync([FromBody]Guid[] ids)
        {
            #region 验证是否是admin
            var retDleteVerifyAdmin = await DleteVerifyAdmin(ids);
            if (retDleteVerifyAdmin != null)
                return retDleteVerifyAdmin;
            #endregion
            await _service.DeleteBatchAsync(ids);
            return this.JsonSuccess();
        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns></returns>
        [HttpGet("Get")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.GetAsync(id);
            model.RolesId = model.UserRoles.Select(r => r.SysRoleId).ToList();

            return this.JsonSuccess(model);
        }

        /// <summary>
        /// 删除时验证用户是否是admin
        /// </summary>
        /// <param name="delIds">删除用户信息</param>
        /// <returns></returns>
        private async Task<IActionResult> DleteVerifyAdmin(Guid[] delIds)
        {
            Guid adminId = Guid.Empty;
            var admin = (await _service.GetAllListAsync(u => u.LoginName == "admin"));
            if (admin != null && admin.Any())
            {
                adminId = (Guid)admin.FirstOrDefault()?.Id;
                if (delIds.Contains(adminId))
                {
                    return this.JsonFaild("admin用户不能删除");
                }
            }
            return null;
        }

        /// <summary>
        /// 获取系统角色信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllRoleList")]
        public async Task<IActionResult> GetAllRoleListAsync()
        {
            var allRoleList = await _roleService.GetAllListAsync();
            var roleSelectList = allRoleList.Select(role => (new { label = role.Name, key = role.Id.ToString() }));
            return this.JsonSuccess(roleSelectList);
        }
    }
}