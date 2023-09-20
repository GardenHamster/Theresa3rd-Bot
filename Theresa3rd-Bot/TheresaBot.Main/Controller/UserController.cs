using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Jwt;
using TheresaBot.Main.Model.Result;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        [HttpPost]
        [Route("login")]
        public ApiResult Login([FromBody] string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ApiResult.Fail("密码错误");
            }
            string configPwd = BotConfig.BackstageConfig.Password;
            string md5Pwd = StringHelper.ToMD5(configPwd);
            if (md5Pwd.ToUpper() != password.ToUpper())
            {
                return ApiResult.Fail("密码错误");
            }
            var claims = new[] { new Claim("userid", "1") };
            var token = JWTHelper.GenerateToken(claims);
            var data = new JwtTokenVo
            {
                Token = token.AccessToken,
                Header = token.TokenType,
                CreateAt = token.CreateTime.ToTimeStamp(),
                ExpiredAt = token.ExpiredTime.ToTimeStamp(),
                ExpiredSeconds = token.ExpiredSeconds
            };
            return ApiResult.Success(data);
        }

    }
}
