using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HapticSeerNeo
{
    class TestProducer
    {
        //private static Producer<int> testProducer = new Producer<int>("TEST", 60*1920*1080);
        private static int cnt = 0;
        private static void myEvent(object source, ElapsedEventArgs e) 
        {
            //testProducer.WriteBuffer(cnt);
            cnt = (cnt+1) % 60;
        }
        static void Main(string[] args)
        {
            var myTimer = new System.Timers.Timer();
            myTimer.Interval = 16;
            myTimer.Enabled = true;
            myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            _ = Console.ReadKey();
            //testProducer.Dispose();
        }
    }
}
