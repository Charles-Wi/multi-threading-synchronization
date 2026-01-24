using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncObjects;

internal class ManualResetEventSlimExample
{
    static ManualResetEventSlim mrsToControlExampleFlow = new ManualResetEventSlim(false);

    public static void Show()
    {
        SlimVersion();
        ClassicVersion();
    }

    private static void SlimVersion()
    {
        Console.WriteLine("==============ManualResetEventSlim=================");

        ManualResetEventSlim mres = new ManualResetEventSlim(false);

        long total = 0;
        int COUNT = 10;

        for (int i = 0; i < COUNT; i++)
        {
            mrsToControlExampleFlow.Reset();
            //счётчик затраченного времени
            Stopwatch sw = Stopwatch.StartNew();

            //запускаем установку в потоке пула
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Method(mres, true);
                mrsToControlExampleFlow.Set();
            });
            //запускаем сброс в основном потоке
            Method(mres, false);

            //Ждём, пока выполнится поток пула
            mrsToControlExampleFlow.Wait();
            sw.Stop();

            Console.WriteLine($"Pass {i}: {sw.ElapsedMilliseconds} ms");
            total += sw.ElapsedMilliseconds;
        }

        Console.WriteLine();
        Console.WriteLine("===============================");
        Console.WriteLine("Done in average=" + total / (double)COUNT);
        Console.WriteLine();
        // Console.ReadLine();
    }

    private static void ClassicVersion()
    {
        Console.WriteLine("==============ManualResetEvent=================");

        ManualResetEvent mre = new ManualResetEvent(false);

        long total = 0;
        int COUNT = 10;

        for (int i = 0; i < COUNT; i++)
        {
            mrsToControlExampleFlow.Reset();
            //счётчик затраченного времени
            Stopwatch sw = Stopwatch.StartNew();

            //запускаем установку в потоке пула
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Method2(mre, true);
                mrsToControlExampleFlow.Set();
            });
            //запускаем сброс в основном потоке
            Method2(mre, false);

            //Ждём, пока выполнится поток пула
            mrsToControlExampleFlow.Wait();
            sw.Stop();

            Console.WriteLine("Pass {0}: {1} ms", i, sw.ElapsedMilliseconds);
            total += sw.ElapsedMilliseconds;
        }

        Console.WriteLine();
        Console.WriteLine("===============================");
        Console.WriteLine("Done in average=" + total / (double)COUNT);
        Console.ReadLine();
    }


    // работаем с ManualResetEventSlim
    private static void Method(ManualResetEventSlim mre, bool value)
    {
        //в цикле повторяем действие достаточно большое число раз
        for (int i = 0; i < 9_000_000; i++)
        {
            if (value)
            {
                mre.Set();
            }
            else
            {
                mre.Reset();
            }
        }
    }

    // работаем с классическим ManualResetEvent
    private static void Method2(ManualResetEvent mre, bool value)
    {
        //в цикле повторяем действие достаточно большое число раз
        for (int i = 0; i < 9_000_000; i++)
        {
            if (value)
            {
                mre.Set();
            }
            else
            {
                mre.Reset();
            }
        }
    }
}
