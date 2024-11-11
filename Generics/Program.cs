
namespace Generics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var chef = new Person<string>();
            List<Person<string>> names = [chef];

            // Problemstellung: Wir haben sehr viele Interfaces (hier nur ICanEat und ICanCook) und wir koennen ein sog. "Super-Interface" erstellen,
            // indem wir mehrere Interfaces verbinden, hier mit IChef welches ICanEat und ICanCook implementiert.
            CookAndEat(chef);
            EatOnly(chef);

            // Was machen wir wenn wir andere Interfaces miteinander kombinieren wollen?
            EatAndClean(chef);

            var p1 = CreateNewInstanceOld<string>();
            var p2 = CreateNewInstance<Person<string>>();

            var dataStore = new DataStore<Person<string>>();
            dataStore.Add(0, new Person<string>());
        }

        // Das funktioniert weil wir hier ein kombiniertes Interface haben
        static void CookAndEat(IChef chef)
        {
            chef.Cook();
            chef.Eat();
        }

        static void EatOnly(ICanEat obj)
        {
            obj.Eat();
        }

        static void EatAndClean<T>(T cleaner) where T : ICanEat, IDoDishes
        {
            cleaner.Eat();
            cleaner.Clean();
        }

        private static Person<T>? CreateNewInstanceOld<T>()
        {
            var person = Activator.CreateInstance(typeof(Person<T>));
            return (Person<T>?)person;
        }

        private static T CreateNewInstance<T>() where T : new()
        {
            return new T();
        }

        public class DataStore<T> 
            where T : class, IDisposable, new()
        {
            public T[] Data { get; set; } = [];

            public void Add(int index, T value)
            {
                Data[index] = value;
            }
        }
    }
}
