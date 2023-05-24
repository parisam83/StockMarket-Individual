namespace StockMarket.Domain
{
    public class StockMarketProcessor
    {
        private long lastOrderId;
        private readonly List<Order> orders;
        private readonly PriorityQueue<Order, Order> buyOrders;
        private readonly PriorityQueue<Order, Order> sellOrders;

        public StockMarketProcessor(long lastOrderId = 0)
        {
            this.lastOrderId = lastOrderId;
        }

        public long EnqueueOrder(TradeSide tradeSide, decimal quantity, decimal price) 
        {
            Interlocked.Increment(ref lastOrderId);
            var order = new Order(lastOrderId, tradeSide, quantity, price);
            orders.Add(order);

            if (tradeSide == TradeSide.Buy) matchOrder(order, buyOrders, sellOrders, (decimal price1, decimal price2) => price1 >= price2);
            else matchOrder(order, sellOrders, buyOrders, (decimal price1, decimal price2) => price1 <= price2);
            return order.Id;
        }

        private void matchOrder(Order order, PriorityQueue<Order, Order> orders, PriorityQueue<Order, Order> matchingOrders, Func<decimal, decimal, bool> comparePriceDeligate) { }
    }
}