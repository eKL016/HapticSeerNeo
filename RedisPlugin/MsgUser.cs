using System;
using SharedMemory;
namespace HapticSeerNeo
{
    public abstract class MsgUser: NeoRedisPlugin
    {
        public MsgUser(string inletName, string outletName, string redisURI = "127.0.0.1:6380") : base(inletName, outletName, redisURI) {
        }
        public new void Dispose(bool disposing)
        {
            if (disposed) return;
            base.Dispose(disposing);
        }
    }
}
