using FluentAssertions;
using StockMarket.Domain.States;

namespace StockMarket.Domain.Tests
{
    public class StockMarketProcessorTests
    {
        private readonly StockMarketProcessor sut;
        public StockMarketProcessorTests() 
        {
            sut = new();
            sut.OpenMarket();
        }
        [Fact]
        public async Task EnqueueOrder_Should_Process_SellOrder_When_BuyOrder_Is_Already_Enqueued_Test()
        {
            // Arrange
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);

            // Act
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 2, price: 1400);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Single(sut.Trades);
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
        public async Task EnqueueOrder_Should_Process_BuyOrder_When_SellOrder_Is_Already_Enqueued_Test()
        {
            // Arrange
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);

            // Act
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Single(sut.Trades);
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
        public async Task EnqueueOrder_Should_Process_SellOrder_When_Multiple_BuyOrders_Are_Already_Enqueued_Test()
        {
            // Arrage
            var buyOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);
            var buyOrderId2 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 2, price: 1500);

            // Act
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Single(sut.Trades);
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1400M
            });
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
        public async Task EnqueueOrder_Should_Process_BuyOrder_When_Multiple_SellOrders_Are_Already_Enqueued_Test()
        {
            // Arrage
            var sellOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            var sellOrderId2 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 2, price: 1500);

            // Act
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Empty(sut.Trades);
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
        public async Task EnqueueOrder_Should_Process_SellOrder_When_Some_BuyOrders_Are_Matched_Enqueued_Test() 
        {
            // Arrage
            var buyOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);
            var buyOrderId2 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 2, price: 1500);

            // Act
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 8, price: 1000);

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
        public async Task EnqueueOrder_Should_Process_BuyOrder_When_Some_SellOrders_Are_Matched_Enqueued_Test()
        {
            // Arrage
            var sellOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            var sellOrderId2 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 2, price: 1500);

            // Act
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 8, price: 1700);

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
        public async Task EnqueueOrder_Should_Not_Process_BuyOrder_When_No_SellOrders_Are_Matched_Test()
        {
            // Arrage
            var sellOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            var sellOrderId2 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 2, price: 1500);

            // Act
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Empty(sut.Trades);
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
        public async Task EnqueueOrder_Should_Not_Proccess_SellOrder_When_No_BuyOrders_Are_Matched_Test()
        {
            // Arrage
            var buyOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);
            var buyOrderId2 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 2, price: 1500);

            // Act
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 6, price: 2000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Empty(sut.Trades);
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
        public async Task EnqueueOrder_Should_Proccess_BuyOrder_When_Demand_Is_More_Than_Supply_Test()
        {
            // Arrage
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);

            // Act
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 3, price: 1400);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Single(sut.Trades);
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
        public async Task CancelOrder_Should_Cancel_Order_Test()
        {
            // Arrange
            var orderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);

            // Act
            await sut.CancelOrderAsync(orderId);

            // Assert
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                IsCanceled = true
            });
        }

        [Fact]
        public async Task CancelOrder_Should_Not_Process_Order_When_Peeked_MatchingOrder_Is_Canceled_Test()
        {
            // Arrange
            var canceledOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1400);
            await sut.CancelOrderAsync(canceledOrderId);

            // Act
            var newOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1400);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            Assert.Empty(sut.Trades);
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
                Price = 1400M
            });
        }

        [Fact]
        public void CloseMarket_Should_Close_StockMarket_Test()
        {
            // Arrange

            // Act
            sut.CloseMarket();

            // Assert
            Assert.Equal(sut.state.GetType(), typeof(CloseState));
        }

        [Fact]
        public async Task EnqueueOrder_Should_Not_Work_When_StockMarket_Is_Closed_Test()
        {
            // Arrange
            sut.CloseMarket();

            // Act
            async Task act() => await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);

            // Assert
            await Assert.ThrowsAsync<NotImplementedException>(act);
        }

        [Fact]
        public async Task CancelOrder_Should_Not_Work_When_StockMarket_Is_Closed_Test()
        {
            // Arrange
            var orderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CloseMarket();

            // Act
            async Task act() => await sut.CancelOrderAsync(orderId);

            // Assert
            await Assert.ThrowsAsync<NotImplementedException>(act);
        }

        [Fact]
        public void OpenMarket_Should_Open_StockMarket_When_StockMarket_Is_Already_Closed_Test()
        {
            // Arrange
            sut.CloseMarket();

            // Act
            sut.OpenMarket();

            // Assert
            Assert.Equal(sut.state.GetType(), typeof(OpenState));
        }

        [Fact]
        public async Task EnqueueOrder_Should_Work_When_StockMarket_Is_Opened_After_Being_Closed_Test()
        {
            // Arrange
            sut.CloseMarket();
            sut.OpenMarket();

            // Act
            var orderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);

            // Assert
            Assert.Single(sut.Orders);
            Assert.Empty(sut.Trades);
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1500M
            });
        }

        [Fact]
        public async Task CancelOrder_Should_Work_When_StockMarket_Is_Opened_After_Being_Closed_Test()
        {
            // Arrange
            var orderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CloseMarket();
            sut.OpenMarket();

            // Act
            await sut.CancelOrderAsync(orderId);

            // Assert
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                IsCanceled = true
            });
        }

        [Fact]
        public async Task ModifyOrder_Test()
        {
            // buy order
            // sell order : doesn't match with sell order
            // buy order modifies so that the orders match

            // Arrange
            var buyOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            var sellOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1, price: 1700);

            // Act
            var buyOrderId2 = sut.ModifyOrder(buyOrderId1, TradeSide.Buy, 1, 2000);

            // Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Single(sut.Trades);
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1500M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                TradeSide = TradeSide.Buy,
                Quantity = 0M,
                Price = 2000M
            });
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId,
                Quantity = 1M,
                Price = 1700M
            });
        }

        [Fact]
        public async Task ModifyOrder_Should_Not_Work_When_StockMarket_Is_Closed_Test()
        {
            // Arrange
            var buyOrderId = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CloseMarket();

            // Act
            void act() => sut.ModifyOrder(buyOrderId, tradeSide: TradeSide.Buy, quantity: 1, price: 2000);

            // Assert
            Assert.Throws<NotImplementedException>(act);
        }

        [Fact]
        public async Task ModifyOrder_Should_Work_When_StockMarket_Is_Opened_Test()
        {
            // Arrange
            var buyOrderId1 = await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CloseMarket();
            sut.OpenMarket();

            // Act
            var buyOrderId2 = sut.ModifyOrder(buyOrderId1, TradeSide.Buy, 1, 2000);

            // Assert
            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Id = buyOrderId1,
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 1500M
            });
            sut.Orders.Last().Should().BeEquivalentTo(new
            {
                Id = buyOrderId2,
                TradeSide = TradeSide.Buy,
                Quantity = 1M,
                Price = 2000M
            });
        }

        [Fact]
        public async Task EnqueueOrder_Should_Process_Concurrent_Orders_Test_Async()
        {
            // Arrange
            sut.OpenMarket();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await sut.EnqueueOrderAsync(tradeSide: TradeSide.Buy, quantity: 1M, price: 1500M);
                }));
            }

            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await sut.EnqueueOrderAsync(tradeSide: TradeSide.Sell, quantity: 1M, price: 1500M);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(10, sut.Orders.Count());
            Assert.Equal(5, sut.Trades.Count());
        }
    }
}