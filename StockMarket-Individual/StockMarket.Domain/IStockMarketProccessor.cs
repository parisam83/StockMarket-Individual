namespace StockMarket.Domain
{
    public interface IStockMarketProccessor
    {
        IEnumerable<Order> Orders { get; }
        IEnumerable<Trade> Trades { get; }

        void CloseMarket();
        void OpenMarket();
        Task<long> EnqueueOrderAsync(TradeSide tradeSide, decimal quantity, decimal price);
        Task<long> CancelOrderAsync(long orderId);
        Task<long> ModifyOrderAsync(long orderId, TradeSide tradeSide, decimal quantity, decimal price);
    }
}