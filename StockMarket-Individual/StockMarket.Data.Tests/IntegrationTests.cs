using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StockMarket.Domain;

namespace StockMarket.Data.Tests
{
    public class IntegrationTests : IClassFixture<StockMarketDbContextFixture>
    {
        private readonly StockMarketDbContext context;
        private readonly StockMarketProcessor processor;
        public IntegrationTests(StockMarketDbContextFixture fixture) 
        {
            context = fixture.Context;
            processor = new(
                lastOrderId: context.Orders.Max(o => (long?)o.Id) ?? 0,
                lastTradeId: context.Trades.Max(t => (long?)t.Id) ?? 0);
            processor.OpenMarket();
        }
        [Fact]
        public void DbContext_Should_Save_Orders_In_Database_Test()
        {
            // Arrange
            var buyOrderId = processor.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            var sellOrderId = processor.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1500);

            var buyOrder = processor.Orders.First(o => o.Id == buyOrderId);
            var sellOrder = processor.Orders.First(o => o.Id == sellOrderId);

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
                Price = 1500M,
                Quantity = 0M,
                IsCanceled = false
            });
        }
        [Fact]
        public void DbContext_Should_Save_Trades_In_Database_Test()
        {
            // Arrange
            var buyOrderId = processor.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            var sellOrderId = processor.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1500);

            var buyOrder = processor.Orders.First(o => o.Id == buyOrderId);
            var sellOrder = processor.Orders.First(o => o.Id == sellOrderId);
            var trade = processor.Trades.First();


            // Act
            context.Orders.Add(buyOrder);
            context.Orders.Add(sellOrder);
            context.Trades.Add(trade);
            context.SaveChanges();

            // Assert
            context.Trades.First(t => t.Id == trade.Id).Should().BeEquivalentTo(new
            {
                SellOrderId = sellOrderId,
                BuyOrderId = buyOrderId,
                Price = 1500M,
                Quantity = 1M
            });
        }
        [Fact]
        public void DbContext_Should_Retrive_Orders_From_Database_Test()
        {
            // Arrange


            // Act


            // Assert
        }
    }
}