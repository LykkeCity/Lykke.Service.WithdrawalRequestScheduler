using AzureStorage;
using Common.Log;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.WithdrawalRequestScheduler.Repositories
{

    public class CashOutAttemptRepository
    {
        private readonly INoSQLTableStorage<CashOutBaseEntity> _tableStorage;
        private readonly ILog _log;

        public CashOutAttemptRepository(INoSQLTableStorage<CashOutBaseEntity> tableStorage, ILog log)
        {
            _tableStorage = tableStorage;
            _log = log;
        }

        public static string GeneratePartition(string clientId)
        {
            return clientId;
        }

        public async Task<IEnumerable<CashOutBaseEntity>> GetAllAttempts()
        {
            try
            {
                return await _tableStorage.GetDataAsync();
            }
            catch (Exception ex)
            {
                await _log.WriteFatalErrorAsync("Lykke.Service.WithdrawalRequestScheduler.CashOutAttemptRepository", "GetAllAttempts", null, ex);
                return new List<CashOutBaseEntity>(0);
            }
        }
    }
}
