using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using SearchService.Models;

namespace SearchService.Data
{
    public class SearchDbContext : DbContext
    {
        public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>().ToCollection("Items");            
        }

        public DbSet<Item> Items{ get; set; }
    }
}
