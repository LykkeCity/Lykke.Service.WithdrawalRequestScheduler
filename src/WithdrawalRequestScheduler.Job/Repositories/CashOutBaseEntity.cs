using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace WithdrawalRequestScheduler.Job.Repositories
{
    public class CashOutBaseEntity : TableEntity
    {
        public string Id => RowKey;
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string PaymentSystem { get; set; }
        public string PaymentFields { get; set; }
        public string BlockchainHash { get; set; }

        public CashOutRequestStatus Status
        {
            get { return (CashOutRequestStatus)StatusVal; }
            set { StatusVal = (int)value; }
        }

        public TransactionStates State
        {
            get { return (TransactionStates)StateVal; }
            set { StateVal = (int)value; }
        }

        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsHidden { get; set; }

        public int StatusVal { get; set; }
        public int StateVal { get; set; }
    }
}
