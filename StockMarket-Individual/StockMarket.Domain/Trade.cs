namespace StockMarket.Domain
{
    public class Trade
    {
        private long id;
        private long buyOrderId;
        private long sellOrderId;
        private decimal quantity;
        private decimal price;

        internal Trade(long id, long buyOrderId, long sellOrderId, decimal quantity, decimal price)
        {
            this.id = id;
            this.buyOrderId = buyOrderId;
            this.sellOrderId = sellOrderId;
            this.quantity = quantity;
            this.price = price;
        }

        public long Id { get => id; }
        public long BuyOrderId { get => buyOrderId; }
        public long SellOrderId { get => sellOrderId; }
        public decimal Quantity { get => quantity; }
        public decimal Price { get => price; }
    }
}