using Microsoft.AspNetCore.Mvc;

namespace tasklist.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
