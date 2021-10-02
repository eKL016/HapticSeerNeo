using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HapticSeerNeo
{
    public class Subscriber : MsgUser
    {
        public Subscriber(string inletName, InletCallBack cb = null) : base(inletName, "")
        {
            SubscribeInletToOutlet(inletName, cb);
        }
    }
}
