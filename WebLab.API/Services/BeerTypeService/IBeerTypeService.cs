using WebLab.Domain.Models;
using WebLab.Domain.Entities;

namespace WebLab.API.Services.BeerTypeService
{
	public interface IBeerTypeService
	{
		public Task<ResponseData<List<BeerType>>> GetBeerTypeListAsync();
	}
}
