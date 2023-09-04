using Microsoft.EntityFrameworkCore;
using WebLab.API.Data;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.API.Services.BeerService
{
	public class BeerService : IBeerService
	{

		private readonly int _maxPageSize = 20;
		private readonly AppDbContext _context;

		public BeerService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<ResponseData<Beer>> CreateBeerAsync(Beer beer)
		{
			_context.Beer.Add(beer);
			await _context.SaveChangesAsync();
			return new ResponseData<Beer>(beer);
		}

		public async Task DeleteBeerAsync(int id)
		{
			var beerToRemove = _context.Beer.Find(id) ?? throw new KeyNotFoundException();
			_context.Remove(beerToRemove);
			await _context.SaveChangesAsync();
		}

		public async Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized, int pageNo = 1, int pageSize = 3)
		{
			if (pageSize > _maxPageSize)
				pageSize = _maxPageSize;
			var query = _context.Beer.AsQueryable();
			var dataList = new ListModel<Beer>();
			if (beerTypeNormalized != null)
			{
				query = query.Where(beer => beer.Type!.NormalizedName.Equals(beerTypeNormalized));

			}

			var count = query.Count();
			if (count == 0)
			{
				return new ResponseData<ListModel<Beer>>(dataList);
			}

			int totalPages = (int)Math.Ceiling(count / (double)pageSize);
			if (pageNo > totalPages)
			{
				return new ResponseData<ListModel<Beer>>(null!)
				{
					IsSuccess = false,
					ErrorMessage = "No such page"
				};
			}

			dataList.Items = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
			dataList.CurrentPage = pageNo;
			dataList.TotalPages = totalPages;

			return new ResponseData<ListModel<Beer>>(dataList)
			{
				Data = dataList
			};
		}

		public Task<ResponseData<Beer>> GetBeerByIdAsync(int id)
		{
			var beer = _context.Beer.Find(id);
			return Task.FromResult(new ResponseData<Beer>(beer!));
		}

		public async Task UpdateBeerAsync(int id, Beer beer)
		{
			var beerToUpdate = _context.Beer.Find(id) ?? throw new KeyNotFoundException();
			
			beerToUpdate.Price = beer.Price;
			beerToUpdate.Description = beer.Description;
			beerToUpdate.Name = beer.Name;
			beerToUpdate.TypeId = beerToUpdate.TypeId;

			_context.Update(beerToUpdate);
			await _context.SaveChangesAsync();
		}

		public Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile)
		{
			throw new NotImplementedException();
		}
	}
}
