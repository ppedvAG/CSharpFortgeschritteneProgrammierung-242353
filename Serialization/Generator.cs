using Bogus;
using System.Drawing;

namespace Serialization
{
    public class Generator
    {
        public static readonly Faker _faker = new();

        public static readonly int ColorCount = Enum.GetValues<KnownColor>().Length;

        public static Car Generate(int id)
        {
            var color = (KnownColor)Random.Shared.Next(30, ColorCount);
            return new Car
            {
                Id = id,
                Manufacturer = _faker.Vehicle.Manufacturer(),
                Model = _faker.Vehicle.Model(),
                Type = _faker.Vehicle.Type(),
                Fuel = _faker.Vehicle.Fuel(),
                Color = color
            };
        }
    }
}
