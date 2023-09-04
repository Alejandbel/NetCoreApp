using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.API.Services.BeerService
{
	public interface IBeerService
	{
		public Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized = null, int pageNo = 1, int pageSize = 3);
		public Task<ResponseData<Beer>> GetBeerByIdAsync(int id);
		public Task UpdateBeerAsync(int id, Beer beer);
		public Task DeleteBeerAsync(int id);
		public Task<ResponseData<Beer>> CreateBeerAsync(Beer beer);
		public Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile);

	}
}
