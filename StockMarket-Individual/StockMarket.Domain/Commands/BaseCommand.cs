using System.Diagnostics;
using System.Drawing;

namespace StockMarket.Domain.Commands
{
    internal abstract class BaseCommand
    {
        private readonly TaskCompletionSource<long> completion;

        internal BaseCommand()
        {
            completion = new();
        }
        internal void Execute()
        {
            var orderId = SpecificExecute();
            completion.SetResult(orderId);
        }

        protected abstract long SpecificExecute();

        internal Task<long> WaitForCompletionAsync()
        {
            return completion.Task;
        }
    }
}