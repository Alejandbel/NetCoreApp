using Microsoft.EntityFrameworkCore;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace WebLab.API.Data
{
	public class DbInitializer
	{
		public static async Task SeedData(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			await dbContext.Database.EnsureDeletedAsync();
			await dbContext.Database.MigrateAsync();

			var lager = new BeerType { Name = "Lager", NormalizedName = "lager" };
			var stout = new BeerType { Name = "Stout", NormalizedName = "stout" };
			var porter = new BeerType { Name = "Porter", NormalizedName = "porter" };

			var beerTypes = new List<BeerType>() { lager, stout, porter };

			await dbContext.BeerType.AddRangeAsync(beerTypes);

			var applicationUrl = app.Configuration.GetValue<string>("ApplicationUrl");

			await dbContext.SaveChangesAsync();

			var beers = new List<Beer> {
				new Beer()
				{
					Name = "Лидское премиум",
					Description = "Легкий ароматный лагер золотистого цвета с пышной пеной и мягким вкусом.",
					ImagePath = $"{applicationUrl}/images/beer-1.png",
					Price = 2.56M,
					TypeId = lager.Id,
					Type = lager,
				},
				new Beer()
				{
					Name = "Лидское Портер",
					Description = "Tемное пиво в стиле Балтийский портер, обладающее глубоким карамельным вкусом и выраженным ароматом с оттенками тостов.",
					ImagePath = $"{applicationUrl}/images/beer-2.png",
					Price = 2.81M,
					TypeId = porter.Id,
					Type = porter,
				},
				new Beer()
				{
					Name = "Koronet stout original",
					Description = "Пиво, сваренное в британском стиле. Глубокий темный цвет, солодово-карамельный аромат, горчинка и пышная кремовая пена.",
					ImagePath = $"{applicationUrl}/images/beer-3.png",
					Price = 3.02M,
					TypeId = stout.Id,
					Type = stout,
				},
				new Beer()
				{
					Name = "Балтика 9 Крепкое",
					Description = "Пиво производится по технологии низового брожения, крепость достигается естественным путем, благодаря использованию дрожжей, рассчитанных на интенсивное и длительное брожение. Напиток отличается свежим вкусом, легкой хмелевой горчинкой, мягким послевкусием с нотками солода.",
					ImagePath = $"{applicationUrl}/images/beer-4.png",
					Price = 2.13M,
					TypeId = lager.Id,
					Type = lager,
				}
			};

			await dbContext.Beer.AddRangeAsync(beers);
			await dbContext.SaveChangesAsync();
		}
	}
}
