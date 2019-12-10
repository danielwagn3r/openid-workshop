using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CalculatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoubleController : ControllerBase
    {
        private readonly ILogger _logger;

        public DoubleController(ILogger<DoubleController> logger)
        {
            _logger = logger;
        }

        // GET api/double/5
        [HttpGet("{number}")]
        [Authorize("calc:double")]
        public JsonResult Get(int number)
        {
            _logger.LogInformation("Get double of {number}", number);

            var result = new JObject
            {
                ["result"] = number * 2
            };

            return new JsonResult(result);
        }
    }
}
