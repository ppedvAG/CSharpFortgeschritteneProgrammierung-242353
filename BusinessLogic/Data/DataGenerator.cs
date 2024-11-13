using Bogus;
using System.Drawing;

namespace BusinessLogic.Data
{
    public class DataGenerator
    {
        public const int ReservedSystemColorNameCount = 27;
        public static readonly int ColorCount = Enum.GetValues(typeof(KnownColor)).Length;

        public static IEnumerable<Car> GenerateVehicles(int count = 100)
        {
            var brandFaker  = new Faker<Manufacturer>()
                .UseSeed(37)
                .RuleFor(m => m.Id, f => f.Random.Guid().ToString())
                .RuleFor(m => m.Name, f => f.Vehicle.Manufacturer())
                .RuleFor(m => m.Country, f => f.Vehicle.Manufacturer());

            var carFaker = new Faker<Car>()
                .UseSeed(37) // Wir setzen einen fixen seed um immer wieder die selben Ausgangsdaten zu erzeugen (Vergleichbarkeit)
                .RuleFor(c => c.Id, f => f.Random.Int())
                .RuleFor(c => c.Manufacturer, f => brandFaker.Generate())
                .RuleFor(c => c.Model, f => f.Vehicle.Model())
                .RuleFor(c => c.Type, f => f.Vehicle.Type())
                .RuleFor(c => c.Fuel, f => f.Vehicle.Fuel())
                .RuleFor(c => c.TopSpeed, f => f.Random.Number(10, 25) * 10)
                .RuleFor(c => c.Color, f => (KnownColor)f.Random.Number(ReservedSystemColorNameCount, ColorCount));

            return carFaker.Generate(count);
        }
    }
}
