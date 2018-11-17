using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets.Internal;
using Newtonsoft.Json.Linq;

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
