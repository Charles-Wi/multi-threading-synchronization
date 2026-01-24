using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncObjects
{
    public class ReaderWriterLockSlimExample
    {
        static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
        // Define the shared resource protected by the ReaderWriterLock.
        static int resource = 0;
        
        const int numThreads = 16;
        static bool running = true;
        static Lock lockRunning = new();

        // Statistics.
        static int readerTimeouts = 0;
        static int writerTimeouts = 0;
        static int reads = 0;
        static int writes = 0;

        public static void Show()
        {
            // Start a series of threads to randomly read from and
            // write to the shared resource.
            Thread[] t = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                t[i] = new Thread(ThreadProc);
                t[i].Name = new String((char)(i + 65), 1);
                t[i].Start();
                if (i > 10)
                    Thread.SpinWait((int)1e7);
            }


            // Tell the threads to shut down and wait until they all finish.
            lock (lockRunning)
            {
                running = false;
            }

            for (int i = 0; i < numThreads; i++)
                t[i].Join();

            // Display statistics.
            Console.WriteLine("\n{0} reads, {1} writes, {2} reader time-outs, {3} writer time-outs.",
                  reads, writes, readerTimeouts, writerTimeouts);
            Console.Write("Press ENTER to continue... ");
            Console.ReadLine();
        }

        static void ThreadProc()
        {
            static bool testRunning()
            {
                bool result;

                lock (lockRunning)
                {
                    result = running;
                }

                return result;
            }

            Random rnd = new Random();

            // Randomly select a way for the thread to read and write from the shared
            // resource.
            while(testRunning())
            {
                double action = rnd.NextDouble();
                if (action < .4)
                    ReadFromResource(10);
                else if (action < .90)
                    Upgrade(rnd, 100);
                else
                    WriteToResource(rnd, 100);
            }
        }

        // Request and release a reader lock, and handle time-outs.
        static void ReadFromResource(int timeOut)
        {
            if (rwl.TryEnterReadLock(timeOut))
            {
                try
                {
                    // It is safe for this thread to read from the shared resource.
                    Display("reads resource value " + resource);
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ExitReadLock();
                }
            }
            else
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        // Request and release the writer lock, and handle time-outs.
        static void WriteToResource(Random rnd, int timeOut)
        {
            if (rwl.TryEnterWriteLock(timeOut))
            {
                try
                {
                    // It's safe for this thread to access from the shared resource.
                    resource = rnd.Next(500);
                    Display("writes resource value " + resource);
                    Interlocked.Increment(ref writes);
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ExitWriteLock();
                }
            }
            else
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
        }

        // Requests a reader lock, upgrades the reader lock to the writer
        // lock
        static void Upgrade(Random rnd, int timeOut)
        {
            if (rwl.TryEnterUpgradeableReadLock(timeOut))
            {
                try
                {
                    // It's safe for this thread to read from the shared resource.
                    Display("reads resource value " + resource);
                    Interlocked.Increment(ref reads);

                    // To write to the resource, either release the reader lock and
                    // request the writer lock, or upgrade the reader lock. Upgrading
                    // the reader lock puts the thread in the write queue, behind any
                    // other threads that might be waiting for the writer lock.
                    if (rwl.TryEnterWriteLock(timeOut))
                    {
                        try
                        {
                            // It's safe for this thread to read or write from the shared resource.
                            resource = rnd.Next(500);
                            Display("writes resource value ----" + resource);
                            Interlocked.Increment(ref writes);
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            rwl.ExitWriteLock();
                        }
                    }
                    else
                    {
                        // The upgrade request timed out.
                        Interlocked.Increment(ref writerTimeouts);
                    }

                    // If the lock was downgraded, it's still safe to read from the resource.
                    Display("reads resource value ----" + resource);
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ExitUpgradeableReadLock();
                }
            }
            else
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        // Helper method briefly displays the most recent thread action.
        static void Display(string msg)
        {
            Console.WriteLine("Thread {0} {1}.       \r", Thread.CurrentThread.Name, msg);
        }
    }
}
