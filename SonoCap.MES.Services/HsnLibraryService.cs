using SonoCap.MES.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.Services
{
    public static class HsnLibraryService // :  :IHsnLibraryService
    {
        // DLL 경로 설정
        private const string DllPath = "YourDllFileName.dll"; // DLL 파일명에 맞게 변경

        // initialize
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool initialize();

        // destroy
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool destroy();

        // listApplication
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool listApplication([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out ApplicationTuple[] applicationList);

        // listPreset
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool listPreset(int applicationId, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out PresetTuple[] presetList);

        // listSubSetting
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool listSubSetting(int settingId, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out SubSettingTuple[] subsettingList);

        // saveAsUserSetting
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool saveAsUserSetting(string settingName, out int settingId);

        // getApplicationName
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getApplicationName(int applicationId, [MarshalAs(UnmanagedType.LPStr)] out string applicationName);

        // getSettingName
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getSettingName(int settingId, [MarshalAs(UnmanagedType.LPStr)] out string settingName);

        // getSubSettingName
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getSubSettingName(int subsettingId, [MarshalAs(UnmanagedType.LPStr)] out string subsettingName);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getDefaultApplication(out int application_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool setDefaultApplication(int application_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getDefaultSetting(int application_id, out int setting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool setDefaultSetting(int application_id, int setting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getDefaultSubSetting(int setting_id, out int subsetting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrentApplication(out int application_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrentSetting(out int setting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrentSubSetting(out int subsetting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool load_Application(int app_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool load_Setting(int setting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool load_SubSetting(int subsetting_id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool startProbeDetection();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool stopProbeDetection();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool isProbeExist();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void activateProbe();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void disactivateProbe();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ipInitialize();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ipRelease();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool isIpInitialized();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ipRenderWithCapture(IntPtr output_final_image_buf, int final_image_buf_length, IntPtr output_raw_data_buf, int raw_data_buf_length, [MarshalAs(UnmanagedType.LPStr)] out string output_metadata);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ipResize(int width, int height);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ipGetLastError(out int length);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool registerCallback_DeviceAttached(Action callback);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool registerCallback_DeviceRemoved(Action callback);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool registerCallback_ENDMotorSpeed(Action<int, int> callback);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool registerCallback_Loading(Action<bool> callback);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool registerCallback_ProbeState(Action<int> callback);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool registerCallback_Error(Action<string, int> callback);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrent_Data_version(out string data_version);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrent_Preset_version(out string preset_version);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrent_IP_version(out string ip_version);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string get_probe_type_name(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern short get_probe_type(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getTransducerID();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string getTransducerCode();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getCurrentBoardName(out string name);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool freeze();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool unfreeze();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool isFrozen();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_device_rtc_viewdepth(double value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern double get_device_rtc_viewdepth(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_device_tx_power(double value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern double get_device_tx_power(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_device_tx_cycle(int value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_device_tx_cycle(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_device_density_factor(int value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_device_density_factor(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_device_viewdepth_and_density(double viewdepth, int density);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getIpGain(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpGain(int value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getIpFrameAverageEnable(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpFrameAverageEnable(bool value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getIpFrameAverageWeight(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpFrameAverageWeight(float value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getIpDynamicRangeMax(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpDynamicRangeMax(float value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getIpDynamicRangeMin(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpDynamicRangeMin(float value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getIpUserTgc(int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpUserTgc(float value, int index);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void getIpGrayMapTable(IntPtr table);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool listGrayMap(out IntPtr list, out int count);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCurrentGrayMapID();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool loadGrayMap(int id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool listSRI(out IntPtr list, out int count);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCurrentSRIID();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool loadSRI(int id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getSRIEnable();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setSRIEnable(bool value);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool listEdgeEnhance(out IntPtr list, out int count);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCurrentEdgeEnhanceID();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool loadEdgeEnhance(int id);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getEdgeEnhanceEnable();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setEdgeEnhanceEnable(bool value);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool getIpCapsuleIsInnerVisible();

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setIpCapsuleIsInnerVisible(bool value);
    }

    // 구조체 정의
    [StructLayout(LayoutKind.Sequential)]
    public struct ApplicationTuple
    {
        public int Id;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PresetTuple
    {
        public int Id;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SubSettingTuple
    {
        public int Id;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
    }
}
