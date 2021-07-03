using System;
using StackExchange.Redis;
using SharedMemory;

namespace HapticSeerNeo
{
    public class NeoRedisPlugin : IDisposable
    {
        protected static ConnectionMultiplexer redis;
        protected bool disposed = false, isRegistered = false;
        protected Guid guid;
        protected IDatabase db;
        protected ISubscriber subscriber;
        protected InletCallBack inletCallBack;

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
        public void SubscribeToSingleChannel(string channelName, InletCallBack cb)
        {
            inletCallBack = cb;
            subscriber = redis.GetSubscriber();
            subscriber.Subscribe(channelName, (msgChannel, msgValue)=> {
                inletCallBack(msgValue.ToString());
            });
        }
        public bool PublishToOutlet(string content)
        {
            return db.Publish(outletName, content, CommandFlags.FireAndForget)>0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                if(isRegistered)
                    db.SetRemove(outletName, guid.ToString());
                redis.Dispose();
            }            
            disposed = true;
        }

        static void Main(string[] args)
        {
            int[] data = new int[1920 * 1080];
            int[] data1 = new int[1920 * 1080];
            int[] data2 = new int[1920 * 1080];
            SharedArray<int> prod = new SharedArray<int>("TEST", 60 * 1920 * 1080);
            SharedArray<int> com = new SharedArray<int>("TEST");
            SharedArray<int> com1 = new SharedArray<int>("TEST");

            for (int i = 0; i < 1920 * 1080; i++) data[i] = 1;
            prod.Write(data, 1920 * 1080);
            com.CopyTo(data1, 1920 * 1080);
            Console.WriteLine(data1[0]);

            for (int i = 0; i < 1920 * 1080; i++) data[i] = 2;
            prod.Write(data, 3 * 1920 * 1080 );
            com1.CopyTo(data2, 3 * 1920 * 1080 );
            Console.WriteLine(data2[0]);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
