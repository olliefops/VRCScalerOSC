using VRCScalerOSC.Controller;
using VRCScalerOSC.Service;
using VRCScalerOSC.ViewModel;

namespace VRCScalerOSC.Model.SupportAvatarTool
{
    internal class VRCScaler(Controller_Scaler controller, ViewModel_Scaler viewModel) : SupportAvatarTool
    {
        private bool _muteState;
        private bool _earmuffsState;
        private bool _waitMtueDoubleClick;
        private float _prevMoveHorizontalValue;
        private float _prevMoveVerticalValue;
        private float _prevLookHorizontalValue;
        private float _prevLookVerticalValue;
        private System.Threading.Timer? _waitMtueDoubleClickTimer;
        private System.Threading.Timer? _setScaleGestureDelay;
        private readonly Dictionary<int, float> _templateHeight = [];
        public override void InitOSCFunctions(OscEventCollection functions)
        {
            #region Scaler
            functions.AddEvent("/avatar/parameters/MuteSelf", DoubleClickMuteCanSetGesture);
            functions.AddEvent("/avatar/parameters/Earmuffs", DoubleClickMuteCanSetGesture);
            functions.AddEvent("/avatar/parameters/GestureLeft", (isInitialized, service, data) =>
            {
                if (data.ValueI.HasValue)
                {
                    switch (data.ValueI.Value)
                    {
                        case 0: //Neutral
                            viewModel.LeftHandTrigger = false;
                            viewModel.LeftHandGrip = false;
                            break;
                        case 1: //Fist
                            viewModel.LeftHandTrigger = true;
                            viewModel.LeftHandGrip = true;
                            break;
                        case 2: //HandOpen
                            viewModel.LeftHandTrigger = false;
                            viewModel.LeftHandGrip = false;
                            break;
                        case 3: //FingerPoint
                            viewModel.LeftHandTrigger = false;
                            viewModel.LeftHandGrip = true;
                            break;
                        case 4: //Victory
                            viewModel.LeftHandTrigger = false;
                            viewModel.LeftHandGrip = false;
                            break;
                        case 5: //RockNRoll
                            viewModel.LeftHandTrigger = true;
                            viewModel.LeftHandGrip = false;
                            break;
                        case 6: //HandGun
                            viewModel.LeftHandTrigger = false;
                            viewModel.LeftHandGrip = true;
                            break;
                        case 7: //ThumbsUp
                            viewModel.LeftHandTrigger = true;
                            viewModel.LeftHandGrip = true;
                            break;
                    }
                }
            });
            functions.AddEvent("/avatar/parameters/GestureRight", (isInitialized, service, data) =>
            {
                if (data.ValueI.HasValue)
                {
                    switch (data.ValueI.Value)
                    {
                        case 0: //Neutral
                            viewModel.RightHandTrigger = false;
                            viewModel.RightHandGrip = false;
                            break;
                        case 1: //Fist
                            viewModel.RightHandTrigger = true;
                            viewModel.RightHandGrip = true;
                            break;
                        case 2: //HandOpen
                            viewModel.RightHandTrigger = false;
                            viewModel.RightHandGrip = false;
                            break;
                        case 3: //FingerPoint
                            viewModel.RightHandTrigger = false;
                            viewModel.RightHandGrip = true;
                            break;
                        case 4: //Victory
                            viewModel.RightHandTrigger = false;
                            viewModel.RightHandGrip = false;
                            break;
                        case 5: //RockNRoll
                            viewModel.RightHandTrigger = true;
                            viewModel.RightHandGrip = false;
                            break;
                        case 6: //HandGun
                            viewModel.RightHandTrigger = false;
                            viewModel.RightHandGrip = true;
                            break;
                        case 7: //ThumbsUp
                            viewModel.RightHandTrigger = true;
                            viewModel.RightHandGrip = true;
                            break;
                    }
                }
            });

            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/MovingPuppetOn", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && !data.ValueB.Value)
                {
                    service?.SendOscMessage(new OSCData("/input/Horizontal", "f", 0f));
                    service?.SendOscMessage(new OSCData("/input/Vertical", "f", 0f));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/Jump", async (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    service?.SendOscMessage(OSCData.GetTrueOSCData("/input/Jump", "T"));
                    await Task.Delay(500);
                    service?.SendOscMessage(OSCData.GetFalseOSCData("/input/Jump", "F"));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/Horizontal", (isInitialized, service, data) =>
            {
                float speed = 0;
                if (isInitialized && data.ValueF.HasValue && MathF.Abs(data.ValueF.Value) > 0.3f)
                {
                    speed = MathF.Round(data.ValueF.Value,1);
                    if (_prevMoveHorizontalValue != speed)
                    {
                        _prevMoveHorizontalValue = speed;
                        speed *= (viewModel.AvatarScaleFactor > 1f ? 1f : viewModel.AvatarScaleFactor);
                        speed = MathF.Abs(speed) < 0.11f ? MathF.Sign(data.ValueF.Value) * 0.11f : speed;
                        service?.SendOscMessage(new OSCData("/input/Horizontal", "f", speed));
                        if (MathF.Abs(data.ValueF.Value) > 0.95f)
                        {
                            service?.SendOscMessage(OSCData.GetTrueOSCData("/input/Run", "T"));
                        }
                        else
                        {
                            service?.SendOscMessage(OSCData.GetFalseOSCData("/input/Run", "F"));
                        }
                    }
                }
                else if(_prevMoveHorizontalValue != 0f)
                {
                    _prevLookHorizontalValue = 0f;
                    service?.SendOscMessage(new OSCData("/input/Horizontal", "f", 0f));
                    service?.SendOscMessage(OSCData.GetFalseOSCData("/input/Run", "F"));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/Vertical", (isInitialized, service, data) => 
            {
                float speed = 0;
                if (isInitialized && data.ValueF.HasValue && MathF.Abs(data.ValueF.Value) > 0.3f)
                {
                    speed = MathF.Round(data.ValueF.Value, 1);
                    if (_prevMoveVerticalValue != speed)
                    {
                        _prevMoveVerticalValue = speed;
                        speed *= (viewModel.AvatarScaleFactor > 1f ? 1f : viewModel.AvatarScaleFactor);
                        speed = MathF.Abs(speed) < 0.11f ? MathF.Sign(data.ValueF.Value) * 0.11f : speed;
                        service?.SendOscMessage(new OSCData("/input/Vertical", "f", speed));
                        if (MathF.Abs(data.ValueF.Value) > 0.95f)
                        {
                            service?.SendOscMessage(OSCData.GetTrueOSCData("/input/Run", "T"));
                        }
                        else
                        {
                            service?.SendOscMessage(OSCData.GetFalseOSCData("/input/Run", "F"));
                        }
                    }
                }
                else if(_prevMoveVerticalValue != 0f)
                {
                    _prevMoveVerticalValue = 0f;
                    service?.SendOscMessage(new OSCData("/input/Vertical", "f", 0f));
                    service?.SendOscMessage(OSCData.GetFalseOSCData("/input/Run", "F"));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/LookHorizontal", (isInitialized, service, data) =>
            {
                float speed = 0;
                if (isInitialized && data.ValueF.HasValue && MathF.Abs(data.ValueF.Value) > 0.3f)
                {
                    speed = MathF.Round(data.ValueF.Value, 1);
                    if (_prevLookHorizontalValue != speed)
                    {
                        if (MathF.Abs(speed) == 0.5f)
                        {
                            speed += MathF.Sign(speed) * 0.01f;
                        }
                        _prevLookHorizontalValue = speed;
                        service?.SendOscMessage(new OSCData("/input/LookHorizontal", "f", speed));
                    }
                }
                else if (_prevLookHorizontalValue != 0f)
                {
                    _prevLookHorizontalValue = 0f;
                    service?.SendOscMessage(new OSCData("/input/LookHorizontal", "f", 0f));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/LookVertical", (isInitialized, service, data) =>
            {
                float speed = 0;
                if (isInitialized && data.ValueF.HasValue && MathF.Abs(data.ValueF.Value) > 0.3f)
                {
                    speed = MathF.Round(data.ValueF.Value, 1) / 2f;
                    if (_prevLookVerticalValue != speed)
                    {
                        if (MathF.Abs(speed) == 0.1f)
                        {
                            speed += MathF.Sign(speed) * 0.01f;
                        }
                        _prevLookVerticalValue = speed;
                        service?.SendOscMessage(new OSCData("/input/LookVertical", "f", speed));
                    }
                }
                else if (_prevLookVerticalValue != 0f)
                {
                    _prevLookVerticalValue = 0f;
                    service?.SendOscMessage(new OSCData("/input/LookVertical", "f", 0f));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/UseRight", async (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    service?.SendOscMessage(OSCData.GetTrueOSCData("/input/UseRight", "T"));
                    await Task.Delay(500);
                    service?.SendOscMessage(OSCData.GetFalseOSCData("/input/UseRight", "F"));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Input/UseLeft", async (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    service?.SendOscMessage(OSCData.GetTrueOSCData("/input/UseLeft", "T"));
                    await Task.Delay(500);
                    service?.SendOscMessage(OSCData.GetFalseOSCData("/input/UseLeft", "F"));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ScaleNow", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingByTime(viewModel.TargetEyeHeight, 0f); //Scaling setting value w/o smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Meters/ScaleNow", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingInMetersByTime(viewModel.TargetEyeHeight, 0f); //Scaling setting value in meters without smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Multiplier/ScaleNow", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingInMultiplierByTime(viewModel.TargetEyeHeight, 0f); //Scaling setting value in multiplier without smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SmoothScaleStart", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, viewModel.TargetEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling setting value w smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Meters/SmoothScaleStart", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, viewModel.TargetEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate); //Scaling setting value in meters with smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Multiplier/SmoothScaleStart", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScaling(true, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, viewModel.TargetEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate); //Scaling setting value in multiplier with smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ScalingNow", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingByTime(viewModel.TargetEyeHeight, 0f); //Scaling setting value w/o smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Meters/ScalingNow", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingInMetersByTime(viewModel.TargetEyeHeight, 0f); //Scaling setting value in meters without smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Multiplier/ScalingNow", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingInMultiplierByTime(viewModel.TargetEyeHeight, 0f); //Scaling setting value in multiplier without smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SmoothScalingStart", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, viewModel.TargetEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling setting value w smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Meters/SmoothScalingStart", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, viewModel.TargetEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling setting value in meters with smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Multiplier/SmoothScalingStart", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScaling(true, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, viewModel.TargetEyeHeight, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling setting value in multiplier with smooth time
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ScalingEyeHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScalingByTime(data.ValueF.Value, 0f);//Scaling a input value without smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Meters/ScalingEyeHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScalingInMetersByTime(data.ValueF.Value, 0f);//Scaling a input value in meters without smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Multiplier/ScalingEyeHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScalingInMultiplierByTime(data.ValueF.Value, 0f);//Scaling a input value in multiplier without smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SmoothScalingEyeHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, data.ValueF.Value, viewModel.ScalingTime); //Scaling a input value with smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Meters/SmoothScalingEyeHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(false, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, data.ValueF.Value, viewModel.ScalingTime); //Scaling a input value in meters with smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Multiplier/SmoothScalingEyeHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(true, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, data.ValueF.Value, viewModel.ScalingTime); //Scaling a input in multiplier value with smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ScalingPercentage", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(false, false, false, data.ValueF.Value * viewModel.CurrentEyeHeight / 100f); //Scaling a input value in percentage without smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SmoothScalingPercentage", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(false, viewModel.FixedRate, false, data.ValueF.Value * viewModel.CurrentEyeHeight / 100f, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling a input value in percentage with smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ScalingDiffPercentage", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(false, false, false, viewModel.CurrentEyeHeight * (1 + data.ValueF.Value / 100f)); //Scaling a input value in diff-percentage with smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SmoothScalingDiffPercentage", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    controller.StartScaling(false, viewModel.FixedRate, false, viewModel.CurrentEyeHeight * (1 + data.ValueF.Value / 100f), viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling a input value in diff-percentage with smoothtime
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetEyeHeight", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    //set target height in meters
                    if (viewModel.IsMultiplier)
                    {
                        viewModel.TargetEyeHeight = data.ValueF.Value / viewModel.AvatarDefaultEyeHeight;
                    }
                    else
                    {
                        viewModel.TargetEyeHeight = data.ValueF.Value;
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetMultiplier", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    //set target height in multiplier 
                    if (viewModel.IsMultiplier)
                    {
                        viewModel.TargetEyeHeight = data.ValueF.Value;
                    }
                    else
                    {
                        viewModel.TargetEyeHeight = viewModel.AvatarDefaultEyeHeight * data.ValueF.Value;
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetPercentage", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    //set target height in percentage
                    if (viewModel.IsMultiplier)
                    {
                        viewModel.TargetEyeHeight = viewModel.AvatarScaleFactor * data.ValueF.Value / 100f;
                    }
                    else
                    {
                        viewModel.TargetEyeHeight = viewModel.CurrentEyeHeight * data.ValueF.Value / 100f;
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetDiffPercentage", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue && data.ValueF.Value != 0)
                {
                    //set target height in diff-percentage 
                    if (viewModel.IsMultiplier)
                    {
                        viewModel.TargetEyeHeight = viewModel.AvatarScaleFactor * (1 + data.ValueF.Value / 100f);
                    }
                    else
                    {
                        viewModel.TargetEyeHeight = viewModel.CurrentEyeHeight * (1 + data.ValueF.Value / 100f);
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetScalingTime", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    if (isInitialized)
                    {
                        if (data.ValueF.Value != 0)
                        {
                            controller.SetScalingTime(data.ValueF.Value < 0.01 ? 0f : MathF.Round(data.ValueF.Value));//set scaling time by imput value
                        }
                    }
                    else
                    {
                        service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/ScalingTimeValue", "f", viewModel.ScalingTime));
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetScalingRate", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    if (isInitialized)
                    {
                        if (data.ValueF.Value != 0)
                        {
                            controller.SetScalingRate(data.ValueF.Value);
                        }
                    }
                    else
                    {
                        service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/ScalingRateValue", "f", viewModel.ScalingRate));
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SwitchAutoAbort", (isInitialized, service, data) =>
            {
                if (viewModel.AutoAbort && data.ValueB.HasValue && !data.ValueB.Value) //switch auto-abort toggle on / off
                {
                    viewModel.AutoAbort = false;
                }
                else if (!viewModel.AutoAbort && data.ValueB.HasValue && data.ValueB.Value)
                {
                    viewModel.AutoAbort = true;
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetMaxEyeHeight", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    if (isInitialized && data.ValueF.Value > 0)
                    {
                        controller.SetMaxEyeHeight(data.ValueF.Value);//set max-height to imput value
                    }
                    else if (!isInitialized)
                    {
                        service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/MaxEyeHeightValue", "f", viewModel.MaxEyeHeight));
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetMinEyeHeight", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    if (isInitialized && data.ValueF.Value > 0f)
                    {
                        controller.SetMinEyeHeight(data.ValueF.Value);//set min-height to imput value
                    }
                    else if (!isInitialized)
                    {
                        service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/MinEyeHeightValue", "f", viewModel.MinEyeHeight));
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SwitchFixedRate", (isInitialized, service, data) =>
            {
                if (viewModel.FixedRate && data.ValueB.HasValue && !data.ValueB.Value) //switch fiexd rate toggle on / off
                {
                    viewModel.FixedRate = false;
                }
                else if (!viewModel.FixedRate && data.ValueB.HasValue && data.ValueB.Value)
                {
                    viewModel.FixedRate = true;
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetFixedRate", (isInitialized, service, data) =>
            {
                if (data.ValueB.HasValue && data.ValueB.Value)
                {
                    viewModel.FixedRate = true; //switch fiexd rate toggle on
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetFixedTime", (isInitialized, service, data) =>
            {
                if (data.ValueB.HasValue && data.ValueB.Value)
                {
                    viewModel.FixedRate = false; //switch fiexd rate toggle off
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/IsMultiplier", (isInitialized, service, data) =>
            {
                viewModel.IsMultiplier = data.ValueB.HasValue && data.ValueB.Value; //switch using real size toggle
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/GrowUp", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    controller.ScaleGrowUp(data.ValueF.Value);
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ShrinkDown", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    controller.ScaleShrinkDown(data.ValueF.Value);
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/StopScaling", (isInitialized, service, data) =>
            {
                controller.StopScaling(); //Stop Scaling
                service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/ScaleState", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    if (data.ValueF > 0.001f)
                    {
                        controller.ScaleGrowUp();
                    }
                    else if (data.ValueF < -0.001f)
                    {
                        controller.ScaleShrinkDown();
                    }
                    else
                    {
                        controller.StopScaling(); //Stop Scaling
                        service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/SetScaleCentimetersPerSecond", (isInitialized, service, data) =>
            {
                if (data.ValueF.HasValue)
                {
                    controller.SetFixedRate(true);
                    controller.SetCentimetersPerSecondRate(true);
                    controller.SetScalingRateByCentimeterPerSecond(data.ValueF.Value);
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/BackAvatarDefaultHeight", (isInitialized, service, data) =>
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    controller.StartScalingByTime();
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Gesture/Mode", (isInitialized, service, data) =>
            {
                if (data.ValueI.HasValue && data.ValueI.Value >= 0 && data.ValueI.Value <= 5)
                {
                    if (viewModel.IsInitialized) //switch Scaling Gesture Mode 
                    {
                        if (data.ValueI.Value != 0)
                        {
                            service?.IgnoreAddrListRemove("#bundle");
                            viewModel.ShowGetWristInfoFailedLabel = true;
                        }
                        else
                        {
                            viewModel.ShowGetWristInfoFailedLabel = false;
                        }
                        viewModel.GestureMode = data.ValueI.Value;
                    }
                    else
                    {
                        viewModel.GestureMode = 0;
                        viewModel.HandDistanceInitial = -1f;
                        viewModel.ShowGetWristInfoFailedLabel = data.ValueI.Value != 0;
                    }
                }
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Gesture/WorldScaling", (isInitialized, service, data) =>
            {
                viewModel.WorldScaling = data.ValueB.HasValue && data.ValueB.Value;
            });
            functions.AddEvent($"{controller.ScalerOSCPathPrefix}/Gesture/DoubleMuteSetGesture", (isInitialized, service, data) =>
            {
                viewModel.DoubleClickMuteCanSetGesture = data.ValueB.HasValue && data.ValueB.Value;
            });
            for (int i = 0; i < viewModel.DefaultHeightValueList.Count; i++)
            {
                functions.AddEvent($"{controller.ScalerOSCPathPrefix}/DefaultValue{i}/Value", GetScalerDefaultValue_Value);
            }
            #endregion
        }
        public override Action<bool, Service_VRCOSCProtocols?, OSCData>? TryAddNewFunction(OSCData initialData)
        {
            if (viewModel == null)
            {
                return null;
            }
            if (initialData.IsVRSystem)
            {
                return (isInitialized, service, data) =>
                {
                    viewModel.ShowGetWristInfoFailedLabel = false;
                    if (viewModel.GestureMode == 0)
                    {
                        service?.IgnoreAddrListAdd(data, "#bundle");
                        controller.GestureScaling(float.NaN);
                    }
                    else if (viewModel.IsInitialized && data.ValueF.HasValue)
                    {
                        //Debug.WriteLine((float)data.Value);                    
                        switch (viewModel.GestureMode)
                        {
                            case 1: //Left Trigger & Right Trigger
                                if (viewModel.LeftHandTrigger && viewModel.RightHandTrigger)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 2: //Left Grip & Right Grip
                                if (viewModel.LeftHandGrip && viewModel.RightHandGrip)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 3: //Left Trigger & Right Grip
                                if (viewModel.LeftHandTrigger && viewModel.RightHandGrip)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 4: //Left Grip & Right Trigger
                                if (viewModel.LeftHandGrip && viewModel.RightHandTrigger)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 5: //Left Trigger+Grip & Right Trigger+Grip
                                if (viewModel.LeftHandTrigger && viewModel.LeftHandGrip && viewModel.RightHandTrigger && viewModel.RightHandGrip)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 6: //Left Trigger+Grip & Right Trigger
                                if (viewModel.LeftHandTrigger && viewModel.LeftHandGrip && viewModel.RightHandTrigger)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 7: //Left Trigger+Grip & Right Trigger
                                if (viewModel.LeftHandTrigger && viewModel.LeftHandGrip && viewModel.RightHandGrip)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 8: //Left Trigger & Right Trigger+Grip
                                if (viewModel.LeftHandTrigger && viewModel.RightHandTrigger && viewModel.RightHandGrip)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            case 9: //Left Grip & Right Trigger+Grip
                                if ( viewModel.LeftHandGrip && viewModel.RightHandTrigger && viewModel.RightHandGrip)
                                {
                                    controller.GestureScaling(data.ValueF.Value);
                                    break;
                                }
                                controller.GestureScaling(float.NaN);
                                break;
                            default: //disable
                                controller.GestureScaling(float.NaN);
                                break;
                        }
                    }
                    else
                    {
                        controller.GestureScaling(float.NaN);
                    }
                };
            }
            else if (viewModel.DefaultHeightValueList != null && GetScalerDefaultValueIndex(initialData, out int index))
            {
                if (initialData.Addr.EndsWith("/SetValue"))
                {
                    return GetScalerDefaultValue_SetValue;
                }
                if (initialData.Addr.EndsWith("/Scaling"))
                {
                    return GetScalerDefaultValue_Scaling;
                }
                if (initialData.Addr.EndsWith("/Smooth"))
                {
                    return GetScalerDefaultValue_Smooth;
                }
                if (initialData.Addr.EndsWith("/PercentageScaling"))
                {
                    return GetScalerDefaultValue_PercentageScaling;
                }
                if (initialData.Addr.EndsWith("/DiffPercentageScaling"))
                {
                    return GetScalerDefaultValue_DiffPercentageScaling;
                }
                if (initialData.Addr.EndsWith("/PercentageSmooth"))
                {
                    return GetScalerDefaultValue_PercentageSmooth;
                }
                if (initialData.Addr.EndsWith("/DiffPercentageSmooth"))
                {
                    return GetScalerDefaultValue_DiffPercentageSmooth;
                }
                if (initialData.Addr.EndsWith("/Save"))
                {
                    return GetScalerDefaultValue_Save;
                }
                if (initialData.Addr.EndsWith("/Delete"))
                {
                    return GetScalerDefaultValue_Delete;
                }
                if (initialData.Addr.EndsWith("/InputValue"))
                {
                    return GetScalerDefaultValue_InputValue;
                }
                return null;
            }
            return null;
        }
        #region Get Scaler DefaultValue OSC
        private bool GetScalerDefaultValueIndex(OSCData data, out int index)
        {
            index = -1;
            if (!data.Addr.StartsWith($"{controller.ScalerOSCPathPrefix}/DefaultValue"))
            {
                return false;
            }
            var defaultValuePath = data.Addr[($"{controller.ScalerOSCPathPrefix}/DefaultValue").Length..];
            return (defaultValuePath.IndexOf('/') >= 0 && int.TryParse(defaultValuePath[..defaultValuePath.IndexOf('/')], out index) && index >= 0 && index < viewModel.DefaultHeightValueList.Count);
        }
        private void GetScalerDefaultValue_Value(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (!isInitialized)
            {
                if (GetScalerDefaultValueIndex(data, out int index))
                {
                    if (_templateHeight.TryGetValue(index, out float valueF))
                    {
                        service?.SendOscMessage(new OSCData(data.Addr, "f", valueF));
                    }
                    else
                    {
                        service?.SendOscMessage(new OSCData(data.Addr, "f", viewModel.DefaultHeightValueList[index]));
                    }
                }

            }
        }
        private void GetScalerDefaultValue_SetValue(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized)
                {
                    if (data.ValueB.HasValue && data.ValueB.Value && !_templateHeight.ContainsKey(index))
                    {
                        SetScalerDefaultValue(index, service);
                    }
                    else if (data.ValueB.HasValue && !data.ValueB.Value && _templateHeight.ContainsKey(index))
                    {
                        DeleteScalerDefaultValue(index, service);
                    }
                }
                else if (!isInitialized)
                {
                    if (_templateHeight.TryGetValue(index, out _))
                    {
                        if (data.ValueB.HasValue && !data.ValueB.Value)
                        {
                            service?.SendOscMessage(OSCData.GetTrueOSCData(data.Addr, data.TypeString));
                        }
                    }
                    else
                    {
                        if (data.ValueB.HasValue && data.ValueB.Value)
                        {
                            service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                        }
                    }
                }
            }
        }
        private void GetScalerDefaultValue_Scaling(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    double value;
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    if (_templateHeight.TryGetValue(index, out float valueF))
                    {
                        value = valueF;
                    }
                    else
                    {
                        value = viewModel.DefaultHeightValueList[index]; // Scaling a default value without smoothtime
                    }
                    value = value == 0f ? -1f : value;
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, value);
                }
            }
        }
        private void GetScalerDefaultValue_Smooth(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    double value;
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    if (_templateHeight.TryGetValue(index, out float valueF))
                    {
                        value = valueF;
                    }
                    else
                    {
                        value = viewModel.DefaultHeightValueList[index]; // Scaling a default value with smoothtime
                    }
                    value = value == 0f ? -1f : value;
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, value, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond);
                }
            }
        }
        private void GetScalerDefaultValue_PercentageScaling(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    double value;
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    if (_templateHeight.TryGetValue(index, out float valueF))
                    {
                        value = valueF;
                    }
                    else
                    {
                        value = viewModel.DefaultHeightValueList[index];//Scaling a default percentage value without smoothtime
                    }
                    value *= viewModel.CurrentEyeHeight;
                    value = value == 0f ? -1f : value;
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, value); //Scaling a input value in percentage without smoothtime
                }
            }
        }
        private void GetScalerDefaultValue_DiffPercentageScaling(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    double value;
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    if (_templateHeight.TryGetValue(index, out float valueF))
                    {
                        value = valueF;
                    }
                    else
                    {
                        value = viewModel.DefaultHeightValueList[index];//Scaling a default diff-percentage value without smoothtime
                    }
                    value = viewModel.CurrentEyeHeight * (1 + value);
                    value = value == 0f ? -1f : value;
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, value); //Scaling a input value in diff-percentage without smoothtime
                }
            }
        }
        private void GetScalerDefaultValue_PercentageSmooth(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    double value;
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    if (!_templateHeight.TryGetValue(index, out float valueF))
                    {
                        value = valueF;
                    }
                    else
                    {
                        value = viewModel.DefaultHeightValueList[index];//Scaling a default percentage value with smoothtime
                    }
                    value *= viewModel.CurrentEyeHeight;
                    value = value == 0f ? -1f : value;
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, value, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond);  //Scaling a input value in percentage with smoothtime
                }
            }
        }
        private void GetScalerDefaultValue_DiffPercentageSmooth(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index))
            {
                if (isInitialized && data.ValueB.HasValue && data.ValueB.Value)
                {
                    double value;
                    service?.SendOscMessage(OSCData.GetFalseOSCData(data.Addr, data.TypeString));
                    if (_templateHeight.TryGetValue(index, out float valueF))
                    {
                        value = valueF;
                    }
                    else
                    {
                        value = viewModel.DefaultHeightValueList[index];//Scaling a default diff-percentage value with smoothtime
                    }
                    value = viewModel.CurrentEyeHeight * (1 + value);
                    value = value == 0f ? -1f : value;
                    controller.StartScaling(viewModel.IsMultiplier, viewModel.FixedRate, viewModel.CentimetersPerSecondRate, value, viewModel.ScalingTime, viewModel.ScalingRate, viewModel.ScalingCentimetersPerSecond); //Scaling a input value in diff-percentage with smoothtime
                }
            }
        }
        private void GetScalerDefaultValue_Save(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (isInitialized && data.ValueB.HasValue && data.ValueB.Value && GetScalerDefaultValueIndex(data, out int index))
            {
                service?.SendOscMessage(OSCData.GetTrueOSCData($"{controller.ScalerOSCPathPrefix}/DefaultValue{index}/SetValue", "T"));
                //SetScalerDefaultValue(index, service);
            }
        }
        private void GetScalerDefaultValue_Delete(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (isInitialized && data.ValueB.HasValue && data.ValueB.Value && GetScalerDefaultValueIndex(data, out int index))
            {
                service?.SendOscMessage(OSCData.GetFalseOSCData($"{controller.ScalerOSCPathPrefix}/DefaultValue{index}/SetValue", "F"));
                //DeleteScalerDefaultValue(index, service);
            }
        }
        private void GetScalerDefaultValue_InputValue(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (GetScalerDefaultValueIndex(data, out int index) && data.ValueF.HasValue && data.ValueF.Value != 0)
            {
                if (data.ValueF.Value > 0)
                {
                    SetScalerDefaultValue(index, service, data.ValueF.Value);
                }
                else
                {
                    DeleteScalerDefaultValue(index, service);
                }
                service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/DefaultValue{index}/InputValue", "f", 0f));
            }
        }
        private void SetScalerDefaultValue(int index, Service_VRCOSCProtocols? service, float value = float.MaxValue)
        {
            _templateHeight.Add(index, -1f);
            if (value == float.MaxValue)
            {
                value = viewModel.CurrentEyeHeight;
            }
            if (viewModel.IsMultiplier)
            {
                _templateHeight[index] = value / viewModel.AvatarDefaultEyeHeight;//save current height (x) to Default Height value list
            }
            else
            {
                _templateHeight[index] = value;//save current height (m) to Default Height value list
            }
            service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/DefaultValue{index}/Value", "f", _templateHeight[index]));
        }
        private void DeleteScalerDefaultValue(int index, Service_VRCOSCProtocols? service)
        {
            _templateHeight.Remove(index);// delete Default Height value
            service?.SendOscMessage(new OSCData($"{controller.ScalerOSCPathPrefix}/DefaultValue{index}/Value", "f", viewModel.DefaultHeightValueList[index]));
        }

        #endregion
        private void DoubleClickMuteCanSetGesture(bool isInitialized, Service_VRCOSCProtocols? service, OSCData data)
        {
            if (data.ValueB.HasValue)
            {
                if (isInitialized)
                {
                    if (viewModel.DoubleClickMuteCanSetGesture)
                    {
                        if (_waitMtueDoubleClick)
                        {
                            if (data.Addr.EndsWith("MuteSelf") && _muteState != data.ValueB.Value)
                            {
                                _waitMtueDoubleClick = false;
                                controller.SetGestureScaling(GetGestureAutoMode());
                            }
                            else if (data.Addr.EndsWith("Earmuffs") && _earmuffsState != data.ValueB.Value)
                            {
                                _waitMtueDoubleClick = false;
                                _setScaleGestureDelay ??= new((state) => { controller.SetGestureScaling(GetGestureAutoMode()); });
                                _setScaleGestureDelay.Change(500, Timeout.Infinite);
                            }
                        }
                        else
                        {
                            _waitMtueDoubleClick = true;
                            _waitMtueDoubleClickTimer ??= new((state) => { _waitMtueDoubleClick = false; });
                            _waitMtueDoubleClickTimer.Change(1000, Timeout.Infinite);
                        }
                    }
                    if (data.Addr.EndsWith("MuteSelf"))
                    {
                        _muteState = data.ValueB.Value;
                    }
                    else if (data.Addr.EndsWith("Earmuffs"))
                    {
                        _earmuffsState = data.ValueB.Value;
                    }
                }
                else if (data.Addr.EndsWith("MuteSelf"))
                {
                    if (_muteState != data.ValueB.Value)
                    {
                        _muteState = data.ValueB.Value;
                    }
                }
                else if (data.Addr.EndsWith("Earmuffs"))
                {
                    if (_earmuffsState != data.ValueB.Value)
                    {
                        _earmuffsState = data.ValueB.Value;
                    }
                }
            }
        }
        private int GetGestureAutoMode()
        {
            if (viewModel.LeftHandTrigger && viewModel.LeftHandGrip && viewModel.RightHandTrigger && viewModel.RightHandGrip)
            {
                return viewModel.GestureMode == 5 ? 0 : 5;//Left Trigger+Grip & Right Trigger+Grip
            }
            if (viewModel.LeftHandTrigger && viewModel.LeftHandGrip && viewModel.RightHandTrigger)
            {
                return viewModel.GestureMode == 6 ? 0 : 6;//Left Trigger+Grip & Right Trigger
            }
            if (viewModel.LeftHandTrigger && viewModel.LeftHandGrip && viewModel.RightHandGrip)
            {
                return viewModel.GestureMode == 7 ? 0 : 7;//Left Trigger+Grip & Right Grip
            }
            if (viewModel.LeftHandTrigger && viewModel.RightHandTrigger && viewModel.RightHandGrip)
            {
                return viewModel.GestureMode == 8 ? 0 : 8;//Left Trigger & Right Trigger+Grip
            }
            if (viewModel.LeftHandGrip && viewModel.RightHandTrigger && viewModel.RightHandGrip)
            {
                return viewModel.GestureMode == 9 ? 0 : 9;//Left Grip & Right Trigger+Grip
            }
            if (viewModel.LeftHandTrigger && viewModel.RightHandTrigger)
            {
                return viewModel.GestureMode == 1 ? 0 : 1;//Left Trigger & Right Trigger
            }
            if (viewModel.LeftHandGrip && viewModel.RightHandGrip)
            {
                return viewModel.GestureMode == 2 ? 0 : 2;//Left Grip & Right Grip
            }
            if (viewModel.LeftHandTrigger && viewModel.RightHandGrip)
            {
                return viewModel.GestureMode == 3 ? 0 : 3;//Left Trigger & Right Grip
            }
            if (viewModel.LeftHandGrip && viewModel.RightHandTrigger)
            {
                return viewModel.GestureMode == 4 ? 0 : 4;//Left Grip & Right Trigger
            }
            return 0;
        }
    }
}
