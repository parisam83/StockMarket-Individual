using StockMarket.Domain;
using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data
{
    public class StockMarketDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Trade> Trades { get; set; }

        public StockMarketDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(b =>
            {
                b.Property(o => o.Id).ValueGeneratedNever();
                b.Property(o => o.TradeSide);
                b.Property(o => o.Price).HasColumnType("Money");
                b.Property(o => o.Quantity).HasColumnType("Money");
            });
            modelBuilder.Entity<Trade>(b =>
            {
                b.Property(t => t.Id).ValueGeneratedNever();
                b.HasOne<Order>().WithMany().IsRequired().HasForeignKey(t => t.SellOrderId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne<Order>().WithMany().IsRequired().HasForeignKey(t => t.BuyOrderId).OnDelete(DeleteBehavior.Restrict);
                b.Property(t => t.Price).HasColumnType("Money");
                b.Property(t => t.Quantity).HasColumnType("Money");
            });
        }
    }
}