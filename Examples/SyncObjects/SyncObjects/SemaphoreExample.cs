using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncObjects
{
    internal class SemaphoreExample
    {
        public CountdownEvent CountdownForWaitEndShowExample { get; set; } = new CountdownEvent(5);

        public void Show()
        {
            // запускаем пять потоков
            for (int i = 1; i < 6; i++)
            {
                Reader reader = new Reader(i, CountdownForWaitEndShowExample);
            }
        }
    }

    /// <summary>
    /// В библиотеке есть место только для трех читателей
    /// </summary>
    class Reader
    {
        // создаем семафор
        static Semaphore _sem = new (3, 3);
        Thread _myThread;
        int _count = 3;// количество обязательных посещений библиотеки
        CountdownEvent _countdownForServiceEvent;
        readonly int _waitIterations = (int)1e6;
        static long _numberOfReadersInLibray = 0; // количество читателей в библиотеке

        public Reader(int i, CountdownEvent countdownEvent)
        {
            _countdownForServiceEvent = countdownEvent;
            _myThread = new Thread(Read);
            _myThread.Name = $"Читатель {i}";
            _myThread.Start();
        }

        public void Read()
        {
            while (_count > 0)
            {
                _sem.WaitOne();  // ожидаем, когда освободиться место

                Interlocked.Increment(ref _numberOfReadersInLibray); // увеличиваем количество читателей в библиотеке

                Console.WriteLine($"{Thread.CurrentThread.Name} входит в библиотеку ({Interlocked.Read(ref _numberOfReadersInLibray)})");

                Console.WriteLine($"{Thread.CurrentThread.Name} читает ({Interlocked.Read(ref _numberOfReadersInLibray)})");
                Thread.SpinWait(_waitIterations);

                Console.WriteLine($"{Thread.CurrentThread.Name} покидает библиотеку ({Interlocked.Read(ref _numberOfReadersInLibray)})");

                Interlocked.Decrement(ref _numberOfReadersInLibray); // уменьшаем количество читателей в библиотеке

                _sem.Release();  // освобождаем место                

                _count--;
                Thread.SpinWait(_waitIterations);
            }

            _countdownForServiceEvent.Signal();
        }
    }

}
