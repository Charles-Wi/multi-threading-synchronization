namespace SyncObjects
{
    public static class MutexStartOnlyOneInstanceOfProgramExample
    {
        public static Mutex mutex = new System.Threading.Mutex(false, "MyUniqueMutexName2026");

        public static void Check()
        {
            
            try
            {
                if (mutex.WaitOne(0, true))
                {
                    // Run the application
                    Console.WriteLine("Application Start");
                }
                else
                {
                    Console.WriteLine("An instance of the application is already running.");
                    //Could close application here
                    PressEnterToContinue();
                    Environment.Exit(0);
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());            
            }
            StopToShowExample();
        }

        private static void PressEnterToContinue()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void StopToShowExample()
        {
            PressEnterToContinue();
            CloseMutex();
        }


        public static void CloseMutex()
        {
            if (mutex != null)
            {
                mutex.Dispose();
                //mutex.Close();
                mutex = null;
            }
        }



    }
}
