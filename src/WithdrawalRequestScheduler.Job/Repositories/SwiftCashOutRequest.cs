using System;
using System.Collections.Generic;
using System.Text;

namespace WithdrawalRequestScheduler.Job.Repositories
{
    public enum CashOutRequestStatus
    {
        ClientConfirmation = 4,
        Pending = 0,
        Confirmed = 1,
        Declined = 2,
        CanceledByClient = 5,
        CanceledByTimeout = 6,
        Processed = 3,
    }

    public enum TransactionStates
    {
        InProcessOnchain,
        SettledOnchain,
        InProcessOffchain,
        SettledOffchain
    }

    public class SwiftCashOutRequest
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string PaymentSystem { get; set; }
        public string PaymentFields { get; set; }
        public string BlockchainHash { get; set; }
        public CashOutRequestStatus Status { get; set; }
        public TransactionStates State { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsHidden { get; set; }
    }
}
