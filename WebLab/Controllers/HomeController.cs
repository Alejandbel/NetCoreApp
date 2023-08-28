using Microsoft.AspNetCore.Mvc;

namespace WebLab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
	}
}
