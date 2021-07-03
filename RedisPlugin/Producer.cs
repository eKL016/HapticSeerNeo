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
            if(db.SetAdd(this.outletName, guid.ToString()))
            {
                this.isRegistered = true;
            }
            else
            {
                throw new Exception("Register component failed");
            }
            buffer = new SharedArray<T>($"{outletName}_{guid}", nodeSize*nodeCount);
        }
        public void WriteBuffer(T[] data)
        {
            curTime = DateTime.Now;
            buffer.Write(data, startIndex: pointer*nodeSize);
            pointer = (pointer + 1) % nodeCount;
            PublishToOutlet($"{pointer.ToString()}|{curTime.Ticks}");
        }
    }


}
