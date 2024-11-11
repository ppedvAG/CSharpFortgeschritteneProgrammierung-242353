namespace Generics
{
    public class Person<T> : IChef, IDoDishes, IDisposable
    {

        public void Cook()
        {
            Console.WriteLine("Ich koche");
        }

        public void Eat()
        {
            Console.WriteLine("Ich esse");
        }

        public void DoSomething(T value)
        {
            Console.WriteLine(value);
        }

        public void Clean()
        {
            Console.WriteLine("Ich wasche ab");
        }

        public void Dispose()
        {
            Console.WriteLine("Ich bin fertig");
        }
    }

    public interface IChef : ICanCook, ICanEat
    {

    }

    public interface ICanEat
    {
        void Eat();
    }

    public interface ICanCook
    {
        void Cook();
    }

    public interface IDoDishes
    {
        void Clean();
    }
}
