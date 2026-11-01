using System.Globalization;
using System.Net;
using VRCScalerOSC.Model;

namespace VRCScalerOSC.Controller
{
    public class Controller_ScalerSetting
    {
        public static ScalerSetting ImportCustomSetting(string filePath)
        {
            ScalerSetting customSetting = new();
            ImportCustomSetting(customSetting, filePath);
            return customSetting;
        }
        public static void ImportCustomSetting(ScalerSetting customSetting, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return;
                }
                string[] data = File.ReadAllLines(filePath);
                foreach (string line in data)
                {
                    if (line.Contains('='))
                    {
                        string value = line.Split('#')[0].Split('=')[1];
                        short shortS;
                        float valueF;
                        switch (line.Split('#')[0].Split('=')[0])
                        {
                            case "ScalerOSCPathPrefix":
                                customSetting.OSCPathPrefix = value;
                                break;
                            case "MenuControlCameraOSCPathPrefix":
                                customSetting.OSCPathPrefixForMCC = value;
                                break;
                            case "UsingOSCQuery":
                            case "UseOSCQuery":
                                if (value == "1" || value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("t", StringComparison.CurrentCultureIgnoreCase) || value.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    customSetting.UsingOSCQuery = true;
                                }
                                if (value == "0" || value.Equals("false", StringComparison.CurrentCultureIgnoreCase) || value.Equals("f", StringComparison.CurrentCultureIgnoreCase) || value.Equals("n", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    customSetting.UsingOSCQuery = false;
                                }
                                break;
                            case "SendTaskDelay":
                                if (short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out shortS))
                                {
                                    customSetting.SendTaskDelay = shortS;
                                }
                                break;
                            case "SmoothScalingIterativeTimesPerSecond":
                                if (short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out shortS))
                                {
                                    customSetting.SmoothScalingIterativeTimesPerSecond = shortS;
                                }
                                break;
                            case "OSC_IP":
                                if (IPAddress.TryParse(value, out IPAddress? ip) && ip != null) customSetting.ServerOSC_IP = ip;
                                break;
                            case "OSC_SendPort":
                                if (short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out shortS))
                                {
                                    customSetting.ServerOSC_SendPort = shortS;
                                }
                                break;
                            case "OSC_ReceivePort":
                                if (short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out shortS))
                                {
                                    customSetting.ServerOSC_ReceivePort = shortS;
                                    customSetting.ServerOSC_RandomReceiverPort = shortS == 0;
                                }
                                break;
                            case "DefaultTargetEyeHeight":
                                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out valueF))
                                {
                                    if (valueF < 0.01f) valueF = 0.01f;
                                    if (valueF > 10000f) valueF = 10000f;
                                    customSetting.FormTargetEyeHeight = valueF;
                                }
                                break;
                            case "DefaultScalingTime":
                                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out valueF) && valueF >= 0)
                                {
                                    customSetting.FormScalingTime = valueF;
                                }
                                break;
                            case "DefaultScalingRate":
                                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out valueF) && valueF > 0)
                                {
                                    customSetting.FormScalingRate = valueF;
                                }
                                break;
                            case "UseFixedRate":
                                if (value == "1" || value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("t", StringComparison.CurrentCultureIgnoreCase) || value.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    customSetting.FormFixedRate = true;
                                }
                                if (value == "0" || value.Equals("false", StringComparison.CurrentCultureIgnoreCase) || value.Equals("f", StringComparison.CurrentCultureIgnoreCase) || value.Equals("n", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    customSetting.FormFixedRate = false;
                                }
                                break;
                            case "UseAutoAbort":
                                if (value == "1" || value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("t", StringComparison.CurrentCultureIgnoreCase) || value.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    customSetting.FormAutoAbort = true;
                                }
                                if (value == "0" || value.Equals("false", StringComparison.CurrentCultureIgnoreCase) || value.Equals("f", StringComparison.CurrentCultureIgnoreCase) || value.Equals("n", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    customSetting.FormAutoAbort = false;
                                }
                                break;
                            case "MinHeight":
                                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out valueF) && valueF <= 1f && valueF >= 0.01f)
                                {
                                    customSetting.MinHeight = valueF;
                                }
                                break;
                            case "MaxHeight":
                                if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out valueF) && valueF <= 10000f && valueF >= 1f)
                                {
                                    customSetting.MaxHeight = valueF;
                                }
                                break;
                            case "TargetEyeHeightSelectItems":
                                LoadUserDataList(customSetting.FormTargetEyeHeightList, value);
                                break;
                            case "ScalingTimeSelectItems":
                                LoadUserDataList(customSetting.FormScalingTimeList, value);
                                break;
                            case "ScalingRateSelectItems":
                                LoadUserDataList(customSetting.FormScalingRateList, value);
                                break;
                            case "ScalerMenuSelectItems":
                                LoadUserDataList(customSetting.MenuScaleValueList, value, false);
                                break;
                        }
                    }
                }
            }
            catch
            { }
            return;
        }
        private static void LoadUserDataList(List<double> list, string listString, bool clear = true)
        {
            if (listString != "")
            {
                if (clear)
                {
                    list.Clear();
                }
                int n = 0;
                foreach (string value in listString.Split('|'))
                {
                    if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueF))
                    {
                        if (n < list.Count)
                        {
                            list[n] = valueF;
                        }
                        else
                        {
                            list.Add(valueF);
                        }
                        n++;
                    }
                }
            }
        }
        public static void ExportCustomSetting(ScalerSetting customSetting, string filePath, string productVersion)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                List<string> data =
                [
                    $"#CustomSetting for VRCScalerOSC (v{productVersion})",
                    "",
                    "#System Setting",
                    $"ScalerOSCPathPrefix={customSetting.OSCPathPrefix}",
                    $"MenuControlCameraOSCPathPrefix={customSetting.OSCPathPrefixForMCC}",
                    $"UsingOSCQuery={(customSetting.UsingOSCQuery ? "Y" : "N")}",
                    $"SendTaskDelay={customSetting.SendTaskDelay}",
                    $"SmoothScalingIterativeTimesPerSecond={customSetting.SmoothScalingIterativeTimesPerSecond}",
                    $"MaxHeight={customSetting.MaxHeight}",
                    $"MinHeight={customSetting.MinHeight}",
                    "#Height Range: MinHeight: 0.01 to 1, MaxHeight: 1 to 10000",
                    "",
                    "#OSC server Setting",
                    $"OSC_IP={customSetting.ServerOSC_IP}",
                    $"OSC_SendPort={customSetting.ServerOSC_SendPort}",
                    customSetting.ServerOSC_RandomReceiverPort ? $"OSC_ReceivePort=0" : $"OSC_ReceivePort={customSetting.ServerOSC_ReceivePort}",
                    "#If OSC_ReceivePort is 0, it will be randomly selected from 9010 to 9100.",
                    "",
                    "#Form Setting",
                    "#ComboBox default selected value",
                    $"DefaultTargetEyeHeight={customSetting.FormTargetEyeHeight}",
                    $"DefaultScalingTime={customSetting.FormScalingTime}",
                    $"DefaultScalingRate={customSetting.FormScalingRate}",
                    $"UseFixedRate={(customSetting.FormFixedRate ? "Y" : "N")}",
                    $"UseAutoAbort={(customSetting.FormAutoAbort ? "Y" : "N")}",
                    "#ComboBox item values",
                    "#Format: Decimal point . and numbers separated by |",
                    $"TargetEyeHeightSelectItems={ListToString(customSetting.FormTargetEyeHeightList)}",
                    $"ScalingTimeSelectItems={ListToString(customSetting.FormScalingTimeList)}",
                    $"ScalingRateSelectItems={ListToString(customSetting.FormScalingRateList)}",
                    "",
                    "#Scaler Menu Select Item Setting (also use in Form quick menu)",
                    "#Format: Decimal point . and numbers separated by |",
                    $"ScalerMenuSelectItems={ListToString(customSetting.MenuScaleValueList)}",
                ];
                File.WriteAllLines(filePath, [.. data]);
            }
            catch
            { }
        }
        private static string ListToString(List<double> list)
        {
            string returnText = "";
            foreach (float item in list)
            {
                returnText += item.ToString("0.##", CultureInfo.InvariantCulture) + "|";
            }
            return returnText.Trim('|');
        }
    }
}
