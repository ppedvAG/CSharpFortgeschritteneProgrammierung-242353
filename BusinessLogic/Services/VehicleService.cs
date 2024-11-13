using BusinessLogic.Contracts;
using LinqSamples.Data;
using Reflection.Attributes;

namespace Reflection.Services
{
    [Service("Fahrzeugverwaltung", 1)]
    public class VehicleService : GenericService<Car>, IVehicleService
    {
        private readonly string? _state;

        public VehicleService()
            : base(DataGenerator.GenerateVehicles(10))
        {
            _state = "Top Secret";
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Anzahl Elemente: {Data.Count}\nState: {_state}");
        }
    }
}
