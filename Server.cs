using System;
using System.Collections.Generic;
using System.Threading;

namespace Client_Server
{
    public class Server
    {
        private readonly int poolSize;
        private readonly Thread[] threads;
        private readonly object lockObject = new object();
        private int numRequests = 0;
        private int numServedRequests = 0;
        private int numRejectedRequests = 0;

        public Server(int poolSize)
        {
            this.poolSize = poolSize;
            this.threads = new Thread[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                threads[i] = new Thread(HandleRequest);
                threads[i].Start();
            }
        }

        public void HandleRequest()
        {
            while (true)
            {
                Request request;
                lock (lockObject)
                {
                    if (requestQueue.Count == 0)
                    {
                        Monitor.Wait(lockObject);
                    }

                    request = requestQueue.Dequeue();
                }

                Interlocked.Increment(ref numServedRequests);
                Console.WriteLine($"Начало обработки запроса: {request.ProcessingTime}");
                // Simulate request processing time
                Thread.Sleep(request.ProcessingTime);
                Console.WriteLine($"Конец обработки запроса: {request.ProcessingTime}");
            }
        }

        private readonly Queue<Request> requestQueue = new Queue<Request>();

        public void EnqueueRequest(Request request)
        {
            Interlocked.Increment(ref numRequests);
            lock (lockObject)
            {
                if (requestQueue.Count < poolSize)
                {
                    requestQueue.Enqueue(request);
                    Monitor.Pulse(lockObject);
                }
                else
                {
                    Interlocked.Increment(ref numRejectedRequests);
                }
            }
        }

        public int NumRequests
        {
            get { return numRequests; }
        }

        public int NumServedRequests
        {
            get { return numServedRequests; }
        }

        public int NumRejectedRequests
        {
            get { return numRejectedRequests; }
        }
    }
}