namespace StockMarket.Domain.States
{
    public class CloseState : MarketState
    {
        public CloseState(StockMarketProcessor stockMarketProcessor) : base(stockMarketProcessor)
        {
        }
        public override void CloseMarket()
        {
        }

        public override void OpenMarket()
        {
            stockMarketProcessor.Open();
        }

        public override long EnqueueOrder(TradeSide tradeSide, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public override long? CancelOrder(long orderId)
        {
            return null;
            // throw new NotImplementedException();
        }

        public override long ModifyOrder(long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }
    }
}