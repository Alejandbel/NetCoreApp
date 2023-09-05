using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebLab.Domain.Entities;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;

namespace WebLab.Areas.Admin.Pages
{
	public class CreateModel : PageModel
	{
		private readonly IBeerService _beerService;
		private readonly IBeerTypeService _beerTypeService;

		public CreateModel(IBeerService beerService, IBeerTypeService beerTypeService)
		{
			_beerService = beerService;
			_beerTypeService = beerTypeService;
		}

		public async Task<IActionResult> OnGet()
		{
			var beerTypes = await _beerTypeService.GetBeerTypeListAsync();
			ViewData["TypeId"] = new SelectList(beerTypes.Data, "Id", "Name");
			return Page();
		}

		[BindProperty]
		public Beer Beer { get; set; } = default!;

		[BindProperty]
		public IFormFile? Image { get; set; }


		// To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid || Beer == null)
			{
				return Page();
			}

			await _beerService.CreateBeerAsync(Beer, Image);

			return RedirectToPage("./Index");
		}
	}
}
