using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HapticSeerNeo
{
    public class Subscriber : MsgUser
    {
        private int timeOut = 30000;
        public bool isPublisherFound = false, isListening = false;

        public Subscriber(string inletName, InletCallBack cb = null) : base(inletName, "")
        {
            isPublisherFound = System.Threading.SpinWait.SpinUntil(() => {
                return !db.SetRandomMember(this.inletName).IsNull;
            }, timeOut);
            if (isPublisherFound)
            {
                SubscribeInletToOutlet(inletName, cb);
                isListening = true;
            }
            else
            {
                throw new Exception($"Timed-out after {timeOut / 1000} seconds waiting for a publisher");
            }
            
        }
    }
}
