using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Web.ViewModels;

namespace Financier.Web.Controllers
{
    public class MonthlyExpensesController : Controller
    {
        public IActionResult Index(int year, int month)
        {
            var count = HttpContext.Request.Query["id"].Count.ToString();
            var ids = HttpContext.Request.Query["id"];
            var id = HttpContext.Request.Query["id"].ToString();

            var myIds = ids.Join(", ");

            var statement = new MonthlyStatement(year, month);
            var viewModel = new MonthlyExpenses(year, month, statement);

            return View(viewModel);
            // return $"yo man {year} and {month} and {nameof(id)} = {id} and {nameof(myIds)} = {myIds}";
        }
    }
}
