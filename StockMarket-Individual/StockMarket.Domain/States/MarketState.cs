namespace StockMarket.Domain.States
{
    public abstract class MarketState : IStockMarketProccessor
    {
        protected StockMarketProcessor stockMarketProcessor { get; set; }
        public IEnumerable<Order> Orders => throw new NotImplementedException();
        public IEnumerable<Trade> Trades => throw new NotImplementedException();

        public MarketState(StockMarketProcessor stockMarketProcessor)
        {
            this.stockMarketProcessor = stockMarketProcessor;
        }

        public virtual void CloseMarket()
        {
            throw new NotImplementedException();
        }

        public virtual void OpenMarket()
        {
            throw new NotImplementedException();
        }

        public virtual Task<long> EnqueueOrderAsync(TradeSide tradeSide, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public virtual Task<long> CancelOrderAsync(long orderId)
        {
            throw new NotImplementedException();
        }

        public virtual Task<long> ModifyOrderAsync(long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            throw new NotImplementedException();
        }
    }
}