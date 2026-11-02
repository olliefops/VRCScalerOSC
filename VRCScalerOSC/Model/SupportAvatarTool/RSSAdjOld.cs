using VRCScalerOSC.Controller;
using VRCScalerOSC.Service;
using VRCScalerOSC.ViewModel;

namespace VRCScalerOSC.Model.SupportAvatarTool
{
    public class RSSAdjOld(Controller_Scaler controller, ViewModel_Scaler viewModel) : SupportAvatarTool
    {
        private float RSSAdjScaleAdjDisplay = 0f;
        private float RSSAdjScaleMultiplier = 100f;
        private bool RSSAdjScaleRealSize = true;
        public override void InitOSCFunctions(OscEventCollection supportAbatarToolOSCFuns)
        {
            supportAbatarToolOSCFuns.AddEvent("/avatar/parameters/ScaleFactorInverse", (IsInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    ScaleFactorInverse = data.ValueF.Value;
                    DefaultEyeHeight = EyeHeightAsMeters * ScaleFactorInverse;
                }
            });
            supportAbatarToolOSCFuns.AddEvent("/avatar/parameters/EyeHeightAsMeters", (IsInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    EyeHeightAsMeters = data.ValueF.Value;
                    DefaultEyeHeight = EyeHeightAsMeters * ScaleFactorInverse;
                }
            });
        }
        public override Action<bool, Service_VRCOSCProtocols?, OSCData>? TryAddNewFunction(OSCData data)
        {
            if (data.Addr.StartsWith("/avatar/parameters/", StringComparison.Ordinal))
            {
                string param = data.Addr.ToString().Replace("/avatar/parameters/", "");
                if (param.Contains("RSSAdj/Scale/RealSize") || param.Contains("RealRate"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (data.ValueB.HasValue)
                        {
                            RSSAdjScaleRealSize = data.ValueB.Value;
                            controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, ScaleModify(), viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond);
                        }
                    };
                }
                else if (param.Contains("RSSAdj/Scale/FlagDisplay") || param.Contains("ScaleFlagDisplay"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (data.ValueI.HasValue)
                        {
                            RSSAdjScaleMultiplier = data.ValueI.Value switch
                            {
                                1 => 0.01f,
                                2 => 10f,
                                3 => 1000f,
                                4 => 10000f,
                                5 => 100000f,
                                _ => 100f,
                            };
                            controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, ScaleModify(), viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond);
                        }
                    };
                }
                else if (param.Contains("RSSAdj/Scale/AdjDisplay") || param.Contains("ScaleAdjDisplay"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (data.ValueF.HasValue)
                        {
                            RSSAdjScaleAdjDisplay = data.ValueF.Value;
                            controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, ScaleModify(), viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond);
                        }
                    };
                }
                else if (param.Contains("RSSAdj/Scale/AdjTpose"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (IsInitialized && data.ValueF.HasValue && data.ValueF.Value != 0f)
                        {
                            service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                        }
                    };
                }
                else if (param.Contains("RSSAdj/Scale/AdjSyncInt") || param.Contains("ScaleAdjSyncInt"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (IsInitialized && data.ValueF.HasValue && data.ValueF.Value > 101f && data.ValueF.Value <= 201f)
                        {
                            service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                        }
                    };
                }
                else if (param.Contains("RSSAdj/Scale/Adj"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (IsInitialized && data.ValueF.HasValue && data.ValueF.Value != 0f)
                        {
                            service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                        }
                    };
                }
                else if (param.Contains("ScaleAdjTpose"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (IsInitialized && data.ValueF.HasValue && data.ValueF.Value != 0f)
                        {
                            service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                        }
                    };
                }
                else if (param.Contains("ScaleAdj") && !param.Contains("OnScaleAdjMenu"))
                {
                    return (IsInitialized, service, data) =>
                    {
                        if (IsInitialized && data.ValueF.HasValue && data.ValueF.Value != 0f)
                        {
                            service?.SendOscMessage(new OSCData("/avatar/parameters/" + param, data.TypeString, 0));
                        }
                    };
                }
            }
            return null;
        }
        private float ScaleModify()
        {
            float newEyeHeight;
            if (RSSAdjScaleAdjDisplay < 0.01)
            {
                newEyeHeight = 0f;
            }
            else if (RSSAdjScaleMultiplier == 0.01f)
            {
                if (1 - RSSAdjScaleAdjDisplay >= 0.01)
                {
                    newEyeHeight = 1 - RSSAdjScaleAdjDisplay;
                }
                else
                {
                    newEyeHeight = 0.01f;
                }
            }
            else if (RSSAdjScaleMultiplier * RSSAdjScaleAdjDisplay >= 10000)
            {
                newEyeHeight = 10000;
            }
            else
            {
                newEyeHeight = RSSAdjScaleMultiplier * RSSAdjScaleAdjDisplay;
            }

            if (RSSAdjScaleRealSize)
            {
                return newEyeHeight;
            }
            else
            {
                return newEyeHeight * DefaultEyeHeight;
            }
        }
    }
}
