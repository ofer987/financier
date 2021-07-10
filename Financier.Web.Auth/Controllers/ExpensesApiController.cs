using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;

namespace Financier.Web.Auth.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExpensesApiController : Controller
    {
        public IEnumerable<DateTime> Model { get; private set; }

        public string Index()
        {
            // Init();
            //
            Console.WriteLine(HttpContext.User.Identity.Name);
            return $"Hello (innerRoute) and {HttpContext.User.Identity.Name}";
        }

        // private void Init()
        // {
        //     Model = GetAllMonths();
        // }
    }
}
