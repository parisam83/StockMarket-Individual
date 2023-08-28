namespace StockMarket.Domain.Commands
{
    internal class CancelCommand : BaseCommand
    {
        private StockMarketProcessor stockMarketProcessor;
        private long orderId;

        public CancelCommand(StockMarketProcessor stockMarketProcessor, long orderId)
        {
            this.stockMarketProcessor = stockMarketProcessor;
            this.orderId = orderId;
        }

        protected override long SpecificExecute()
        {
            return stockMarketProcessor.Cancel(orderId);
        }
    }
}