using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebLab.Domain.Entities;
using WebLab.Services.BeerService;

namespace WebLab.Areas.Admin.Pages
{
	public class DetailsModel : PageModel
	{
		private readonly IBeerService _beerService;

		public DetailsModel(IBeerService beerService)
		{
			_beerService = beerService;
		}

		[BindProperty]
		public Beer Beer { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var beer = await _beerService.GetBeerByIdAsync((int)id);
			if (beer.Data == null)
			{
				return NotFound();
			}
			else
			{
				Beer = beer.Data;
			}
			return Page();
		}
	}
}
