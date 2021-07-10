using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace Financier.Web.Auth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : Controller
    {
        public IEnumerable<DateTime> Model { get; private set; }

        public string Index(string innerRoute)
        {
            // Init();
            //
            Console.WriteLine(HttpContext.User.Identity.Name);
            return $"Hello ({innerRoute}) and {HttpContext.User.Identity.Name}";
        }

        [HttpGet("{*innerRoute}")]
        public string Get(string innerRoute)
        {
            Console.WriteLine(HttpContext.User.Identity.Name);
            return $"Get ({innerRoute}) and {HttpContext.User.Identity.Name}";
        }
    }
}
