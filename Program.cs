using System;
using System.Threading;

namespace Client_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(5);
            Client client = new Client(server);
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                int processingTime = random.Next(1000, 5000);
                client.SendRequest(processingTime);
                Console.WriteLine($"Sent request {i + 1} with processing time {processingTime} ms");
            }

            Thread.Sleep(10_000);

            Console.WriteLine($"Total number of requests: {server.NumRequests}");
            Console.WriteLine($"Number of served requests: {server.NumServedRequests}");
            Console.WriteLine($"Number of rejected requests: {server.NumRejectedRequests}");
        }
    }
}
