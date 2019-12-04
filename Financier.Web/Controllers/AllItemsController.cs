using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Financier.Web.Controllers
{
    public class AllItemsController : Controller
    {
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }

        public IActionResult GetYear(int year)
        {
            Init(year, 1, 12);
            var items = GetItems().ToArray();

            return View("Get", items);
        }

        public IActionResult GetMonth(int year, int month)
        {
            Init(year, month, month);
            var items = GetItems().ToArray();

            return View("Get", items);
        }

        private void Init(int year, int startMonth, int endMonth)
        {
            From = new DateTime(year, startMonth, 1);
            To = new DateTime(year, startMonth, 1).AddMonths(1);
        }

        private IEnumerable<ViewModels.Item> GetItems()
        {
            return Financier.Common.Expenses.Models.Item
                .GetAllBy(From, To)
                .Select(model => new ViewModels.Item(model))
                .OrderBy(model => model.At);
        }
    }
}
