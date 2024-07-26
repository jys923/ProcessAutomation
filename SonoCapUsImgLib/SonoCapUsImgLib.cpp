#include "pch.h"
#include "SonoCapUsImgLib.h"
#include <chrono>
#include <ratio>

using namespace Hsnlibrary;

double ForTest(int length)
{
	auto start_time = std::chrono::high_resolution_clock::now();
	int j = 0;
	for (int i = 0; i < length; i++)
	{
		j++;
	}
	auto end_time = std::chrono::high_resolution_clock::now();
	std::chrono::duration<double, std::milli> elapsed_time = end_time - start_time;

	double elapsed_ms = elapsed_time.count();

	return elapsed_ms;
}

bool Initialize() {
	return initialize();
}

bool Destroy()
{
	return destroy();
}

bool ListApplication(std::vector<std::tuple<int32_t, std::string>>& application_list)
{
	return listApplication(application_list);
}

bool ListPreset(int32_t application_id, std::vector<std::tuple<int32_t, std::string>>& preset_list)
{
	return listPreset(application_id, preset_list);
}

bool ListSubSetting(const int32_t setting_id, std::vector<std::tuple<int32_t, std::string>>& subsetting_list)
{
	return listSubSetting(setting_id, subsetting_list);
}

bool SaveAsUserSetting(const std::string setting_name, int32_t& setting_id)
{
	return saveAsUserSetting(setting_name, setting_id);
}

bool GetApplicationName(const int32_t application_id, std::string& application_name)
{
	return getApplicationName(application_id, application_name);
}

bool GetSettingName(const int32_t setting_id, std::string& setting_name)
{
	return getSettingName(setting_id, setting_name);
}

bool GetSubSettingName(const int32_t subsetting_id, std::string& subsetting_name)
{
	return getSubSettingName(subsetting_id, subsetting_name);
}

bool GetDefaultApplication(int32_t& application_id)
{
	return getDefaultApplication(application_id);
}

bool SetDefaultApplication(int32_t application_id)
{
	return setDefaultApplication(application_id);
}

bool GetDefaultSetting(const int32_t application_id, int32_t& setting_id)
{
	return getDefaultSetting(application_id, setting_id);
}

bool SetDefaultSetting(const int32_t application_id, int32_t setting_id)
{
	return setDefaultSetting(application_id, setting_id);
}

bool GetDefaultSubSetting(const int32_t setting_id, int32_t& subsetting_id)
{
	return getDefaultSubSetting(setting_id, subsetting_id);
}

bool GetCurrentApplication(int32_t& application_id)
{
	return getCurrentApplication(application_id);
}

bool GetCurrentSetting(int32_t& setting_id)
{
	return getCurrentSetting(setting_id);
}

bool GetCurrentSubSetting(int32_t& subsetting_id)
{
	return getCurrentSubSetting(subsetting_id);
}

bool LoadApplication(int32_t app_id)
{
	return load_Application(app_id);
}

bool LoadSetting(int32_t setting_id)
{
	return load_Setting(setting_id);
}

bool LoadSubSetting(int32_t subsetting_id)
{
	return load_SubSetting(subsetting_id);
}

bool StartProbeDetection()
{
	return startProbeDetection();
}

bool StopProbeDetection()
{
	return stopProbeDetection();
}

bool IsProbeExist()
{
	return isProbeExist();
}

void ActivateProbe()
{
	activateProbe();
}

void DisactivateProbe()
{
	disactivateProbe();
}

bool IpInitialize()
{
	return ipInitialize();
}

bool IpRelease()
{
	return ipRelease();
}

bool IsIpInitialized()
{
	return isIpInitialized();
}

size_t IpRenderWithCapture(void* output_final_image_buf, size_t final_image_buf_length, void* output_raw_data_buf, size_t raw_data_buf_length, std::string& output_metadata)
{
	return ipRenderWithCapture(output_final_image_buf,final_image_buf_length,output_raw_data_buf,raw_data_buf_length,output_metadata);
}

bool IpResize(int width, int height)
{
	return ipResize(width, height);
}

const char* IpGetLastError(int& length)
{
	return ipGetLastError(length);
}

void RegisterCallbackDeviceAttached(std::function<void(void)> callback)
{
	registerCallback_DeviceAttached(callback);
}

void RegisterCallbackDeviceRemoved(std::function<void(void)> callback)
{
	registerCallback_DeviceRemoved(callback);
}

bool RegisterCallbackMotorSpeed(std::function<void(int prf_hz, int density)> callback)
{
	return registerCallback_ENDMotorSpeed(callback);
}

bool RegisterCallbackLoading(std::function<void(bool)> callback)
{
	return registerCallback_Loading(callback);
}

bool RegisterCallbackProbeState(std::function<void(int)> callback)
{
	return registerCallback_ProbeState(callback);
}

bool RegisterCallbackError(std::function<void(std::string, int)> callback)
{
	return registerCallback_Error(callback);
}

bool GetCurrentDataVersion(std::string& data_version)
{
	return getCurrent_Data_version(data_version);
}

bool GetCurrentPresetVersion(std::string& preset_version)
{
	return getCurrent_Preset_version(preset_version);
}

bool GetCurrentIPVersion(std::string& ip_version)
{
	return getCurrent_IP_version(ip_version);
}

std::string GetProbeTypeName(size_t index)
{
	return get_probe_type_name(index);
}

int16_t GetProbeType(size_t index)
{
	return get_probe_type(index);
}

int GetTransducerID()
{
	return getTransducerID();
}

std::string GetTransducerCode()
{
	return std::string();
}

bool GetCurrentBoardName(std::string& name)
{
	return getCurrentBoardName(name);
}

bool Freeze()
{
	return freeze();
}

bool Unfreeze()
{
	return unfreeze();
}

bool IsFrozen()
{
	return isFrozen();
}

void SetDeviceRtcViewdepth(double value, size_t index)
{
	set_device_rtc_viewdepth(value, index);
}

double GetDeviceRtcViewdepth(size_t index)
{
	return get_device_rtc_viewdepth(index);
}

void SetDeviceTxPower(double value, size_t index)
{
	set_device_tx_power(value, index);
}

double GetDeviceTxPower(size_t index)
{
	return get_device_tx_power(index);
}

void SetDeviceTxCycle(int32_t value, size_t index)
{
	set_device_tx_cycle(value, index);
}

int32_t GetDeviceTxCycle(size_t index)
{
	return get_device_tx_cycle(index);
}

void SetDeviceDensityFactor(int32_t value, size_t index)
{
	set_device_density_factor(value, index);
}

int32_t GetDeviceDensityFactor(size_t index)
{
	return get_device_density_factor(index);
}

void SetDeviceViewdepthAndDensity(double viewdepth, int32_t density)
{
	set_device_viewdepth_and_density(viewdepth, density);
}

int32_t GetIpGain(size_t index)
{
	return getIpGain(index);
}

void SetIpGain(int32_t value, size_t index)
{
	setIpGain(value, index);
}

bool GetIpFrameAverageEnable(size_t index)
{
	return getIpFrameAverageEnable(index);
}

void SetIpFrameAverageEnable(bool value, size_t index)
{
	setIpFrameAverageEnable(value, index);
}

float GetIpFrameAverageWeight(size_t index)
{
	return getIpFrameAverageWeight(index);
}

void SetIpFrameAverageWeight(float value, size_t index)
{
	setIpFrameAverageWeight(value, index);
}

float GetIpDynamicRangeMax(size_t index)
{
	return getIpDynamicRangeMax(index);
}

void SetIpDynamicRangeMax(float value, size_t index)
{
	setIpDynamicRangeMax(value, index);
}

float GetIpDynamicRangeMin(size_t index)
{
	return getIpDynamicRangeMin(index);
}

void SetIpDynamicRangeMin(float value, size_t index)
{
	setIpDynamicRangeMin(value, index);
}

float GetIpUserTgc(size_t index)
{
	return getIpUserTgc(index);
}

void SetIpUserTgc(float value, size_t index)
{
	setIpUserTgc(value, index);
}

void GetIpGrayMapTable(std::vector<float>& table)
{
	getIpGrayMapTable(table);
}

bool ListGrayMap(std::vector<std::tuple<int32_t, std::string>>& list)
{
	return listGrayMap(list);
}

int32_t GetCurrentGrayMapID()
{
	return getCurrentGrayMapID();
}

bool LoadGrayMap(int32_t id)
{
	return loadGrayMap(id);
}

bool ListSRI(std::vector<std::tuple<int32_t, std::string>>& list)
{
	return listSRI(list);
}

int32_t GetCurrentSRIID()
{
	return getCurrentSRIID();
}

bool LoadSRI(int32_t id)
{
	return loadSRI(id);
}

bool GetSRIEnable()
{
	return getSRIEnable();
}

void SetSRIEnable(bool value)
{
	setSRIEnable(value);
}

bool ListEdgeEnhance(std::vector<std::tuple<int32_t, std::string>>& list)
{
	return listEdgeEnhance(list);
}

int32_t GetCurrentEdgeEnhanceID()
{
	return getCurrentEdgeEnhanceID();
}

bool LoadEdgeEnhance(int32_t id)
{
	return loadEdgeEnhance(id);
}

bool GetEdgeEnhanceEnable()
{
	return getEdgeEnhanceEnable();
}

void SetEdgeEnhanceEnable(bool value)
{
	setEdgeEnhanceEnable(value);
}

bool GetIpCapsuleIsInnerVisible()
{
	return getIpCapsuleIsInnerVisible();
}

void SetIpCapsuleIsInnerVisible(bool value)
{
	setIpCapsuleIsInnerVisible(value);
}
