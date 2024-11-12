namespace Multitasking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //CreateTaskSamples();

            //CreateTaskWaitSample();

            //CreateTaskCancellationToken();

            TaskExceptionHandlingSample();

            Console.WriteLine("\n\nPress any key to exit");
            Console.ReadKey();
        }

        #region Task erstellen

        private static void CreateTaskSamples()
        {
            var task = new Task(DoSomething);
            task.Start();

            // ab .Net 4.0
            Task.Factory.StartNew(DoSomething);

            // ab .Net 4.5
            Task.Run(DoSomething);

            var taskWithArg = new Task(DoSomething, 42);
            taskWithArg.Start();

            var taskWithReturnValue = new Task<int>(CreateRandomNumber);
            taskWithReturnValue.Start();

            if (taskWithReturnValue.IsCompleted)
            {
                Console.WriteLine($"Return value is {taskWithReturnValue.Result}");
            }
        }

        private static void DoSomething()
        {
            DoSomething(100);
        }

        private static void DoSomething(object max)
        {
            Thread.Sleep(1000);

            int number = Random.Shared.Next(0, (int)max);
            Console.WriteLine($"Random number {number} from thread {Thread.CurrentThread.ManagedThreadId}");
        }

        private static int CreateRandomNumber()
        {
            Thread.Sleep(1000);
            int number = Random.Shared.Next();
            return number;
        }

        #endregion

        #region Task Wait All/Any

        private static void CreateTaskWaitSample()
        {
            // Auf einen Task warten
            var task = new Task(() => Thread.Sleep(1000));
            task.Start();

            // Warte hier auf den Task und blockiere den Main Thread
            task.Wait();

            // Auf mehrere Tasks warten
            var someTasks = CreateTasks((_) => Thread.Sleep(1000));

            // Warten bis alle Tasks abgearbeitet wurden
            Task.WaitAll(someTasks.ToArray());

            // Warten bis mindestens ein Task abgearbeitet wurden
            Task.WaitAny(someTasks.ToArray());
        }

        private static IEnumerable<Task> CreateTasks(Action<object?> action, int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Task.Factory.StartNew(action, i);
            }
        }

        #endregion

        #region Task Cancellation

        private static void CreateTaskCancellationToken()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token; // Ein struct. Die Source kann beliebig viele Tokens erzeugen
            var t1 = new Task(RunTaskWithCancellationToken, token);

            t1.Start();

            Thread.Sleep(500);

            cts.Cancel(); // Auf der Source das Cancel Signal senden
        }

        private static void RunTaskWithCancellationToken(object arg)
        {
            if (arg is CancellationToken token)
            {
                for (int i = 0; i < 20; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine($"Running task {i} wit id {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(100);
                }
            }
        }


        #endregion

        #region Task AggregateException

        private static void TaskExceptionHandlingSample()
        {
            try
            {
                var tasks = CreateTasks((i) => 
                { 
                    Thread.Sleep(200);
                    throw new NotImplementedException("Not implemented #" + i);
                }, 3);

                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException aggregates)
            {
                foreach (var ex in aggregates.InnerExceptions)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        #endregion
    }
}
