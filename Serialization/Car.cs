using System.Drawing;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Serialization
{
    [XmlRoot("Vehicle")]
    public class Car
    {
        [XmlIgnore]
        public long Id { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        [XmlAttribute("VehicleType")]
        [JsonPropertyName("VehicleType")]
        public string Type { get; set; }

        public string Fuel { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KnownColor Color { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Manufacturer: {Manufacturer}, Model: {Model}, Type: {Type}, Fuel: {Fuel}, Color: {Color}";
        }
    }
}
