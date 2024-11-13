using Bogus.DataSets;
using BusinessLogic.Data;
using BusinessLogic.Services;
using Reflection.Attributes;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reflection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Reflection arbeitet mit Typen
            // d.h. zwei Moeglichkeiten
            var car = DataGenerator.GenerateVehicles(1).First();
            //car = null;
            var carType = car.GetType();

            // oder besser
            var carType2 = typeof(Car);
            string carInfo = PrintProperties(car);

            Console.WriteLine(carInfo);

            var members = typeof(VehicleService).GetMembers();
            var info = members.Aggregate(new StringBuilder(), (sb, m) => sb.Append($"{m.Name}, {m.MemberType}, {m.DeclaringType}, {m.ReflectedType}\n")).ToString();
            //Console.WriteLine("Members von VehicleService");
            //Console.WriteLine(info);

            // aktuelles Projekt, also "Reflection"
            _ = Assembly.GetExecutingAssembly();

            // Class Library "BusinessLogic" wo der Type "DataGenerator" definiert ist
            var businessAssembly = Assembly.GetAssembly(typeof(DataGenerator)); 

            // Alternativ DLL aus bin Verzeich laden
            var path = Environment.CurrentDirectory.Replace(nameof(Reflection), "BusinessLogic") + "\\BusinessLogic.dll";
            if (!File.Exists(path)) 
                throw new FileNotFoundException("DLL BusinessLogic.dll nicht gefunden");

            Console.WriteLine("Assembly laden von " + path);
            businessAssembly = Assembly.LoadFrom(path);

            var services = businessAssembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null)
                .ToList();

            services.ForEach(s =>
            {
                var attr = s.GetCustomAttribute<ServiceAttribute>();
                Console.WriteLine($"{attr.Order} {s.Name}: {attr.Description}");
            });

            // Wir koennen auch auf private Members zugreifen
            Console.WriteLine("\n\nPrivate readonly Fields von VehicleService setzen");
            var privateFields = typeof(VehicleService).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            var service = new VehicleService();
            privateFields[0].SetValue(service, "__Infiltrated__");
            service.ShowInfo();

            // Welche Interfaces hat unser Service
            Console.WriteLine("\n\nInterfaces von VehicleService");
            var interfaces = typeof(VehicleService).GetInterfaces();
            interfaces.ToList().ForEach(i =>
            {
                var genericArgs = string.Join(", ", i.GenericTypeArguments.Select(a => a.Name));
                Console.WriteLine($"Interface: {i.Name}, {genericArgs}");
            });

            var customCar = new Car
            {
                Manufacturer = new Manufacturer { Name = "Reflection" },
                Model = "Test",
                Color = System.Drawing.KnownColor.AliceBlue,
                TopSpeed = 500
            };

            // Methode aufrufen
            Console.WriteLine($"\n\nMethode {nameof(VehicleService.AddVehicle)} aufrufen");
            var addMethod = typeof(VehicleService).GetMethod(nameof(VehicleService.AddVehicle));
            addMethod.Invoke(service, [customCar]);
            service.ShowInfo();

            EventHandler<DataChangedArgs<Car>> customHandler = (o, e) => Console.WriteLine($"Event {nameof(VehicleService.DataChanged)} ausloesen\n{PrintProperties(e)}");
            typeof(VehicleService).GetEvent(nameof(VehicleService.DataChanged)).AddEventHandler(service, customHandler);

            Console.WriteLine($"\tDavor: {service.Data[0]}");
            var updateMethod = typeof(VehicleService).GetMethod(nameof(VehicleService.Update));
            updateMethod.Invoke(service, [0, customCar]);
            Console.WriteLine($"\tDanach: {service.Data[0]}");

            Console.WriteLine("\n\nPress any key to continue");
            Console.ReadKey();
        }

        private static string PrintProperties<T>(T car)
        {
            return typeof(T)
                .GetProperties()
                .Aggregate(new StringBuilder(), (sb, p) => sb.Append($"{p.Name}: {p.GetValue(car)}\t"))
                .ToString();
        }
    }
}
