using System;
using StackExchange.Redis;
using SharedMemory;

namespace HapticSeerNeo
{
    public class Comsumer<T> : SharedBufferUser<T> where T : struct
    {
        private BufferCallback bufferCallback;
        public T[] printBuf;
        public delegate void BufferCallback(Comsumer<T> self, string message);
        public ChannelMessageQueue msgQueue;
        public void CopyToBuffer(RedisValue msg)
        {
            string[] splited = msg.ToString().Split('|');
            int index = Int32.Parse(splited[0])+1;
            long frameTick = long.Parse(splited[1]);
            buffer.CopyTo(printBuf, index * nodeSize);
        }

        public Comsumer(string inletName, string outletName, int nodeSize, ref T[] printBuf, BufferCallback cb,string redisURI = "127.0.0.1:6380")
            : base(inletName, outletName, redisURI)
        {
            this.bufferCallback = cb;
            this.nodeSize = nodeSize;
            this.printBuf = printBuf;
            string guidString = db.SetRandomMember(this.inletName);
            buffer = new SharedArray<T>($"{inletName}_{guidString}");
            Console.WriteLine($"Connected to a shared buffer: ${guidString}");
            SubscribeToSingleChannel(inletName, (msg) =>
            {
                CopyToBuffer(msg);
                bufferCallback(this, msg);
            });
            Console.WriteLine("Press any key to stop feature extraction");
            _ = Console.ReadKey();
        }
    }


}
