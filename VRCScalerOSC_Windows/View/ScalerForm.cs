using System.Globalization;
using System.Net;
using VRCScalerOSC.Controller;
using VRCScalerOSC.Localization;
using VRCScalerOSC.Model;
using VRCScalerOSC.ViewModel;

namespace VRCScalerOSC_Windows
{
    public partial class ScalerForm : Form
    {
        private readonly Controller_Scaler _controllerScaler;
        private readonly ViewModel_Scaler _viewModelScaler;
        private readonly ScalerSetting _customSetting;
        private readonly BindingSource _bindingSource;
        private Localization loc = new();
        public ScalerForm(Controller_Scaler controller)
        {
            InitializeComponent();
            comboBoxGesture.SelectedIndex = 0;
            comboBoxIsMultiplier.SelectedIndex = 0;
            _customSetting = controller.CustomSetting;
            _viewModelScaler = controller.ViewModelScaler;
            _controllerScaler = controller;
            _bindingSource = [];
            SetupBindings();
            _bindingSource.DataSource = _viewModelScaler;
            _bindingSource.ResetBindings(false);
            _viewModelScaler.PropertyChanged += ViewModel_PropertyChanged;
            Text += $" v{ViewModel_Scaler.AppVersion.Major}.{ViewModel_Scaler.AppVersion.Minor}.{ViewModel_Scaler.AppVersion.Build}";
            buttonOSCStop.Text = _viewModelScaler.IsOSCRunning ? loc.OSCStop : loc.OSCStart;
            saveFileDialog1.InitialDirectory = AppContext.BaseDirectory;
            openFileDialog1.InitialDirectory = AppContext.BaseDirectory;
            InitScaleButtonSetText(_customSetting.MenuScaleValueList);
            controller.OSCSetup();
            comboBoxScalingRate.TextChanged += comboBoxScalingRate_TextChanged;
            comboBoxScalingTime.TextChanged += comboBoxScalingTime_TextChanged;
            comboBoxGesture.SelectedIndexChanged += comboBoxGesture_SelectedIndexChanged;
            checkBoxFixedRate.CheckedChanged += checkBoxFixedRate_CheckedChanged;

            comboBoxTargetEyeHeight.Text = _customSetting.FormTargetEyeHeight.ToString("0.##");
        }

        private void LoadUserData()
        {
            comboBoxScalingRate.TextChanged -= comboBoxScalingRate_TextChanged;
            comboBoxScalingTime.TextChanged -= comboBoxScalingTime_TextChanged;
            comboBoxGesture.SelectedIndexChanged -= comboBoxGesture_SelectedIndexChanged;

            _viewModelScaler.IP = _customSetting.ServerOSC_IP.ToString();
            _viewModelScaler.SendPort = _customSetting.ServerOSC_SendPort.ToString();
            _viewModelScaler.ReceivePort = _customSetting.ServerOSC_ReceivePort.ToString();

            _viewModelScaler.TargetEyeHeightList.Clear();
            _viewModelScaler.TargetEyeHeightList.AddRange(_customSetting.FormTargetEyeHeightList);
            _viewModelScaler.ScalingRateDefaultList.Clear();
            _viewModelScaler.ScalingRateDefaultList.AddRange(_customSetting.FormScalingRateList);
            _viewModelScaler.ScalingTimeDefaultList.Clear();
            _viewModelScaler.ScalingTimeDefaultList.AddRange(_customSetting.FormScalingTimeList);

            _viewModelScaler.TargetEyeHeight = _customSetting.FormTargetEyeHeight;
            _viewModelScaler.ScalingRate = _customSetting.FormScalingRate;
            _viewModelScaler.ScalingTime = _customSetting.FormScalingTime;
            _viewModelScaler.FixedRate = _customSetting.FormFixedRate;
            _viewModelScaler.AutoAbort = _customSetting.FormAutoAbort;
            _viewModelScaler.MinEyeHeight = _customSetting.MinHeight;
            _viewModelScaler.MaxEyeHeight = _customSetting.MaxHeight;
            toolStripMenuItemHeightRangeUserSettings.Checked = true;

            InitScaleButtonSetText(_customSetting.MenuScaleValueList);
            _controllerScaler.InitMenuScaleValueList();

            comboBoxScalingRate.TextChanged += comboBoxScalingRate_TextChanged;
            comboBoxScalingTime.TextChanged += comboBoxScalingTime_TextChanged;
            comboBoxGesture.SelectedIndexChanged += comboBoxGesture_SelectedIndexChanged;
        }

        public void InitScaleButtonSetText(List<double> MenuScaleValueList)
        {
            Control[] ScaleSetList = [buttonSet1, buttonSet2, buttonSet3, buttonSet4, buttonSet5, buttonSet6, buttonSet7, buttonSet8, buttonSet9, buttonSet10, buttonSet11, buttonSet12, buttonSet13, buttonSet14, buttonSet15, buttonSet16, buttonSet17, buttonSet18, buttonSet19, buttonSet20, buttonSet21, buttonSet22, buttonSet23, buttonSet24, buttonSet25, buttonSet26, buttonSet27, buttonSet28, buttonSet29, buttonSet30, buttonSet31, buttonSet32, buttonSet33];
            int n = 0;
            foreach (float value in MenuScaleValueList)
            {
                if (n < ScaleSetList.Length)
                {
                    if (n < 25)
                    {
                        ScaleSetList[n].Text = value.ToString("0.##", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        ScaleSetList[n].Text = value.ToString("+0%;-0%;0%", CultureInfo.CurrentCulture);
                    }
                }
                n++;
            }
        }

        private void SetupBindings()
        {
            textBoxIP.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.IP), true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxReceivePort.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.ReceivePort), true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSendPort.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.SendPort), true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxOSCRandomReceiverPort.DataBindings.Add("Checked", _bindingSource, nameof(_viewModelScaler.RandomReceiverPort), true, DataSourceUpdateMode.OnPropertyChanged);

            checkBoxAutoAbort.DataBindings.Add("Checked", _bindingSource, nameof(_viewModelScaler.AutoAbort), true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxFixedRate.DataBindings.Add("Checked", _bindingSource, nameof(_viewModelScaler.FixedRate), true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxIsMultiplier.DataBindings.Add("Checked", _bindingSource, nameof(_viewModelScaler.IsMultiplier), true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxWorldScaling.DataBindings.Add("Checked", _bindingSource, nameof(_viewModelScaler.WorldScaling), true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxGestureMuteDoubleClickMode.DataBindings.Add("Checked", _bindingSource, nameof(_viewModelScaler.DoubleClickMuteCanSetGesture), true, DataSourceUpdateMode.OnPropertyChanged);
            progressBarScaling.DataBindings.Add("Value", _bindingSource, nameof(_viewModelScaler.ScalingPercentage), true);
            labelSCS.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.ScalingCountdownSeconds), true, DataSourceUpdateMode.Never, 1, "0").Format += labelSCS_Format;

            comboBoxTargetEyeHeight.DataSource = _viewModelScaler.TargetEyeHeightList;
            comboBoxScalingRate.DataSource = _viewModelScaler.ScalingRateDefaultList;
            comboBoxScalingTime.DataSource = _viewModelScaler.ScalingTimeDefaultList;

            comboBoxScalingRate.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.ScalingRate), true, DataSourceUpdateMode.OnValidation, 0, "0.#####");
            comboBoxScalingTime.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.ScalingTime), true, DataSourceUpdateMode.OnValidation, 1, "0.##");
            _viewModelScaler.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModelScaler.GestureMode))
                {
                    comboBoxGesture.SelectedIndex = _viewModelScaler.GestureMode;
                }
                else if (e.PropertyName == nameof(_viewModelScaler.TargetEyeHeight) && !comboBoxTargetEyeHeight.Focused && !comboBoxTargetEyeHeight.DroppedDown)
                {
                    comboBoxTargetEyeHeight.Text = _viewModelScaler.TargetEyeHeight.ToString("0.##");
                }
            };

            labelGetWristInfoFailed.DataBindings.Add("Visible", _bindingSource, nameof(_viewModelScaler.ShowGetWristInfoFailedLabel), true, DataSourceUpdateMode.Never);
            labelAvatarScalingDisabled.DataBindings.Add("Visible", _bindingSource, nameof(_viewModelScaler.ScalerUnUsable), true, DataSourceUpdateMode.Never);

            labelCEHV.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.CurrentEyeHeight), true, DataSourceUpdateMode.Never, loc.WaitLoading).Format += EyeHeight_Format;
            labelDEHV.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.AvatarDefaultEyeHeight), true, DataSourceUpdateMode.Never, loc.WaitLoading).Format += EyeHeight_Format;
            labelSFV.DataBindings.Add("Text", _bindingSource, nameof(_viewModelScaler.AvatarScaleFactor), true, DataSourceUpdateMode.Never, loc.WaitLoading).Format += ScaleFactor;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModelScaler.MinEyeHeight) || e.PropertyName == nameof(_viewModelScaler.MaxEyeHeight))
            {
                SetHeightRange();
            }
        }

        private void SetHeightRange()
        {
            labelHeightRange.Text = labelHeightRange.Text.Split(':')[0] + $": {_viewModelScaler.MinEyeHeight:0.##} ~ {_viewModelScaler.MaxEyeHeight:0}{loc.LabelMeters}";
        }

        private void EyeHeight_Format(object? sender, ConvertEventArgs e)
        {
            if (_viewModelScaler.IsInitialized && e.Value is float valueF)
            {
                if (valueF >= 100f)
                {
                    e.Value = valueF.ToString("0");
                }
                else if (valueF >= 10f)
                {
                    e.Value = valueF.ToString("0.0");
                }
                else
                {
                    e.Value = valueF.ToString("0.00");
                }
            }
            else
            {
                e.Value = loc.WaitLoading;
            }
        }

        private void labelSCS_Format(object? sender, ConvertEventArgs e)
        {
            if (_viewModelScaler.IsInitialized && e.Value is int valueI && valueI >= 0)
            {
                if (valueI == 0 && _viewModelScaler.ScalingPercentage > 0 && _viewModelScaler.ScalingPercentage < 100)
                {
                    e.Value = $"<0 ({_viewModelScaler.ScalingPercentage}%) → {_viewModelScaler.CurrentTargetEyeHeight:0.##}m";
                }
                else if (valueI < 60)
                {
                    e.Value = $"{valueI:0} ({_viewModelScaler.ScalingPercentage}%) → {_viewModelScaler.CurrentTargetEyeHeight:0.##}m";
                }
                else if (valueI < 3600)
                {
                    e.Value = $"{valueI / 60 % 60:00}:{valueI % 60:00} ({_viewModelScaler.ScalingPercentage}%) → {_viewModelScaler.CurrentTargetEyeHeight:0.##}m";
                }
                else
                {
                    e.Value = $"{valueI / 3600:00}:{valueI / 60 % 60:00}:{valueI % 60:00} ({_viewModelScaler.ScalingPercentage}%) → {_viewModelScaler.CurrentTargetEyeHeight:0.##}m";
                }
            }
            else
            {
                e.Value = "";
            }
        }

        private void ScaleFactor(object? sender, ConvertEventArgs e)
        {
            if (_viewModelScaler.IsInitialized && e.Value is float valueF)
            {
                e.Value = valueF.ToString("0.#####");
            }
            else
            {
                e.Value = loc.WaitLoading;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (SynchronizationContext.Current != null)
            {
                _viewModelScaler.SetContext(SynchronizationContext.Current);
            }
            AutoSelectLanguage(sender, e);
        }

        private void buttonChangeScale_Click(object sender, EventArgs e)
        {
            if (float.TryParse(comboBoxTargetEyeHeight.Text, out float eyeHeight))
            {
                _controllerScaler.StartScaling(_viewModelScaler.IsMultiplier, _viewModelScaler.FixedRate, eyeHeight,
                    float.TryParse(comboBoxScalingTime.Text, out float scalingTime) ? scalingTime : 0f,
                    double.TryParse(comboBoxScalingRate.Text, out double scalingRate) ? scalingRate : double.MaxValue);
            }
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            _controllerScaler.StopScaling();
        }
        private void buttonResetHeight_Click(object sender, EventArgs e)
        {
            _controllerScaler.StartScalingByTime();
        }

        private void buttonOSCSetup_Click(object sender, EventArgs e)
        {
            if (short.TryParse(_viewModelScaler.SendPort, out short sendPort) && short.TryParse(_viewModelScaler.ReceivePort, out short receivePort) && IPAddress.TryParse(_viewModelScaler.IP, out IPAddress? ip) && ip != null)
            {
                _customSetting.ServerOSC_IP = ip;
                _customSetting.ServerOSC_ReceivePort = checkBoxOSCRandomReceiverPort.Checked ? 0 : receivePort;
                _customSetting.ServerOSC_SendPort = sendPort;
                _controllerScaler.OSCSetup();
                buttonOSCStop.Text = _viewModelScaler.IsOSCRunning ? loc.OSCStop : loc.OSCStart;
            }
        }

        private void buttonOSCStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (_viewModelScaler.IsOSCRunning)
                {
                    _controllerScaler.OSCStop();
                }
                else
                {
                    _controllerScaler.OSCStart();
                }
                buttonOSCStop.Text = _viewModelScaler.IsOSCRunning ? loc.OSCStop : loc.OSCStart;
            }
            catch { }
        }


        int currentSize = 1;
        private void formSize_Click(object sender, EventArgs e)
        {
            if (sender == toolStripMenuItemFormSize1x)
            {
                currentSize = 1;
            }
            else if (sender == toolStripMenuItemFormSize2x)
            {
                currentSize = 2;
            }
            else if (sender == toolStripMenuItemFormSize3x)
            {
                currentSize = 3;
            }
            else if (sender == toolStripMenuItemFormSize4x)
            {
                currentSize = 4;
            }
            ChangeFomtSize(currentSize);
        }
        private void ChangeFomtSize(int currentSize = 0)
        {
            if (currentSize == 0)
            {
                currentSize = this.currentSize;
            }
            this.currentSize = currentSize;
            float normalFontSize = 14.25f;
            float smallFontSize = 9f;
            switch (this.currentSize)
            {
                case 1:
                    normalFontSize = 14.25f;
                    smallFontSize = 9f;
                    break;
                case 2:
                    normalFontSize = 18f;
                    smallFontSize = 12f;
                    break;
                case 3:
                    normalFontSize = 21.75f;
                    smallFontSize = 14.25f;
                    break;
                case 4:
                    normalFontSize = 26.25f;
                    smallFontSize = 18f;
                    break;
            }
            this.Font = new Font(this.Font.FontFamily, normalFontSize);
            comboBoxTargetEyeHeight.Font = new Font(this.Font.FontFamily, normalFontSize);
            contextMenuStripLanguage.Font = new Font(this.Font.FontFamily, normalFontSize);
            labelHeightRange.Font = new Font(labelHeightRange.Font.FontFamily, smallFontSize);
            flowLayoutPanelAvatarHeight.Font = new Font(flowLayoutPanelAvatarHeight.Font.FontFamily, smallFontSize);
            //labelSec.Font = new Font(labelSec.Font.FontFamily, checkBoxFixedRate.Checked ? smallFontSize : normalFontSize);
        }
        private void buttonHeightRange_Click(object sender, EventArgs e)
        {
            contextMenuStripHetightRange.Show(MousePosition);
        }
        private void buttonFormSize_Click(object sender, EventArgs e)
        {
            contextMenuStripFormSize.Show(MousePosition);
        }

        private void buttonLanguage_Click(object sender, EventArgs e)
        {
            contextMenuStripLanguage.Show(MousePosition);
        }
        private void buttonCombo_Click(object sender, EventArgs e)
        {
            comboBoxTargetEyeHeight.DroppedDown = true;
        }

        private void buttonComboScalingTime_Click(object sender, EventArgs e)
        {
            if (comboBoxScalingTime.Visible)
            {
                comboBoxScalingTime.DroppedDown = true;
            }
            else
            {
                comboBoxScalingRate.DroppedDown = true;
            }
        }

        private void AutoSelectLanguage(object sender, EventArgs e)
        {
            if (sender == toolStripMenuItemLangEN)
            {
                SelectLanguage(Localization_enUS.LocalizationId.enUS);
            }
            else if (sender == toolStripMenuItemLangJP)
            {
                SelectLanguage(Localization_enUS.LocalizationId.jaJP);
            }
            else if (sender == toolStripMenuItemLangKR)
            {
                SelectLanguage(Localization_enUS.LocalizationId.koKR);
            }
            else if (sender == toolStripMenuItemLangCN)
            {
                SelectLanguage(Localization_enUS.LocalizationId.zhCN);
            }
            else if (sender == toolStripMenuItemLangTW)
            {
                SelectLanguage(Localization_enUS.LocalizationId.zhTW);
            }
            else
            {
                switch (CultureInfo.CurrentUICulture.Name)
                {
                    case "zh-CN":
                        SelectLanguage(Localization_enUS.LocalizationId.zhCN);
                        break;
                    case "zh-TW":
                    case "zh-HK":
                        SelectLanguage(Localization_enUS.LocalizationId.zhTW);
                        break;
                    case "ja-JP":
                        SelectLanguage(Localization_enUS.LocalizationId.jaJP);
                        break;
                    case "ko-KR":
                        SelectLanguage(Localization_enUS.LocalizationId.koKR);
                        break;
                    case "en-US":
                    default:
                        SelectLanguage(Localization_enUS.LocalizationId.enUS);
                        break;
                }
            }
        }
        private void SelectLanguage(Localization_enUS.LocalizationId localizationId)
        {
            this.loc = localizationId switch
            {
                Localization.LocalizationId.jaJP => new Localization_jaJP(),
                Localization.LocalizationId.koKR => new Localization_koKR(),
                Localization.LocalizationId.zhTW => new Localization_zhTW(),
                Localization.LocalizationId.zhCN => new Localization_zhCN(),
                _ => new Localization_enUS(),
            };
            groupBoxHeight.Text = loc.GroupBoxHeight;
            labelHeightRange.Text = loc.LabelHeightRange; SetHeightRange();
            toolStripMenuItemHeightRangeVRChatAvatar.Text = loc.HeightRangeVRChatAvatar;
            toolStripMenuItemHeightRangeVRChatWorld.Text = loc.HeightRangeVRChatWorld;
            toolStripMenuItemHeightRangeAdvanced.Text = loc.HeightRangeAdvanced;
            toolStripMenuItemHeightRangeMaximum.Text = loc.HeightRangeLimit;
            toolStripMenuItemHeightRangeUserSettings.Text = loc.HeightRangeUserSettings + $" ({_customSetting.MinHeight:0.##} ~ {_customSetting.MaxHeight:0}{loc.LabelMeters})";
            toolStripMenuItemHeightRangeSetUpper.Text = loc.HeightRangeSetUpper;
            toolStripMenuItemHeightRangeSetLower.Text = loc.HeightRangeSetLower;
            buttonResetHeight.Text = loc.ButtonResetHeight;
            buttonChangeScale.Text = loc.ButtonChangeScale;
            buttonStop.Text = loc.ButtonStop;
            groupBoxScalingTime.Text = checkBoxFixedRate.Checked ? loc.GroupBoxScalingRate : loc.GroupBoxScalingTime;
            labelSec.Text = _viewModelScaler.FixedRate ? loc.LabelRate : loc.LabelSec;
            comboBoxIsMultiplier.Items[0] = loc.LabelMeters;
            comboBoxIsMultiplier.Items[1] = loc.LabelMultiplier;
            checkBoxAutoAbort.Text = loc.CheckBoxAutoAbort;
            groupBoxSetting.Text = loc.GroupBoxSetting;
            buttonLanguage.Text = loc.ButtonLanguage;
            buttonFormSize.Text = loc.ButtonFormSize;
            groupBoxOSCConfig.Text = loc.GroupBoxOSCConfig;
            labelOSCIP.Text = loc.LabelOSCIP;
            labelOSCSendPort.Text = loc.LabelOSCSendPort;
            labelOSCReceivePort.Text = loc.LabelOSCReceivePort;
            checkBoxOSCRandomReceiverPort.Text = loc.CheckBoxOSCRandomReceiverPort;
            buttonOSCSetup.Text = loc.ButtonOSCSetup;
            labelCEH.Text = loc.LabelCEH;
            labelDEH.Text = loc.LabelDEH;
            labelSF.Text = loc.LabelSF;
            labelGetWristInfoFailed.Text = loc.LabelGetWristInfoFailed;
            labelAvatarScalingDisabled.Text = loc.LabelAvatarScalingDisabled;
            buttonOSCStop.Text = _viewModelScaler.IsOSCRunning ? loc.OSCStop : loc.OSCStart;
            buttonLite.Text = loc.ButtonLite;
            buttonStd.Text = loc.ButtonStd;
            buttonStop2.Text = loc.ButtonStop;
            buttonResetHeightLite.Text = loc.ButtonResetHeightLite;
            checkBoxInstant.Text = loc.CheckBoxInstant;
            checkBoxFixedRate.Text = loc.CheckBoxFixedRate;
            checkBoxIsMultiplier.Text = loc.CheckBoxIsMultiplier;
            groupBoxGesture.Text = loc.GroupBoxGesture;
            if (comboBoxGesture.Items.Count > 0) comboBoxGesture.Items[0] = loc.ComboBoxGesture0;
            if (comboBoxGesture.Items.Count > 1) comboBoxGesture.Items[1] = loc.ComboBoxGesture1;
            if (comboBoxGesture.Items.Count > 2) comboBoxGesture.Items[2] = loc.ComboBoxGesture2;
            if (comboBoxGesture.Items.Count > 3) comboBoxGesture.Items[3] = loc.ComboBoxGesture3;
            if (comboBoxGesture.Items.Count > 4) comboBoxGesture.Items[4] = loc.ComboBoxGesture4;
            if (comboBoxGesture.Items.Count > 5) comboBoxGesture.Items[5] = loc.ComboBoxGesture5;
            if (comboBoxGesture.Items.Count > 6) comboBoxGesture.Items[6] = loc.ComboBoxGesture6;
            if (comboBoxGesture.Items.Count > 7) comboBoxGesture.Items[7] = loc.ComboBoxGesture7;
            if (comboBoxGesture.Items.Count > 8) comboBoxGesture.Items[8] = loc.ComboBoxGesture8;
            if (comboBoxGesture.Items.Count > 9) comboBoxGesture.Items[9] = loc.ComboBoxGesture9;
            checkBoxGestureMuteDoubleClickMode.Text = loc.CheckBoxGestureMuteDoubleClickMode;
            checkBoxWorldScaling.Text = loc.CheckBoxWorldScaling;
            groupBoxCustom.Text = loc.GroupBoxCustom;
            buttonCustomImport.Text = loc.ButtonCustomImport;
            buttonCustomExport.Text = loc.ButtonCustomExport;
            if (!_viewModelScaler.IsInitialized)
            {
                labelCEHV.Text = loc.WaitLoading;
                labelDEHV.Text = loc.WaitLoading;
                labelSFV.Text = loc.WaitLoading;
            }
        }

        private void buttonLite_Click(object sender, EventArgs e)
        {
            tableLayoutPanelLite.Visible = true;
            tableLayoutPanelStd.Visible = false;
        }

        private void buttonStandard_Click(object sender, EventArgs e)
        {
            tableLayoutPanelLite.Visible = false;
            tableLayoutPanelStd.Visible = true;
        }

        private void buttonLiteScaler_Click(object sender, EventArgs e)
        {
            string value = ((Control)sender).Text;
            if (float.TryParse(value.Trim('m').Trim('x').Trim('%'), CultureInfo.CurrentCulture, out float eyeHeight))
            {
                if (value.Contains('%'))
                {
                    _controllerScaler.StartScaling(_viewModelScaler.IsMultiplier, !checkBoxInstant.Checked, _viewModelScaler.CurrentEyeHeight * (1 + eyeHeight / 100f), -1, checkBoxInstant.Checked ? float.MaxValue : 2f);
                }
                else
                {
                    _controllerScaler.StartScaling(_viewModelScaler.IsMultiplier, !checkBoxInstant.Checked, eyeHeight, -1, checkBoxInstant.Checked ? float.MaxValue : 2f);
                }

            }
        }

        private void comboBoxScalingTime_TextChanged(object? sender, EventArgs e)
        {
            if (!checkBoxFixedRate.Checked && float.TryParse(comboBoxScalingTime.Text, out float scalingTime) && float.TryParse(comboBoxTargetEyeHeight.Text, out float eyeHeight))
            {
                comboBoxScalingRate.TextChanged -= comboBoxScalingRate_TextChanged;
                comboBoxScalingTime.TextChanged -= comboBoxScalingTime_TextChanged;

                _viewModelScaler.TargetEyeHeight = eyeHeight;
                _controllerScaler.SetScalingTime(scalingTime);
                comboBoxScalingTime.SelectionStart = comboBoxScalingTime.Text.Length;
                comboBoxScalingRate.TextChanged += comboBoxScalingRate_TextChanged;
                comboBoxScalingTime.TextChanged += comboBoxScalingTime_TextChanged;
            }
        }
        private void comboBoxScalingRate_TextChanged(object? sender, EventArgs e)
        {
            if (checkBoxFixedRate.Checked && double.TryParse(comboBoxScalingRate.Text, out double scalingRate) && float.TryParse(comboBoxTargetEyeHeight.Text, out float eyeHeight))
            {
                comboBoxScalingRate.TextChanged -= comboBoxScalingRate_TextChanged;
                comboBoxScalingTime.TextChanged -= comboBoxScalingTime_TextChanged;
                _viewModelScaler.TargetEyeHeight = eyeHeight;
                _controllerScaler.SetScalingRate(scalingRate);
                comboBoxScalingRate.SelectionStart = comboBoxScalingRate.Text.Length;
                comboBoxScalingRate.TextChanged += comboBoxScalingRate_TextChanged;
                comboBoxScalingTime.TextChanged += comboBoxScalingTime_TextChanged;
            }
        }
        private void checkBoxFixedRate_CheckedChanged(object? sender, EventArgs e)
        {
            _viewModelScaler.FixedRate = checkBoxFixedRate.Checked;
            _controllerScaler.SetFixedRate(_viewModelScaler.FixedRate);
            if (!checkBoxFixedRate.Checked)
            {
                groupBoxScalingTime.Text = loc.GroupBoxScalingTime;
                comboBoxScalingTime.Visible = true;
                comboBoxScalingRate.Visible = false;
                labelSec.Text = loc.LabelSec;
                ChangeFomtSize();
            }
            else
            {
                groupBoxScalingTime.Text = loc.GroupBoxScalingRate;
                comboBoxScalingTime.Visible = false;
                comboBoxScalingRate.Visible = true;
                labelSec.Text = loc.LabelRate;
                ChangeFomtSize();
            }
        }
        private void comboBoxTargetEyeHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                comboBoxEyeHeight_TextChanged(sender, e);
            }
        }
        private void comboBoxEyeHeight_TextChanged(object sender, EventArgs e)
        {
            string text = comboBoxTargetEyeHeight.Text.Trim();
            if (Controller_Scaler.TryConvertFtInToMeters(text, out float meters))
            {
                _controllerScaler.SetIsMultiplier(false);
                comboBoxTargetEyeHeight.Text = meters.ToString("0.##", CultureInfo.CurrentCulture);
            }
            else if (text.EndsWith('%') && float.TryParse(text.Trim('%'), out float value))
            {
                comboBoxTargetEyeHeight.Text = ((_viewModelScaler.IsMultiplier ? _viewModelScaler.AvatarScaleFactor : _viewModelScaler.CurrentEyeHeight) * (((text.StartsWith('+') || text.StartsWith('-')) ? 1f : 0f) + value / 100f)).ToString("0.##");
            }

            if (float.TryParse(comboBoxTargetEyeHeight.Text.Trim('%'), out float eyeHeight))
            {
                _controllerScaler.SetTargetEyeHeight(eyeHeight);
                comboBoxTargetEyeHeight.SelectionStart = comboBoxTargetEyeHeight.Text.Length;
            }
        }
        private void checkBoxAutoAbort_CheckedChanged(object sender, EventArgs e)
        {
            _controllerScaler.SetAutoAbort(checkBoxAutoAbort.Checked);
        }
        private void comboBoxGesture_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _controllerScaler.SetGestureScaling(comboBoxGesture.SelectedIndex);
        }

        private void checkBoxWorldScaling_CheckedChanged(object sender, EventArgs e)
        {
            _controllerScaler.SetWorldScaling(checkBoxWorldScaling.Checked);
        }

        private void buttonCustomImport_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Controller_ScalerSetting.ImportCustomSetting(_customSetting, openFileDialog1.FileName);
                LoadUserData();
            }
        }

        private void buttonCustomExport_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _customSetting.FormScalingTime = _viewModelScaler.ScalingTime;
                _customSetting.FormScalingRate = _viewModelScaler.ScalingRate;
                _customSetting.MaxHeight = _viewModelScaler.MaxEyeHeight;
                _customSetting.MinHeight = _viewModelScaler.MinEyeHeight;
                _customSetting.FormFixedRate = _viewModelScaler.FixedRate;
                _customSetting.FormAutoAbort = _viewModelScaler.AutoAbort;
                Controller_ScalerSetting.ExportCustomSetting(_customSetting, saveFileDialog1.FileName, $"{ViewModel_Scaler.AppVersion.Major}.{ViewModel_Scaler.AppVersion.Minor}.{ViewModel_Scaler.AppVersion.Build}");
            }
        }

        private void comboBoxIsMultiplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBoxIsMultiplier.Checked && comboBoxIsMultiplier.SelectedIndex == 0)
            {
                checkBoxIsMultiplier.Checked = false;
            }
            else if (!checkBoxIsMultiplier.Checked && comboBoxIsMultiplier.SelectedIndex == 1)
            {
                checkBoxIsMultiplier.Checked = true;
            }
            _controllerScaler?.SetIsMultiplier(checkBoxIsMultiplier.Checked);
        }

        private void checkBoxIsMultiplier_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIsMultiplier.Checked && comboBoxIsMultiplier.SelectedIndex == 0)
            {
                comboBoxIsMultiplier.SelectedIndex = 1;
            }
            else if (!checkBoxIsMultiplier.Checked && comboBoxIsMultiplier.SelectedIndex == 1)
            {
                comboBoxIsMultiplier.SelectedIndex = 0;
            }
        }

        private void toolStripMenuItemHeightRange_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHeightRangeVRChatAvatar.Checked = false;
            toolStripMenuItemHeightRangeVRChatWorld.Checked = false;
            toolStripMenuItemHeightRangeAdvanced.Checked = false;
            toolStripMenuItemHeightRangeMaximum.Checked = false;
            toolStripMenuItemHeightRangeUserSettings.Checked = false;

            if (sender == toolStripMenuItemHeightRangeVRChatAvatar)
            {
                toolStripMenuItemHeightRangeVRChatAvatar.Checked = true;
                _controllerScaler.SetMinEyeHeight(0.2f);
                _controllerScaler.SetMaxEyeHeight(5f);
            }
            else if (sender == toolStripMenuItemHeightRangeVRChatWorld)
            {
                toolStripMenuItemHeightRangeVRChatWorld.Checked = true;
                _controllerScaler.SetMinEyeHeight(0.1f);
                _controllerScaler.SetMaxEyeHeight(100f);
            }
            else if (sender == toolStripMenuItemHeightRangeAdvanced)
            {
                toolStripMenuItemHeightRangeAdvanced.Checked = true;
                _controllerScaler.SetMinEyeHeight(0.05f);
                _controllerScaler.SetMaxEyeHeight(3000f);
            }
            else if (sender == toolStripMenuItemHeightRangeMaximum)
            {
                toolStripMenuItemHeightRangeMaximum.Checked = true;
                _controllerScaler.SetMinEyeHeight(0.01f);
                _controllerScaler.SetMaxEyeHeight(10000f);
            }
            else if (sender == toolStripMenuItemHeightRangeUserSettings)
            {
                toolStripMenuItemHeightRangeUserSettings.Checked = true;
                _controllerScaler.SetMinEyeHeight(_customSetting.MinHeight);
                _controllerScaler.SetMaxEyeHeight(_customSetting.MaxHeight);
            }
            else if (sender == toolStripMenuItemHeightRangeSetUpper)
            {
                _controllerScaler.SetMaxEyeHeight(_viewModelScaler.TargetEyeHeight);
            }
            else if (sender == toolStripMenuItemHeightRangeSetLower)
            {
                _controllerScaler.SetMinEyeHeight(_viewModelScaler.TargetEyeHeight);
            }
        }
    }
}
