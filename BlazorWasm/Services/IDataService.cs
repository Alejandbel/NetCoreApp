using WebLab.Domain.Entities;

namespace BlazorWasm.Services
{
    public interface IDataService
    {
        event Action DataLoaded;

        List<BeerType> Categories { get; set; }

        List<Beer> ObjectsList { get; set; }

        bool Success { get; set; }

        string ErrorMessage { get; set; }

        int TotalPages { get; set; }

        int CurrentPage { get; set; }

        public Task GetProductListAsync(string? categoryNormalizedName, int pageNo = 1);

        public Task<Beer> GetProductByIdAsync(int id);

        public Task GetCategoryListAsync();
    }
}
