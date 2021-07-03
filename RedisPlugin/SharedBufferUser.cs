using System;
using SharedMemory;

namespace HapticSeerNeo
{
    public class SharedBufferUser<T> : NeoRedisPlugin where T : struct
    {
        protected SharedArray<T> buffer;
        protected DateTime curTime;
        protected int nodeSize, nodeCount;
        public SharedBufferUser(string inletName, string outletName, string redisURI = "127.0.0.1:6380") : base(inletName, outletName, redisURI) {}
        public new void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                buffer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
