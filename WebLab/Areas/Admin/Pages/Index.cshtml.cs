using Microsoft.AspNetCore.Mvc.RazorPages;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;
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

		public ListModel<Beer> Beer { get; set; } = default!;

		public async Task OnGetAsync(int pageNo = 1)
		{
			var beers = await _beerService.GetBeerListAsync(pageNo: pageNo);
			Beer = beers.Data;
		}
	}
}
