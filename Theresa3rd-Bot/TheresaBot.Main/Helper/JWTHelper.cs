using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Jwt;

namespace TheresaBot.Main.Helper
{
    public static class JWTHelper
    {
        public static void ConfigureJWT(this IServiceCollection services)
        {
            var authenticationBuilder = services.AddAuthentication(option =>
            {
                //认证middleware配置
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            authenticationBuilder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = JwtConfig.Issuer,//发行商
                    ValidAudience = JwtConfig.Audience,//受众者
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey)),
                    ValidateLifetime = true,//验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                };
            });
        }

        public static JwtToken GenerateToken(Claim[] claims)
        {
            var issuer = JwtConfig.Issuer;
            var audience = JwtConfig.Audience;
            var createTime = DateTime.UtcNow;
            var expiredTime = createTime.AddSeconds(JwtConfig.ExpiredSeconds);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer, audience, claims, createTime, expiredTime, credentials);
            return new JwtToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Header = JwtBearerDefaults.AuthenticationScheme,
                CreateTime = createTime,
                ExpiredTime = expiredTime,
                ExpiredSeconds = JwtConfig.ExpiredSeconds
            };
        }



    }
}
