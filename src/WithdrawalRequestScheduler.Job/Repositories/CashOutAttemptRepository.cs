using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WithdrawalRequestScheduler.Job.Repositories
{

    public class CashOutAttemptRepository
    {
        readonly INoSQLTableStorage<CashOutBaseEntity> _tableStorage;

        public static string GeneratePartition(string clientId)
        {
            return clientId;
        }

        public async Task<CashOutBaseEntity> GetAsync(string clientId, string requestId)
        {
            return await _tableStorage.GetDataAsync(clientId, requestId);
        }

        public async Task<IEnumerable<CashOutBaseEntity>> GetAllAttempts()
        {
            return await _tableStorage.GetDataAsync();
        }
    }
}
