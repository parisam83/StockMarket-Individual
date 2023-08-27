using StockMarket.Domain.States;
using StockMarket.Domain.Comparer;
using System.Drawing;

namespace StockMarket.Domain
{
    public class StockMarketProcessor : IStockMarketProccessor
    {
        private long lastOrderId;
        private long lastTradeId;
        private readonly List<Order> orders;
        private readonly List<Trade> trades;
        private readonly PriorityQueue<Order, Order> buyOrders;
        private readonly PriorityQueue<Order, Order> sellOrders;
        public MarketState state;

        public IEnumerable<Order> Orders => orders;
        public IEnumerable<Trade> Trades => trades;

        public StockMarketProcessor(List<Order>? orders = null, long lastOrderId = 0, long lastTradeId = 0)
        {
            state = new CloseState(this);
            this.lastOrderId = lastOrderId;
            this.lastTradeId = lastTradeId;
            this.orders = orders ?? new();  // if (orders == null) this.orders = new();
            trades = new();
            buyOrders = new(new MaxComparer());
            sellOrders = new(new MinComparer());

            foreach (var order in this.orders)
                enqueueOrder(order);
        }

        internal long Enqueue(TradeSide tradeSide, decimal quantity, decimal price) 
        {
            var order = makeOrder(tradeSide, quantity, price);
            enqueueOrder(order);
            return order.Id;
        }
        public long EnqueueOrder(TradeSide tradeSide, decimal quantity, decimal price)
        {
            return state.EnqueueOrder(tradeSide, quantity, price);
        }

        internal long Cancel(long orderId) 
        {
            var order = orders.Single(order => order.Id == orderId);
            order.Cancel();
            return order.Id;
        }
        public long? CancelOrder(long orderId)
        {
            return state.CancelOrder(orderId);
        }

        internal void Close() 
        {
            state = new CloseState(this);
        }
        public void CloseMarket()
        {
            state.CloseMarket();
        }

        internal void Open()
        {
            state = new OpenState(this);
        }
        public void OpenMarket()
        {
            state.OpenMarket();
        }

        internal long Modify(long orderId, TradeSide tradeSide, decimal quantity, decimal price) 
        {
            Cancel(orderId);
            return EnqueueOrder(tradeSide, quantity, price);
            
        }
        public long ModifyOrder(long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            return state.ModifyOrder(orderId, tradeSide, quantity, price);
        }
        private Order makeOrder(TradeSide tradeSide, decimal quantity, decimal price)
        {
            Interlocked.Increment(ref lastOrderId);
            var order = new Order(lastOrderId, tradeSide, quantity, price);
            orders.Add(order);
            return order;
        }

        private void enqueueOrder(Order order)
        {
            if (order.TradeSide == TradeSide.Buy) 
                processOrder(order: order, 
                    orders: buyOrders,
                    matchingOrders: sellOrders,
                    comparePriceDeligate: (decimal price1, decimal price2) => price1 >= price2);
            else 
                processOrder(order: order, 
                    orders : sellOrders, 
                    matchingOrders: buyOrders,
                    comparePriceDeligate: (decimal price1, decimal price2) => price1 <= price2);
        }
        private void processOrder(Order order, PriorityQueue<Order, Order> orders, PriorityQueue<Order, Order> matchingOrders, Func<decimal, decimal, bool> comparePriceDeligate) 
        {
            while (matchingOrders.Count > 0 && order.Quantity > 0 && comparePriceDeligate(order.Price, matchingOrders.Peek().Price))
            {
                var peekedOrder = matchingOrders.Peek();
                if (peekedOrder.IsCanceled)
                {
                    matchingOrders.Dequeue();
                    continue;
                }
                makeTrade(order, peekedOrder);
                if (peekedOrder.Quantity == 0) matchingOrders.Dequeue();
            }

            if (order.Quantity > 0) orders.Enqueue(order, order);
        }

        private void makeTrade(Order order1, Order order2)
        {
            (var buyOrder, var sellOrder) = FindOrders(order1, order2);

            decimal minQuantity = Math.Min(sellOrder.Quantity, buyOrder.Quantity);

            Interlocked.Increment(ref lastTradeId);
            var trade = new Trade(lastTradeId, buyOrder.Id, sellOrder.Id, minQuantity, sellOrder.Price);
            trades.Add(trade);

            buyOrder.DecreaseQuantity(minQuantity);
            sellOrder.DecreaseQuantity(minQuantity);
        }

        private static (Order BuyOrder, Order SellOrder) FindOrders(Order order1, Order order2)
        {
            if (order1.TradeSide == TradeSide.Buy) return (BuyOrder: order1, SellOrder: order2);
            else return (BuyOrder: order2, SellOrder: order1);
        }
    }
}