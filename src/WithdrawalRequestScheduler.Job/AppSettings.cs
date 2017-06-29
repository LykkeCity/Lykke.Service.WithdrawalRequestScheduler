using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WithdrawalRequestScheduler.Job
{
    public class AppSettings
    {
        [JsonProperty("WithdrawalRequestScheduler")]
        public WithdrawalRequestSchedulerSettings Settings { get; set; }
    }

    public class WithdrawalRequestSchedulerSettings
    {
        public string CashOutAttemptConnString { get; set; }
        public string LogsConnString { get; set; }
        public double HoursTillCanceledByTimeout { get; set; }
        public string ApiHost { get; set; }
    }
}
