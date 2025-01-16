using Serilog;
using SonoCap.MES.Models.Enums;
using System;

namespace SonoCap.MES.UI.Model
{
    public class GlobalModel
    {
        public event Action<int, int>? MotorStateChanged;

        public GlobalModel() { }

        public bool InitializeLibrary()
        {
            if (!HsnlibraryCS.HsnInterface.initialize())
            {
                return false;
            }
            HsnlibraryCS.Callback.registerProbeStateCallback(ProbeStateCallback);
            HsnlibraryCS.Callback.registerLoadingCallback(LoadingCallback);
            HsnlibraryCS.Callback.registerErrorStateCallback(ErrorCallback);
            HsnlibraryCS.Callback.registerENDMotorCallback(MotorCallback);

            HsnlibraryCS.HsnInterface.DeviceAttached += DeviceAttached;
            HsnlibraryCS.HsnInterface.DeviceDetached += DeviceDettached;
            HsnlibraryCS.HsnInterface.startProbeDetection();

            return true;
        }

        private void MotorCallback(int prf_hz, int density)
        {
            // 처리 로직
            Log.Information($"{nameof(MotorCallback)}: prf_hz:{prf_hz}, density:{density}");
            MotorStateChanged?.Invoke(prf_hz, density);
        }

        public void DestroyLibrary()
        {
            HsnlibraryCS.HsnInterface.destroy();
        }

        private void ErrorCallback(string err_str, int err_num)
        {
            string message = $"Error {err_num}: {err_str}";
            System.Windows.MessageBox.Show(message);
        }

        public List<Tuple<int, string>> Applications
        {
            get
            {
                var applications = new List<Tuple<int, string>>();
                if (!HsnlibraryCS.HsnInterface.listApplication(ref applications))
                {
                    throw new Exception("Failed to list applications.");
                }
                return applications;
            }
        }

        public List<Tuple<int, string>> Presets
        {
            get
            {
                var presets = new List<Tuple<int, string>>();
                if (!HsnlibraryCS.HsnInterface.listPreset(Application, ref presets))
                {
                    throw new Exception("Failed to list presets.");
                }
                return presets;
            }
        }

        public List<Tuple<int, string>> SubSettings
        {
            get
            {
                var subSettings = new List<Tuple<int, string>>();
                if (!HsnlibraryCS.HsnInterface.listSubSetting(Setting, ref subSettings))
                {
                    throw new Exception("Failed to list subsettings.");
                }
                return subSettings;
            }
        }

        public double ViewDepthCm
        {
            get { return HsnlibraryCS.HsnInterface.getDeviceRtcViewdepth(); }
            set { HsnlibraryCS.HsnInterface.setDeviceRtcViewdepth(value); }
        }

        public int LineDensity
        {
            get { return HsnlibraryCS.HsnInterface.getDeviceDensityFactor(); }
            set { HsnlibraryCS.HsnInterface.setDeviceDensityFactor(value); }
        }

        public int IPGain
        {
            get { return HsnlibraryCS.HsnInterface.getIpGain(); }
            set { HsnlibraryCS.HsnInterface.setIpGain(value); }
        }

        public float DRMin
        {
            get { return HsnlibraryCS.HsnInterface.getIpDynamicRangeMin(); }
            set { HsnlibraryCS.HsnInterface.setIpDynamicRangeMin(value); }
        }

        public float DRMax
        {
            get { return HsnlibraryCS.HsnInterface.getIpDynamicRangeMax(); }
            set { HsnlibraryCS.HsnInterface.setIpDynamicRangeMax(value); }
        }

        public double TxPower
        {
            get { return HsnlibraryCS.HsnInterface.getDeviceTxPower(); }
            set { HsnlibraryCS.HsnInterface.setDeviceTxPower(value); }
        }

        public double TransducerID
        {
            get { return HsnlibraryCS.HsnInterface.getTransducerID(); }
        }

        public string TransducerCode
        {
            get { return HsnlibraryCS.HsnInterface.getTransducerCode(); }
        }

        public string LibraryVersion
        {
            get
            {
                string value = "";
                if (!HsnlibraryCS.HsnInterface.getLibraryVersion(ref value))
                {
                    return "";
                }
                return value;
            }
        }

        public int Application
        {
            get
            {
                int application = 0;
                if (HsnlibraryCS.HsnInterface.getCurrentApplication(ref application))
                {
                    return application;
                }
                else
                {
                    throw new Exception("Failed to get current subsetting.");
                }
            }
            set
            {
                if (!HsnlibraryCS.HsnInterface.loadApplication(value))
                {
                    throw new Exception("Failed to load subsetting.");
                }
            }
        }

        public int Setting
        {
            get
            {
                int currentSetting = 0;
                if (HsnlibraryCS.HsnInterface.getCurrentSetting(ref currentSetting))
                {
                    return currentSetting;
                }
                else
                {
                    throw new Exception("Failed to get current subsetting.");
                }
            }
            set
            {
                if (!HsnlibraryCS.HsnInterface.loadSetting(value))
                {
                    throw new Exception("Failed to load subsetting.");
                }
            }
        }

        public int Subsetting
        {
            get
            {
                int currentSubsetting = 0;
                if (HsnlibraryCS.HsnInterface.getCurrentSubSetting(ref currentSubsetting))
                {
                    return currentSubsetting;
                }
                else
                {
                    throw new Exception("Failed to get current subsetting.");
                }
            }
            set
            {
                if (!HsnlibraryCS.HsnInterface.loadSubSetting(value))
                {
                    throw new Exception("Failed to load subsetting.");
                }
            }
        }

        public bool loadSubSetting(int value)
        {
            return HsnlibraryCS.HsnInterface.loadSubSetting(value);
        }

        public string DataVersion
        {
            get
            {
                string value = "";
                if (!HsnlibraryCS.HsnInterface.getCurrentDataVersion(ref value))
                {
                    return "";
                }
                return value;
            }
        }

        public string PresetVersion
        {
            get
            {
                string value = "";
                if (!HsnlibraryCS.HsnInterface.getCurrentPresetVersion(ref value))
                {
                    return "";
                }
                return value;
            }
        }

        public string IPVersion
        {
            get
            {
                string value = "";
                if (!HsnlibraryCS.HsnInterface.getCurrentIPVersion(ref value))
                {
                    return "";
                }
                return value;
            }
        }

        public string MainboardSN
        {
            get
            {
                string value = "";
                if (!HsnlibraryCS.HsnInterface.getMainboardSN(ref value))
                {
                    return "";
                }
                return value;
            }
        }

        public bool IpCapsuleIsInnerVisible
        {
            get { return HsnlibraryCS.HsnInterface.getIpCapsuleIsInnerVisible(); }
            set { HsnlibraryCS.HsnInterface.setIpCapsuleIsInnerVisible(value); }
        }

        public int CalibrateScanline(int offset)
        {
            int ret = HsnlibraryCS.HsnInterface.calibrateDeviceCapsuleScanline(offset);
            if (ret < 0)
            {
                System.Windows.MessageBox.Show("Calibration timeout occured");
            }
            return ret;
        }

        private void DeviceDettached(object sender, EventArgs e)
        {
            HsnlibraryCS.HsnInterface.disactivateProbe();
        }

        private void DeviceAttached(object sender, EventArgs e)
        {
            HsnlibraryCS.HsnInterface.activateProbe();
        }

        private ProbeStateInfoEnum previousState = ProbeStateInfoEnum.DIsabled;

        private void ProbeStateCallback(int state)
        {
            previousState = (ProbeStateInfoEnum)state;
        }

        private void LoadingCallback(bool value)
        {
            IsLoading = value;
        }

        public bool IsLoading { get; private set; }
    }
}
