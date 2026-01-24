namespace SyncObjects;

internal class MutexExample
{
    int x = 0;
    Mutex mutexObj = new(false, "TestExample2026");
    public CountdownEvent CountdownForWaitEndShowExample { get; set; } = new CountdownEvent(5);
    public CountdownEvent CountdownForStartTryingEnterCriticalSectionAllThreadTogether { get; set; } = new CountdownEvent(5);

    public void Show()
    {
        // запускаем пять потоков
        for (int i = 1; i < 6; i++)
        {
            Thread myThread = new(Print);
            myThread.Name = $"Поток {i}";
            myThread.Start();
            CountdownForStartTryingEnterCriticalSectionAllThreadTogether.Signal();
        }
    }

    ~MutexExample()
    {
        mutexObj?.Dispose();
    }

    void Print()
    {
        CountdownForStartTryingEnterCriticalSectionAllThreadTogether.Wait();
        mutexObj.WaitOne();     // приостанавливаем поток до получения мьютекса
        x = 0;
        for (int i = 1; i < 6; i++)
        {           
            x++;
            Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
            Thread.SpinWait((int)5e5);
        }
        mutexObj.ReleaseMutex();    // освобождаем мьютекс

        CountdownForWaitEndShowExample.Signal();
    }
}
