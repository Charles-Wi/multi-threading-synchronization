using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SyncObjects
{
    public enum ParamsEnum
    {
        Param1,
        Param2,
        Param3
    }

    public class SemaphoreSlimExample : IDisposable
    {
        private readonly ConcurrentQueue<(ParamsEnum paramCode, string paramValue)> _eventQueue =
            new ConcurrentQueue<(ParamsEnum, string)>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _processingTask;
        private bool _disposed;

        public event Action<ParamsEnum, string> SomethingHappen;

        public SemaphoreSlimExample()
        {
            _processingTask = Task.Run(() => ProcessEventsAsync(_cts.Token));
        }

        public void AddParamValue(ParamsEnum paramCode, string paramValues)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SemaphoreSlimExample));

            _eventQueue.Enqueue((paramCode, paramValues));
            _signal.Release();
        }

        private async Task ProcessEventsAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(ct).ConfigureAwait(false);

                    // Process all available items in the queue
                    while (_eventQueue.TryDequeue(out var item))
                    {
                        try
                        {
                            SomethingHappen?.Invoke(item.paramCode, item.paramValue);
                        }
                        catch (Exception ex)
                        {
                            // Log exception or handle it appropriately
                            // Prevents one handler exception from stopping the entire queue
                            Console.WriteLine($"Error processing event: {ex.Message}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected during cancellation, exit gracefully
                    break;
                }
            }

            // Process remaining items in queue during shutdown
            while (_eventQueue.TryDequeue(out var item))
            {
                try
                {
                    SomethingHappen?.Invoke(item.paramCode, item.paramValue);
                }
                catch
                {
                    // Suppress exceptions during shutdown
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            // Signal cancellation
            _cts.Cancel();

            // Give processing task a moment to complete gracefully
            if (!_processingTask.Wait(TimeSpan.FromSeconds(5)))
            {
                // Log warning: processing task did not complete in time
            }

            _signal.Dispose();
            _cts.Dispose();
        }
    }
}
