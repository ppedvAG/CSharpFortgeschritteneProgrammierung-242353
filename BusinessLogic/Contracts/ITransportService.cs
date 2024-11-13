using BusinessLogic.Data;

namespace BusinessLogic.Contracts
{
    public interface ITransportService
    {
        void Load(Manufacturer manufacturer);
        List<Car>? Unload();
        void ShowInfo();
    }
}