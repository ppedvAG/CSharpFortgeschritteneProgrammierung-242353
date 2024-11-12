
namespace ThreadSync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Threads werden alle parallel abgearbeitet
            //SpanNewThreads(DoWork);

            // 2. Mit Lock werden die Threads sequentiell abgearbeitet
            //SpanNewThreads(DoWorkUsingLock);

            // 3. Exceptions fangen um andere Threads nicht zu blockeren
            //SpanNewThreads(DoWorkUsingMonitor);

            // 4. Wenn auf ein schreibenden Thread gewartet werden soll
            //SpawnNewThreadsWithManualReset();

            // 5. Wenn schreibende Threads sequentiell abgearbeitet werden sollen
            //SpawnNewThreadsWithAutoReset();

            // 6. Wenn schreibende Threads sequentiell abgearbeitet werden sollen
            //SpawnNewThreadsWithMutex();

            // 7. Wenn mehrere Threads schreiben duerfen, wird eine Semaphore verwendet
            SpawnNewThreadsWithSemaphore();
        }

        private static void SpawnNewThreads(Action action, int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                new Thread(() => action()).Start();
            }
        }

        private static void DoWork()
        {
            Console.WriteLine("Thread {0} started", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1000);
            Console.WriteLine("Thread {0} finished", Thread.CurrentThread.ManagedThreadId);
        }

        private static object _lock = new object();

        private static void DoWorkUsingLock()
        {
            lock (_lock)
            {
                try
                {
                    Console.WriteLine("Thread mit lock {0} started", Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);

                    if (Random.Shared.Next(0, 2) == 0)
                    {
                        throw new Exception();
                    }

                    Console.WriteLine("Thread mit lock {0} finished", Thread.CurrentThread.ManagedThreadId);
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception in lock gefangen");
                }
            }
        }

        private static void DoWorkUsingMonitor()
        {
            // Monitor ist im Gegensatz zu lock eine .NET Klasse und bietet bessere Performance als auch Kontrolle wenn die Synchronisation komplexer wird
            try
            {
                Monitor.Enter(_lock);

                Console.WriteLine("Thread mit Monitor {0} started", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);

                if (Random.Shared.Next(0, 2) == 0)
                {
                    throw new Exception();
                }

                Console.WriteLine("Thread mit Monitor {0} finished", Thread.CurrentThread.ManagedThreadId);
            }
            catch 
            {
                // Exception loggen
                Console.WriteLine("Exception mit Monitor gefangen");
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        #region 4. Manual Reset Event

        static readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false); // false == non-signaled

        private static void SpawnNewThreadsWithManualReset()
        {
            new Thread(WriteSomething).Start();

            SpawnNewThreads(ReadThatSomething);
        }

        private static void ReadThatSomething()
        {
            Console.WriteLine("Thread {0} reading", Thread.CurrentThread.ManagedThreadId);

            // Der lock wird hier sozusagen abgefragt
            _manualResetEvent.WaitOne();

            Thread.Sleep(1000);
            Console.WriteLine("Thread {0} reading completed", Thread.CurrentThread.ManagedThreadId);
        }

        private static void WriteSomething(object? obj)
        {
            Console.WriteLine("Thread {0} writing", Thread.CurrentThread.ManagedThreadId);
            _manualResetEvent.Reset();
            Thread.Sleep(5000);
            Console.WriteLine("Thread {0} writing completed", Thread.CurrentThread.ManagedThreadId);
            _manualResetEvent.Set();
        }

        #endregion

        #region 5. Auto Reset Event

        static readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true); // true damit der erste Thread den Anfang machen kann

        private static void SpawnNewThreadsWithAutoReset()
        {
            SpawnNewThreads(WriteSomethingDifferent, 5);

            // Auf keinen Fall sollte Set() vom Main Thread gesetzt werden
            //Thread.Sleep(5000);
            //_autoResetEvent.Set();
        }

        private static void WriteSomethingDifferent()
        {
            Console.WriteLine("Thread {0} waiting...", Thread.CurrentThread.ManagedThreadId);
            _autoResetEvent.WaitOne();

            Console.WriteLine("Thread {0} writing...", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
            Console.WriteLine("Thread {0} writing completed", Thread.CurrentThread.ManagedThreadId);
            _autoResetEvent.Set();

        }

        #endregion

        #region 6. Mutex
        
        static readonly Mutex _mutex = new Mutex(true); // true damit der erste Thread den Anfang machen kann

        private static void SpawnNewThreadsWithMutex()
        {
            // Unterschied zum AutoResetEvent ist, dass Release eine ApplicationException wirft
            // "Object synchronization method was called from an unsznchronized bock of code"
            SpawnNewThreads(WriteSomethingWithMutex, 5);

            _mutex.ReleaseMutex();
        }

        private static void WriteSomethingWithMutex()
        {
            Console.WriteLine("Thread {0} waiting...", Thread.CurrentThread.ManagedThreadId);
            _mutex.WaitOne();

            Console.WriteLine("Thread {0} writing...", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
            Console.WriteLine("Thread {0} writing completed", Thread.CurrentThread.ManagedThreadId);
            _mutex.ReleaseMutex();
        }

        #endregion

        #region 7. Semaphore

        static readonly Semaphore _semaphore = new Semaphore(initialCount: 1, maximumCount: 3);

        private static void SpawnNewThreadsWithSemaphore()
        {
            SpawnNewThreads(WriteSomethingWithSemaphore, 5);

            // Release() stoert hier nicht
            Thread.Sleep(5000);
            _semaphore.Release();
        }
        private static void WriteSomethingWithSemaphore()
        {
            Console.WriteLine("Thread {0} waiting...", Thread.CurrentThread.ManagedThreadId);
            _semaphore.WaitOne();

            Console.WriteLine("Thread {0} writing...", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
            Console.WriteLine("Thread {0} writing completed", Thread.CurrentThread.ManagedThreadId);
            _semaphore.Release();
        }

        #endregion
    }
}
