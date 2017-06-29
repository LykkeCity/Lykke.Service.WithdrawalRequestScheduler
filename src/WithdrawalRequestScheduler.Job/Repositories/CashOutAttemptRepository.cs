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

        public CashOutAttemptRepository(INoSQLTableStorage<CashOutBaseEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

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

        public async Task<CashOutBaseEntity> SetCanceledByClient(string clientId, string requestId)
        {
            return await ChangeStatus(clientId, requestId, CashOutRequestStatus.CanceledByClient);
        }

        private async Task<CashOutBaseEntity> ChangeStatus(string clientId, string requestId, CashOutRequestStatus status)
        {
            var entity = await _tableStorage.DeleteAsync(clientId, requestId);

            entity.PartitionKey = "Processed";
            entity.Status = status;

            return await _tableStorage.InsertAndGenerateRowKeyAsDateTimeAsync(entity, entity.DateTime);
        }
    }
}
