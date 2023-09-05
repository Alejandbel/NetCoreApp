using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;

namespace WebLab.Areas.Admin.Pages
{
	public class DeleteModel : PageModel
	{
		private readonly IBeerService _beerService;

		public DeleteModel(IBeerService beerService)
		{
			_beerService = beerService;
		}

		[BindProperty]
		public Domain.Entities.Beer Beer { get; set; } = default!;

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

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var beer = await _beerService.GetBeerByIdAsync((int)id);

			if (beer.Data != null)
			{
				Beer = beer.Data;
				await _beerService.DeleteBeerAsync(beer.Data.Id);
			}

			return RedirectToPage("./Index");
		}
	}
}
