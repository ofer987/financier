using Microsoft.AspNetCore.Mvc;

namespace Financier.Web.Controllers
{
    public class ItemsController : Controller
    {
        public IActionResult Chart()
        {
            return View();
        }
    }
}
