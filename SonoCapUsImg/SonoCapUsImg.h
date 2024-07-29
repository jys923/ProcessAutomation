//SonoCapUsImg.h

#pragma once

#include <Windows.h>
#include <cstdint>
#include <string>
#include <vector>
#include <functional>
#include "Hsnlibrary.h"
#include <tchar.h>
#include <chrono>
#include <ratio>
#include "glad/glad.h"
#include "nlohmann/json.hpp"

typedef void(__stdcall* DeviceAttached)();
typedef void(__stdcall* DeviceRemoved)();
typedef void(__stdcall* MotorSpeed)(int value, int value2);
typedef void(__stdcall* Loading)(bool value);
typedef void(__stdcall* ProbeState)(int value);
typedef void(__stdcall* Error)(std::string message, int value);
typedef unsigned char byte;

#include <mutex>


class HsnBufferCreator
{

public:
	HsnBufferCreator(int width, int height);
	virtual ~HsnBufferCreator();
	bool initiate();
	bool destroy();

	bool resizeWindow(int width, int height);
	uint8_t* getBitmapBuffer(size_t& length);
	inline int getWidth() { return width; }
	inline int getHeight() { return height; }

	void registerUnitMmPixelCallback(std::function<void(float)> callback) {
		unit_mm_per_pixel_callback = callback;
	}

	std::string output_metadata;

private:
	std::mutex g_mutex;
	HWND handle = nullptr;
	int width;
	int height;
	size_t buffer_size;
	uint8_t* buffer = nullptr;
	uint8_t* envdata_buffer = nullptr;
	size_t envdata_buffer_size;

	HDC device_context;
	HGLRC rendering_context;
	bool initiated = false;
	bool renderBitmap();
	std::function<void(float)> unit_mm_per_pixel_callback = nullptr;
	//std::function<void(uint8_t*, int)> callback;
};

HsnBufferCreator* buffer_creator = nullptr;

extern "C" {
	__declspec(dllexport) double ForTest(int length);
	__declspec(dllexport) void HsnBufferCreatorInitialize();
	__declspec(dllexport) void HsnBufferCreatorGetBitmapBuffer();
	__declspec(dllexport) bool Initialize();
	__declspec(dllexport) bool Destroy();
	__declspec(dllexport) bool ListApplication(std::vector<std::tuple<int32_t, std::string>>& on_list);
	__declspec(dllexport) bool ListPreset(int32_t application_id, std::vector<std::tuple<int32_t, std::string>>& preset_list);
	__declspec(dllexport) bool ListSubSetting(const int32_t setting_id, std::vector<std::tuple<int32_t, std::string>>& subsetting_list);
	__declspec(dllexport) bool SaveAsUserSetting(const std::string setting_name, int32_t& setting_id);
	__declspec(dllexport) bool GetApplicationName(const int32_t application_id, std::string& on_name);
	__declspec(dllexport) bool GetSettingName(const int32_t setting_id, std::string& setting_name);
	__declspec(dllexport) bool GetSubSettingName(const int32_t subsetting_id, std::string& g_name);
	__declspec(dllexport) bool GetDefaultApplication(int32_t& application_id);
	__declspec(dllexport) bool SetDefaultApplication(int32_t application_id);
	__declspec(dllexport) bool GetDefaultSetting(const int32_t application_id, int32_t& setting_id);
	__declspec(dllexport) bool SetDefaultSetting(const int32_t application_id, int32_t setting_id);
	__declspec(dllexport) bool GetDefaultSubSetting(const int32_t setting_id, int32_t& subsetting_id);
	__declspec(dllexport) bool GetCurrentApplication(int32_t& application_id);
	__declspec(dllexport) bool GetCurrentSetting(int32_t& setting_id);
	__declspec(dllexport) bool GetCurrentSubSetting(int32_t& subsetting_id);
	__declspec(dllexport) bool LoadApplication(int32_t app_id);
	__declspec(dllexport) bool LoadSetting(int32_t setting_id);
	__declspec(dllexport) bool LoadSubSetting(int32_t subsetting_id);
	__declspec(dllexport) bool StartProbeDetection();
	__declspec(dllexport) bool StopProbeDetection();
	__declspec(dllexport) bool IsProbeExist();
	__declspec(dllexport) void ActivateProbe();
	__declspec(dllexport) void DeactivateProbe();
	__declspec(dllexport) bool IpInitialize();
	__declspec(dllexport) bool IpRelease();
	__declspec(dllexport) bool IsIpInitialized();
	__declspec(dllexport) size_t IpRenderWithCapture(void* output_final_image_buf,size_t ge_buf_length,void* output_raw_data_buf,size_t raw_data_buf_length,std::string& output_metadata);
	__declspec(dllexport) bool IpResize(int width, int height);
	__declspec(dllexport) const char* IpGetLastError(int& length);
	__declspec(dllexport) void RegisterCallbackDeviceAttached(DeviceAttached callback);
	__declspec(dllexport) void RegisterCallbackDeviceRemoved(DeviceRemoved callback);
	__declspec(dllexport) bool RegisterCallbackMotorSpeed(MotorSpeed callback);
	__declspec(dllexport) bool RegisterCallbackLoading(Loading callback);
	__declspec(dllexport) bool RegisterCallbackProbeState(ProbeState callback);
	__declspec(dllexport) bool RegisterCallbackError(Error callback);
	__declspec(dllexport) bool GetCurrentDataVersion(std::string& data_version);
	__declspec(dllexport) bool GetCurrentPresetVersion(std::string& preset_version);
	__declspec(dllexport) bool GetCurrentIPVersion(std::string& ip_version);
	__declspec(dllexport) std::string GetProbeTypeName(size_t index = 0);
	__declspec(dllexport) int16_t GetProbeType(size_t index = 0);
	__declspec(dllexport) int GetTransducerID();
	__declspec(dllexport) std::string GetTransducerCode();
	__declspec(dllexport) bool GetCurrentBoardName(std::string& name);
	__declspec(dllexport) bool Freeze();
	__declspec(dllexport) bool Unfreeze();
	__declspec(dllexport) bool IsFrozen();
	__declspec(dllexport) void SetDeviceRtcViewdepth(double value, size_t index = 0);
	__declspec(dllexport) double GetDeviceRtcViewdepth(size_t index = 0);
	__declspec(dllexport) void SetDeviceTxPower(double value, size_t index = 0);
	__declspec(dllexport) double GetDeviceTxPower(size_t index = 0);

	__declspec(dllexport) void SetDeviceTxCycle(int32_t value, size_t index = 0);
	__declspec(dllexport) int32_t GetDeviceTxCycle(size_t index = 0);

	__declspec(dllexport) void SetDeviceDensityFactor(int32_t value, size_t index = 0);
	__declspec(dllexport) int32_t GetDeviceDensityFactor(size_t index = 0);
	__declspec(dllexport) void SetDeviceViewdepthAndDensity(double viewdepth, int32_t density);
	__declspec(dllexport) int32_t GetIpGain(size_t index = 0);
	__declspec(dllexport) void SetIpGain(int32_t value, size_t index = 0);
	__declspec(dllexport) bool GetIpFrameAverageEnable(size_t index = 0);
	__declspec(dllexport) void SetIpFrameAverageEnable(bool value, size_t index = 0);
	__declspec(dllexport) float GetIpFrameAverageWeight(size_t index = 0);
	__declspec(dllexport) void SetIpFrameAverageWeight(float value, size_t index = 0);
	__declspec(dllexport) float GetIpDynamicRangeMax(size_t index = 0);
	__declspec(dllexport) void SetIpDynamicRangeMax(float value, size_t index = 0);
	__declspec(dllexport) float GetIpDynamicRangeMin(size_t index = 0);
	__declspec(dllexport) void SetIpDynamicRangeMin(float value, size_t index = 0);
	__declspec(dllexport) float GetIpUserTgc(size_t index = 0);
	__declspec(dllexport) void SetIpUserTgc(float value, size_t index = 0);
	__declspec(dllexport) void GetIpGrayMapTable(std::vector<float>& table);
	__declspec(dllexport) bool ListGrayMap(std::vector<std::tuple<int32_t, std::string>>& list);
	__declspec(dllexport) int32_t GetCurrentGrayMapID();
	__declspec(dllexport) bool LoadGrayMap(int32_t id);
	__declspec(dllexport) bool ListSRI(std::vector<std::tuple<int32_t, std::string>>& list);
	__declspec(dllexport) int32_t GetCurrentSRIID();
	__declspec(dllexport) bool LoadSRI(int32_t id);
	__declspec(dllexport) bool GetSRIEnable();
	__declspec(dllexport) void SetSRIEnable(bool value);
	__declspec(dllexport) bool ListEdgeEnhance(std::vector<std::tuple<int32_t, std::string>>& list);
	__declspec(dllexport) int32_t GetCurrentEdgeEnhanceID();
	__declspec(dllexport) bool LoadEdgeEnhance(int32_t id);
	__declspec(dllexport) bool GetEdgeEnhanceEnable();
	__declspec(dllexport) void SetEdgeEnhanceEnable(bool value);
	__declspec(dllexport) bool GetIpCapsuleIsInnerVisible();
	__declspec(dllexport) void SetIpCapsuleIsInnerVisible(bool value);
}