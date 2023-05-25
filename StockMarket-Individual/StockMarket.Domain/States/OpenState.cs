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

        public override long EnqueueOrder(TradeSide tradeSide, decimal quantity, decimal price)
        {
            return stockMarketProcessor.Enqueue(tradeSide, quantity, price);
        }

        public override long? CancelOrder(long orderId)
        {
            return stockMarketProcessor.Cancel(orderId);
        }

        public override long ModifyOrder(long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            return stockMarketProcessor.Modify(orderId, tradeSide, quantity, price);
        }
    }
}