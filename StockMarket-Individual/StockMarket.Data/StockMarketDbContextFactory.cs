using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StockMarket.Data
{
    internal class StockMarketDbContextFactory : IDesignTimeDbContextFactory<StockMarketDbContext>
    {
        public StockMarketDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();
            optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=StockMarket;MultipleActiveResultSets=true;trusted_connection=true;encrypt=yes;trustservercertificate=yes;");
            return new StockMarketDbContext(optionsBuilder.Options);
        }
    }
}