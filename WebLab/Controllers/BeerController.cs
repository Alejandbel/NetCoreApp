using Microsoft.AspNetCore.Mvc;
using WebLab.Extensions;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;

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

        [Route("beer/{beerType?}")]
        public async Task<IActionResult> Index(string? beerType, int pageNo)
		{
            var beerTypes = await _beerTypeService.GetBeerTypeListAsync();

            if (!beerTypes.IsSuccess)
                return NotFound(beerTypes.ErrorMessage);

            var productResponse = await _beerService.GetBeerListAsync(beerType, pageNo);
            
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
