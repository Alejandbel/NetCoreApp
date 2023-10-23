using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebLab.API.Data;
using WebLab.API.Services.BeerService;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.Tests
{
    public class ApiBeerServiceTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public ApiBeerServiceTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_contextOptions);

            context.Database.EnsureCreated();

            BeerType beerType1 = new() { Name = "Beer1", NormalizedName = "beer1" };
            BeerType beerType2 = new() { Name = "Beer2", NormalizedName = "beer2" };
            context.BeerType.AddRange(beerType1, beerType2);
            context.SaveChanges();

            List<Beer> beers = new();
            for (int i = 0; i < 30; i++)
            {
                Beer beer = new() { Name = $"Beew {i}", Description = $"Description {i}", Price = i, TypeId = i % 2 + 1 };
                beers.Add(beer);
            }
            context.AddRange(beers);

            context.SaveChanges();
        }

        AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();

        [Fact]
        public void GetBeerListAsync_NoFilter_ReturnsFirstPageOfThreeItems()
        {
            using var context = CreateContext();
            var service = new BeerService(context, null!, null!);
            var result = service.GetBeerListAsync(null).Result;
            Assert.True(result.IsSuccess);
            Assert.IsType<ResponseData<ListModel<Beer>>>(result);
            Assert.Equal(1, result.Data?.CurrentPage);
            Assert.Equal(3, result.Data?.Items.Count);
            Assert.Equal(10, result.Data?.TotalPages);
            Assert.Equal(context.Beer.AsEnumerable().Take(3), result.Data?.Items);
        }

        [Fact]
        public void GetBeerListAsync_WithPageNumber_ReturnsRightPageOfThreeItems()
        {
            using var context = CreateContext();
            var service = new BeerService(context, null!, null!);
            var result = service.GetBeerListAsync(null, pageNo: 2).Result;
            Assert.True(result.IsSuccess);
            Assert.IsType<ResponseData<ListModel<Beer>>>(result);
            Assert.Equal(2, result.Data?.CurrentPage);
            Assert.Equal(3, result.Data?.Items.Count);
            Assert.Equal(10, result.Data?.TotalPages);
            Assert.Equal(context.Beer.AsEnumerable().Skip(3).Take(3), result.Data?.Items);
        }

        [Fact]
        public void GetBeerListAsync_WithCategoryFilter_ReturnsFilteredByCategory()
        {
            using var context = CreateContext();
            var service = new BeerService(context, null!, null!);
            var result = service.GetBeerListAsync("beer1").Result;
            Assert.True(result.IsSuccess);
            Assert.IsType<ResponseData<ListModel<Beer>>>(result);
            Assert.Equal(1, result.Data?.CurrentPage);
            Assert.Equal(3, result.Data?.Items.Count);
            Assert.Equal(5, result.Data?.TotalPages);
            Assert.Equal(context.Beer.AsEnumerable().Where((b) => b.TypeId == 1).Take(3), result.Data?.Items);
        }

        [Fact]
        public void GetBeerListAsync_MaxSizeSucceded_ReturnsMaximumMaxSize()
        {
            using var context = CreateContext();
            var service = new BeerService(context, null!, null!);
            var result = service.GetBeerListAsync(null, pageSize: 30).Result;
            Assert.True(result.IsSuccess);
            Assert.IsType<ResponseData<ListModel<Beer>>>(result);
            Assert.Equal(1, result.Data?.CurrentPage);
            Assert.Equal(20, result.Data?.Items.Count);
            Assert.Equal(2, result.Data?.TotalPages);
        }

        [Fact]
        public void GetBeerListAsync_PageNoIncorrect_ReturnsError()
        {
            using var context = CreateContext();
            var service = new BeerService(context, null!, null!);
            var result = service.GetBeerListAsync(null, pageNo: 15).Result;
            Assert.False(result.IsSuccess);
        }
    }
}
