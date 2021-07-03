//using Emgu.CV;
//using HapticSeerNeo;
//using ImageProcessModule.ProcessingClass;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Drawing;
//using static ImageProcessModule.ImageProcessBase;

//namespace ScreenCapture
//{
//    class ImageForwarder : IDisposable
//    {
//        private List<ImageProcess> ImageProcessesList = new List<ImageProcess>();
//        private string outletChannelName;
//        private Producer<int> producer;
//        private int[] tempArray;
//        private void sendToSharedBuffer(ImageProcess sender, Mat mat)
//        {
//            mat.CopyTo<int>(tempArray);
//            producer.WriteBuffer(tempArray);
//        }
//        public ImageForwarder(string outletChannelName, int width, int height, int frameRate=60)
//        {
//            this.outletChannelName = outletChannelName;
//            this.producer = new Producer<int>(this.outletChannelName, width*height, 5);
//            this.tempArray = new int[width * height];

//            ImageProcessesList.Add(new ImageProcess(0, 1, 0, 1, ImageScaleType.OriginalSize, FrameRate: frameRate));
//            ImageProcessesList.Last().NewFrameArrivedEvent += sendToSharedBuffer;
//        }
//        public void Dispose()
//        {
//            ImageProcessesList.Last().NewFrameArrivedEvent -= sendToSharedBuffer;
//            producer.Dispose(true);
//        }
//    }
//}