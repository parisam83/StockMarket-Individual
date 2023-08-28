using System.Drawing;

namespace StockMarket.Domain.Commands
{
    internal class EnqueueCommand : BaseCommand
    {
        private StockMarketProcessor stockMarketProcessor;
        private readonly TradeSide tradeSide;
        private readonly decimal quantity;
        private readonly decimal price;

        internal EnqueueCommand(StockMarketProcessor stockMarketProcessor, TradeSide tradeSide, decimal quantity, decimal price)
        {
            this.stockMarketProcessor = stockMarketProcessor;
            this.tradeSide = tradeSide;
            this.quantity = quantity;
            this.price = price;
        }

        protected override long SpecificExecute()
        {
            return stockMarketProcessor.Enqueue(tradeSide, quantity, price);
        }
    }
}