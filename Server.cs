using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private double totalTimeSpentBusy = 0;
        private int numThreadsBusy = 0;

        private readonly Stopwatch stopWatch = new Stopwatch();
        private int numIdleThreads = 0;

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

        private void HandleRequest()
        {
            while (true)
            {
                Request request;
                lock (lockObject)
                {
                    if (requestQueue.Count == 0)
                    {
                        Interlocked.Increment(ref numIdleThreads);
                        Monitor.Wait(lockObject);
                        Interlocked.Decrement(ref numIdleThreads);
                        continue;
                    }

                    request = requestQueue.Dequeue();
                }

                Interlocked.Increment(ref numServedRequests);
                Console.WriteLine($"Начало обработки запроса: {request.ProcessingTime}");
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

            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
            }
        }

        public int NumRequests => numRequests;

        public int NumServedRequests => numServedRequests;

        public int NumRejectedRequests => numRejectedRequests;


        public double GetIdleProbability()
        {
            int numIdleThreads = 0;

            lock (lockObject)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    if (!threads[i].IsAlive)
                    {
                        numIdleThreads++;
                    }
                }
            }

            return (double)numIdleThreads / poolSize;
        }

        public double GetRejectProbability()
        {
            return (double)numRejectedRequests / numRequests;
        }

        public double GetRelativeThroughput()
        {
            return (double)numServedRequests / numRequests;
        }

        public double GetAbsoluteThroughput()
        {
            return (double)numServedRequests / poolSize;
        }

        public double GetAverageBusyThreads()
        {
            int numBusyThreads = 0;

            lock (lockObject)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    if (threads[i].IsAlive)
                    {
                        numBusyThreads++;
                    }
                }
            }

            return numBusyThreads;
        }
    }
}