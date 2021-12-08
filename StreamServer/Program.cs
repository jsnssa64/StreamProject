using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StreamServer.FileStorage.FSManager;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StreamServer
{
    class Program
    {
        static IConfigurationRoot configurationRoot { get; set; }

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            //FileStorageBuilderBase builder = new FileStorageBuilderBase(configurationRoot);
            CompositionRoot(configurationRoot).Resolve<Application>().Run();

            await host.RunAsync();
        }

        public const string FileStorageKeyName = "FileStorage";

        private static IContainer CompositionRoot(IConfiguration configuration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Application>();
            builder.RegisterType<AmazonSecretsManagerConfig>();
            builder.RegisterType<AmazonSecretsManagerClient>();
            builder.RegisterType<GetSecretValueRequest>();

            builder.Register(fileStorageModel => new FileStorageManager(configuration.GetSection(FileStorageKeyName).GetChildren().ToList()).fileStorageModel);

            return builder.Build();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();                   

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    var envName = env.EnvironmentName == "Local" ? "Development" : env.EnvironmentName;


                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{envName}.json", true, true);

                    //  Use User Secrets instead of AppSettings if on Local Machine
                    if (env.EnvironmentName == "Local")
                    {
                        configuration.AddUserSecrets<Program>();
                    }

                    configurationRoot = configuration.Build();
                });

       
    }
}
