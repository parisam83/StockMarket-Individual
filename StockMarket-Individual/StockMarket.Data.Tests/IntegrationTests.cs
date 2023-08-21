using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StockMarket.Domain;

namespace StockMarket.Data.Tests
{
    public class IntegrationTests
    {
        private StockMarketProcessor stockMarketProcessor;
        public IntegrationTests() 
        {
            stockMarketProcessor = new();
            stockMarketProcessor.OpenMarket();
        }

        [Fact]
        public void DbContext_Should_Save_Orders_In_Database_Test()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();
            optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=StockMarket;MultipleActiveResultSets=true;trusted_connection=true;encrypt=yes;trustservercertificate=yes;");
            var context = new StockMarketDbContext(optionsBuilder.Options);

            var buyOrderId = stockMarketProcessor.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            var sellOrderId = stockMarketProcessor.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 2, price: 1400);

            var buyOrder = stockMarketProcessor.Orders.Single(o => o.Id == buyOrderId);
            var sellOrder = stockMarketProcessor.Orders.Single(o => o.Id == sellOrderId);

            // Act
            context.Orders.Add(buyOrder);
            context.Orders.Add(sellOrder);
            context.SaveChanges();

            // Assert
            context.Orders.First(o => o.Id == buyOrderId).Should().BeEquivalentTo(new
            {
                Id = buyOrderId,
                TradeSide = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M,
                IsCanceled = false
            });
            context.Orders.First(o => o.Id == sellOrderId).Should().BeEquivalentTo(new
            {
                Id = sellOrderId,
                TradeSide = TradeSide.Sell,
                Price = 1400M,
                Quantity = 1M,
                IsCanceled = false
            });
        }
    }
}