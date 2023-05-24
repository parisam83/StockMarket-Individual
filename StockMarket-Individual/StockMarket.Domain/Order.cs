namespace StockMarket.Domain
{
    public class Order
    {
        private long id;
        private TradeSide tradeSide;
        private decimal quantity;
        private decimal price;
        private bool isCanceled;

        internal Order(long id, TradeSide tradeSide, decimal quantity, decimal price)
        {
            this.id = id;
            this.tradeSide = tradeSide;
            this.quantity = quantity;
            this.price = price;
            IsCanceled = false;
        }

        internal void DecreaseQuantity(decimal decreaseAmount)
        {
            quantity -= decreaseAmount;
        }

        internal void Cancel()
        {
            IsCanceled = true;
        }

        public long Id { get => id; }
        public decimal Price { get => price; }
        public decimal Quantity { get => quantity; private set => quantity = value; }
        public TradeSide TradeSide { get => tradeSide; }
        public bool IsCanceled { get => isCanceled; set => isCanceled = value; }
    }
}