using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CalculatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokeninfoController : ControllerBase
    {
        // GET api/tokeninfo
        [HttpGet]
        [Authorize]
        public JsonResult Get()
        {
            var bearerToken = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var token = new JwtSecurityToken(jwtEncodedString: bearerToken);

            return new JsonResult(token);
        }
    }
}