using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncObjects
{
    class BlockingCollectionExample
    {
        static BlockingCollection<Func<(bool,string)>> actions = new();

        public static void ShowTest()
        {
            Barrier barrier = new Barrier(2);

            var thread = new Thread(() =>
            {
                bool stop = false;
                string message = "";
                string name = "Second Thread";

                while (!stop)
                {
                    (stop, message) = actions.Take()();

                    Console.WriteLine("The {0} receive: {1} Ok.", name, message);
                }

                Console.WriteLine("The {0} wait Barrier for finish.", name);

                barrier.SignalAndWait();

                Console.WriteLine("The {0} is Completed....", name);
            });

            thread.Start();

            actions.Add(() =>
            {
                bool stop = false;
                string message = "A little bit of information.";
                return (stop, message);
            });

            actions.Add(() =>
            {
                bool stop = true;
                string message = "Finish, please.";
                return (stop, message);
            });

            Console.WriteLine("Main thread wait Barrier for finish.");
            barrier.SignalAndWait();

            Console.WriteLine("Completed....");
        }

    }
}
