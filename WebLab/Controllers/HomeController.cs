using Microsoft.AspNetCore.Mvc;

namespace WebLab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Lab2()
		{
			return View();
		}
	}
}
