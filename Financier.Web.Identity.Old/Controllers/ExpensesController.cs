using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Financier.Web.Identity.Models;

namespace Financier.Web.Identity.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly ILogger<ExpensesController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpensesController(
            ILogger<ExpensesController> logger,
            UserManager<ApplicationUser> userManager,
            IHttpClientFactory clientFactory
        )
        {
            _logger = logger;
            _userManager = userManager;
            _clientFactory = clientFactory;
        }

        public string Index(string id)
        {
            // Init();
            //
            Console.WriteLine(HttpContext.User.Identity.Name);

            return $"Identity:\n\t- Expenses/Index: ({id})\n\t- email: ({HttpContext.User.Identity.Name})";
        }

        [HttpGet("{*internalRoute}")]
        public string Get(string internalRoute)
        {
            // return $"hello {internalRoute}";

            var i = 0;
            Console.WriteLine($"User has ({User.Claims.Count()}) claims");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine("Trying claim");
                var users = _userManager.GetUsersForClaimAsync(claim).GetAwaiter().GetResult();

                foreach (var user in users)
                {
                    Console.WriteLine($"\t{user}");
                }

                i += 1;
            }

            Console.WriteLine($"Identity: {HttpContext.User.Identity.Name}");
            return $"Identity:\n\t- Expenses/Get: ({internalRoute})\n\t- email: ({HttpContext.User.Identity.Name})";

            // var client = _clientFactory.CreateClient();
            // internalRoute = "AllItems/Get/year/2019";
            // var myUser = this._userManager.GetUsersForClaimAsync(this.User.Claims.First()).GetAwaiter().GetResult();
            // var myUser2 = this._userManager.GetUserName(this.User);
            // var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5002/{internalRoute}");
            // var user = _userManager.GetUserAsync(this.User).GetAwaiter().GetResult();
            // var userEmail = "dan@ofer.to";
            // request.Headers.Add("Account-Name", userEmail);
            //
            // var response = client.SendAsync(request).GetAwaiter().GetResult();
            // if (response.IsSuccessStatusCode)
            // {
            //     return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            // }
            // else
            // {
            //     return "failed to retrieve response";
            // }
        }
    }
}
