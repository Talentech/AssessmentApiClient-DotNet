using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Exas;
using Auth;

namespace ExasTestClient
{
    class Program
    {
        private static ServiceProvider ServiceProvider;


        static void Main(string[] args)
        {
            SetupDI();

            var exasClient = ServiceProvider.GetService<IExasClient>();

            string tenantId = "<Add your TenantId>";

            var v = exasClient.GetCustomer(tenantId);

            Console.WriteLine(v.Result);
            Console.ReadLine();

            ServiceProvider.Dispose();
        }

        private static void SetupDI()
        {
            var collection = new ServiceCollection();

            //TODO Add to enviroment settings
            //var enviroment = "Test";
            var enviroment = "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.{enviroment}.json", false, true);

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{enviroment}.json", false, true)
                .Build();

            collection.AddSingleton<IConfiguration>(config);
            collection.AddScoped<IAuthClient, AuthClient>();
            collection.AddScoped<IExasClient, ExasClient>();

            ServiceProvider = collection.BuildServiceProvider();
        }
    }
}
