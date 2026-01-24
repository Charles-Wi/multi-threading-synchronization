namespace SyncObjects
{
    class BoundedBuffer<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly SemaphoreSlim emptySlots;
        private readonly SemaphoreSlim filledSlots = new SemaphoreSlim(0, int.MaxValue);
        private readonly SemaphoreSlim lockForProd = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim lockForCons = new SemaphoreSlim(1, 1);

        public BoundedBuffer(int capacity)
        {
            emptySlots = new SemaphoreSlim(capacity, capacity);
        }

        public async Task ProduceAsync(T item)
        {
            // wait until there's an empty slot
            await emptySlots.WaitAsync();

            await lockForProd.WaitAsync();
            queue.Enqueue(item);
            lockForProd.Release();
            Console.WriteLine($"Produced: {item}");

            // signal that a new item is available
            filledSlots.Release();
        }

        public async Task<T> ConsumeAsync(CancellationToken cancellationToken)
        {
            // wait until there's something to consume
            await filledSlots.WaitAsync(cancellationToken);

            T item;
            await lockForCons.WaitAsync(cancellationToken);
            item = queue.Dequeue();
            lockForCons.Release();
            Console.WriteLine($"\tConsumed: {item}");

            // signal that a slot is free
            emptySlots.Release();

            return item;
        }
    }
}
