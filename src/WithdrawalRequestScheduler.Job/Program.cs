using System;

namespace WithdrawalRequestScheduler.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new AppHost();
            host.Run();
        }
    }
}