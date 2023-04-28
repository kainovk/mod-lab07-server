using System;
using System.Threading;

namespace Client_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int numRequests = 50;
            int poolSize = 5;
            int requestIntensity = 1000 / 10; // количество запросов в секунду
            int serviceIntensity = 1000 / 5; // количество запросов, обрабатываемых сервером в секунду

            Server server = new Server(poolSize, serviceIntensity);
            Client client = new Client(server);

            for (int i = 0; i < numRequests; i++)
            {
                var processingTime = serviceIntensity;
                Console.WriteLine($"Отправлен запрос {i + 1} с временем обработки {processingTime} ms");
                client.SendRequest(processingTime);
                Thread.Sleep(requestIntensity);
            }


            Thread.Sleep((requestIntensity + serviceIntensity) * (poolSize + 1));

            double idleProb = server.GetIdleThreadProbability();
            double rejectProb = server.GetRejectProbability();
            double throughputRel = server.GetRelativeThroughput();
            double throughputAbs = server.GetAbsoluteThroughput();
            double avgBusyThreads = server.GetAverageBusyThreads();

            Console.WriteLine($"Total number of requests: {server.NumRequests}");
            Console.WriteLine($"Number of served requests: {server.NumServedRequests}");
            Console.WriteLine($"Number of rejected requests: {server.NumRejectedRequests}");
            Console.WriteLine($"Idle probability: {idleProb}");
            Console.WriteLine($"Reject probability: {rejectProb}");
            Console.WriteLine($"Relative throughput: {throughputRel}");
            Console.WriteLine($"Absolute throughput: {throughputAbs}");
            Console.WriteLine($"Average number of busy threads: {avgBusyThreads}");
        }
    }
}