using System.Diagnostics;

namespace StockMarket.Domain
{
    public class Trade
    {
        public long Id { get; }
        public long BuyOrderId { get; }
        public long SellOrderId { get; }
        public decimal Quantity { get; }
        public decimal Price { get; }

        internal Trade(long id, long buyOrderId, long sellOrderId, decimal quantity, decimal price)
        {
            Id = id;
            BuyOrderId = buyOrderId;
            SellOrderId = sellOrderId;
            Quantity = quantity;
            Price = price;
        }
    }
}