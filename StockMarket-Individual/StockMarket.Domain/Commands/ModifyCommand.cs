namespace StockMarket.Domain.Commands
{
    internal class ModifyCommand : BaseCommand
    {
        private StockMarketProcessor stockMarketProcessor;
        private long orderId;
        private TradeSide tradeSide;
        private decimal quantity;
        private decimal price;

        public ModifyCommand(StockMarketProcessor stockMarketProcessor, long orderId, TradeSide tradeSide, decimal quantity, decimal price)
        {
            this.stockMarketProcessor = stockMarketProcessor;
            this.orderId = orderId;
            this.tradeSide = tradeSide;
            this.quantity = quantity;
            this.price = price;
        }

        protected override long SpecificExecute()
        {
            return stockMarketProcessor.Modify(orderId, tradeSide, quantity, price);
        }
    }
}