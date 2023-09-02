using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebLab
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddDbContext<BeerContext>(options =>
			    options.UseSqlServer(builder.Configuration.GetConnectionString("BeerContext") ?? throw new InvalidOperationException("Connection string 'BeerContext' not found.")));

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			AddServices(builder.Services);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}

		private static void AddServices(IServiceCollection services)
		{
			services.AddScoped<IBeerTypeService, MemoryBeerTypeService>();
			services.AddScoped<IBeerService, MemoryBeerService>();
		}
	}
}