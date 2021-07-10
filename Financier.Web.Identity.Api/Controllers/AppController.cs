using Microsoft.AspNetCore.Mvc;

namespace Financier.Web.Identity
{
    [Route("[controller]")]
    public class AppController : Controller
    {
        public AppController()
        {
        }

        [HttpGet("{*route}")]
        public IActionResult Index(string route)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index2()
        {
            return View();
        }
    }
}
