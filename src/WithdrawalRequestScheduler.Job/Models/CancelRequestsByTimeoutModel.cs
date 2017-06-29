
using System;
using System.Collections.Generic;
using System.Text;

namespace WithdrawalRequestScheduler.Job.Models
{
    public class CancelRequestsByTimeoutModel
    {
        public List<KeyValuePair<string, string>> RequestsToCancel { get; set; }
    }
}
