using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StockMarket.Domain;
using Xunit.Abstractions;

namespace StockMarket.Data.Tests
{
    public class IntegrationTests : IClassFixture<StockMarketDbContextFixture>, IAsyncDisposable
    {
        private readonly StockMarketDbContext context;
        public IntegrationTests(StockMarketDbContextFixture fixture, ITestOutputHelper output) 
        {
            context = fixture.Context;
            fixture.Output = output;
            context.Database.OpenConnection();
        }

        public async ValueTask DisposeAsync()
        {
            await context.Database.CloseConnectionAsync();
        }

        [Fact]
        public async Task DbContext_Should_Save_Orders_In_Database_Test_Async()
        {
            // Arrange
            var processor = new StockMarketProcessor(
                lastOrderId: await context.Orders.MaxAsync(o => (long?)o.Id) ?? 0);
            processor.OpenMarket();

            var buyOrderId = await processor.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1M, price: 1500M);
            var sellOrderId = await processor.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1M, price: 1500M);

            var buyOrder = processor.Orders.First(o => o.Id == buyOrderId);
            var sellOrder = processor.Orders.First(o => o.Id == sellOrderId);

            // Act
            await context.Orders.AddAsync(buyOrder);
            await context.Orders.AddAsync(sellOrder);
            await context.SaveChangesAsync();

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
        public async Task DbContext_Should_Save_Trades_In_Database_Test_Async()
        {
            // Arrange
            var processor = new StockMarketProcessor(
                lastOrderId: await context.Orders.MaxAsync(o => (long?)o.Id) ?? 0,
                lastTradeId: await context.Trades.MaxAsync(t => (long?)t.Id) ?? 0);
            processor.OpenMarket();

            var buyOrderId = await processor.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1M, price: 1500M);
            var sellOrderId = await processor.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1M, price: 1500M);

            var buyOrder = processor.Orders.First(o => o.Id == buyOrderId);
            var sellOrder = processor.Orders.First(o => o.Id == sellOrderId);
            var trade = processor.Trades.First();

            // Act
            await context.Orders.AddAsync(buyOrder);
            await context.Orders.AddAsync(sellOrder);
            await context.Trades.AddAsync(trade);
            await context.SaveChangesAsync();

            // Assert
            context.Trades.First(t => t.Id == trade.Id).Should().BeEquivalentTo(new
            {
                trade.Id,
                SellOrderId = sellOrderId,
                BuyOrderId = buyOrderId,
                Price = 1500M,
                Quantity = 1M
            });
        }
        [Fact]
        public async Task DbContext_Should_Retrive_Orders_From_Database_Test_Async()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();
            optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=StockMarketTests;MultipleActiveResultSets=true;trusted_connection=true;encrypt=yes;trustservercertificate=yes;");


            // Act
            var context1 = new StockMarketDbContext(optionsBuilder.Options);

            var processor1 = new StockMarketProcessor(
                lastOrderId: await context1.Orders.MaxAsync(o => (long?)o.Id) ?? 0,
                lastTradeId: await context1.Trades.MaxAsync(t => (long?)t.Id) ?? 0);
            processor1.OpenMarket();

            var buyOrderId = await processor1.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1M, price: 1500M);
            var buyOrder = processor1.Orders.First(o => o.Id == buyOrderId);

            await context1.Orders.AddAsync(buyOrder);
            await context1.SaveChangesAsync();
            await context1.DisposeAsync();


            var context2 = new StockMarketDbContext(optionsBuilder.Options);

            var processor2 = new StockMarketProcessor(
                orders: await context2.Orders.Where(o => !o.IsCanceled && o.Quantity > 0).ToListAsync(),
                lastOrderId: await context2.Orders.MaxAsync(o => (long?)o.Id) ?? 0,
                lastTradeId: await context2.Trades.MaxAsync(t => (long?)t.Id) ?? 0);
            processor2.OpenMarket();

            var sellOrderId = await processor2.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1M, price: 1500M);
            var sellOrder = processor2.Orders.First(o => o.Id == sellOrderId);
            var trade = processor2.Trades.First();

            await context2.Orders.AddAsync(sellOrder);
            await context2.Trades.AddAsync(trade);
            await context2.SaveChangesAsync();
            await context2.DisposeAsync();

            var context3 = new StockMarketDbContext(optionsBuilder.Options);

            // Assert
            context3.Orders.First(o => o.Id == buyOrderId).Should().BeEquivalentTo(new
            {
                Id = buyOrderId,
                TradeSide = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M,
                IsCanceled = false,
            });
            context3.Orders.First(o => o.Id == sellOrderId).Should().BeEquivalentTo(new
            {
                Id = sellOrderId,
                TradeSide = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M,
                IsCanceled = false,
            });
            context3.Trades.First(t => t.Id == trade.Id).Should().BeEquivalentTo(new
            {
                trade.Id,
                SellOrderId = sellOrderId,
                BuyOrderId = buyOrderId,
                Price = 1500M,
                Quantity = 1,
            });
        }
    }
}