using System.Diagnostics;

namespace StockMarket.Domain
{
    public class Order
    {
        public long Id { get; }
        public decimal Price { get; }
        public decimal Quantity { get; private set; }
        public TradeSide TradeSide { get; }
        public bool IsCanceled { get; private set; }

        internal Order(long id, TradeSide tradeSide, decimal quantity, decimal price)
        {
            Id = id;
            TradeSide = tradeSide;
            Quantity = quantity;
            Price = price;
            IsCanceled = false;
        }

        internal void DecreaseQuantity(decimal decreaseAmount)
        {
            Quantity -= decreaseAmount;
        }

        internal void Cancel()
        {
            IsCanceled = true;
        }
    }
}