using ImageProcessModule;
using System;
using System.Runtime.InteropServices;

namespace ScreenCapture
{
    class ScreenCapturer
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        private static SetConsoleCtrlEventHandler InstanceOfConsoleCtrlHandler;
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        
        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        
        public static long startTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public static BitmapBuffer bitmapBuffer = new BitmapBuffer();
        public static CaptureMethod captureMethod;

        /// <summary>
        /// Parse Argument to get the Capture Method
        /// </summary>
        /// <param name="args">Args from Main</param>
        /*static void ArgumentParser(string[] args)
        {
            if (String.Compare(args[0], "Local", StringComparison.OrdinalIgnoreCase) == 0)
                captureMethod = new LocalCapture(bitmapBuffer); //Default: Local Capture
            else if (String.Compare(args[0], "CaptureCard", StringComparison.OrdinalIgnoreCase) == 0)
                captureMethod = new CardCapture(bitmapBuffer);  //Fetch Image From Capture Card
            else
            {
                // Unknown Argument
                Console.WriteLine();
                Console.WriteLine("Wrong Argument!");
                Console.WriteLine("Usage:");
                Console.WriteLine($"\t{Process.GetCurrentProcess().ProcessName} [ImageSource]");
                Console.WriteLine($"\t[ImageSource]: Local or CaptureCard");
                Console.WriteLine();
                // Force close the process
                Process.GetCurrentProcess().Kill();
            }
        }*/

        private static bool Handler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                    Console.WriteLine("Closing and cleanup...");
                    captureMethod.Stop();
                    Console.WriteLine("Done! Press any key to exit...");
                    _ = Console.ReadKey();
                    Environment.Exit(0);
                    return false;
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    Console.WriteLine("Closing and cleanup...");
                    captureMethod.Stop();
                    Console.WriteLine("Done! Press any key to exit...");
                    _ = Console.ReadKey();
                    Environment.Exit(0);
                    return false;

                default:
                    return false;
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start capturing...");
            _ = Console.ReadKey();
            InstanceOfConsoleCtrlHandler = new SetConsoleCtrlEventHandler(Handler);
            SetConsoleCtrlHandler(InstanceOfConsoleCtrlHandler, true);
            captureMethod = new LocalCapture();

            // Check the Capture Method from args
            //if (args.Length == 0)
            //    captureMethod = new LocalCapture(bitmapBuffer); // Default: Local Capture
            //else
            //    ArgumentParser(args); // Parse Arguments

            //if (captureMethod == null)
            //    throw new Exception("Error! CaptureMethod is null!");

            // Start Capture
            captureMethod.Start();

            // Start dispatch frames
            bitmapBuffer.StartDispatchToImageProcessBase();
           
            // Do Cache Optimizer
            CacheOptimizer.Init();
            CacheOptimizer.ResetAllAffinity();

            Console.WriteLine("Started.");
        }

    }

}
