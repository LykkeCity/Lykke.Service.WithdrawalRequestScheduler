using System;

namespace WithdrawalRequestScheduler.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var host = new AppHost();
            host.Run();
        }
    }
}