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
    public class SquareController : ControllerBase
    {
        private readonly ILogger _logger;

        public SquareController(ILogger<SquareController> logger)
        {
            _logger = logger;
        }

        // GET api/square/5
        [HttpGet("{number}")]
        [Authorize("calc:square")]
        public ActionResult<string> Get(int number)
        {
            _logger.LogInformation("Get double of {number}", number);

            var result = new JObject
            {
                ["result"] = number * number
            };

            return new JsonResult(result);
        }
    }
}
