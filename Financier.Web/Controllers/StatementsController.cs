using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Text.Encodings.Web;

using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Web.ViewModels;

namespace Financier.Web.Controllers
{
    public class StatementsController : Controller
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

            var viewModel = new Statement(year, month);

            return View(viewModel);
            // return $"yo man {year} and {month} and {nameof(id)} = {id} and {nameof(myIds)} = {myIds}";
        }

        private IEnumerable<Statement> GetStatements(int year, int startMonth, int endMonth)
        {
            for (var i = startMonth; i <= endMonth; i += 1) 
            {
                yield return new Statement(year, i);
            }
        }
    }
}
