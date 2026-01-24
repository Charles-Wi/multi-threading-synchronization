using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncObjects
{
    public static class SemaphoreSlimForBoundedBufferExample
    {
        public static async Task ShowExample()
        {
            try
            {
                var buffer = new BoundedBuffer<int>(5); // capacity = 5
                using var cts = new CancellationTokenSource();

                var producers = new[]
                {
                   Producer(buffer, 1),
                   Producer(buffer, 2)
                };

                var consumers = new[]
                {
                   Consumer(buffer, 1, cts.Token),
                   Consumer(buffer, 2, cts.Token)
                };

                await Task.WhenAll(producers);

                // give consumers time to finish work
                await Task.Delay(5000);
                cts.Cancel();

                await Task.WhenAll(consumers);


            }
            catch (Exception ex) { }
            finally { Console.WriteLine("All done."); }

        }

        static async Task Producer(BoundedBuffer<int> buffer, int id)
        {
            for (int i = 1; i <= 10; i++)
            {
                await buffer.ProduceAsync(id * 100 + i);
                await Task.Delay(200); // simulate variable production time
            }
        }

        static async Task Consumer(BoundedBuffer<int> buffer, int id, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    int item = await buffer.ConsumeAsync(token);
                    Console.WriteLine($"\tConsumer {id} processed {item}");
                    await Task.Delay(500); // simulate variable consumption time
                }
            }
            catch (OperationCanceledException)
            {
                // gracefully exit
                throw;
            }
        }
    }
}
