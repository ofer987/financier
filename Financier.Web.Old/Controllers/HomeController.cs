using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Web.Controllers
{
    public class HomeController : Controller
    {
        public IEnumerable<DateTime> Model { get; private set; } = Enumerable.Empty<DateTime>();

        public IActionResult Index()
        {
            Init();

            return View(Model);
        }

        private void Init()
        {
            Model = GetAllMonths();
        }

        private IEnumerable<DateTime> GetAllMonths()
        {
            return Financier.Common.Expenses.Models.Item
                .GetAllMonths();
        }
    }
}
