using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Financier.Web.Identity.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(ILogger<ExpensesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{internalRoute}")]
        public string Get(string internalRoute)
        {
            return $"Internal route is ({internalRoute})";
        }
    }
}
