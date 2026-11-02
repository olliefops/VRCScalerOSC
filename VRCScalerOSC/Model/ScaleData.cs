using System.Diagnostics;

namespace VRCScalerOSC.Model
{
    public class ScaleData
    {
        public double SmoothScalingIterativeTimeInterval
        {
            get { return 1000f / SmoothScalingIterativeTimesPerSecond; }
        }
        public double SmoothScalingIterativeTimesPerSecond { get; }
        public float HeightOrginal { get; }
        public double HeightTarget { get; }
        public float HeightMin { get; }
        public float HeightMax { get; }
        public double ScalingTime { get; }
        public double CentimetersPerSecond { get; }

        public float RealScalingTime
        {
            get
            {
                _realScalingTime ??= _stopwatch?.ElapsedMilliseconds ?? 0;
                return _realScalingTime.Value / 1000f;
            }
            set
            {
                _realScalingTime = value;
            }
        }
        private readonly Stopwatch? _stopwatch;

        private float? _realScalingTime;
        private double _heightNow;
        private double _iterativeRate;
        
        public bool IsAutoAbort { get; set; } = false;
        public double HeightNow
        {
            get { return _heightNow; }
        }
        public double IterativeRate
        {
            get { return _iterativeRate; }
        }
        public bool AutoAbort { get; }
        public ScaleData(double eyeheightTarget, float eyeheightNow, double scalingTime = 0f, double centimetersPerSecond = 0d, bool autoAbort = true, double iterativeRate = 1f, double smoothScalingIterativeTimesPerSecond = 50f, float eyeheightMin = 0.01f, float eyeheightMax = 10000f)
        {
            HeightMin = eyeheightMin;
            HeightMax = eyeheightMax;
            HeightOrginal = eyeheightNow;
            CentimetersPerSecond = centimetersPerSecond;
            SmoothScalingIterativeTimesPerSecond = smoothScalingIterativeTimesPerSecond;
            HeightTarget = eyeheightTarget < eyeheightMin ? eyeheightMin : eyeheightTarget > eyeheightMax ? eyeheightMax : eyeheightTarget;
            _heightNow = eyeheightNow < eyeheightMin ? eyeheightMin : eyeheightNow > eyeheightMax ? eyeheightMax : eyeheightNow;
            AutoAbort = scalingTime != 0f && autoAbort;
            _iterativeRate = iterativeRate;
            ScalingTime = scalingTime;
            HeightMin = eyeheightMin;
            HeightMax = eyeheightMax;
            _stopwatch = Stopwatch.StartNew();
        }
        public double NextEyeheight
        {
            get
            {
                if (CentimetersPerSecond > 0d)
                {
                    double delta = (CentimetersPerSecond / 100d) / SmoothScalingIterativeTimesPerSecond;

                    if (_heightNow < HeightTarget)
                        _heightNow += delta;
                    else if (_heightNow > HeightTarget)
                        _heightNow -= delta;

                    if (Math.Abs(_heightNow - HeightTarget) <= delta)
                        _heightNow = HeightTarget;

                    return _heightNow;
                }
                else
                {
                    if (_heightNow < 0.05f && Math.Abs(_heightNow - _heightNow * _iterativeRate) < 0.001f)
                    {
                        if (_iterativeRate > 1)
                        {
                            _heightNow += 0.001f;
                        }
                        if (_iterativeRate < 1)
                        {
                            _heightNow -= 0.001f;
                        }
                    }
                    else
                    {
                        _heightNow *= _iterativeRate;
                    }

                    if (_iterativeRate == 1f ||
                        (_iterativeRate > 1f && _heightNow > HeightTarget) ||
                        (_iterativeRate < 1f && _heightNow < HeightTarget) ||
                        Math.Abs(_heightNow - HeightTarget) / _heightNow < 0.01f)
                    {
                        _heightNow = HeightTarget;
                    }
                    return _heightNow;   
                }
            }
        }
        public bool IsFinish
        {
            get
            {
                return Math.Abs(_heightNow - HeightTarget) / _heightNow < 0.01f;
            }
        }
        public void UpdateIterativeRate(double newIterativeRate)
        {
            _iterativeRate = newIterativeRate;
        }
    }
}
