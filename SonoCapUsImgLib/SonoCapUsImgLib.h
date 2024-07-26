#include <Windows.h>
#include <cstdint>
#include <string>
#include <vector>
#include <functional>
#include "Hsnlibrary.h"

#pragma once
extern "C" __declspec(dllexport) double ForTest(int length);

extern "C" __declspec(dllexport) bool Initialize();
extern "C" __declspec(dllexport) bool Destroy();
extern "C" __declspec(dllexport) bool ListApplication(std::vector<std::tuple<int32_t, std::string>>& application_list);
extern "C" __declspec(dllexport) bool ListPreset(int32_t application_id, std::vector<std::tuple<int32_t, std::string>>& preset_list);
extern "C" __declspec(dllexport) bool ListSubSetting(const int32_t setting_id, std::vector<std::tuple<int32_t, std::string>>& subsetting_list);
extern "C" __declspec(dllexport) bool SaveAsUserSetting(const std::string setting_name, int32_t& setting_id);
extern "C" __declspec(dllexport) bool GetApplicationName(const int32_t application_id, std::string& application_name);
extern "C" __declspec(dllexport) bool GetSettingName(const int32_t setting_id, std::string& setting_name);
extern "C" __declspec(dllexport) bool GetSubSettingName(const int32_t subsetting_id, std::string& subsetting_name);
extern "C" __declspec(dllexport) bool GetDefaultApplication(int32_t& application_id);
extern "C" __declspec(dllexport) bool SetDefaultApplication(int32_t application_id);
extern "C" __declspec(dllexport) bool GetDefaultSetting(const int32_t application_id, int32_t& setting_id);
extern "C" __declspec(dllexport) bool SetDefaultSetting(const int32_t application_id, int32_t setting_id);
extern "C" __declspec(dllexport) bool GetDefaultSubSetting(const int32_t setting_id, int32_t& subsetting_id);
extern "C" __declspec(dllexport) bool GetCurrentApplication(int32_t& application_id);
extern "C" __declspec(dllexport) bool GetCurrentSetting(int32_t& setting_id);
extern "C" __declspec(dllexport) bool GetCurrentSubSetting(int32_t& subsetting_id);
extern "C" __declspec(dllexport) bool LoadApplication(int32_t app_id);
extern "C" __declspec(dllexport) bool LoadSetting(int32_t setting_id);
extern "C" __declspec(dllexport) bool LoadSubSetting(int32_t subsetting_id);
extern "C" __declspec(dllexport) bool StartProbeDetection();
extern "C" __declspec(dllexport) bool StopProbeDetection();
extern "C" __declspec(dllexport) bool IsProbeExist();
extern "C" __declspec(dllexport) void ActivateProbe();
extern "C" __declspec(dllexport) void DisactivateProbe();
extern "C" __declspec(dllexport) bool IpInitialize();
extern "C" __declspec(dllexport) bool IpRelease();
extern "C" __declspec(dllexport) bool IsIpInitialized();
extern "C" __declspec(dllexport) size_t IpRenderWithCapture(void* output_final_image_buf,size_t final_image_buf_length,void* output_raw_data_buf,size_t raw_data_buf_length,std::string& output_metadata);
extern "C" __declspec(dllexport) bool IpResize(int width, int height);
extern "C" __declspec(dllexport) const char* IpGetLastError(int& length);
extern "C" __declspec(dllexport) void RegisterCallbackDeviceAttached(std::function<void(void)> callback);
extern "C" __declspec(dllexport) void RegisterCallbackDeviceRemoved(std::function<void(void)> callback);
extern "C" __declspec(dllexport) bool RegisterCallbackMotorSpeed(std::function<void(int prf_hz, int density)> callback);
extern "C" __declspec(dllexport) bool RegisterCallbackLoading(std::function<void(bool)> callback);
extern "C" __declspec(dllexport) bool RegisterCallbackProbeState(std::function<void(int)> callback);
extern "C" __declspec(dllexport) bool RegisterCallbackError(std::function<void(std::string, int)> callback);
extern "C" __declspec(dllexport) bool GetCurrentDataVersion(std::string& data_version);
extern "C" __declspec(dllexport) bool GetCurrentPresetVersion(std::string& preset_version);
extern "C" __declspec(dllexport) bool GetCurrentIPVersion(std::string& ip_version);
extern "C" __declspec(dllexport) std::string GetProbeTypeName(size_t index = 0);
extern "C" __declspec(dllexport) int16_t GetProbeType(size_t index = 0);
extern "C" __declspec(dllexport) int GetTransducerID();
extern "C" __declspec(dllexport) std::string GetTransducerCode();
extern "C" __declspec(dllexport) bool GetCurrentBoardName(std::string& name);
extern "C" __declspec(dllexport) bool Freeze();
extern "C" __declspec(dllexport) bool Unfreeze();
extern "C" __declspec(dllexport) bool IsFrozen();
extern "C" __declspec(dllexport) void SetDeviceRtcViewdepth(double value, size_t index = 0);
extern "C" __declspec(dllexport) double GetDeviceRtcViewdepth(size_t index = 0);
extern "C" __declspec(dllexport) void SetDeviceTxPower(double value, size_t index = 0);
extern "C" __declspec(dllexport) double GetDeviceTxPower(size_t index = 0);

extern "C" __declspec(dllexport) void SetDeviceTxCycle(int32_t value, size_t index = 0);
extern "C" __declspec(dllexport) int32_t GetDeviceTxCycle(size_t index = 0);

extern "C" __declspec(dllexport) void SetDeviceDensityFactor(int32_t value, size_t index = 0);
extern "C" __declspec(dllexport) int32_t GetDeviceDensityFactor(size_t index = 0);
extern "C" __declspec(dllexport) void SetDeviceViewdepthAndDensity(double viewdepth, int32_t density);
extern "C" __declspec(dllexport) int32_t GetIpGain(size_t index = 0);
extern "C" __declspec(dllexport) void SetIpGain(int32_t value, size_t index = 0);
extern "C" __declspec(dllexport) bool GetIpFrameAverageEnable(size_t index = 0);
extern "C" __declspec(dllexport) void SetIpFrameAverageEnable(bool value, size_t index = 0);
extern "C" __declspec(dllexport) float GetIpFrameAverageWeight(size_t index = 0);
extern "C" __declspec(dllexport) void SetIpFrameAverageWeight(float value, size_t index = 0);
extern "C" __declspec(dllexport) float GetIpDynamicRangeMax(size_t index = 0);
extern "C" __declspec(dllexport) void SetIpDynamicRangeMax(float value, size_t index = 0);
extern "C" __declspec(dllexport) float GetIpDynamicRangeMin(size_t index = 0);
extern "C" __declspec(dllexport) void SetIpDynamicRangeMin(float value, size_t index = 0);
extern "C" __declspec(dllexport) float GetIpUserTgc(size_t index = 0);
extern "C" __declspec(dllexport) void SetIpUserTgc(float value, size_t index = 0);
extern "C" __declspec(dllexport) void GetIpGrayMapTable(std::vector<float>& table);
extern "C" __declspec(dllexport) bool ListGrayMap(std::vector<std::tuple<int32_t, std::string>>& list);
extern "C" __declspec(dllexport) int32_t GetCurrentGrayMapID();
extern "C" __declspec(dllexport) bool LoadGrayMap(int32_t id);
extern "C" __declspec(dllexport) bool ListSRI(std::vector<std::tuple<int32_t, std::string>>& list);
extern "C" __declspec(dllexport) int32_t GetCurrentSRIID();
extern "C" __declspec(dllexport) bool LoadSRI(int32_t id);
extern "C" __declspec(dllexport) bool GetSRIEnable();
extern "C" __declspec(dllexport) void SetSRIEnable(bool value);
extern "C" __declspec(dllexport) bool ListEdgeEnhance(std::vector<std::tuple<int32_t, std::string>>& list);
extern "C" __declspec(dllexport) int32_t GetCurrentEdgeEnhanceID();
extern "C" __declspec(dllexport) bool LoadEdgeEnhance(int32_t id);
extern "C" __declspec(dllexport) bool GetEdgeEnhanceEnable();
extern "C" __declspec(dllexport) void SetEdgeEnhanceEnable(bool value);
extern "C" __declspec(dllexport) bool GetIpCapsuleIsInnerVisible();
extern "C" __declspec(dllexport) void SetIpCapsuleIsInnerVisible(bool value);