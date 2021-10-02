using System;
using StackExchange.Redis;
using SharedMemory;

namespace HapticSeerNeo
{
    
    public abstract class NeoRedisPlugin : IDisposable
    {
        public enum OutletMode
        {
            MSG_ONLY,
            SHARED_BUFFER
        }
        protected static ConnectionMultiplexer redis;
        protected bool disposed = false, outletRegistered = false;
        protected Guid guid;
        protected IDatabase db;
        protected ISubscriber subscriber;
        private InletCallBack inletCallBack;

        public delegate void InletCallBack(string content);
        public readonly string inletName, outletName;
        

        public NeoRedisPlugin(string inletName, string outletName,string redisURI = "127.0.0.1:6380")
        {
            this.inletName = inletName;
            this.outletName = outletName;
            redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    Password = "password",
                    Ssl = false,
                    ConnectTimeout = 6000,
                    SyncTimeout = 6000,
                    EndPoints = { redisURI }
                });
            guid = Guid.NewGuid();
            db = redis.GetDatabase();
        }
        protected void SubscribeInletToOutlet(string channelName, InletCallBack cb)
        {
            inletCallBack = cb;
            subscriber = redis.GetSubscriber();
            subscriber.Subscribe(channelName, (msgChannel, msgValue)=> {
                inletCallBack(msgValue.ToString());
            });
        }

        public void RegisterOutlet(OutletMode outletMode, string metadata = "")
        {
            if (db.KeyExists(guid.ToString())) throw new InvalidOperationException("GUID already existed");
            switch (outletMode)
            {
                case OutletMode.MSG_ONLY:
                    db.SetAdd(outletName, guid.ToString());
                    db.StringSet(guid.ToString(), $"MSG|{metadata}");
                    break;
                case OutletMode.SHARED_BUFFER:
                    db.SetAdd(outletName, guid.ToString());
                    db.StringSet(guid.ToString(), $"MEM|{metadata}");
                    break;
                default:
                    throw new NotImplementedException("Unknown outlet mode provided");
            }
            outletRegistered = true;
        }

        public void UnregisterOutlet()
        {
            if (outletRegistered) 
            {
                db.SetRemove(outletName, guid.ToString());
                db.KeyDelete(guid.ToString());
                outletRegistered = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                if (outletRegistered)
                {
                    UnregisterOutlet();
                }
                redis.Dispose();
            }            
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        static void Main()
        {

        }
    }
}


