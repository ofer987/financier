using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Financier.Web.Auth.Controllers
{
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

        // private void Init()
        // {
        //     Model = GetAllMonths();
        // }
    }
}
