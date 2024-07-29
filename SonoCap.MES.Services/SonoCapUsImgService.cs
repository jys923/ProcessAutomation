using System.Runtime.InteropServices;
using System.Text;

namespace SonoCap.MES.Services
{
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

    public delegate void DeviceAttached();
    public delegate void DeviceRemoved();
    public delegate void MotorSpeed(int value, int value2);
    public delegate void Loading();
    public delegate void ProbeState(int value);
    public delegate void Error(string message, int value);

    public static class SonoCapUsImgService
    {
        // DLL 경로 설정
        private const string DllName = "./SonoCapUsImgD.dll";

        // 기본 함수
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double ForTest(int length);

        // Initialization and cleanup
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Initialize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Destroy();

        // Lists and settings
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        //public static extern bool ListApplication([In, Out] List<Tuple<int, string>> application_list);
        public static extern bool ListApplication([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out ApplicationTuple[] applicationList);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        //public static extern bool ListPreset(int application_id, [In, Out] List<Tuple<int, string>> preset_list);
        public static extern bool ListPreset(int applicationId, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out PresetTuple[] presetList);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        //public static extern bool ListSubSetting(int setting_id, [In, Out] List<Tuple<int, string>> subsetting_list);
        public static extern bool ListSubSetting(int settingId, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out SubSettingTuple[] subsettingList);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SaveAsUserSetting(string setting_name, out int setting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetApplicationName(int application_id, out string application_name);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetSettingName(int setting_id, out string setting_name);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetSubSettingName(int subsetting_id, out string subsetting_name);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetDefaultApplication(out int application_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetDefaultApplication(int application_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetDefaultSetting(int application_id, out int setting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetDefaultSetting(int application_id, int setting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetDefaultSubSetting(int setting_id, out int subsetting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentApplication(out int application_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentSetting(out int setting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentSubSetting(out int subsetting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LoadApplication(int app_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LoadSetting(int setting_id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LoadSubSetting(int subsetting_id);

        // Probe functions
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool StartProbeDetection();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool StopProbeDetection();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsProbeExist();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ActivateProbe();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeactivateProbe();

        // IP functions
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IpInitialize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IpRelease();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsIpInitialized();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr IpRenderWithCapture(
            IntPtr output_final_image_buf,
            UIntPtr final_image_buf_length,
            IntPtr output_raw_data_buf,
            UIntPtr raw_data_buf_length,
            StringBuilder output_metadata);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IpResize(int width, int height);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr IpGetLastError(out int length);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterCallbackDeviceAttached([MarshalAs(UnmanagedType.FunctionPtr)] DeviceAttached callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterCallbackDeviceRemoved([MarshalAs(UnmanagedType.FunctionPtr)] DeviceRemoved callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RegisterCallbackMotorSpeed([MarshalAs(UnmanagedType.FunctionPtr)] MotorSpeed callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RegisterCallbackLoading([MarshalAs(UnmanagedType.FunctionPtr)]Loading callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RegisterCallbackProbeState([MarshalAs(UnmanagedType.FunctionPtr)] ProbeState callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RegisterCallbackError([MarshalAs(UnmanagedType.FunctionPtr)] Error callback);
        // Versions
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentDataVersion(out string data_version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentPresetVersion(out string preset_version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentIPVersion(out string ip_version);

        // Probe type
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetProbeTypeName(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern short GetProbeType(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTransducerID();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTransducerCode();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetCurrentBoardName(out string name);

        // Freeze
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Freeze();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Unfreeze();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool IsFrozen();

        // Device settings
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDeviceRtcViewdepth(double value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetDeviceRtcViewdepth(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDeviceTxPower(double value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetDeviceTxPower(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDeviceTxCycle(int value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceTxCycle(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDeviceDensityFactor(int value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceDensityFactor(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDeviceViewdepthAndDensity(double viewdepth, int density);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetIpGain(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpGain(int value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetIpFrameAverageEnable(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpFrameAverageEnable(bool value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetIpFrameAverageWeight(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpFrameAverageWeight(float value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetIpDynamicRangeMax(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpDynamicRangeMax(float value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetIpDynamicRangeMin(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpDynamicRangeMin(float value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetIpUserTgc(UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpUserTgc(float value, UIntPtr index = default);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetIpGrayMapTable([In, Out] List<float> table);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ListGrayMap([In, Out] List<Tuple<int, string>> list);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetCurrentGrayMapID();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LoadGrayMap(int id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ListSRI([In, Out] List<Tuple<int, string>> list);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetCurrentSRIID();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LoadSRI(int id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetSRIEnable();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSRIEnable(bool value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ListEdgeEnhance([In, Out] List<Tuple<int, string>> list);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetCurrentEdgeEnhanceID();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool LoadEdgeEnhance(int id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetEdgeEnhanceEnable();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetEdgeEnhanceEnable(bool value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetIpCapsuleIsInnerVisible();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIpCapsuleIsInnerVisible(bool value);
    }
}
