namespace TPL_Parallel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Parallel.For(0, 100_000, i => Console.WriteLine($"Main Thread {Thread.CurrentThread.ManagedThreadId}: " + i));

            Parallel.Invoke(() => Count(),
                () => Console.Beep(),
                Count, Count, Count, Count);

            Console.ReadKey();
        }

        static void Count()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Side Thread {Thread.CurrentThread.ManagedThreadId}: " + i);
            }
        }
    }
}
