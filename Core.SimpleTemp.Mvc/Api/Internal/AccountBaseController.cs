using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.SimpleTemp.Application;
using Core.SimpleTemp.Entitys;
using Core.SimpleTemp.Mvc.Api;
using Core.SimpleTemp.Mvc.Controllers.Internal;
using Core.SimpleTemp.Mvc.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.SimpleTemp.Mvc.Api.Internal
{
   
    public class AccountBaseController : CoreApiController
    {
        public ISysLoginService _sysLoginService;

        public AccountBaseController(ISysLoginService sysLoginService)
        {
            //var a = base.HttpContext;
            _sysLoginService = sysLoginService;
        }

        #region Token
        [HttpGet("GetToken")]
        public async Task<IActionResult> GetToken(string userName, string pwd)
        {
            var tokenStr = await _sysLoginService.JwtAuthenticate(userName, pwd);
            return this.JsonSuccess(tokenStr);
        }
        #endregion
    }
}