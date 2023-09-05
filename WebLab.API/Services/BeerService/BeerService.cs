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
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IWebHostEnvironment _environment;
		private readonly string _imageFolder;

		public BeerService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
		{
			_context = context;
			_contextAccessor = httpContextAccessor;
			_environment = environment;
			_imageFolder = Path.Combine(_environment.WebRootPath, "Images");
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

			dataList.Items = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).Include(beer => beer.Type).ToListAsync();
			dataList.CurrentPage = pageNo;
			dataList.TotalPages = totalPages;

			return new ResponseData<ListModel<Beer>>(dataList)
			{
				Data = dataList
			};
		}

		public Task<ResponseData<Beer>> GetBeerByIdAsync(int id)
		{
			var beer = _context.Beer.Include(beer => beer.Type).FirstOrDefault(beer => beer.Id == id);
			return Task.FromResult(new ResponseData<Beer>(beer!));
		}

		public async Task UpdateBeerAsync(int id, Beer beer)
		{
			var types = _context.BeerType.ToList();
			var beerToUpdate = _context.Beer.Find(id) ?? throw new KeyNotFoundException();

			beerToUpdate.Price = beer.Price;
			beerToUpdate.Description = beer.Description;
			beerToUpdate.Name = beer.Name;
			beerToUpdate.TypeId = beer.TypeId;

			_context.Update(beerToUpdate);
			await _context.SaveChangesAsync();
		}

		public async Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile)
		{
			var responseData = new ResponseData<string>(String.Empty);
			var beer = await _context.Beer.FindAsync(id);

			if (beer == null)
			{
				responseData.IsSuccess = false;
				responseData.ErrorMessage = "No item found";
				return responseData;
			}

			var host = "http://" + _contextAccessor.HttpContext!.Request.Host;
			if (formFile != null)
			{
				if (!string.IsNullOrEmpty(beer.ImagePath))
				{
					var prevImage = Path.GetFileName(beer.ImagePath);
					File.Delete(Path.Combine(_imageFolder, prevImage));
				}

				var ext = Path.GetExtension(formFile.FileName);
				var fName = Path.ChangeExtension(Path.GetRandomFileName(), ext);

				using var fileStream = File.Create(Path.Combine(_imageFolder, fName));
				await formFile.CopyToAsync(fileStream);

				beer.ImagePath = $"{host}/images/{fName}";
				await _context.SaveChangesAsync();
			}
			responseData.Data = beer.ImagePath;
			return responseData;
		}
	}
}
