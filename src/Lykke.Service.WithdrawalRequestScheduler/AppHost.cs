using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Loader;
using System.Reflection;
using System.Threading;
using System.Net.Http;
using Lykke.Service.WithdrawalRequestScheduler.Binders;
using Lykke.JobTriggers.Triggers;
using Autofac.Extensions.DependencyInjection;

namespace Lykke.Service.WithdrawalRequestScheduler
{
    public class AppHost
    {
        public IConfigurationRoot Configuration { get; }

        public AppHost()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void Run()
        {

            // Load settings
            var settingsUrl = Configuration["SettingsUrl"];

            if (string.IsNullOrWhiteSpace(settingsUrl))
            {
                // Environment variable "SettingsUrl" with URL to settings file (e.g. "https://settings-dev.lykkex.net/xJNaS5XHZg6DfuUccyKfNcSiiIPMzM1E_WithdrawalRequestScheduler") should exist
                // You can do that at Project's properties -> Debug -> Environment Variables

                Console.WriteLine($"''SettingsUrl'' environment variable empty or not found");
                return;
            }

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(settingsUrl).Result;
            var settingsString = response.Content.ReadAsStringAsync().Result;
            var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(settingsString);


            var containerBuilder = new AzureBinder().Bind(settings.Settings);
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
