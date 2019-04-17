using Core.SimpleTemp.Application;
using Core.SimpleTemp.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
#pragma warning disable CS1591
namespace Core.SimpleTemp.Mvc.Api.Internal
{
    public class AccountBaseController : CoreApiController
    {
        public ISysLoginService _sysLoginService;
        
        public AccountBaseController(ISysLoginService sysLoginService)
        {
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