using Microsoft.AspNetCore.Mvc;
using WebLab.Domain.Entities;
using WebLab.Extensions;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebLab.Controllers
{
	public class BeerController : Controller
	{
		private readonly IBeerService _beerService;
		private readonly IBeerTypeService _beerTypeService;

		public BeerController(IBeerService beerService, IBeerTypeService beerTypeService)
		{
			_beerService = beerService;
			_beerTypeService = beerTypeService;
		}

		public async Task<IActionResult> Index(string? beerType, int pageNo)
		{
			var productResponse = await _beerService.GetBeerListAsync(beerType, pageNo);
			var beerTypes = await _beerTypeService.GetBeerTypeListAsync();

			Console.WriteLine(productResponse.ToString());

			if (!productResponse.IsSuccess)
				return NotFound(productResponse.ErrorMessage);

			ViewData["beerTypes"] = beerTypes.Data;
			ViewData["beerType"] = beerType;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_BeerListPartial", productResponse.Data);
            }
            return View(productResponse.Data);
		}
	}
}
