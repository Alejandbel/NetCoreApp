using WebLab.API.Data;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.API.Services.BeerTypeService
{
    public class BeerTypeService : IBeerTypeService
    {
        private readonly AppDbContext _context;

        public BeerTypeService(AppDbContext context)
        {
            _context = context;
        }

        public Task<ResponseData<List<BeerType>>> GetBeerTypeListAsync()
        {
            var beerTypes = _context.BeerType.ToList();
            var result = new ResponseData<List<BeerType>>(beerTypes);
            return Task.FromResult(result);
        }
    }
}
