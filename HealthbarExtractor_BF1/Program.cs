using System;
using System.Drawing;
using Emgu.CV;

namespace HapticSeerNeo
{
    class Program
    {
        public static int[] videoBuffer;
        public static Mat mat, croppedImg;
        public static Comsumer<int> comsumer;
        private static long sourceTick, receivedTick, processedTick;

       static void GetHealthBarArea(Comsumer<int> comsumer, string msg)
        {
            mat.SetTo<int>(videoBuffer);
            receivedTick = DateTime.Now.Ticks;
            sourceTick = long.Parse(msg.Split('|')[1]);
            double BloodValue;
            bool IsRedImpluse = false;
            croppedImg = new Mat(mat, new Rectangle(1700, 1000, 166, 25));
            unsafe
            {
                byte* OriginalImageByteArray = (byte*)croppedImg.DataPointer;

                int Offset = 0;
                int Area = 0;
                //Only read the first row
                for (int x = 0; x < croppedImg .Width; ++x)
                {
                    //White
                    if (OriginalImageByteArray[Offset + 2] > 180 && OriginalImageByteArray[Offset] > 180)
                        Area++;

                    //Red
                    else if (OriginalImageByteArray[Offset + 2] > 120)
                    {
                        //Is Leftmost?
                        if (x != 4)
                            Area++;
                        else if (x == 4)//Left shouldn't be red. It must be impluse
                            IsRedImpluse = true;
                    }
                    Offset += 4;
                }
                BloodValue = Area / (double)(croppedImg .Width);
                processedTick = DateTime.Now.Ticks;
            }
            
            Console.WriteLine($"Transmission Lantency: { (receivedTick - sourceTick) / TimeSpan.TicksPerMillisecond}, " +
                $"Processing Time: { (processedTick - receivedTick) / TimeSpan.TicksPerMillisecond}");
            if (!IsRedImpluse)
            {
                comsumer.PublishToOutlet(BloodValue.ToString());
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { "VIDEO", "HEALTH", "1920x1080" };
            }
            int w = Int32.Parse(args[2].Split('x')[0]);
            int h = Int32.Parse(args[2].Split('x')[1]);
            videoBuffer = new int[w * h];
            mat = new Mat(h, w, Emgu.CV.CvEnum.DepthType.Cv8U, 4);

            Console.WriteLine("Press any key to start extraction...");
            _ = Console.ReadKey();
            comsumer = new Comsumer<int>(args[0], args[1], w*h, ref videoBuffer, GetHealthBarArea);
            _  = Console.ReadKey();
        }
    }
}
