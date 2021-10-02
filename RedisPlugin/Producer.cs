using System;
using StackExchange.Redis;
using SharedMemory;

namespace HapticSeerNeo
{
    public class Producer<T> : SharedBufferUser<T> where T : struct
    {
        private int pointer = 0;
        public Producer(string outletName, int nodeSize, int nodeCount, string redisURI = "127.0.0.1:6380") 
            : base("", outletName, redisURI)
         {
            this.nodeSize = nodeSize;
            this.nodeCount = nodeCount;
            try
            {
                buffer = new SharedArray<T>($"{outletName}_{guid}", nodeSize * nodeCount);
                RegisterOutlet(OutletMode.SHARED_BUFFER, $"{{nodeSize:{nodeSize},nodeCount:{nodeCount}}}");
            } 
            catch (Exception e)
            {
                UnregisterOutlet();
                throw new InvalidOperationException("Register outlet failed.",  e);
            }
            
        }
        public bool PublishToOutlet(string message)
        {
            if (outletRegistered)
            {
                return db.Publish(outletName, message, CommandFlags.FireAndForget) > 0;
            }
            else
            {
                return false;
            }
        }

        public void WriteBuffer(T[] data)
        {
            curTime = DateTime.Now;
            buffer.Write(data, startIndex: pointer*nodeSize);
            pointer = (pointer + 1) % nodeCount;
            PublishToOutlet($"{pointer.ToString()}|{curTime.Ticks}");
        }
        public void WriteBuffer(ref T data)
        {
            curTime = DateTime.Now;
            buffer.Write(ref data, pointer * nodeSize);
            pointer = (pointer + 1) % nodeCount;
            PublishToOutlet($"{pointer.ToString()}|{curTime.Ticks}");
        }

    }


}
