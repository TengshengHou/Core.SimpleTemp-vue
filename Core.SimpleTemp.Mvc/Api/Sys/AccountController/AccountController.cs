using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.SimpleTemp.Application;
using Core.SimpleTemp.Common;
using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Mvc.Api.Internal;
using Core.SimpleTemp.Mvc.Controllers.Internal;
using Core.SimpleTemp.Mvc.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Core.SimpleTemp.Mvc.Api.Sys.AccountController
{
    [Route("api/[controller]")]
    [EnableCors("any")]
    [ApiController]
    public class AccountController: AccountBaseController
    {
        private IHostingEnvironment _env;
        readonly WebAppOptions _webAppOptions;


        public AccountController(ISysLoginService sysLoginService, IHostingEnvironment env, IOptionsMonitor<WebAppOptions> webAppOptions, IHttpContextAccessor accessor) :base(sysLoginService)
        {
            _env = env;
            _webAppOptions = webAppOptions.CurrentValue;
        }

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
    }
}
