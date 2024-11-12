using System.Collections.Concurrent;

namespace Concurrency
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Beispiel 1
            ConcurrentBag<string> bag = ["a", "b", "c"];
            //bag[0] // Index nicht möglich

            // Daten nur mit Linq zugänglich sind
            var query = from s in bag select s;
            foreach (var s in query)
            {
                Console.WriteLine(s);
            }

            // Beispiel 2
            // benoetigt das NuGet Package System.ServiceModel.Primitives
            SynchronizedCollection<string> list = new();
            list.Add("foo");
            list.Add("bar");
            list.Add("baz");

            // Hier koennen wir im Gegensatz zum Bag mit Index darauf zugreifen
            Console.WriteLine(list[0]);


            // Beispiel 3
            ConcurrentDictionary<string, int> dict = new ConcurrentDictionary<string, int>();
            dict.TryAdd("foo", 1);
            dict.TryAdd("bar", 2);

            var updateClause = (string key, int oldValue) => oldValue + 1;
            dict.AddOrUpdate("baz", 8, updateClause);

            // Versuche einen Wert heraus zu holen, falls ein anderer Thread ihn nicht entfernt hat
            bool success = dict.TryGetValue("foo", out int value);

            dict.GetOrAdd("foo", (key) => value);

        }
    }
}
