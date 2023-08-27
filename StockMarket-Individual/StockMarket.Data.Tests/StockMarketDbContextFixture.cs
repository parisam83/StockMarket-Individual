using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace StockMarket.Data.Tests
{
    public class StockMarketDbContextFixture : IAsyncDisposable
    {
        public StockMarketDbContext Context { get; }
        public ITestOutputHelper? Output { get; set; }

        public StockMarketDbContextFixture() 
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();
            optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=StockMarketTests;MultipleActiveResultSets=true;trusted_connection=true;encrypt=yes;trustservercertificate=yes;");
            optionsBuilder.LogTo(msg => Output?.WriteLine(msg));
            Context = new(optionsBuilder.Options);

            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }
    }
}