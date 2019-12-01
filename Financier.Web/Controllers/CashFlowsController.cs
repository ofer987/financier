using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Expenses;

namespace Financier.Web.Controllers
{
    public class CashFlowsController : Controller
    {
        public IActionResult GetYear(int year)
        {
            var viewModel = GetStatements(year, 1, 12).ToArray();
            return View(viewModel);
        }

        public IActionResult GetMonth(int year, int month)
        {
            // var count = HttpContext.Request.Query["id"].Count.ToString();
            // var ids = HttpContext.Request.Query["id"];
            // var id = HttpContext.Request.Query["id"].ToString();
            //
            // var myIds = ids.Join(", ");

            var viewModel = new MonthlyCashFlow(year, month);

            return View(viewModel);
            // return $"yo man {year} and {month} and {nameof(id)} = {id} and {nameof(myIds)} = {myIds}";
        }

        private IEnumerable<MonthlyCashFlow> GetStatements(int year, int startMonth, int endMonth)
        {
            for (var i = startMonth; i <= endMonth; i += 1) 
            {
                yield return new MonthlyCashFlow(year, i);
            }
        }
    }
}
