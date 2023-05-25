using StockMarket.Domain;
using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data
{
    public class StockMarketDbContext : DbContext
    {
        public StockMarketDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(b =>
            {
                b.Property(o => o.Id);
                b.Property(o => o.TradeSide);
                b.Property(o => o.Price).HasColumnType("Money");
                b.Property(o => o.Quantity).HasColumnType("Money");
            });
        }
    }
}