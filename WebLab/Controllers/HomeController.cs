using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebLab.Models;

namespace WebLab.Controllers
{
	public class HomeController : Controller
	{
		public IEnumerable<ListDemo> Lists { get; set; } = new List<ListDemo>
		{
			new ListDemo(1, "Item 1"),
			new ListDemo(2, "Item 2"),
			new ListDemo(3, "Item 3"),
		};

		public IActionResult Index()
		{
			ViewData["LabName"] = "Лабораторная работа №2";
			ViewBag.Lists = new SelectList(Lists, "Id", "Name");

			return View();
		}
	}
}
