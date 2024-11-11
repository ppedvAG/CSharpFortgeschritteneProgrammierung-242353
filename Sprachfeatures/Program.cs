


namespace Sprachfeatures
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tuple Samples");
            TupleSamples();

            Console.WriteLine("\nLocal Functions Sample");
            LocalFunctionSample();

            Console.WriteLine("\nYield Sample");
            YieldSample();

            // Module 002 Sprachfeatures ab C# 10
            // global Usings aus Usings.g.cs wo System.Drawing definiert wurde
            Console.WriteLine($"Farbe: {KnownColor.Turquoise}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void TupleSamples()
        {
            var hugo = new PersonUsingTuples
            {
                FirstName = "Hugo",
                LastName = "Boss",
                MiddleName = "Von"
            };

            // Alter Ansatz
            var tuples = hugo.GetFullNameOld();
            Console.WriteLine("Full Name old");
            Console.WriteLine(string.Format("Name: {0} {1} {2}", tuples.Item1, tuples.Item2, tuples.Item3));

            // Neuer Ansatz welcher deutlich lesbarer und dadurch besser ist
            // Im Hintergrund aber uebersetzt der Compiler das in das alte Pattern
            var namedTuples = hugo.GetFullName();
            Console.WriteLine("Full Name with named tuples");
            Console.WriteLine($"Name: {namedTuples.FirstName} {namedTuples.MiddleName} {namedTuples.LastName}");

            PersonUsingTuples karla = new()
            {
                FirstName = "Karla",
                MiddleName = "Viola",
                LastName = "Friedrich"
            };

            // Dekonstruierende Zuweisung
            // _ verwenden um ihn als "discarded" zu kennzeichnen (benoetigt noch nicht mal mehr "var")
            var (firstname, middlename, lastname, _) = karla.GetFullNameOld();
            Console.WriteLine(firstname);
            Console.WriteLine(middlename);
            Console.WriteLine(lastname);
        }


        private static void LocalFunctionSample()
        {
            DoubleValue(5_345);

            var result = Fibonacci(10);
            Console.WriteLine($"10. Fibonacci-Zahl: {result}");
        }

        private static void DoubleValue(int value = 123)
        {
            Action doubleValue = () => value *= 2;
            doubleValue();
            Console.WriteLine(value);

            // locale function als Alternative
            // Nuetzlich wenn man mehr als nur ein Einzeiler hat
            void localFunc()
            {
                value *= 2;
            }
            localFunc();
            Console.WriteLine(value);
        }

        public static int Fibonacci(int start)
        {
            return recursiveCalc(start).currentValue;

            static (int currentValue, int previousValue) recursiveCalc(int start)
            {
                if (start == 0) return (1, 0);
                var (current, previous) = recursiveCalc(start - 1);
                return (current + previous, current);
            }
        }

        private static void YieldSample()
        {
            IEnumerable<PersonUsingTuples> students = GetStudents();
            Console.WriteLine("Students wurde noch nicht evaluiert.");

            List<PersonUsingTuples> studentList = students.ToList();
            Console.WriteLine("Students wurde evaluiert.");

            studentList.ForEach(s => Console.WriteLine(s.FirstName));

            // Ein IEnumerable wird nicht sofort aufgeloest sondern erst beim ersten Zugriff auf die Daten
            // vgl. Builder-Pattern
            IEnumerable<PersonUsingTuples> GetStudents()
            {
                yield return new PersonUsingTuples { FirstName = "Hugo", LastName = "Boss", MiddleName = "Von" };
                yield return new PersonUsingTuples { FirstName = "Karla", LastName = "Friedrich", MiddleName = "Viola" };
            }
        }
    }
}
