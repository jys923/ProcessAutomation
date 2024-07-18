using System.Runtime.InteropServices;

namespace SonoCap.MES.Services.Interfaces
{
    public interface IHsnLibraryService
    {
        // initialize
        bool initialize();

        // destroy
        bool destroy();

        // listApplication
        bool listApplication([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out ApplicationTuple[] applicationList);

        // listPreset
        bool listPreset(int applicationId, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out PresetTuple[] presetList);

        // listSubSetting
        bool listSubSetting(int settingId, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out SubSettingTuple[] subsettingList);

        // saveAsUserSetting
        bool saveAsUserSetting(string settingName, out int settingId);

        // getApplicationName
        bool getApplicationName(int applicationId, [MarshalAs(UnmanagedType.LPStr)] out string applicationName);

        // getSettingName
        bool getSettingName(int settingId, [MarshalAs(UnmanagedType.LPStr)] out string settingName);

        // getSubSettingName
        bool getSubSettingName(int subsettingId, [MarshalAs(UnmanagedType.LPStr)] out string subsettingName);

        bool getDefaultApplication(out int application_id);

        bool setDefaultApplication(int application_id);

        bool getDefaultSetting(int application_id, out int setting_id);

        bool setDefaultSetting(int application_id, int setting_id);

        bool getDefaultSubSetting(int setting_id, out int subsetting_id);

        bool getCurrentApplication(out int application_id);

        bool getCurrentSetting(out int setting_id);

        bool getCurrentSubSetting(out int subsetting_id);

        bool load_Application(int app_id);

        bool load_Setting(int setting_id);

        bool load_SubSetting(int subsetting_id);

        bool startProbeDetection();

        bool stopProbeDetection();

        bool isProbeExist();

        void activateProbe();

        void disactivateProbe();

        bool ipInitialize();

        bool ipRelease();

        bool isIpInitialized();

        IntPtr ipRenderWithCapture(IntPtr output_final_image_buf, int final_image_buf_length, IntPtr output_raw_data_buf, int raw_data_buf_length, [MarshalAs(UnmanagedType.LPStr)] out string output_metadata);

        bool ipResize(int width, int height);

        IntPtr ipGetLastError(out int length);

        bool registerCallback_DeviceAttached(Action callback);

        bool registerCallback_DeviceRemoved(Action callback);

        bool registerCallback_ENDMotorSpeed(Action<int, int> callback);

        bool registerCallback_Loading(Action<bool> callback);

        bool registerCallback_ProbeState(Action<int> callback);

        bool registerCallback_Error(Action<string, int> callback);

        bool getCurrent_Data_version(out string data_version);

        bool getCurrent_Preset_version(out string preset_version);

        bool getCurrent_IP_version(out string ip_version);

        string get_probe_type_name(int index);

        short get_probe_type(int index);

        int getTransducerID();

        string getTransducerCode();

        bool getCurrentBoardName(out string name);

        bool freeze();

        bool unfreeze();

        bool isFrozen();

        void set_device_rtc_viewdepth(double value, int index);

        double get_device_rtc_viewdepth(int index);

        void set_device_tx_power(double value, int index);

        double get_device_tx_power(int index);

        void set_device_tx_cycle(int value, int index);

        int get_device_tx_cycle(int index);

        void set_device_density_factor(int value, int index);

        int get_device_density_factor(int index);

        void set_device_viewdepth_and_density(double viewdepth, int density);

        int getIpGain(int index);

        void setIpGain(int value, int index);

        bool getIpFrameAverageEnable(int index);

        void setIpFrameAverageEnable(bool value, int index);

        float getIpFrameAverageWeight(int index);

        void setIpFrameAverageWeight(float value, int index);

        float getIpDynamicRangeMax(int index);

        void setIpDynamicRangeMax(float value, int index);

        float getIpDynamicRangeMin(int index);

        void setIpDynamicRangeMin(float value, int index);

        float getIpUserTgc(int index);

        void setIpUserTgc(float value, int index);

        void getIpGrayMapTable(IntPtr table);

        bool listGrayMap(out IntPtr list, out int count);

        int getCurrentGrayMapID();

        bool loadGrayMap(int id);

        bool listSRI(out IntPtr list, out int count);

        int getCurrentSRIID();

        bool loadSRI(int id);

        bool getSRIEnable();

        void setSRIEnable(bool value);

        bool listEdgeEnhance(out IntPtr list, out int count);

        int getCurrentEdgeEnhanceID();

        bool loadEdgeEnhance(int id);

        bool getEdgeEnhanceEnable();

        void setEdgeEnhanceEnable(bool value);

        bool getIpCapsuleIsInnerVisible();
        
        void setIpCapsuleIsInnerVisible(bool value);
    }
}
