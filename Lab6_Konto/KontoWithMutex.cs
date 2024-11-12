namespace Lab_Konto
{
    public class KontoWithMutex : IKonto
    {
        public string Type { get; init; }
        public int Balance { get; set; } = 0;
        public int TransactionCount { get; set; } = 0;

        private readonly Mutex _mutex = new Mutex();

        public void Deposite(int value)
        {
            _mutex.WaitOne();
            Balance += value;
            TransactionCount++;
            Console.WriteLine($"Kontostand ({Type}): {Balance}");
            _mutex.ReleaseMutex();
        }

        public void Disburse(int value)
        {
            Deposite(-value);
        }
    }
}