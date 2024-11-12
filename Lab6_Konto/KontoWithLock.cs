namespace Lab_Konto
{
    public class KontoWithLock : IKonto
    {
        public string Type { get; init; }
        public int Balance { get; set; } = 0;
        public int TransactionCount { get; set; } = 0;

        private static object LockFlagEinzahlen = new object();
        private static object LockFlagAuszahlen = new object();

        public void Deposite(int value)
        {
            try
            {
                //Variablen werden gesperrt wenn Thread drauf zugreifen möchte
                //Threads die auf gelockte Blöcke zugreifen wollen müssen warten
                lock (LockFlagEinzahlen)
                {
                    Balance += value;
                    TransactionCount++;
                    Console.WriteLine($"Kontostand ({Type}): {Balance}");
                }
            }
            catch (Exception) //Wenn 2 Threads zum genau gleichen Zeitpunkt zugreifen wollen
            {
                Console.WriteLine("Deadlock");
            }
        }

        public void Disburse(int value)
        {
            try
            {
                lock (LockFlagAuszahlen)
                {
                    Balance -= value;
                    TransactionCount++;
                    Console.WriteLine($"Kontostand ({Type}): {Balance}");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Deadlock");
            }
        }
    }
}