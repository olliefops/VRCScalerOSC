using System.Diagnostics;
using System.Net;
using System.Text;
using VRCScalerOSC.Controller;
using VRCScalerOSC.Localization;
using VRCScalerOSC.Model;
using VRCScalerOSC.ViewModel;

namespace VRCScalerOSC.View
{
    public class ScalerCosole
    {
        private readonly Controller_Scaler _controller_Scaler;
        private readonly ViewModel_Scaler _viewModelScaler;
        private readonly ScalerSetting _customSetting;
        private readonly Localization_enUS _loc = new();
        private readonly LinkedList<string> _commandHistory = new();
        private int _commandHistoryIndex = -1;
        private int _lastWidth = 0;
        private int _page = 1;
        private int _cursorN = 20;
        private char _cursor = '_';
        private char _loadingAnimationChar = ' ';
        public ScalerCosole(Controller_Scaler controller)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            _customSetting = controller.CustomSetting;
            _viewModelScaler = controller.ViewModelScaler;
            _controller_Scaler = controller;
        }

        public void Run()
        {
            _controller_Scaler.OSCSetup();
            StringBuilder inputBuffer = new();
            StringBuilder screenBuilder = new();

            var dfH01 = _viewModelScaler.DefaultHeightValueList[1].ToString("0.##m").PadRight(7).AsSpan();
            var dfH02 = _viewModelScaler.DefaultHeightValueList[2].ToString("0.##m").PadRight(7).AsSpan();
            var dfH03 = _viewModelScaler.DefaultHeightValueList[3].ToString("0.##m").PadRight(7).AsSpan();
            var dfH04 = _viewModelScaler.DefaultHeightValueList[4].ToString("0.##m").PadRight(7).AsSpan();
            var dfH05 = _viewModelScaler.DefaultHeightValueList[5].ToString("0.##m").PadRight(7).AsSpan();
            var dfH06 = _viewModelScaler.DefaultHeightValueList[6].ToString("0.##m").PadRight(7).AsSpan();
            var dfH07 = _viewModelScaler.DefaultHeightValueList[7].ToString("0.##m").PadRight(7).AsSpan();
            var dfH08 = _viewModelScaler.DefaultHeightValueList[8].ToString("0.##m").PadRight(7).AsSpan();
            var dfH09 = _viewModelScaler.DefaultHeightValueList[9].ToString("0.##m").PadRight(7).AsSpan();
            var dfH10 = _viewModelScaler.DefaultHeightValueList[10].ToString("0.##m").PadRight(7).AsSpan();
            var dfH11 = _viewModelScaler.DefaultHeightValueList[11].ToString("0.##m").PadRight(7).AsSpan();
            var dfH12 = _viewModelScaler.DefaultHeightValueList[12].ToString("0.##m").PadRight(7).AsSpan();
            var dfH13 = _viewModelScaler.DefaultHeightValueList[13].ToString("0.##m").PadRight(7).AsSpan();
            var dfH14 = _viewModelScaler.DefaultHeightValueList[14].ToString("0.##m").PadRight(7).AsSpan();
            var dfH15 = _viewModelScaler.DefaultHeightValueList[15].ToString("0.##m").PadRight(7).AsSpan();
            var dfH16 = _viewModelScaler.DefaultHeightValueList[16].ToString("0.##m").PadRight(7).AsSpan();
            var dfH17 = _viewModelScaler.DefaultHeightValueList[17].ToString("0.##m").PadRight(7).AsSpan();
            var dfH18 = _viewModelScaler.DefaultHeightValueList[18].ToString("0.##m").PadRight(7).AsSpan();
            var dfH19 = _viewModelScaler.DefaultHeightValueList[19].ToString("0.##m").PadRight(7).AsSpan();
            var dfH20 = _viewModelScaler.DefaultHeightValueList[20].ToString("0.##m").PadRight(7).AsSpan();
            var dfH21 = _viewModelScaler.DefaultHeightValueList[21].ToString("0.##m").PadRight(7).AsSpan();
            var dfH22 = _viewModelScaler.DefaultHeightValueList[22].ToString("0.##m").PadRight(7).AsSpan();
            var dfH23 = _viewModelScaler.DefaultHeightValueList[23].ToString("0.##m").PadRight(7).AsSpan();
            var dfH24 = _viewModelScaler.DefaultHeightValueList[24].ToString("0.##m").PadRight(7).AsSpan();
            var dfH25 = _viewModelScaler.DefaultHeightValueList[25].ToString("0.##m").PadRight(7).AsSpan();

            Console.CursorVisible = false;
            while (true)
            {
                _cursorN -= 1;
                _cursorN = _cursorN < 0 ? 20 : _cursorN;
                _cursor = (_cursorN < 10 ? ' ' : '_');
                var versionS = $"v{ViewModel_Scaler.AppVersion.Major}.{ViewModel_Scaler.AppVersion.Minor}.{ViewModel_Scaler.AppVersion.Build}".PadRight(10).AsSpan();
                var oscPortS = _viewModelScaler.SendPort.AsSpan();
                var oscPortR = _viewModelScaler.ReceivePort.AsSpan();
                var oscIp = _viewModelScaler.IP.AsSpan();
                var crHeight = EyeHeight_Format(_viewModelScaler.CurrentEyeHeight).PadRight(10).AsSpan();
                var dfHeight = EyeHeight_Format(_viewModelScaler.AvatarDefaultEyeHeight).PadRight(10).AsSpan();
                var scalTime = _viewModelScaler.ScalingTime.ToString("0.#####s").PadRight(10).AsSpan();
                var scalRate = _viewModelScaler.ScalingRate.ToString("0.#####x").PadRight(10).AsSpan();
                var scalMS = _viewModelScaler.ScalingMS.ToString("0.#####m/s").PadRight(10).AsSpan();
                var scalFact = _viewModelScaler.AvatarScaleFactor.ToString("0.#####x").PadRight(10).AsSpan();
                var gestureMode = (GestureTextWorldScaling(_viewModelScaler.WorldScaling) + GestureTextDoubletapMute(_viewModelScaler.DoubleClickMuteCanSetGesture) + GestureText(_viewModelScaler.GestureMode)).AsSpan();
                var min = _viewModelScaler.MinEyeHeight.ToString("0.00m").AsSpan();
                var maxH = EyeHeight_Format(_viewModelScaler.MaxEyeHeight).PadRight(6).AsSpan();
                var fxdrat = (_viewModelScaler.FixedRate ? "Enabled " : "Disabled").AsSpan();
                var msrat = (_viewModelScaler.MSRate ? "Enabled " : "Disabled").AsSpan();
                var autabo = (_viewModelScaler.AutoAbort ? "Enabled " : "Disabled").AsSpan();

                int windowWidth = Console.WindowWidth - 2;
                if (windowWidth != _lastWidth)
                {
                    Console.Clear();
                    _lastWidth = windowWidth;
                    Console.CursorVisible = false;
                }
                screenBuilder.Clear();
                //title 3 line
                screenBuilder.Append($"==============================================================================".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($" VRC Scaler OSC {versionS}     connection: {oscPortS}:{oscIp}:{oscPortR}      ".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"==============================================================================".PadRight(windowWidth)[..windowWidth] + "\n");
                //infopage 14 line
                if (_page == 1) //page1 Scaler commands
                {
                    screenBuilder.Append($"[*]Scaler commands [ ]Quick size menu [ ]Setting commands  (Press Z to switch)".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                                                                              ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"exit               Exit application                                           ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"c                  Cancel scaling                                             ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"d                  Return to default height                                   ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"h[number][m][x][p] Instantly scale height to [number]                         ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"s[number][m][x][p] Smoothly scale height to [number] e.g. s100m  h10x  h+50p  ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                       [m] unit is meter, [x] unit is multiplier,             ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                       [p] +/- percentage of height                           ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"g[number]          Scaling Gesture, [number] in [0] to [9] 0: Disable 1:T+T   ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                       2:G+G 3:T+G 4:G+T 5:TG+TG 6:TG+T 7:TG+G 8:T+TG 9:G+TG  ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"ws                 Switch world scaling Enabled / Disabled                    ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"dm                 Switch Double-tap Mute to set gestures Enabled / Disabled  ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"Press Enter to send command                                                   ".PadRight(windowWidth)[..windowWidth] + "\n");
                }
                if (_page == 2) //page2 Quick size menu
                {
                    screenBuilder.Append($"[ ]Scaler commands [*]Quick size menu [ ]Setting commands  (Press Z to switch)".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                                                                              ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"exit               Exit application                                           ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"c                  Cancel scaling                                             ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"d                  Return to default height                                   ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"q[number]          Instantly scale to the height of the selected [number]     ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"w[number]          Smoothly scale to the height of the selected [number]      ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                   Quick size options:                       [number]: size   ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                   01: {dfH01} 02: {dfH02} 03: {dfH03} 04: {dfH04} 05: {dfH05}".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                   06: {dfH06} 07: {dfH07} 08: {dfH08} 09: {dfH09} 10: {dfH10}".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                   11: {dfH11} 12: {dfH12} 13: {dfH13} 14: {dfH14} 15: {dfH15}".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                   16: {dfH16} 17: {dfH17} 18: {dfH18} 19: {dfH19} 20: {dfH20}".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                   21: {dfH21} 22: {dfH22} 23: {dfH23} 24: {dfH24} 25: {dfH25}".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"Press Enter to send command                                                   ".PadRight(windowWidth)[..windowWidth] + "\n");
                }
                if (_page == 3) //page3 setting commands
                {
                    screenBuilder.Append($"[ ]Scaler commands [ ]Quick size menu [*]Setting commands  (Press Z to switch)".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                                                                              ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"exit               Exit application                                           ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"ip[IP Address]     Set IP address                                             ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"port s[number]     Set send port number                                       ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"port r[number]     Set receive port number (set 0 to enable Random Port)      ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"                                                                              ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"t[number]          Set scaling time to [number]sec and disable FixedRate      ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"r[number]          Set scaling rate to [number]x and enable FixedRate         ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"ms[number]         Set scaling time to [number]m/s and disable FixedRate      ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"f                  Switch FixedRate status Enabled / Disabled                 ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"a                  Switch Auto-Abort status Enabled / Disabled                ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"m[number]          Set the scaler max height to [number]m                     ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"n[number]          Set the scaler min height to [number]m                     ".PadRight(windowWidth)[..windowWidth] + "\n");
                    screenBuilder.Append($"Press Enter to send command                                                   ".PadRight(windowWidth)[..windowWidth] + "\n");
                }
                //current value 7 line
                screenBuilder.Append($"------------------------------------------------------------------------------".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"[Current height]: {crHeight}[Scaling time]: {scalTime}[Scale]:      {scalFact}".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"[Default height]: {dfHeight}[Scaling rate]: {scalRate}[Gesture]:{gestureMode} ".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"[Min Height]: {min}         [Scaling M/S]: {scalMS} [Auto-Abort]: {autabo}  ".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"[Max Height]: {maxH}        [FixedRate]:    {fxdrat}  [MSRate]:    {msrat}    ".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"------------------------------------------------------------------------------".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"{StateText(_viewModelScaler)}                                                 ".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"                                                                              ".PadRight(windowWidth)[..windowWidth] + "\n");
                screenBuilder.Append($"Command > {inputBuffer}{_cursor}                                              ".PadRight(windowWidth)[..windowWidth] + "\n");
                Console.SetCursorPosition(0, 0);
                if (Console.WindowHeight - 2 > screenBuilder.ToString().Split('\n').Length)
                {
                    Console.WriteLine(screenBuilder);
                }
                else
                {
                    string[] temp = [.. screenBuilder.ToString().Split('\n').TakeLast(Console.WindowHeight - 2)];
                    screenBuilder.Clear();
                    foreach (string s in temp)
                    {
                        screenBuilder.AppendLine(s);
                    }
                    Console.WriteLine(screenBuilder);
                }
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                    if (keyInfo.Key == ConsoleKey.Z)
                    {
                        _page = _page switch
                        {
                            1 => 2,
                            2 => 3,
                            _ => 1,
                        };
                    }
                    else if (keyInfo.Key == ConsoleKey.UpArrow && (_commandHistoryIndex + 1) < _commandHistory.Count)
                    {
                        _commandHistoryIndex += 1;
                        Debug.WriteLine($"{_commandHistoryIndex}: {_commandHistory.ElementAt(_commandHistoryIndex)}");
                        inputBuffer.Clear();
                        inputBuffer.Append(_commandHistory.ElementAt(_commandHistoryIndex));
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow && _commandHistoryIndex > 0 && (_commandHistoryIndex - 1) < _commandHistory.Count)
                    {
                        _commandHistoryIndex -= 1;
                        Debug.WriteLine($"{_commandHistoryIndex}: {_commandHistory.ElementAt(_commandHistoryIndex)}");
                        inputBuffer.Clear();
                        inputBuffer.Append(_commandHistory.ElementAt(_commandHistoryIndex));
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        _commandHistoryIndex = -1;
                        string command = inputBuffer.ToString().Trim();
                        _commandHistory.AddFirst(command);
                        if (_commandHistory.Count > 100)
                        {
                            _commandHistory.RemoveLast();
                        }
                        inputBuffer.Clear();
                        if (!DoCommend(command))
                        {
                            _controller_Scaler.OSCStop();
                            break;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (inputBuffer.Length > 0)
                        {
                            inputBuffer.Remove(inputBuffer.Length - 1, 1);
                        }
                    }
                    else if (keyInfo.KeyChar != '\0')
                    {
                        inputBuffer.Append(keyInfo.KeyChar);
                    }
                }
                Thread.Sleep(50);
            }

            Console.CursorVisible = true;
        }
        private bool DoCommend(string command)
        {
            if (string.IsNullOrEmpty(command?.Trim())) return true;

            //pathc space
            if (command != "dm" && command.Length > 1 && command[1] != ' ' && float.TryParse(command[1..].Trim('m').Trim('x').Trim('p').Trim(), out _))
            {
                command = command.Insert(1, " ");
            }
            if (command.Length > 2 && command.Contains("ip") && IPAddress.TryParse(command.AsSpan(2), out _))
            {
                command = command.Insert(2, " ");
            }
            if (command.Length > 2 && command.Contains("ms"))
            {
                command = command.Insert(2, " ");
            }
            if (command.Length > 5 && (command.Contains("portr") || command.Contains("ports")))
            {
                command = command.Insert(4, " ");
            }
            if (command.Length > 6 && (command.Contains("port r") || command.Contains("port s")) && float.TryParse(command[6..].Trim(), out _))
            {
                command = command.Insert(6, " ");
            }

            //do commands
            string[] commandOrder = command.ToLower().Split(' ');
            if (command.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) return false;
            else if (command.Equals("d", StringComparison.CurrentCultureIgnoreCase)) _controller_Scaler.StartScalingByTime();
            else if (command.Equals("c", StringComparison.CurrentCultureIgnoreCase)) _controller_Scaler.StopScaling();
            else if (command.Equals("f", StringComparison.CurrentCultureIgnoreCase))
            {
                _controller_Scaler.SetFixedRate(!_viewModelScaler.FixedRate);
            }
            else if (command.Equals("a", StringComparison.CurrentCultureIgnoreCase))
            {
                _controller_Scaler.SetAutoAbort(!_viewModelScaler.AutoAbort);
            }
            else if (command.Equals("ws", StringComparison.CurrentCultureIgnoreCase))
            {
                _controller_Scaler.SetWorldScaling(!_viewModelScaler.WorldScaling);
            }
            else if (command.Equals("dm", StringComparison.CurrentCultureIgnoreCase))
            {
                _controller_Scaler.SetGestureMuteDoubleClickMode(!_viewModelScaler.DoubleClickMuteCanSetGesture);
            }
            else if (commandOrder.Length == 3 && commandOrder[0].Equals("port", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(commandOrder[2], out int portNumber) && portNumber > 0)
                {
                    if (commandOrder[1] == "s")
                    {
                        _viewModelScaler.SendPort = portNumber.ToString();
                        _customSetting.ServerOSC_SendPort = portNumber;
                        _controller_Scaler.OSCSetup();
                    }
                    else if (commandOrder[1] == "r")
                    {
                        _viewModelScaler.ReceivePort = portNumber.ToString();
                        _customSetting.ServerOSC_ReceivePort = portNumber;
                        _controller_Scaler.OSCSetup();
                    }
                }
            }
            else if (commandOrder.Length == 2 && commandOrder[0] == "ip" && IPAddress.TryParse(commandOrder[1], out IPAddress? address))
            {
                _viewModelScaler.IP = address.ToString();
                _customSetting.ServerOSC_IP = address;
                _controller_Scaler.OSCSetup();
                _controller_Scaler.OSCSetup();
            }
            else if (commandOrder.Length == 2 && float.TryParse(commandOrder[1].Trim('m').Trim('x').Trim('p').Trim(), out float valueF))
            {
                if (commandOrder[0] == "t")
                {
                    _viewModelScaler.FixedRate = false;
                    _viewModelScaler.MSRate = false;
                    _controller_Scaler.SetFixedRate(_viewModelScaler.FixedRate);
                    _controller_Scaler.SetMSRate(_viewModelScaler.MSRate);
                    _controller_Scaler.SetScalingTime(valueF);
                }
                if (commandOrder[0] == "ms")
                {
                    _viewModelScaler.FixedRate = false;
                    _viewModelScaler.MSRate = true;
                    _controller_Scaler.SetFixedRate(_viewModelScaler.FixedRate);
                    _controller_Scaler.SetMSRate(_viewModelScaler.MSRate);
                    _controller_Scaler.SetScalingTimeByMS(valueF);
                }
                else if (commandOrder[0] == "r")
                {
                    _viewModelScaler.FixedRate = true;
                    _viewModelScaler.MSRate = false;
                    _controller_Scaler.SetFixedRate(_viewModelScaler.FixedRate);
                    _controller_Scaler.SetMSRate(_viewModelScaler.MSRate);
                    _controller_Scaler.SetScalingRate(valueF);
                }
                else if (commandOrder[1].Last() == 'p')
                {
                    valueF = _viewModelScaler.CurrentEyeHeight * ((commandOrder[1].StartsWith('+') || commandOrder[1].StartsWith('-') ? 1f : 0f) + valueF / 100f);
                }
                if (commandOrder[0] == "h")
                {
                    _controller_Scaler.StartScaling(commandOrder[1].Last() == 'x', _viewModelScaler.FixedRate, valueF);
                }
                else if (commandOrder[0] == "s")
                {
                    _controller_Scaler.StartScaling(commandOrder[1].Last() == 'x', _viewModelScaler.FixedRate, valueF, _viewModelScaler.ScalingTime, _viewModelScaler.ScalingRate);
                }
                else if (commandOrder[0] == "g")
                {
                    _controller_Scaler.SetGestureScaling(Convert.ToInt32(valueF));
                }
                else if (commandOrder[0] == "m")
                {
                    _controller_Scaler.SetMaxEyeHeight(valueF);

                }
                else if (commandOrder[0] == "n")
                {
                    _controller_Scaler.SetMinEyeHeight(valueF);
                }
                else if (commandOrder[0] == "q" && valueF >= 1 && valueF <= 25)
                {
                    _controller_Scaler.StartScaling(false, _viewModelScaler.FixedRate, _viewModelScaler.DefaultHeightValueList[Convert.ToInt32(valueF)]);
                }
                else if (commandOrder[0] == "w" && valueF >= 1 && valueF <= 25)
                {
                    _controller_Scaler.StartScaling(false, _viewModelScaler.FixedRate, _viewModelScaler.DefaultHeightValueList[Convert.ToInt32(valueF)], _viewModelScaler.ScalingTime, _viewModelScaler.ScalingRate);
                }
            }
            return true;
        }
        private string EyeHeight_Format(float height)
        {
            if (_viewModelScaler.IsInitialized || true)
            {
                if (height >= 100f)
                {
                    return height.ToString("0") + "m";
                }
                else if (height >= 10f)
                {
                    return height.ToString("0.0") + "m";
                }
                else
                {
                    return height.ToString("0.00") + "m";
                }
            }
            else
            {
                return _loc.WaitLoading;
            }
        }

        private string StateText(ViewModel_Scaler viewModel)
        {
            string cancelled = (!viewModel.IsScalingRunning && viewModel.ScalingCountdownSeconds > 0) ? " (Cancelled)" : "";
            if (viewModel.IsInitialized && viewModel.ScalingCountdownSeconds >= 0)
            {
                if (viewModel.ScalingCountdownSeconds == 0 && viewModel.ScalingPercentage > 0 && viewModel.ScalingPercentage < 100)
                {
                    return $"Remaining scaling time: <0 ({viewModel.ScalingPercentage}%) → {viewModel.CurrentTargetEyeHeight:0.##}m{cancelled}";
                }
                else if (viewModel.ScalingCountdownSeconds == 0 && viewModel.ScalingPercentage == 0)
                {
                    return $"Ready to scaling!";
                }
                else if (viewModel.ScalingCountdownSeconds < 60)
                {
                    return $"Remaining scaling time: {viewModel.ScalingCountdownSeconds:0} ({viewModel.ScalingPercentage}%) → {viewModel.CurrentTargetEyeHeight:0.##}m{cancelled}";
                }
                else if (viewModel.ScalingCountdownSeconds < 3600)
                {
                    return $"Remaining scaling time: {viewModel.ScalingCountdownSeconds / 60 % 60:00}:{viewModel.ScalingCountdownSeconds % 60:00} ({viewModel.ScalingPercentage}%) → {viewModel.CurrentTargetEyeHeight:0.##}m{cancelled}";
                }
                else
                {
                    return $"Remaining scaling time: {viewModel.ScalingCountdownSeconds / 3600:00}:{viewModel.ScalingCountdownSeconds / 60 % 60:00}:{viewModel.ScalingCountdownSeconds % 60:00} ({viewModel.ScalingPercentage}%) → {viewModel.CurrentTargetEyeHeight:0.##}m{cancelled}";
                }
            }
            else
            {
                _loadingAnimationChar = _loadingAnimationChar switch
                {
                    '/' => '|',
                    '|' => '\\',
                    '\\' => '-',
                    _ => '/',
                };
                return "Waiting for initialization " + _loadingAnimationChar;
            }
        }

        private static string GestureText(int gestureMode)
        {
            return gestureMode switch
            {
                0 => "Disabled",
                1 => "LT+RT",
                2 => "LG+RG",
                3 => "LT+RG",
                4 => "LG+RT",
                5 => "LT+LG+RT+RG",
                6 => "LT+LG + RT",
                7 => "LT+LG + RG",
                8 => "LT + RT+RG",
                9 => "LG + RT+RG",
                _ => "",
            };
        }

        private static string GestureTextWorldScaling(bool worldScaling)
        {
            return worldScaling ? "W" : " ";
        }
        private static string GestureTextDoubletapMute(bool doubletapMute)
        {
            return doubletapMute ? "D " : "  ";
        }
    }
}