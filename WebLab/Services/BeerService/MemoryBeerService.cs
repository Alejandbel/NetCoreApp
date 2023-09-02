using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;
using WebLab.Services.BeerTypeService;

namespace WebLab.Services.BeerService
{
	public class MemoryBeerService : IBeerService
	{

		List<Beer> _beers;
		List<BeerType> _beerTypes;

		int _pageSize;

		public MemoryBeerService([FromServices] IConfiguration config, IBeerTypeService beerTypeService)
		{
			_beerTypes = beerTypeService.GetBeerTypeListAsync().Result.Data;
			_pageSize = int.Parse(config["BeerPageSize"]!);

			SetupBeers();
		}

		private void SetupBeers()
		{
			var lager = _beerTypes.Find((type) => type.NormalizedName == "lager")!;
			var stout = _beerTypes.Find((type) => type.NormalizedName == "stout")!;
			var porter = _beerTypes.Find((type) => type.NormalizedName == "porter")!;
			_beers = new List<Beer> {
				new Beer()
				{
					Id = 1,
					Name = "Лидское премиум",
					Description = "Легкий ароматный лагер золотистого цвета с пышной пеной и мягким вкусом.",
					ImagePath = "images/beer-1.png",
					Price = 2.56M,
					TypeId = lager.Id,
					Type = lager,
				},
				new Beer()
				{
					Id = 2,
					Name = "Лидское Портер",
					Description = "Tемное пиво в стиле Балтийский портер, обладающее глубоким карамельным вкусом и выраженным ароматом с оттенками тостов.",
					ImagePath = "images/beer-2.png",
					Price = 2.81M,
					TypeId = porter.Id,
					Type = porter,
				},
				new Beer()
				{
					Id = 3,
					Name = "Koronet stout original",
					Description = "Пиво, сваренное в британском стиле. Глубокий темный цвет, солодово-карамельный аромат, горчинка и пышная кремовая пена.",
					ImagePath = "images/beer-3.png",
					Price = 3.02M,
					TypeId = stout.Id,
					Type = stout,
				},
				new Beer()
				{
					Id = 4,
					Name = "Балтика 9 Крепкое",
					Description = "Пиво производится по технологии низового брожения, крепость достигается естественным путем, благодаря использованию дрожжей, рассчитанных на интенсивное и длительное брожение. Напиток отличается свежим вкусом, легкой хмелевой горчинкой, мягким послевкусием с нотками солода.",
					ImagePath = "images/beer-4.png",
					Price = 2.13M,
					TypeId = lager.Id,
					Type = lager,
				}
			};
		}

		public Task<ResponseData<Beer>> CreateProductAsync(Beer beer, IFormFile? formFile)
		{
			_beers.Add(beer);
			return Task.FromResult(new ResponseData<Beer>(beer));
		}

		public Task DeleteProductAsync(int id)
		{
			var index = _beers.FindIndex((beer) => beer.Id == id);

			if (index == -1)
			{
				throw new KeyNotFoundException();
			}

			_beers.RemoveAt(index);
			return Task.CompletedTask;
		}

		public Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized, int pageNo = 1)
		{
			pageNo = pageNo == 0 ? 1 : pageNo;

			var beers = string.IsNullOrEmpty(beerTypeNormalized) ? _beers : _beers.FindAll((beer) => beer.Type?.NormalizedName == beerTypeNormalized);
			int totalPages = (beers.Count - 1) / _pageSize + 1;

			if (totalPages < pageNo)
			{
				return Task.FromResult(new ResponseData<ListModel<Beer>>(default) { IsSuccess = false });
			}

			var beersPaginated = beers.Skip((pageNo - 1) * _pageSize).Take(_pageSize).ToList();
			var listModel = new ListModel<Beer>(beersPaginated) { CurrentPage = pageNo, TotalPages = totalPages };
			return Task.FromResult(new ResponseData<ListModel<Beer>>(listModel));

		}

		public Task<ResponseData<Beer>> GetProductByIdAsync(int id)
		{
			var beer = _beers.Find((beer) => beer.Id == id) ?? throw new KeyNotFoundException();
			return Task.FromResult(new ResponseData<Beer>(beer));
		}

		public Task UpdateProductAsync(int id, Beer beer, IFormFile? formFile)
		{
			var index = _beers.FindIndex((beer) => beer.Id == id);

			if (index == -1)
			{
				throw new KeyNotFoundException();
			}

			_beers[index] = beer;
			return Task.CompletedTask;
		}
	}
}
