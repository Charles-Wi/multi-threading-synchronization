using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SyncObjects
{
    public class SpinLockExample
    {
        const int N = 5_000;
        static Queue<Data> _queue = new Queue<Data>();
        static Lock _lock = new();
        static SpinLock _spinlock = new SpinLock();
        static int numberOfThreads = 400;
        private Action[] actionsWithSpinLock = new Action[numberOfThreads];
        private Action[] actionsWithLock = new Action[numberOfThreads];


        class Data
        {
            public string Name { get; set; }
            public double Number { get; set; }
        }
        public void Show()
        {
            for (int i = 0; i < numberOfThreads; i++) 
            {
                actionsWithSpinLock[i] = new Action(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        UpdateWithSpinLock(new Data() { Name = i.ToString(), Number = i });
                    }
                });
                actionsWithLock[i] = new Action(() =>
                {
                    for (int i = 0; i < N; i++)
                    {
                        UpdateWithLock(new Data() { Name = i.ToString(), Number = i });
                    }
                });
            }

            // First use a standard lock for comparison purposes.
            UseLock();
            _queue.Clear();
            UseSpinLock();

            //Console.WriteLine("Press a key");
            //Console.ReadKey();
        }

        

        private static void UpdateWithSpinLock(Data d)
        {
            bool lockTaken = false;
            
            try
            {
                _spinlock.Enter(ref lockTaken);
                _queue.Enqueue(d);
            }
            finally
            {
                if (lockTaken)
                {
                    //lockTaken = false;
                    _spinlock.Exit(false);
                }
            }
        }

        private  void UseSpinLock()
        {

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(actionsWithSpinLock);
            sw.Stop();
            Console.WriteLine("elapsed ms with spinlock: {0}", sw.ElapsedMilliseconds);
        }

        private void UpdateWithLock(Data d)
        {
            lock (_lock)
            {
                _queue.Enqueue(d);
            }
        }

        private  void UseLock()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(actionsWithLock);
            sw.Stop();
            Console.WriteLine("elapsed ms with lock: {0}", sw.ElapsedMilliseconds);
        }


    }

    public static class SharedResource
    {
        public static Queue<int> intsQ = new Queue<int>();
    }
}
