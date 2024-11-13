using BusinessLogic.Contracts;
using BusinessLogic.Data;
using Reflection.Attributes;

namespace BusinessLogic.Services
{
    [Service("Spediteur", 2)]
    public class TransportService : ITransportService
    {
        private readonly IVehicleService _vehicleService;

        private List<Car> CarPool => _vehicleService.GetAll();

        public List<Car> Payload { get; set; }

        public TransportService(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public void Load(Manufacturer manufacturer)
        {
            Payload = CarPool.Where(c => c.Manufacturer.Name == manufacturer.Name).ToList();
        }

        public List<Car>? Unload()
        {
            var copy = Payload.ToList();
            Payload = null;
            return copy;
        }

        public void ShowInfo()
        {
            if (Payload is null)
            {
                Console.WriteLine("Nichts geladen");
            }
            else
            {
                Console.WriteLine($"Geladene Autos: {Payload.Count}");
            }
        }
    }
}
