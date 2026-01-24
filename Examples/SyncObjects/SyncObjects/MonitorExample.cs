namespace SyncObjects
{


    internal class MonitorExample
    {
        /// <summary>
        /// Shared resource
        /// </summary>
        int x = 0;
        /// <summary>
        /// Lock object
        /// </summary>
        readonly object lockObj = new object();
        /// <summary>
        /// Property for waiting end of show example in Program.cs
        /// </summary>
        public CountdownEvent CountdownForWaitEndShowExample { get; set; } = new CountdownEvent(5);
        /// <summary>
        /// Property for waiting start of trying enter critical section all thread together
        /// </summary>
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


        void Print()
        {
            CountdownForStartTryingEnterCriticalSectionAllThreadTogether.Wait();

            Monitor.Enter(lockObj);    // приостанавливаем поток до освобождения монитором lockObject
            x = 0;  // reset shared resource for every new thread
            for (int i = 1; i < 6; i++)
            {                
                x++; //increment x - change shared resource
                Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
                Thread.SpinWait(1000000);
            }
            Monitor.Exit(lockObj);    // освобождаем  lockObject


            CountdownForWaitEndShowExample.Signal();
        }


    }
}
