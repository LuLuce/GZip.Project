using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GZipTest.Action
{
    public class BlockHandler
    {
        private readonly object inputLockObj;
        private readonly object outputLockObj;
        private readonly Queue<Block> input;
        private readonly Queue<Block> output;

        private readonly Thread[] workers;

        private const int MAX_QUEUE_SIZE = 30;
        private event Action<string> callback;

        private readonly EventWaitHandle addUnhandledBlockWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly EventWaitHandle getUnhandledBlockWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly EventWaitHandle addHandledBlockWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);

        public BlockHandler(int threadsCount, OperationType operationType, Action<string> callback)
        {
            this.inputLockObj = new object();
            this.outputLockObj = new object();

            this.input = new Queue<Block>();
            this.output = new Queue<Block>();

            this.workers = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                this.workers[i] = new Thread(() => Run(operationType));
                this.workers[i].Priority = ThreadPriority.Lowest;
            }
            this.callback = callback;
        }

        public void Start()
        {
            for (int i = 0; i < this.workers.Length; i++)
            {
                this.workers[i].IsBackground = true;
                this.workers[i].Start();
            }

            Console.WriteLine("Started {0} workers.", this.workers.Count());
        }

        public void Stop()
        {
            lock (this.inputLockObj)
            {
                for (int i = 0; i < this.workers.Length; i++)
                    this.input.Enqueue(null);
            }

            foreach (var w in this.workers)
            {
                w.Join();
            }
        }

        private void Run(OperationType operationType)
        {
            IBlockHandlingAction action = operationType == OperationType.Compress
                                          ? (IBlockHandlingAction)(new CompressAction())
                                          : (IBlockHandlingAction)(new DecompressAction());
            while (true)
            {
                Block block = null;
                try
                {
                    lock (this.inputLockObj)
                    {
                        if (this.input.Count > 0)
                        {
                            block = this.input.Dequeue();

                            if (block == null)
                                return;
                        }
                    }
                    this.addUnhandledBlockWaitHandler.Set();

                    if (block != null)
                    {
                        action.Act(block);
                        this.EnqueueHandledBlock(block);
                    }
                    else
                    {
                        this.getUnhandledBlockWaitHandler.WaitOne();
                    }
                }
                catch (ThreadAbortException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                catch (Exception e)
                {
                    callback?.Invoke(e.Message);
                    return;
                }
            }
        }

        public void Abort()
        {
            for (int i = 0; i < this.workers.Length; i++)
            {
                this.workers[i].Abort();
            }
        }

        public void AddUnhandledBlock(Block block)
        {
            bool isFullQueue;
            lock (this.inputLockObj)
            {
                isFullQueue = this.input.Count >= MAX_QUEUE_SIZE;
            }

            if (isFullQueue)
                this.addUnhandledBlockWaitHandler.WaitOne();

            lock (this.inputLockObj)
            {
                this.input.Enqueue(block);
            }

            this.getUnhandledBlockWaitHandler.Set();
        }

        public List<Block> GetAvailableBlocks()
        {
            List<Block> result;
            lock (this.outputLockObj)
            {
                result = this.output.ToList();
                this.output.Clear();
            }

            this.addHandledBlockWaitHandler.Set();
            return result;
        }

        private void EnqueueHandledBlock(Block block)
        {
            bool isFullQueue;
            lock (this.outputLockObj)
            {
                isFullQueue = this.output.Count >= MAX_QUEUE_SIZE;
            }

            if (isFullQueue)
                this.addHandledBlockWaitHandler.WaitOne();

            lock (this.outputLockObj)
            {
                this.output.Enqueue(block);
            }
        }
    }
}
