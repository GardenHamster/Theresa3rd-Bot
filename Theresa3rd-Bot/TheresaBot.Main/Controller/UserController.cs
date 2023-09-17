using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Jwt;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        [HttpPost]
        [Route("login")]
        public JwtTokenResult Login(string pwd)
        {
            var claims = new[] { new Claim("userid", "1") };
            var token = JWTHelper.GenerateToken(claims);
            return new JwtTokenResult
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
                CreateAt = token.CreateTime.ToTimeStamp(),
                ExpiredAt = token.ExpiredTime.ToTimeStamp(),
                ExpiredSeconds = token.ExpiredSeconds
            };
        }

    }
}
