using Microsoft.EntityFrameworkCore;
using WebLab.Domain.Entities;

    public class BeerContext : DbContext
    {
        public BeerContext (DbContextOptions<BeerContext> options)
            : base(options)
        {
        }

        public DbSet<Beer> Beer { get; set; } = default!;
    }
