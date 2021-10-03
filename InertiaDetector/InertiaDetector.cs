using HapticSeerNeo;
using System;

namespace PC2Detectors
{
    class InertiaDetector
    {
        private Subscriber speedSubscriber, inputSubscriber;
        private Publisher  inertiaPublisher;
        private StateObject state;

        public InertiaDetector(string url, ushort port, 
            string speedInlet, string xinputInlet, 
            string accOutlet)
        {
            try
            {
                using (inertiaPublisher = new Publisher(accOutlet))
                {
                    state = new StateObject(inertiaPublisher)
                    {
                        speedInlet = speedInlet,
                        xinputInlet = xinputInlet,
                        accOutlet = accOutlet
                    };
                    using (inputSubscriber = new Subscriber(xinputInlet, msg => InertiaFunctions.Router(xinputInlet, msg, ref state)))
                    using (speedSubscriber = new Subscriber(speedInlet, msg => InertiaFunctions.Router(speedInlet, msg, ref state)))
                    {
                        _ = Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                inertiaPublisher.Dispose();
                throw e;
            }
        }
    }
}
