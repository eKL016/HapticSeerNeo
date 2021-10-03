using System;
using System.IO;
using System.Text;
namespace PC2Detectors
{
    public static class InertiaFunctions
    {
        const double HANDLER_MAX_ANGLE = 15d;
        public static void Router(string channelName, string msg, ref StateObject state)
        {
            if (channelName == state.xinputInlet)
            {
                int headerPos = msg.IndexOf('|');
                if (msg.Substring(0, headerPos) == "ThumbLX")
                    UpdateNormalState(msg.Substring(headerPos + 1), ref state);
            }
            else if (channelName == state.speedInlet)
            {
                UpdateSpeedState(msg, ref state);
            }
        }
        public static void UpdateSpeedState(string msg, ref StateObject state)
        {
            string[] splited = msg.Split(',');
            try
            {
                ushort.TryParse(splited[0], out ushort parsedSpeed);
                if (parsedSpeed == 0) return;
                state.Speed = parsedSpeed;
                state.Angle = state.LastAngle;
                var accX = state.AccelX;
                var accY = state.AccelY;
#if DEBUG
                Console.WriteLine($"{accX.ToString()}|{accY.ToString()}");
#endif
                state.inertiaPublisher.PublishToOutlet($"{accX.ToString()}|{accY.ToString()}");
            }
            catch (Exception e) 
            {
                if (e is IndexOutOfRangeException) throw e;
                Console.WriteLine(e.ToString());
            }
        }
        public static void UpdateNormalState(string msg, ref StateObject state)
        {
            
            try
            {
                short.TryParse( msg.Split('|')[1], out short parsedHandler );
                state.LastAngle = (double) parsedHandler / (double) short.MaxValue * HANDLER_MAX_ANGLE;
            } 
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException) throw e;
                Console.WriteLine(e.Message);
            }
        }
    }
}
