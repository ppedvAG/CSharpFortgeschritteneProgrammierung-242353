namespace Multithreading
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Thread starten
            //ThreadStarten();

            // Parameter uebergeben
            //ThreadMitParametern();

            // Thread beenden
            //ThreadMitCancellationToken();

            // Thread Pool
            ThreadImHintergrund();
        }

        private static void ThreadStarten()
        {
            Thread t1 = new Thread(RunThread);
            t1.Start();

            Thread t2 = new Thread(RunThread);
            t2.Start();

            Thread t3 = new Thread(RunThread);
            t3.Start();

            // Ab hier parallel

            // Auf Threads warten
            t1.Join(); // Hier auf t1 warten, t2 und t3 laufen weiter
            t2.Join(); // Hier auf t2 warten, t3 laeuft weiter
            t3.Join(); // Hier auf t3 warten


            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Main Thread {Thread.CurrentThread.ManagedThreadId}: {i}");
            }
        }

        static void RunThread()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Side Thread {Thread.CurrentThread.ManagedThreadId}: " + i);
            }
        }

        private static void ThreadMitParametern()
        {
            var t1 = new Thread(RunThreadWithParameter);
            t1.Start(42);

            object result = null;
            var t2 = new Thread(RunThreadWithCallback);
            t2.Start((object r) => { result = r; });
        }

        static void RunThreadWithParameter(object param)
        {
            if (param is int n)
            {
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine($"Side Thread {Thread.CurrentThread.ManagedThreadId}: " + i);
                }
            }
        }

        static void RunThreadWithCallback(object callback)
        {
            if (callback is Delegate cb)
            {
                Thread.Sleep(1000);

                // Wir benutzen DynamicInvoke da wir die Methoden-Signatur an dieser Stelle nicht kennen
                cb.DynamicInvoke(37);
            }
        }

        private static void ThreadMitCancellationToken()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token; // Ein struct. Die Source kann beliebig viele Tokens erzeugen
            var t1 = new Thread(RunThreadWithCancellationToken);

            try
            {
                t1.Start(token);

                Thread.Sleep(200);

                cts.Cancel(); // Auf der Source das Cancel Signal senden
            }
            catch (OperationCanceledException)
            {
                // Exception kann nicht hier oben gefangen werden
            }
        }

        private static void RunThreadWithCancellationToken(object? obj)
        {
            try
            {
                if (obj is CancellationToken token)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(20);

                        Console.WriteLine($"Side Thread {Thread.CurrentThread.ManagedThreadId}: " + i);

                        if (token.IsCancellationRequested)
                        {
                            Console.WriteLine("Cancellation Requested");
                            // Hier werfen wir die OperationCanceledException
                            token.ThrowIfCancellationRequested();
                        }
                    }
                }
            } 
            catch(OperationCanceledException)
            {
                Console.WriteLine("Exception im Thread gefangen");
            }
        }

        private static void ThreadImHintergrund()
        {
            ThreadPool.QueueUserWorkItem(RunThreadWithCallback);
            ThreadPool.QueueUserWorkItem(RunThreadWithCallback);
            ThreadPool.QueueUserWorkItem(RunThreadWithCallback);

            // Vordergrund- vs. Hintergrundthreads
            var t1 = new Thread(RunThread);
            t1.Start(); // Vordergrundthread

            // Alle Items im Thread Pool abgebrochen werden wenn alle Vordergrundthreads fertig sind

            Thread.Sleep(1000);
        }
    }
}
