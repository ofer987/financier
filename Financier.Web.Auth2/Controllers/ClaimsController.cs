using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

using Financier.Web.Auth2.Models;

namespace Financier.Web.Auth2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : Controller
    {
        public IEnumerable<ApplicationUser> Model { get; private set; }

        public ClaimsController(UserManager userManager)

        [HttpGet]
        public string Index()
        {
            // Init();
            //
            Console.WriteLine(HttpContext.User.Identity.Name);
            return $"Hello () and {HttpContext.User.Identity.Name}";
        }

        [HttpGet]
        public string Get(string innerRoute)
        {
            Console.WriteLine(HttpContext.User.Identity.Name);
            return $"Get ({innerRoute}) and {HttpContext.User.Identity.Name}";
        }

        [HttpPost]
        public void Create()
        {
            var user = 
        }
    }
}
