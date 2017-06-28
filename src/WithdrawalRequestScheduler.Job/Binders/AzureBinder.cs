using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Features.ResolveAnything;
using AzureStorage.Tables;
using WithdrawalRequestScheduler.Job.Repositories;
using Common.Log;
using Common;

namespace WithdrawalRequestScheduler.Job.Binders
{
    public class AzureBinder
    {
        public const string DefaultConnectionString = "UseDevelopmentStorage=true";

        public ContainerBuilder Bind(AppSettings settings)
        {
            var logToTable = new LogToTable(new AzureTableStorage<LogEntity>(settings.LogsConnString, "LogWithdrawalRequestSchedulerError", null),
                                            new AzureTableStorage<LogEntity>(settings.LogsConnString, "LogWithdrawalRequestSchedulerWarning", null),
                                            new AzureTableStorage<LogEntity>(settings.LogsConnString, "LogWithdrawalRequestSchedulerInfo", null));
#if DEBUG
            var log = new LogToTableAndConsole(logToTable, new LogToConsole());
#else
            var log = logToTable;
#endif
            var ioc = new ContainerBuilder();
            InitContainer(ioc, settings, log);
            return ioc;
        }

        private void InitContainer(ContainerBuilder ioc, AppSettings settings, ILog log)
        {
#if DEBUG
            log.WriteInfoAsync("WithdrawalRequestScheduler.Job", "App start", null, $"AppSettings : {settings.ToJson()}").Wait();
#else
            log.WriteInfoAsync("WithdrawalRequestScheduler.Job", "App start", null, $"AppSettings : private").Wait();
#endif
            ioc.RegisterInstance(log);
            ioc.RegisterInstance(settings);
            ioc.RegisterType<CashOutAttemptRepository>().SingleInstance();
            
            ioc.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }
    }
}
