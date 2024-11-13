using System.Collections;

namespace Reflection.Services
{
    public class DataChangedArgs<T> : EventArgs
    {
        public T OldValue { get; init; }

        public T NewValue { get; init; }
    }

    public abstract class GenericService<T> : IEnumerable<T>
    {
        public List<T> Data { get; }

        public event EventHandler<DataChangedArgs<T>> DataChanged;

        protected GenericService(IEnumerable<T> initialData)
        {
            Data = initialData.ToList();
        }

        public List<T> GetAll()
        {
            return Data;
        }

        public void AddVehicle(T car)
        {
            Data.Add(car);
        }

        public void Update(int index, T car)
        {
            var old = Data[index];
            Data[index] = car;

            DataChanged?.Invoke(this, new DataChangedArgs<T> { OldValue = old, NewValue = car });
        }

        public void RemoveVehicle(T car)
        {
            Data.Remove(car);
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
