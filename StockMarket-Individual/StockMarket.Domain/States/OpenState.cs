using System.Drawing;

namespace StockMarket.Domain.States
{
    public class OpenState : MarketState
    {
        public OpenState(StockMarketProcessor stockMarketProcessor) : base(stockMarketProcessor)
        {
        }

        public override void CloseMarket()
        {
            stockMarketProcessor.Close();
        }

        public override void OpenMarket()
        {
        }

        public override async Task<long> EnqueueOrderAsync(TradeSide tradeSide, decimal quantity, decimal price)
        {
            return await stockMarketProcessor.EnqueueAsync(tradeSide, quantity, price);
        }

        public override async Task<long> CancelOrderAsync(long orderId)
        {
            return await stockMarketProcessor.CancelAsync(orderId);
        }

        public override long ModifyOrder(long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            return stockMarketProcessor.Modify(orderId, tradeSide, quantity, price);
        }
    }
}