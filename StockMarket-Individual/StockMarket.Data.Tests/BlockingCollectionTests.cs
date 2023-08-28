using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace StockMarket.Data.Tests
{
    public class BlockingCollectionTests
    {
        [Fact]
        public async Task BlockingCollection_Add_And_Take_Test_Async()
        {
            // Arrange
            var queue = new BlockingCollection<int>();
            var sum = 0;

            // Act
            var producer = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    queue.Add(i);
                }
                queue.CompleteAdding();
            });

            var consumer = Task.Run(() =>
            {
                while (!queue.IsAddingCompleted || queue.Count > 0)
                {
                    try
                    {
                        var item = queue.Take();
                        sum += item;
                    } 
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });

            await Task.WhenAll(consumer, producer);

            // Assert
            Assert.Equal(45, sum);
        }

        public struct QueueItem
        {
            public int Data { get; private set; }
            public TaskCompletionSource<int> Completion { get; }
            
            public QueueItem(int data)
            {
                Data = data;
                Completion = new TaskCompletionSource<int>();
            }

            internal void IncreaseData(int amount)
            {
                Data += amount;
            }
        }
        [Fact]
        public async Task BlockingCollection_With_TaskCompletionSource_Test_Async()
        {
            // Arrange
            var queue = new BlockingCollection<QueueItem>();
            var producers = new Task[10];
            var sum = 0;
            var queueItemData = -1;

            // Act 
            for (int i = 0; i < 10; i++)
            {
                producers[i] = Task.Run(async () =>
                {
                    var item = new QueueItem(Interlocked.Increment(ref queueItemData));
                    queue.Add(item);

                    if (queueItemData == 9) queue.CompleteAdding();
                    var result = await item.Completion.Task;
                    Interlocked.Add(ref sum, result);
                });
            }

            var consumer = Task.Run(() =>
            {
                while (!queue.IsAddingCompleted || queue.Count > 0)
                {
                    try
                    {
                        var item = queue.Take();
                        item.IncreaseData(1);
                        item.Completion.SetResult(item.Data);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
            await Task.WhenAll(producers);
            await consumer;

            // Assert
            Assert.Equal(55, sum);
        }
    }
}
