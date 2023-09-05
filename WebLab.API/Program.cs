
using Microsoft.EntityFrameworkCore;
using WebLab.API.Data;
using WebLab.API.Services.BeerService;
using WebLab.API.Services.BeerTypeService;

namespace WebLab.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			AddServices(builder.Services);

			AddDbContext(builder);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.UseStaticFiles();


			app.MapControllers();

			// DbInitializer.SeedData(app).Wait();

			app.Run();
		}

		private static void AddDbContext(WebApplicationBuilder builder)
		{
			string connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
			builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
		}

		private static void AddServices(IServiceCollection services)
		{
			services.AddScoped<IBeerTypeService, BeerTypeService>();
			services.AddScoped<IBeerService, BeerService>();

			services.AddHttpContextAccessor();
		}
	}
}