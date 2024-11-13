using System.Drawing;

namespace LinqSamples.Data
{
    public class Car
    {
        public long Id { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public string Model { get; set; }

        public string Type { get; set; }

        public string Fuel { get; set; }

        public int TopSpeed { get; set; }

        public KnownColor Color { get; set; }

        public override string ToString()
        {
            return $"{Fuel}\t{Color}\tTopSpeed: {TopSpeed}\t{Type}\t{Manufacturer}\t{Model}";
        }
    }
}
