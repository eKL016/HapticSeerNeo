using static StackExchange.Redis.CommandFlags;

namespace HapticSeerNeo
{
    public class Publisher : MsgUser
    {
        public bool PublishToOutlet(string message)
        {
            if (outletRegistered)
            {
                return db.Publish(outletName, message, FireAndForget) > 0;
            }
            else
            {
                return false;
            }
        }

        public Publisher(string outletName):base("", outletName)
        {
            RegisterOutlet(OutletMode.MSG_ONLY);
        }
    }
}
