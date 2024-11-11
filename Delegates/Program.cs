

namespace Delegates;

class Program
{
    // Function Deklaration fuer einen Function Type den wir spaeter benutzen werden
    public delegate void Hello(string name);

    static void Main(string[] args)
    {
        HelloDelegates();

        Console.WriteLine("\n\nAction und Funcs");
        ActionSample();
    }

    private static void HelloDelegates()
    {
        var hello = new Hello(HalloDe);

        hello("Steven"); // Ausfuehrung des Delegates

        // Mit += koennen wir weitere function pointers hinzufuegen
        hello += HalloDe;
        hello("Andreas");

        hello += HelloEn;
        hello += HelloEn;
        hello("Felix");

        hello -= HelloEn;
        hello -= HelloEn;
        hello("Luca");

        hello -= HalloDe;
        hello -= HalloDe;
        //hello("Fabian"); // hello ist null

        // Immer ein Null-Check vor Ausfuehrung des Delegates durchfuehren
        if (hello != null)
        {
            hello("Fabian");
        }

        hello?.Invoke("Jonas");

        // nichts passiert
        hello -= HalloDe;
    }

    private static void HalloDe(string name)
    {
        Console.WriteLine("Hallo, mein Name ist " + name);
    }

    private static void HelloEn(string name)
    {
        Console.WriteLine("Hello, my name is " + name);
    }

    private static void ActionSample()
    {
        var printNumber = new Action<int, int>(PrintRandomNumber);

        printNumber(10, 20);
        // So mit null check aufrufen
        printNumber?.Invoke(10, 20);

        var addNumbers = new Func<int, int, int>((x, y) => x + y);
        var result = addNumbers(10, 20);
        Console.WriteLine($"10 + 20 = {result}");

        bool IsEven(int number) => number % 2 == 0;

        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var firstEvenNumber = numbers.ToList().Find(IsEven);
        Console.WriteLine("First even number is " + firstEvenNumber);

    }

    private static void PrintRandomNumber(int max, int min)
    {
        Console.WriteLine("Random number " + Random.Shared.Next(max, min));
    }
}
