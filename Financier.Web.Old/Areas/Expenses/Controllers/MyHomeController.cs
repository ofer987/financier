using Microsoft.AspNetCore.Mvc;

namespace Financier.Web.Areas.Expenses.Controllers
{
    [Area("Expenses")]
    public class MyHomeController : Controller
    {
        public IActionResult GetMonthlyPayments()
        {
            return View();
        }
    }
}
