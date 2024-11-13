using BusinessLogic.Data;

namespace BusinessLogic.Contracts
{
    public interface IVehicleService : IGenericService<Car>
    {
    }

    public interface IGenericService<T>
    {
        void AddVehicle(T car);
        List<T> GetAll();
        IEnumerator<T> GetEnumerator();
        void RemoveVehicle(T car);
        void Update(int index, T car);
    }
}