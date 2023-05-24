using FluentAssertions;

namespace StockMarket.Domain.Tests
{
    public class StockMarketProcessorTests
    {
        [Fact]
        public void EnqueueOrder_Should_Process_SellOrder_When_BuyOrder_Is_Already_Enqueued_Test()
        {
            // Arrange
            var sut = new StockMarketProcessor();
            var buyOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);

            // Act
            var sellOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 2, price: 1400);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Equal(1, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 0M,
                Price = 1500M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 1M,
                Price = 1400M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Quantity = 1M,
                Price = 1400M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_BuyOrder_When_SellOrder_Is_Already_Enqueued_Test()
        {
            // Arrange
            var sut = new StockMarketProcessor();
            var sellOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);

            // Act
            var buyOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Equal(1, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 0M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 0M,
                Price = 1500M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Quantity = 1M,
                Price = 1400M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_SellOrder_When_Multiple_BuyOrders_Are_Already_Enqueued_Test()
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var buyOrderId1 = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);
            var buyOrderId2 = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 2, price: 1500);

            // Act
            var sellOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Equal(1, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1400M
            });
            // Question: how to access the second element?
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 0M,
                Price = 1000M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId,
                Quantity = 1M,
                Price = 1000M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_BuyOrder_When_Multiple_SellOrders_Are_Already_Enqueued_Test()
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var sellOrderId1 = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            var sellOrderId2 = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 2, price: 1500);

            // Act
            var buyOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Equal(0, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 1M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1000M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_SellOrder_When_Some_BuyOrders_Are_Matched_Enqueued_Test() 
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var buyOrderId1 = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);
            var buyOrderId2 = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 2, price: 1500);

            // Act
            var sellOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 8, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Equal(2, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 0M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 5M,
                Price = 1000M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId,
                Quantity = 2M,
                Price = 1000M
            });
            sut.Trades.Last().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId1,
                SellOrderId = sellOrderId,
                Quantity = 1M,
                Price = 1000M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_BuyOrder_When_Some_SellOrders_Are_Matched_Enqueued_Test()
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var sellOrderId1 = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            var sellOrderId2 = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 2, price: 1500);

            // Act
            var buyOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 8, price: 1700);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Equal(2, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 0M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 5M,
                Price = 1700M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId1,
                Quantity = 1M,
                Price = 1400M
            });
            sut.Trades.Last().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId2,
                Quantity = 2M,
                Price = 1500M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Not_Process_BuyOrder_When_No_SellOrders_Are_Matched_Test()
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var sellOrderId1 = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            var sellOrderId2 = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 2, price: 1500);

            // Act
            var buyOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Equal(0, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 1M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1000M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Not_Proccess_SellOrder_When_No_BuyOrders_Are_Matched_Test()
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var buyOrderId1 = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);
            var buyOrderId2 = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 2, price: 1500);

            // Act
            var sellOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 6, price: 2000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Equal(0, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 6M,
                Price = 2000M
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Proccess_BuyOrder_When_Demand_Is_More_Than_Supply_Test()
        {
            // Arrage
            var sut = new StockMarketProcessor();
            var sellOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);

            // Act
            var buyOrderId = sut.EnqueueOrder(tradeSide: TradeSide.Buy, quantity: 3, price: 1400);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Equal(1, sut.Trades.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Sell,
                Quantity = 0M,
                Price = 1400M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 2M,
                Price = 1400M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Quantity = 1M,
                Price = 1400M
            });
        }

        [Fact]

    }
}