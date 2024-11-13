using BusinessLogic.Contracts;
using BusinessLogic.Data;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Reflection.Attributes;
using System.Reflection;
using System.Text;

namespace DependencyInjection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ServiceProvider serviceProvider = RegisterTypes();
            ServiceProvider serviceProvider = RegisterTypesFromServiceAttribute();

            TransportServiceSample(serviceProvider);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static ServiceProvider RegisterTypes()
        {
            // Hier werden die ganzen Typinformationen gespeichert
            var container = new ServiceCollection();

            container.AddTransient<IVehicleService, VehicleService>();
            container.AddTransient<ITransportService, TransportService>();

            var serviceProvider = container.BuildServiceProvider();
            return serviceProvider;
        }

        private static ServiceProvider RegisterTypesFromServiceAttribute()
        {
            // Hier werden die ganzen Typinformationen gespeichert
            var container = new ServiceCollection();

            Assembly.GetAssembly(typeof(GenericService<>))
                .GetTypes()
                .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null)
                .ToList()
                .ForEach(t =>
                {
                    var serviceType = t.GetInterfaces()
                        .FirstOrDefault(i => i.Name.EndsWith(t.Name));
                    if (serviceType == null)
                    {
                        container.AddTransient(t);
                    } 
                    else
                    {
                        container.AddTransient(serviceType, t);
                    }
                });

            var serviceProvider = container.BuildServiceProvider();
            return serviceProvider;
        }

        private static void TransportServiceSample(ServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<ITransportService>();

            service.ShowInfo();

            service.Load(new Manufacturer() { Name = "BMW" });

            service.ShowInfo();

            var result = service.Unload();

            Console.WriteLine(result.Aggregate(new StringBuilder(), (sb, item) => sb.AppendLine($"{item}")));
        }
    }
}
