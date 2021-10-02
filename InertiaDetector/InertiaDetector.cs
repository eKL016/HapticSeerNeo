using HapticSeerNeo;
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
            using (inertiaPublisher = new Publisher(accOutlet))
            using (speedSubscriber = new Subscriber(speedInlet, msg => InertiaFunctions.Router(speedInlet, msg, ref state)))
            using(inputSubscriber = new Subscriber(xinputInlet, msg => InertiaFunctions.Router(xinputInlet, msg, ref state))){
                state = new StateObject(inertiaPublisher);
                state.speedInlet = speedInlet;
                state.xinputInlet = xinputInlet;
                state.accOutlet = accOutlet;
            }
        }
    }
}
