using CalculatorApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

            return new JsonResult(new ResultModel { Result = number * number });
        }
    }
}