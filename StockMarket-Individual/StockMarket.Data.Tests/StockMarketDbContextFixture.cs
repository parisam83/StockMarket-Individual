using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data.Tests
{
    public class StockMarketDbContextFixture : IDisposable
    {
        public StockMarketDbContext Context { get; }
        public StockMarketDbContextFixture() 
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();
            optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=StockMarketTests;MultipleActiveResultSets=true;trusted_connection=true;encrypt=yes;trustservercertificate=yes;");
            Context = new(optionsBuilder.Options);

            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}