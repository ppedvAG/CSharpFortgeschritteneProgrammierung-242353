namespace Lab_Konto
{
    public interface IKonto
    {
        string Type { get; init; }

        int Balance { get; set; }
        int TransactionCount { get; set; }

        void Disburse(int value);
        void Deposite(int value);
    }
}