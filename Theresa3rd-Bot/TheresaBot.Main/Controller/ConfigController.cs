using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheresaBot.Main.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : BaseController
    {
        [HttpGet]
        [Authorize]
        [Route("get")]
        public string GetConfig()
        {
            return "ok";
        }

    }
}
