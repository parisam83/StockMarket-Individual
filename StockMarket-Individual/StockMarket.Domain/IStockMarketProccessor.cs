namespace StockMarket.Domain
{
    public interface IStockMarketProccessor
    {
        IEnumerable<Order> Orders { get; }
        IEnumerable<Trade> Trades { get; }

        void CloseMarket();
        void OpenMarket();
        long EnqueueOrder(TradeSide tradeSide, decimal quantity, decimal price);
        long? CancelOrder(long orderId);
        long ModifyOrder(long orderId, TradeSide tradeSide, decimal quantity, decimal price);
    }
}