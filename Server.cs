using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Client_Server
{
    public class Server
    {
        private readonly int _poolSize;
        private readonly int _intensity;
        private int _totalBusyThreads;

        private readonly object _queueLock = new object();
        private readonly Mutex _mtx = new Mutex();
        private readonly ConcurrentQueue<Request> _requestQueue = new ConcurrentQueue<Request>();
        private DateTime _lastRequestTime;

        public Server(int poolSize, int intensity)
        {
            _poolSize = poolSize;
            _intensity = intensity;
            _lastRequestTime = DateTime.Now.AddMilliseconds(-_intensity);
            _totalBusyThreads = 0;
            var pool = new Thread[poolSize];
            for (var i = 0; i < poolSize; i++)
            {
                var threadId = i;
                pool[i] = new Thread(() => HandleRequest(threadId));
                pool[i].Start();
            }
        }

        private void HandleRequest(int threadId)
        {
            while (true)
            {
                if (_requestQueue.IsEmpty) continue;
                _mtx.WaitOne();
                var currentTime = DateTime.Now;
                var diff = (currentTime - _lastRequestTime).TotalMilliseconds;
                if (diff < _intensity)
                {
                    _mtx.ReleaseMutex();
                    continue;
                }

                if (!_requestQueue.TryDequeue(out var request))
                {
                    _mtx.ReleaseMutex();
                    continue;
                }

                NumServedRequests++;
                _totalBusyThreads += _requestQueue.Count + 1;
                Console.WriteLine($"Начало обработки запроса {request.ProcessingTime} потоком {threadId + 1}");
                _lastRequestTime = currentTime;
                _mtx.ReleaseMutex();
                Thread.Sleep(request.ProcessingTime);
                Console.WriteLine($"Конец обработки запроса {request.ProcessingTime}  потоком {threadId + 1}");
            }
        }

        public void EnqueueRequest(Request request)
        {
            lock (_queueLock)
            {
                NumRequests++;
                if (_requestQueue.Count < _poolSize)
                {
                    _requestQueue.Enqueue(request);
                }
                else
                {
                    Console.WriteLine($"Запрос с временем обработки {request.ProcessingTime} отклонен");
                    NumRejectedRequests++;
                }
            }
        }

        public int NumRequests { get; private set; }

        public int NumServedRequests { get; private set; }

        public int NumRejectedRequests { get; private set; }

        public double GetRejectProbability()
        {
            return (double)NumRejectedRequests / NumRequests;
        }

        public double GetRelativeThroughput()
        {
            return (double)NumServedRequests / NumRequests;
        }

        public double GetAbsoluteThroughput()
        {
            return (double)NumServedRequests / _poolSize;
        }

        public double GetIdleThreadProbability()
        {
            return (double)_totalBusyThreads / (NumRequests * _poolSize);
        }

        public double GetAverageBusyThreads()
        {
            if (NumServedRequests == 0)
            {
                return 0.0;
            }

            return (double)_totalBusyThreads / NumServedRequests;
        }
    }
}