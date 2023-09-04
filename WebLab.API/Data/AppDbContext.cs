using Microsoft.EntityFrameworkCore;
using WebLab.Domain.Entities;

namespace WebLab.API.Data
{

	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Beer> Beer { get; set; } = default!;
		public DbSet<BeerType> BeerType { get; set; } = default!;
	}

}
