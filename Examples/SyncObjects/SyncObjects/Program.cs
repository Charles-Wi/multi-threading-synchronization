
using SyncObjects;

Console.WriteLine("Monitor example");
var mo = new MonitorExample();
mo.Show();
mo.CountdownForWaitEndShowExample.Wait();

//Console.WriteLine("Mutex example");
//var me = new MutexExample();
//me.Show();
//me.CountdownForWaitEndShowExample.Wait();

//SyncBlockingIndexExample.SyncBlockIndexExample();


//Console.WriteLine("MutexStartOnlyOneInstanceOfProgramExample example");
//MutexStartOnlyOneInstanceOfProgramExample.Check();

//Console.WriteLine("Semaphore example");
//var sem = new SemaphoreExample();
//sem.Show();
//sem.CountdownForWaitEndShowExample.Wait();

//Console.WriteLine("SemaphoreSlimExample example");
//var sem = new SemaphoreSlimExample();
//sem.SomethingHappen += new Action<ParamsEnum, string>((paramCode, paramValue) =>
//{
//    Console.WriteLine("Item {0}: {1} is received.", paramCode, paramValue);
//});
//sem.AddParamValue(ParamsEnum.Param1, "One");
//sem.AddParamValue(ParamsEnum.Param2, "Two");
//sem.AddParamValue(ParamsEnum.Param3, "Three");

//Console.WriteLine("ReaderWriterLock example");
//ReaderWriterLockExample.Show();

//Console.WriteLine("ReaderWriterLockSlim example");
//ReaderWriterLockSlimExample.Show();

//await SemaphoreSlimForBoundedBufferExample.ShowExample();
//Console.WriteLine("Press Enter to continue");
//Console.ReadLine();

//Console.WriteLine("Difference between Lock and SpinLock example");
//for (int i = 0; i < 10; i++)
//{
//    Console.WriteLine($"attempt #{i + 1} ----------");
//    new SpinLockExample().Show();
//}

//Console.WriteLine("ManualResetEventSlim example");
//ManualResetEventSlimExample.Show();


//BlockingCollectionExample.ShowTest();

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

MutexStartOnlyOneInstanceOfProgramExample.CloseMutex();
