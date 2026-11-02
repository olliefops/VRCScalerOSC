using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VRCScalerOSC.ViewModel
{
    public class ViewModel_Scaler : INotifyPropertyChanged
    {
        public static Version AppVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0); }
        }
        private SynchronizationContext? _uiContext;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void SetContext(SynchronizationContext context)
        {
            _uiContext = context;
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "null")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            if (_uiContext != null && _uiContext != SynchronizationContext.Current)
            {
                _uiContext.Post(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "null")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name == "null" ? null : name));

        // --- OSC setting ---
        private string _receivePort = "0";
        public string ReceivePort { get => _receivePort; set => SetProperty(ref _receivePort, value); }

        private string _sendPort = "9000";
        public string SendPort { get => _sendPort; set => SetProperty(ref _sendPort, value); }

        private string _ip = "127.0.0.1";
        public string IP { get => _ip; set => SetProperty(ref _ip, value); }

        private bool _randomReceiverPort = true;
        public bool RandomReceiverPort { get => _randomReceiverPort; set => SetProperty(ref _randomReceiverPort, value); }
        // --- toggle ---

        private bool _isOSCRunning = false;
        public bool IsOSCRunning { get => _isOSCRunning; set => SetProperty(ref _isOSCRunning, value); }

        private bool _isInitFinish = false;
        public bool IsInitialized { get => _isInitFinish; set => SetProperty(ref _isInitFinish, value); }

        private bool _autoAbort = true;
        public bool AutoAbort { get => _autoAbort; set => SetProperty(ref _autoAbort, value); }

        private bool _fixedRate = true;
        public bool FixedRate { get => _fixedRate; set => SetProperty(ref _fixedRate, value); }

        private bool _centimetersPerSecondRate = false;
        public bool CentimetersPerSecondRate { get => _centimetersPerSecondRate; set => SetProperty(ref _centimetersPerSecondRate, value); }

        private bool _isMultiplier = false;
        public bool IsMultiplier { get => _isMultiplier; set => SetProperty(ref _isMultiplier, value); }

        private bool _isScalingRunning = false;
        public bool IsScalingRunning { get => _isScalingRunning; set => SetProperty(ref _isScalingRunning, value); }

        private int _scalingPercentage = 0;
        public int ScalingPercentage { get => _scalingPercentage; set => SetProperty(ref _scalingPercentage, value); }

        private int _scalingCountdownSeconds = 0;
        public int ScalingCountdownSeconds { get => _scalingCountdownSeconds; set => SetProperty(ref _scalingCountdownSeconds, value); }

        private bool _scalerUnUsable = false;
        public bool ScalerUnUsable { get => _scalerUnUsable; set => SetProperty(ref _scalerUnUsable, value); }
        // --- avatar height ---
        private float _upright = 1f;
        public float Upright { get => _upright; set => SetProperty(ref _upright, value); }

        private float _realHeightRatio = -1f;
        public float RealHeightRatio { get => _realHeightRatio; set => SetProperty(ref _realHeightRatio, value); }

        private float _avatarScaleFactor = 1f;
        public float AvatarScaleFactor { get => _avatarScaleFactor; set => SetProperty(ref _avatarScaleFactor, value); }

        private float _avatarDefaultEyeHeight = 1f;
        public float AvatarDefaultEyeHeight { get => _avatarDefaultEyeHeight; set => SetProperty(ref _avatarDefaultEyeHeight, value); }

        private double _prevMemuScalingRate = 1f;
        public double PrevScalingRate { get => _prevMemuScalingRate; set => SetProperty(ref _prevMemuScalingRate, value); }

        private float _currentEyeHeight = 1f;
        public float CurrentEyeHeight { get => _currentEyeHeight; set => SetProperty(ref _currentEyeHeight, value); }

        private float _targetEyeHeight = 10f;
        public float TargetEyeHeight { get => _targetEyeHeight; set => SetProperty(ref _targetEyeHeight, value); }

        private double _currenttargetEyeHeight = 1f;
        public double CurrentTargetEyeHeight { get => _currenttargetEyeHeight; set => SetProperty(ref _currenttargetEyeHeight, value); }

        private float _minEyeHeight = 1f;
        public float MinEyeHeight { get => _minEyeHeight; set => SetProperty(ref _minEyeHeight, value); }

        private float _maxEyeHeight = 1f;
        public float MaxEyeHeight { get => _maxEyeHeight; set => SetProperty(ref _maxEyeHeight, value); }
        // --- scale value ---

        private double _scalingTime = 3f;
        public double ScalingTime { get => _scalingTime; set => SetProperty(ref _scalingTime, value); }

        private double _scalingRate = 2f;
        public double ScalingRate { get => _scalingRate; set => SetProperty(ref _scalingRate, value); }
        
        private double _scalingCentimetersPerSecond = 0f;
        public double ScalingCentimetersPerSecond { get => _scalingCentimetersPerSecond; set => SetProperty(ref _scalingCentimetersPerSecond, value); }

        private double _iterativeRate = 0f;
        public double IterativeRate { get => _iterativeRate; set => SetProperty(ref _iterativeRate, value); }

        private double _smoothScalingIterativeTimesPerSecond = 50f;
        public double SmoothScalingIterativeTimesPerSecond { get => _smoothScalingIterativeTimesPerSecond; set => SetProperty(ref _smoothScalingIterativeTimesPerSecond, value); }

        // --- scaling gesture ---
        public int _gestureMode = 0;
        /// <summary>
        /// <para>0: disable</para>
        /// <para>1: Left Trigger & Right Trigger</para>
        /// <para>2: Left Grip & Right Grip</para>
        /// <para>3: Left Trigger & Right Grip</para>
        /// <para>4: Left Grip & Right Trigger</para>
        /// <para>5: Left Trigger+Grip & Right Trigger+Grip</para>
        /// </summary>
        public int GestureMode { get => _gestureMode; set => SetProperty(ref _gestureMode, value); }

        public bool _doubleClickMuteCanSetGesture = true;
        public bool DoubleClickMuteCanSetGesture { get => _doubleClickMuteCanSetGesture; set => SetProperty(ref _doubleClickMuteCanSetGesture, value); }

        public bool _leftHandTrigger = false;
        public bool LeftHandTrigger { get => _leftHandTrigger; set => SetProperty(ref _leftHandTrigger, value); }

        public bool _leftHandGrip = false;
        public bool LeftHandGrip { get => _leftHandGrip; set => SetProperty(ref _leftHandGrip, value); }

        public bool _rightHandTrigger = false;
        public bool RightHandTrigger { get => _rightHandTrigger; set => SetProperty(ref _rightHandTrigger, value); }

        public bool _rightHandGrip = false;
        public bool RightHandGrip { get => _rightHandGrip; set => SetProperty(ref _rightHandGrip, value); }

        public bool _worldScaling = false;
        public bool WorldScaling { get => _worldScaling; set => SetProperty(ref _worldScaling, value); }

        public float _handDistanceInitial = -1f;
        public float HandDistanceInitial { get => _handDistanceInitial; set => SetProperty(ref _handDistanceInitial, value); }

        public float _gestureScalingEyeHeightInitial = -1f;
        public float GestureScalingEyeHeightInitial { get => _gestureScalingEyeHeightInitial; set => SetProperty(ref _gestureScalingEyeHeightInitial, value); }

        public bool _showGetWristInfoFailedLabel = false;
        public bool ShowGetWristInfoFailedLabel { get => _showGetWristInfoFailedLabel; set => SetProperty(ref _showGetWristInfoFailedLabel, value); }
        // --- combobox ---

        public List<double> TargetEyeHeightList { get; } = [0.01f, 0.05f, 0.1f, 0.5f, 1f, 1.5f, 2f, 3f, 5f, 10f, 15f, 20f, 30f, 50f, 100f, 150f, 200f, 300f, 500f, 1000f, 1500f, 2000f, 3000f, 5000f, 10000f];

        public List<double> ScalingTimeDefaultList { get; } = [0f, 1f, 2f, 3f, 5f, 10f, 15f, 30f, 60f, 120f, 300f, 600f, 900f, 1800f, 3600f, 7200f, 10800f, 14400f, 18000f, 21600f, 25600f, 28800f];

        public List<double> ScalingRateDefaultList { get; } = [1.1f, 1.2f, 1.3f, 1.5f, 2f, 5f, 10f, 20f, 50f, 100f, 200f, 500f, 1000f, 2000f, 5000f, 10000f];

        // --- defaultHeightValue ---
        public List<double> DefaultHeightValueList { get; } = [ 0f,
            0.01f, 0.05f, 0.1f, 0.5f, 1f,
            1.5f, 2f, 3f, 5f, 10f,
            15f, 20f, 30f, 50f, 100f,
            150f, 200f, 300f, 500f, 1000f,
            1500f, 2000f, 3000f, 5000f, 10000f,
            0.5f, 0.25f, 0.1f, 0.05f, -0.05f,
            -0.1f, -0.25f, -0.5f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f,
            0f, 0f, 0f, 0f, 0f ];
    }
}