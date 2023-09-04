using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.Services.BeerService
{
	public interface IBeerService
	{
		public Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized = null, int pageNo = 1);
		public Task<ResponseData<Beer>> GetBeerByIdAsync(int id);
		public Task UpdateBeerAsync(int id, Beer beer, IFormFile? formFile);
		public Task DeleteBeerAsync(int id);
		public Task<ResponseData<Beer>> CreateBeerAsync(Beer beer, IFormFile? formFile);

	}
}
