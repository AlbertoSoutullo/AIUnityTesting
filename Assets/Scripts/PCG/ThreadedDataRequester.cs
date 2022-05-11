using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace PCG
{
    public class ThreadedDataRequester: MonoBehaviour
    {
        private static ThreadedDataRequester _instance;
        private readonly Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

        private void Awake()
        {
            _instance = FindObjectOfType<ThreadedDataRequester>();
        }

        public static void RequestData(Func<object> generateData, Action<object> callback)
        {
            ThreadStart threadStart = delegate
            {
                _instance.DataThread(generateData, callback);
            };
            
            new Thread(threadStart).Start();
        }
        
        void DataThread(Func<object> generateData, Action<object> callback)
        {
            object data = generateData();
            lock (_dataQueue)
            {
                _dataQueue.Enqueue(new ThreadInfo(callback, data));    
            }
        }
        
        private void Update()
        {
            if (_dataQueue.Count > 0)
            {
                for (int i = 0; i < _dataQueue.Count; i++)
                {
                    ThreadInfo threadInfo = _dataQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }

        private readonly struct ThreadInfo
        {
            public readonly Action<object> Callback;
            public readonly object Parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                Callback = callback;
                Parameter = parameter;
            }
        }
    }
}