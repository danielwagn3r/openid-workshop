using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalculatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SquareController : ControllerBase
    {
        // GET api/square/5
        [HttpGet("{id}")]
        [Authorize("calc:square")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
}
