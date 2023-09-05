using Microsoft.AspNetCore.Mvc.RazorPages;
using WebLab.Domain.Entities;
using WebLab.Services.BeerService;

namespace WebLab.Areas.Admin.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IBeerService _beerService;
		public IndexModel(IBeerService beerService)
		{
			_beerService = beerService;
		}

		public IList<Beer> Beer { get; set; } = default!;

		public async Task OnGetAsync()
		{
			var beers = await _beerService.GetBeerListAsync(pageSize: 1000);
			Beer = beers.Data.Items;
		}
	}
}
