using System.Text.Json;
using System.Xml.Serialization;
using LegacyJson = Newtonsoft.Json;

namespace Serialization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var list = Enumerable.Range(0, 200).Select(i => Generator.Generate(i)).ToList();

            XmlSerialization(list);

            JsonSerialization(list);

            JsonNewtonsoftSample(list);

        }

        private static void JsonNewtonsoftSample(List<Car> list)
        {
            var settings = new LegacyJson.JsonSerializerSettings
            {
                Formatting = LegacyJson.Formatting.Indented,
                TypeNameHandling = LegacyJson.TypeNameHandling.Objects // Vererbung ermöglichen
            };

            var json = LegacyJson.JsonConvert.SerializeObject(list, settings);
            File.WriteAllText("list2.json", json);

            var fileContent = File.ReadAllText("list2.json");
            var result = LegacyJson.JsonConvert.DeserializeObject<List<Car>>(fileContent);

            foreach (var car in result.Take(5))
            {
                Console.WriteLine(car);
            }
        }

        private static void JsonSerialization(List<Car> list)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(list, options);
            File.WriteAllText("list.json", json);

            var fileContent = File.ReadAllText("list.json");
            var result = JsonSerializer.Deserialize<List<Car>>(fileContent);

            foreach (var car in result.Take(5))
            {
                Console.WriteLine(car);
            }
        }

        private static void XmlSerialization<T>(List<T> list)
        {
            var serializer = new XmlSerializer(typeof(List<T>));

            using (var stream = File.Create("list.xml"))
            {
                serializer.Serialize(stream, list);
            }

            using (var reader = new StreamReader("list.xml"))
            {
                var result = serializer.Deserialize(reader) as List<T>;

                foreach (var car in result.Take(5))
                {
                    Console.WriteLine(car);
                }
            }
        }
    }
}
