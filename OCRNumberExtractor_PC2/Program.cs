using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Emgu.CV;
using Tesseract;

namespace HapticSeerNeo
{
    class Program
    {
        public static int[] videoBuffer;
        public static Mat mat, croppedImg;
        public static Comsumer<int> comsumer;

        private static int resetLimit = 3 * 60;
        private static SpeedImageProcess speedImageProcess = new SpeedImageProcess();
        private static int resetCounter = 0;
        private static int speed = 0; // current speed
        private static int? preSpeed = null; // previous speed
        private static TesseractEngine tesseractEngine;
        private static long sourceTick, receivedTick, processedTick;
        private static Mutex mut = new Mutex();

        public static void SpeedDetectionEvent(Comsumer<int> self, string msg)
        {
            receivedTick = DateTime.Now.Ticks;

            /* declare variables for Tesseract */
            Pix pixImage;
            Page page;
            string speedStr;
            Bitmap BitmapFrame;
#if !DEBUG
            try
            {
# endif
                unsafe {
                    fixed (int* p = videoBuffer)
                    {
                        mat = new Mat(new Size(1920, 1080), Emgu.CV.CvEnum.DepthType.Cv8U,4, (IntPtr) p, 4*1920);
                        croppedImg = new Mat(mat, new Rectangle(1725, 965, 50, 35));
                        //croppedImg.Save("SPEED.bmp");
                        sourceTick = long.Parse(msg.Split('|')[1]);
                        BitmapFrame = croppedImg.ToBitmap();
                    }
                }
                
                
                /* image processing */
                speedImageProcess.ToBlackWhite(BitmapFrame); // grayscale(black and white)
                //speedImageProcess.ResizeImage(BitmapFrame, 120, 80); // enlarge image(x2)
                pixImage = PixConverter.ToPix(BitmapFrame); // PixConverter is unable to work at Tesseract 3.3.0

                if (mut.WaitOne(20))
                {
                    page = tesseractEngine.Process(pixImage, PageSegMode.SingleBlock);
                    speedStr = page.GetText(); // Recognized result
                    BitmapFrame.Dispose();
                    page.Dispose();
                    pixImage.Dispose();
                    mut.ReleaseMutex();
                }
                else
                {
                    Console.WriteLine("Buffer timed out");
                    BitmapFrame.Dispose();
                    pixImage.Dispose();
                    return;
                }

                ///* Parse str to int */
                bool isParsable = Int32.TryParse(speedStr, out speed);
                processedTick = DateTime.Now.Ticks;
                /*Console.WriteLine($"Transmission Lantency: { (receivedTick - sourceTick) / TimeSpan.TicksPerMillisecond}, " +
    $"Processing Time: { (processedTick - receivedTick) / TimeSpan.TicksPerMillisecond}");*/
                if (preSpeed.HasValue && (!isParsable || speed < 0 || speed > 350 || Math.Abs(preSpeed.Value - speed) > 6)) // 6 = 200m/s^2
                {
                    resetCounter++;
                    Console.WriteLine($"Error: {speed}, {preSpeed}");
                    speed = preSpeed.Value; // Can't detect speed, use the previous speed value
                    if (resetLimit < resetCounter)
                    {
                        resetCounter = 0;
                        preSpeed = null;
                    }
                }
                else if (isParsable)
                {
                    preSpeed = speed;
                    if (self.outletName.Length != 0)
                    {
                        Console.WriteLine($"Speed: {speed}");
                        // Send extracted digits by the publisher to a Redis channel named as the value of "speedOulet"
                        self.PublishToOutlet($"{speed}");
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
#endif
        }

        static void Main(string[] args)
        {
            tesseractEngine = new TesseractEngine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "tessdata/KomuB", EngineMode.Default);
            if (args.Length == 0)
            {
                args = new string[] { "VIDEO", "SPEED", "1920x1080" };
            }
            int w = Int32.Parse(args[2].Split('x')[0]);
            int h = Int32.Parse(args[2].Split('x')[1]);
            videoBuffer = new int[w * h];
            mat = new Mat(h, w, Emgu.CV.CvEnum.DepthType.Cv8U, 4);

            Console.WriteLine("Press any key to start extraction...");
            _ = Console.ReadKey();
            comsumer = new Comsumer<int>(args[0], args[1], w * h, ref videoBuffer, SpeedDetectionEvent);
            _ = Console.ReadKey();
        }
    }
}
