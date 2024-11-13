using BusinessLogic.Contracts;
using BusinessLogic.Data;
using Reflection.Attributes;

namespace BusinessLogic.Services
{
    [Service("Fahrzeugverwaltung", 1)]
    public class VehicleService : GenericService<Car>, IVehicleService
    {
        private readonly string? _state;

        public VehicleService()
            : base(DataGenerator.GenerateVehicles(200))
        {
            _state = "Top Secret";
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Anzahl Elemente: {Data.Count}\nState: {_state}");
        }
    }
}
