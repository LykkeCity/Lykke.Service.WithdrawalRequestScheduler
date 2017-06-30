using System;

namespace Lykke.Service.WithdrawalRequestScheduler
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