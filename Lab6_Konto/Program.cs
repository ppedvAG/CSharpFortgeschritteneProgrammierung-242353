namespace Lab_Konto
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            CreateThreads<KontoWithLock>(15);

            CreateThreads<KontoWithMutex>(15);

            Console.WriteLine("Fertig");
            Console.Read();
        }

        private static void CreateThreads<T>(int count) where T : IKonto, new()
        {
            var konto = new T() { Type = typeof(T).Name };
            for (int i = 0; i < count; i++)
            {
                new Thread(() => Run(konto)).Start();
            }
        }

        static void Run(object arg) //Random Einzahlungen und Auszahlungen ausführen
        {
            var konto = (IKonto)arg;
            for (int i = 0; i < 10; i++)
            {
                int betrag = Random.Shared.Next(0, 10) * 10;

                if (Random.Shared.Next() % 2 == 0)
                    konto.Deposite(betrag);
                else
                    konto.Disburse(betrag);

                Thread.Sleep(100);
            }
        }
    }
}