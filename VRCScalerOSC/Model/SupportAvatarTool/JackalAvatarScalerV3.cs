using VRCScalerOSC.Controller;
using VRCScalerOSC.Service;
using VRCScalerOSC.ViewModel;

namespace VRCScalerOSC.Model.SupportAvatarTool
{
    internal class JackalAvatarScalerV3(Controller_Scaler controller, ViewModel_Scaler viewModel) : SupportAvatarTool
    {
        public override void InitOSCFunctions(OscEventCollection supportAbatarToolOSCFuns)
        {
            supportAbatarToolOSCFuns.AddEvent("/avatar/parameters/ScaleFactorInverse", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    ScaleFactorInverse = data.ValueF.Value;
                    DefaultEyeHeight = EyeHeightAsMeters * ScaleFactorInverse;
                }
            });
            supportAbatarToolOSCFuns.AddEvent("/avatar/parameters/EyeHeightAsMeters", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    EyeHeightAsMeters = data.ValueF.Value;
                    DefaultEyeHeight = EyeHeightAsMeters * ScaleFactorInverse;
                }
            });
        }
        public override Action<bool, Service_VRCOSCProtocols?, OSCData>? TryAddNewFunction(OSCData initialData)
        {
            if (initialData.Addr.StartsWith("/avatar/parameters/", StringComparison.Ordinal))
            {
                string param = initialData.Addr.ToString().Replace("/avatar/parameters/", "");
                if (param.Contains("LocalJackalScale"))
                {
                    return (isInitialized, service, data) =>
                    {
                        if (isInitialized && data.ValueI.HasValue)
                        {
                            float nextEyeHeight = DefaultEyeHeight;
                            switch (data.ValueI.Value)
                            {
                                //case 0: NextEyeHeight *= 1f; break;
                                case 1: nextEyeHeight *= 0.01f; break;
                                case 2: nextEyeHeight *= 0.025f; break;
                                case 3: nextEyeHeight *= 0.05f; break;
                                case 4: nextEyeHeight *= 0.1f; break;
                                case 5: nextEyeHeight *= 0.2f; break;
                                case 6: nextEyeHeight *= 0.4f; break;
                                case 7: nextEyeHeight *= 0.6f; break;
                                case 8: nextEyeHeight *= 0.8f; break;
                                case 9: nextEyeHeight *= 1.25f; break;
                                case 10: nextEyeHeight *= 1.5f; break;
                                case 11: nextEyeHeight *= 1.75f; break;
                                case 12: nextEyeHeight *= 2f; break;
                                case 13: nextEyeHeight *= 2.5f; break;
                                case 14: nextEyeHeight *= 3f; break;
                                case 15: nextEyeHeight *= 5f; break;
                                case 16: nextEyeHeight *= 10f; break;
                                case 17: nextEyeHeight *= 15f; break;
                                case 18: nextEyeHeight *= 20f; break;
                                case 19: nextEyeHeight *= 30f; break;
                                case 20: nextEyeHeight *= 40f; break;
                                case 21: nextEyeHeight *= 50f; break;
                                case 22: nextEyeHeight *= 75f; break;
                                case 23: nextEyeHeight *= 100f; break;
                                case 24: nextEyeHeight *= 150f; break;
                                case 25: nextEyeHeight *= 200f; break;
                                case 26: nextEyeHeight *= 300f; break;
                                case 27: nextEyeHeight *= 500f; break;
                                case 28: nextEyeHeight *= 1000f; break;
                                case 29: nextEyeHeight *= 2000f; break;
                                case 30: nextEyeHeight *= 4000f; break;
                                case 31: nextEyeHeight *= 6000f; break;
                                case 32: nextEyeHeight *= 8000f; break;
                                case 33: nextEyeHeight *= 10000f; break;
                                case 34: nextEyeHeight *= 50000f; break;
                                case 35: nextEyeHeight *= 100000f; break;
                                default: return;
                            }
                            if (nextEyeHeight < 0.01f)
                            {
                                nextEyeHeight = 0.01f;
                            }
                            if (nextEyeHeight > 10000)
                            {
                                nextEyeHeight = 10000;
                            }
                            if (isInitialized)
                            {
                                service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                            }
                            controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, nextEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond);
                        }
                    };
                }
                else if (param.Contains("JackalScale"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (IsInitialized)
                        {
                            service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                        }
                    };
                }
            }
            return null;
        }
    }
}
