using System.Collections.Concurrent;
using StockMarket.Domain.Commands;

namespace StockMarket.Domain
{
    internal class StockMarketQueue : IAsyncDisposable
    {
        private readonly BlockingCollection<BaseCommand> queue;
        private readonly Task consumerTask;
        public StockMarketQueue()
        {
            queue = new();
            consumerTask = Task.Run(() =>
            {
                while (!queue.IsAddingCompleted || queue.Count > 0)
                {
                    try
                    {
                        var command = queue.Take();
                        command.Execute();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
        }

        internal async Task<long> ExecuteAsync(BaseCommand command)
        {
            queue.Add(command);
            return await command.WaitForCompletionAsync();
        }

        public async ValueTask DisposeAsync()
        {
            queue.CompleteAdding();
            await consumerTask;
        }
    }
}