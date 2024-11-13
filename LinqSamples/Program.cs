using LinqSamples.Data;
using LinqSamples.Extensions;
using System.Text;

namespace LinqSamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LinqSamples();

            var number = 4711;
            Console.WriteLine($"Quersumme von {number} ist {number.DigitSum()}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void LinqSamples()
        {
            var vehicles = DataGenerator.GenerateVehicles(100);

            Console.WriteLine("Top 10 Vehicles:");
            vehicles.Take(10)
                .ToList()
                .ForEach(Console.WriteLine);

            var averageSpeed = vehicles.Take(10).Average(v => v.TopSpeed);
            var maxSpeed = vehicles.Take(10).Max(v => v.TopSpeed);
            var minSpeed = vehicles.Take(10).Min(v => v.TopSpeed);
            Console.WriteLine($"\nAverage Speed: {averageSpeed}, Max Speed: {maxSpeed}, Min Speed: {minSpeed}");

            // Exception wenn Liste leer waere
            Console.WriteLine($"Erstes Fahrzeug: {vehicles.First()}");

            // Der Default-Wert für LastOrDefault ist null
            Console.WriteLine($"Letztes Fahrzeug: {vehicles.LastOrDefault()}");

            // Wenn wir Single verwenden und mehr als ein Element vorkommt fliegt eine Exception
            Console.WriteLine($"11. Fahrzeug: {vehicles.Skip(10).Take(1).SingleOrDefault()}");

            Console.WriteLine("\n\nAlle Fahrzeuge mit einem gelben Farbton.");
            vehicles.Where(v => v.Color.ToString().Contains("Yellow"))
                .ToList()
                .ForEach(Console.WriteLine);

            Console.WriteLine("\n\nFahrzeuge sortieren nach TopSpeed und Model.");
            vehicles
                .OrderByDescending(v => v.TopSpeed)
                .ThenBy(v => v.Model)
                .Take(10)
                .ToList()
                .ForEach(Console.WriteLine);

            Console.WriteLine("\n\nAutos nach FuelType gruppieren.");
            IEnumerable<IGrouping<string, Car>> groups = vehicles.GroupBy(v => v.Fuel);
            groups.Select(g => new { Fuel = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList()
                .ForEach(g => Console.WriteLine($"{g.Count} Autos mit {g.Fuel}"));

            static StringBuilder AppendLine(StringBuilder sb, Car v) => sb.AppendLine($"Der {v.Color} {v.Model} faehrt mit {v.TopSpeed} km/h.");

            var sb = vehicles.Take(10).Aggregate(new StringBuilder(), AppendLine);
            Console.WriteLine(sb.ToString());

            Dictionary<string, string> dict = vehicles.Take(20)
                .Select(v => new { Brand = v.Manufacturer.Name, Vehicle = v })
                .GroupBy(v => v.Brand)
                .ToDictionary(g => g.Key, g => g.Select(v => v.Vehicle).Aggregate(new StringBuilder(), AppendLine).ToString());

            // Select wird an dieser Stelle noch nicht evaluiert sondern erst bei ToList() weil IEnumerable "lazy" ist.
            var output = dict.Select(p =>
            {
                Console.WriteLine($"{p.Key}: {p.Value}");
                return p;
            });


            Console.WriteLine("\n\nEs wurde noch nichts in die Console geschrieben. Erst ToList() fuehrt zur Ausfuehrung des Codes.");
            _ = output.ToList();
        }
    }
}
