using WebLab.Models;
using WebLab.Services.BeerService;
using WebLab.Services.BeerTypeService;

namespace WebLab
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();

			AddServices(builder.Services);

			var uriData = builder.Configuration.GetSection("UriData").Get<UriData>()!;
			builder.Services.AddHttpClient("API", opt => opt.BaseAddress = new(uriData.ApiUri));


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
				  name: "Admin",
				  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
			);

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}"
			);

			app.MapRazorPages();

			app.Run();
		}

		private static void AddServices(IServiceCollection services)
		{
			services.AddScoped<IBeerService, ApiBeerService>();
			services.AddScoped<IBeerTypeService, ApiBeerTypeService>();
		}
	}
}