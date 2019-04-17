using Core.SimpleTemp.Application;
using Core.SimpleTemp.Common;
using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Mvc.Api.Internal;
using Core.SimpleTemp.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;


namespace Core.SimpleTemp.Mvc.Api.Sys
{
    /// <summary>
    /// 用户登录
    /// </summary>
    [Route("api/[controller]")]
    [EnableCors("any")]
    [ApiExplorerSettings(GroupName = "Sys")]
    [ApiController]
    public class AccountController : AccountBaseController
    {
        private IHostingEnvironment _env;
        readonly WebAppOptions _webAppOptions;

        /// <summary>
        /// AccountController
        /// </summary>
        /// <param name="sysLoginService"></param>
        /// <param name="env"></param>
        /// <param name="webAppOptions"></param>
        /// <param name="accessor"></param>
        public AccountController(ISysLoginService sysLoginService, IHostingEnvironment env, IOptionsMonitor<WebAppOptions> webAppOptions, IHttpContextAccessor accessor) : base(sysLoginService)
        {
            _env = env;
            _webAppOptions = webAppOptions.CurrentValue;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model">用户登录Model</param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            bool ret = await _sysLoginService.LoginAsync(HttpContext, new SysUser()
            {
                LoginName = model.LoginName,
                Password = model.Password
            });
            if (ret)
            {
                return this.JsonSuccess();
            }
            return this.JsonFaild("用户名密码失败");
        }

        /// <summary>
        /// 根据当前登录用户获取菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenu")]
        [Authorize]

        public async Task<IActionResult> GetMenuAsync()
        {
            return this.JsonSuccess(await _sysLoginService.GetMenusByCurrentUserAsync());
        }
    }
}
