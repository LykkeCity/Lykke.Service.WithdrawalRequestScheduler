using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Loader;
using System.Reflection;
using System.Threading;
using System.Net.Http;
using WithdrawalRequestScheduler.Job.Binders;
using Lykke.JobTriggers.Triggers;
using Autofac.Extensions.DependencyInjection;

namespace WithdrawalRequestScheduler.Job
{
    public class AppHost
    {
        public IConfigurationRoot Configuration { get; }

        public AppHost()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void Run()
        {

            // Load settings
            var settingsUrl = Configuration["SettingsUrl"];
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(settingsUrl).Result;
            var settingsString = response.Content.ReadAsStringAsync().Result;
            var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(settingsString);


            var containerBuilder = new AzureBinder().Bind(settings);
            var ioc = containerBuilder.Build();

            var triggerHost = new TriggerHost(new AutofacServiceProvider(ioc));

            triggerHost.ProvideAssembly(GetType().GetTypeInfo().Assembly);

            var end = new ManualResetEvent(false);

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                Console.WriteLine("SIGTERM recieved");
                triggerHost.Cancel();

                end.WaitOne();
            };

            triggerHost.Start().Wait();
            end.Set();
        }
    }
}
