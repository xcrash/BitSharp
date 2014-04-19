﻿using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSharp.Common
{
    public abstract class ProducerConsumerWorker<T> : IDisposable
    {
        private readonly string name;
        private readonly bool isConcurrent;
        private readonly WorkerMethod queueWorker;
        private ProducerConsumer<T> queue;

        public ProducerConsumerWorker(string name, bool isConcurrent, Logger logger)
        {
            this.name = name;
            this.isConcurrent = isConcurrent;
            this.queueWorker = new WorkerMethod(name, WorkAction, initialNotify: false, minIdleTime: TimeSpan.Zero, maxIdleTime: TimeSpan.MaxValue, logger: logger);
            this.queueWorker.Start();
        }

        public void Dispose()
        {
            this.SubDispose();
            this.queueWorker.Dispose();
        }

        public string Name { get { return this.name; } }

        public IDisposable Start()
        {
            if (this.queue != null)
                throw new InvalidOperationException();

            this.queue = new ProducerConsumer<T>();
            this.queueWorker.NotifyWork();

            return new Stopper(this);
        }

        public void CompleteAdding()
        {
            if (this.queue == null)
                throw new InvalidOperationException();

            this.queue.CompleteAdding();
        }

        public void WaitToComplete()
        {
            if (this.queue == null)
                throw new InvalidOperationException();

            this.queue.WaitToComplete();
        }

        public void Add(T value)
        {
            if (this.queue == null)
                throw new InvalidOperationException();

            this.queue.Add(value);
        }

        protected virtual void SubDispose() { }

        protected abstract void ConsumeItem(T value);

        private void Stop()
        {
            if (this.queue == null || !this.queue.IsCompleted)
                throw new InvalidOperationException();

            this.queue.Dispose();
            this.queue = null;
        }

        private void WorkAction()
        {
            if (this.queue == null)
                throw new InvalidOperationException();

            if (this.isConcurrent)
            {
                Parallel.ForEach(
                    this.queue.GetConsumingEnumerable(),
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },
                    value => ConsumeItem(value));
            }
            else
            {
                foreach (var value in this.queue.GetConsumingEnumerable())
                    ConsumeItem(value);
            }
        }

        private sealed class Stopper : IDisposable
        {
            private readonly ProducerConsumerWorker<T> worker;

            public Stopper(ProducerConsumerWorker<T> worker)
            {
                this.worker = worker;
            }

            public void Dispose()
            {
                this.worker.Stop();
            }
        }
    }
}
