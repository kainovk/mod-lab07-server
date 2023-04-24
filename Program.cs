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
            int requestIntensity = 1000 / 20; // количество запросов в секунду
            int serviceIntensity = 1000 / 1; // время выполнения запроса в секундах

            Server server = new Server(poolSize);
            Client client = new Client(server);

            for (int i = 0; i < numRequests; i++)
            {
                client.SendRequest(serviceIntensity);
                Console.WriteLine($"Sent request {i + 1} with processing time {serviceIntensity} ms");
                Thread.Sleep(requestIntensity);
            }

            // ожидаем завершения всех запросов
            while (server.NumRequests > server.NumServedRequests + server.NumRejectedRequests)
            {
                Thread.Sleep(5_000);
            }

            Thread.Sleep(serviceIntensity + 100);

            double idleProb = server.GetIdleProbability();
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