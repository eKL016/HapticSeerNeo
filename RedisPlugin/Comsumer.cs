using System;
using StackExchange.Redis;
using SharedMemory;

namespace HapticSeerNeo
{
    public class Comsumer<T> : SharedBufferUser<T> where T : struct
    {
        private int timeOut = 30000;

        public T[] printBuf;
        public ChannelMessageQueue msgQueue;
        public bool isProducerFound = false, isListening = false;
        public InletCallBack cb;

        public void CopyToBuffer(RedisValue msg)
        {
            string[] splited = msg.ToString().Split('|');
            int index = int.Parse(splited[0]);
            long frameTick = long.Parse(splited[1]);
            if (index >= buffer.Length) { throw new AccessViolationException("Received a read msg that is out-of-range from the target Redis channel"); }
            buffer.CopyTo(printBuf, index * nodeSize);
        }
        public void ConnectToSharedBuffer(string inletName, string guid, InletCallBack cb)
        {
            buffer = new SharedArray<T>($"{inletName}_{guid}");
#if DEBUG
            Console.WriteLine($"Connected to a shared buffer: ${guid}");
#endif
            SubscribeInletToOutlet(inletName, (msg) =>
            {
                CopyToBuffer(msg);
                cb?.Invoke(msg);
            });
#if DEBUG
            Console.WriteLine("Press any key to stop feature extraction");
#endif
            isListening = true;
            _ = Console.ReadKey();
        }
        public Comsumer(string inletName, int nodeSize, ref T[] printBuf, InletCallBack cb = null, string redisURI = "127.0.0.1:6380")
            : base(inletName, "",  redisURI)
        {
            this.nodeSize = nodeSize;
            this.printBuf = printBuf;

            string guidString = null;

            isProducerFound = System.Threading.SpinWait.SpinUntil(() => {
                guidString = db.SetRandomMember(this.inletName);
                return guidString != null;
            }, timeOut);
            if (isProducerFound)
            {
                ConnectToSharedBuffer(this.inletName, guidString, cb);
                isListening = true;
            } else
            {
                throw new Exception($"Timed-out after {timeOut/1000} seconds  waiting for a producer");
            }
        }
    }


}
