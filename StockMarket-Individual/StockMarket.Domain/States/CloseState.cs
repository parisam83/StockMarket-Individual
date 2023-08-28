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

        public override Task<long> EnqueueOrderAsync(TradeSide tradeSide, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public override Task<long> CancelOrderAsync(long orderId)
        {
            throw new NotImplementedException();
        }

        public override Task<long> ModifyOrderAsync(long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }
    }
}