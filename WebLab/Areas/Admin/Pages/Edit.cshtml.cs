using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebLab.Domain.Entities;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;

namespace WebLab.Areas.Admin.Pages
{
	public class EditModel : PageModel
	{
		private readonly IBeerService _beerService;
		private readonly IBeerTypeService _beerTypeService;

		public EditModel(IBeerService beerService, IBeerTypeService beerTypeService)
		{
			_beerService = beerService;
			_beerTypeService = beerTypeService;
		}

		[BindProperty]
		public Beer Beer { get; set; } = default!;

		[BindProperty]
		public IFormFile? Image { get; set; }

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

			Beer = beer.Data;
			var beerTypes = await _beerTypeService.GetBeerTypeListAsync();
			ViewData["TypeId"] = new SelectList(beerTypes.Data, "Id", "Name");
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			await _beerService.UpdateBeerAsync(Beer.Id, Beer, Image);

			return RedirectToPage("./Index");
		}
	}
}
