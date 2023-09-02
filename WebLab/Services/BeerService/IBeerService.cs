using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.Services.BeerService
{
	public interface IBeerService
	{
		public Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized = null, int pageNo = 1);
		public Task<ResponseData<Beer>> GetProductByIdAsync(int id);
		public Task UpdateProductAsync(int id, Beer beer, IFormFile? formFile);
		public Task DeleteProductAsync(int id);
		public Task<ResponseData<Beer>> CreateProductAsync(Beer beer, IFormFile? formFile);

	}
}
