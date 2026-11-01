using System.Net;

namespace VRCScalerOSC.Model
{
    public class ScalerSetting
    {
        public string OSCPathPrefix = "/avatar/parameters/VRCScaleOSC";
        public string OSCPathPrefixForMCC = "/avatar/parameters/MCC";
        public bool UsingOSCQuery = true;
        public IPAddress ServerOSC_IP = IPAddress.Loopback;
        public int ServerOSC_SendPort = 9000;
        public int ServerOSC_ReceivePort = 0;
        public bool ServerOSC_RandomReceiverPort = true;
        public int SendTaskDelay = 5;
        public int SmoothScalingIterativeTimesPerSecond = 50;
        public float FormTargetEyeHeight = 10f;
        public double FormScalingTime = 3f;
        public double FormScalingRate = 2f;
        public bool FormFixedRate = true;
        public bool FormMSRate = true;
        public bool FormAutoAbort = true;
        public float MaxHeight = 10000f;
        public float MinHeight = 0.01f;
        public List<double> FormTargetEyeHeightList = [0.01f, 0.05f, 0.1f, 0.5f, 1f, 1.5f, 2f, 3f, 5f, 10f, 15f, 20f, 30f, 50f, 100f, 150f, 200f, 300f, 500f, 1000f, 1500f, 2000f, 3000f, 5000f, 10000f];
        public List<double> FormScalingTimeList = [0f, 1f, 2f, 3f, 5f, 10f, 15f, 30f, 60f, 120f, 300f, 600f, 900f, 1800f, 3600f, 7200f, 10800f, 14400f, 18000f, 21600f, 25600f, 28800f];
        public List<double> FormScalingRateList = [1.1f, 1.2f, 1.3f, 1.5f, 2f, 5f, 10f, 20f, 50f, 100f, 200f, 500f, 1000f, 2000f, 5000f, 10000f];
        public List<double> MenuScaleValueList = [0.01f, 0.05f, 0.1f, 0.5f, 1f, 1.5f, 2f, 3f, 5f, 10f, 15f, 20f, 30f, 50f, 100f, 150f, 200f, 300f, 500f, 1000f, 1500f, 2000f, 3000f, 5000f, 10000f, 0.5f, 0.25f, 0.1f, 0.05f, -0.05f, -0.1f, -0.25f, -0.5f];
    }
}
