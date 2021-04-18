using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Web.Controllers
{
    public class AllItemsController : Controller
    {
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public Account RequestedUser { get; private set; }
        public string RequestedUserName => HttpContext.Request.Headers["Account-Name"].ToString();

        public IActionResult GetYear(int year)
        {
            Init(year);
            var items = GetItems().ToArray();

            return View("Get", items);
        }

        public IActionResult GetMonth(int year, int month)
        {
            Init(year, month);
            var items = GetItems().ToArray();

            return View("Get", items);
        }

        private void Init(int year)
        {
            From = new DateTime(year, 1, 1);
            To = new DateTime(year, 1, 1).AddYears(1);
            RequestedUser = Account.FindByName(RequestedUserName);
        }

        private void Init(int year, int month)
        {
            From = new DateTime(year, month, 1);
            To = new DateTime(year, month, 1).AddMonths(1);
            RequestedUser = Account.FindByName(RequestedUserName);
        }

        private IEnumerable<ViewModels.Item> GetItems()
        {
            return RequestedUser.GetAllItems(From, To)
                .Select(model => new ViewModels.Item(model))
                .OrderBy(model => model.Model.PostedAt);
        }
    }
}
