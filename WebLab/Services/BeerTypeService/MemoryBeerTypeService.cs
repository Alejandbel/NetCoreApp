using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.Services.BeerTypeService
{
	public class MemoryBeerTypeService : IBeerTypeService
	{
		public Task<ResponseData<List<BeerType>>> GetBeerTypeListAsync()
		{
			var beerTypes = new List<BeerType>()
			{
				new BeerType { Id = 1, Name="Stout", NormalizedName="stout"},
				new BeerType { Id = 2, Name="Lager", NormalizedName="lager"},
				new BeerType { Id = 3, Name="Porter", NormalizedName="porter"},
			};

			var result = new ResponseData<List<BeerType>>(beerTypes);
			return Task.FromResult(result);
		}
	}
}
