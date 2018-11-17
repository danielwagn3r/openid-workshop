using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CalculatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoubleController : ControllerBase
    {
        // GET api/double/5
        [HttpGet("{id}")]
        [Authorize("calc:double")]
        public JsonResult Get(int id)
        {
            var result = new JObject
            {
                ["result"] = id * 2
            };

            return new JsonResult(result);
        }
    }
}
