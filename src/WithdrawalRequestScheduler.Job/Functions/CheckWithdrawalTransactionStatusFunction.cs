using Lykke.JobTriggers.Triggers.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WithdrawalRequestScheduler.Job.Repositories;

namespace WithdrawalRequestScheduler.Job.Functions
{
    public class CheckWithdrawalTransactionStatusFunction
    {
        private readonly CashOutAttemptRepository _cashOutAttemptRepository;

        public CheckWithdrawalTransactionStatusFunction(CashOutAttemptRepository cashOutAttemptRepository)
        {
            _cashOutAttemptRepository = cashOutAttemptRepository;
        }


        [TimerTrigger("00:00:10")]
        public async Task Process()
        {
            var transactions = _cashOutAttemptRepository.GetAllAttempts();
        }
    }
}
