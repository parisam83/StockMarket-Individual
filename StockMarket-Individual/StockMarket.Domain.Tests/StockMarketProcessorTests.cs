/*
// Arrange
// Act
// Assert
 */

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

            // Assert
        }
    }
}