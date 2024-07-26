#pragma once
#ifdef HSNLIBRARYEND_EXPORTS
#define HSNLIBRARYEND_API __declspec(dllexport)
#else
#define HSNLIBRARYEND_API __declspec(dllimport)
#endif

#include <Windows.h>
#include <cstdint>
#include <string>
#include <vector>
#include <functional>

namespace Hsnlibrary
{
	extern HSNLIBRARYEND_API bool initialize();
	extern HSNLIBRARYEND_API bool destroy();

	//get list of setting
	extern HSNLIBRARYEND_API bool listApplication(std::vector<std::tuple<int32_t, std::string>>& application_list);
	extern HSNLIBRARYEND_API bool listPreset(int32_t application_id, std::vector<std::tuple<int32_t, std::string>>& preset_list);
	extern HSNLIBRARYEND_API bool listSubSetting(const int32_t setting_id, std::vector<std::tuple<int32_t, std::string>>& subsetting_list);

	extern HSNLIBRARYEND_API bool saveAsUserSetting(const std::string setting_name, int32_t& setting_id);


	extern HSNLIBRARYEND_API bool getApplicationName(const int32_t application_id, std::string& application_name);
	extern HSNLIBRARYEND_API bool getSettingName(const int32_t setting_id, std::string& setting_name);
	extern HSNLIBRARYEND_API bool getSubSettingName(const int32_t subsetting_id, std::string& subsetting_name);

	extern HSNLIBRARYEND_API bool getDefaultApplication(int32_t& application_id);
	extern HSNLIBRARYEND_API bool setDefaultApplication(int32_t application_id);
	extern HSNLIBRARYEND_API bool getDefaultSetting(const int32_t application_id, int32_t& setting_id);
	extern HSNLIBRARYEND_API bool setDefaultSetting(const int32_t application_id, int32_t setting_id);
	extern HSNLIBRARYEND_API bool getDefaultSubSetting(const int32_t setting_id, int32_t& subsetting_id);


	extern HSNLIBRARYEND_API bool getCurrentApplication(int32_t& application_id);
	extern HSNLIBRARYEND_API bool getCurrentSetting(int32_t& setting_id);
	extern HSNLIBRARYEND_API bool getCurrentSubSetting(int32_t& subsetting_id);

	//setting load
	extern HSNLIBRARYEND_API bool load_Application(int32_t app_id);
	extern HSNLIBRARYEND_API bool load_Setting(int32_t setting_id);
	extern HSNLIBRARYEND_API bool load_SubSetting(int32_t subsetting_id);

	//device connection handle
	extern HSNLIBRARYEND_API bool startProbeDetection();
	extern HSNLIBRARYEND_API bool stopProbeDetection();
	extern HSNLIBRARYEND_API bool isProbeExist();
	extern HSNLIBRARYEND_API void activateProbe();
	extern HSNLIBRARYEND_API void disactivateProbe();

	//image ip control
	extern HSNLIBRARYEND_API bool ipInitialize();
	extern HSNLIBRARYEND_API bool ipRelease();
	extern HSNLIBRARYEND_API bool isIpInitialized();
	extern HSNLIBRARYEND_API size_t ipRenderWithCapture(
		void* output_final_image_buf,
		size_t final_image_buf_length,
		void* output_raw_data_buf,
		size_t raw_data_buf_length,
		std::string& output_metadata);
	extern HSNLIBRARYEND_API bool ipResize(int width, int height);
	extern HSNLIBRARYEND_API const char* ipGetLastError(int& length);


	//callback
	extern HSNLIBRARYEND_API void registerCallback_DeviceAttached(std::function<void(void)> callback);
	extern HSNLIBRARYEND_API void registerCallback_DeviceRemoved(std::function<void(void)> callback);
	extern HSNLIBRARYEND_API bool registerCallback_ENDMotorSpeed(std::function<void(int prf_hz, int density)> callback);
	extern HSNLIBRARYEND_API bool registerCallback_Loading(std::function<void(bool)> callback);
	extern HSNLIBRARYEND_API bool registerCallback_ProbeState(std::function<void(int)> callback);
	extern HSNLIBRARYEND_API bool registerCallback_Error(std::function<void(std::string, int)> callback);


	extern HSNLIBRARYEND_API bool getCurrent_Data_version(std::string& data_version);
	extern HSNLIBRARYEND_API bool getCurrent_Preset_version(std::string& preset_version);
	extern HSNLIBRARYEND_API bool getCurrent_IP_version(std::string& ip_version);
	extern HSNLIBRARYEND_API std::string get_probe_type_name(size_t index = 0);
	extern HSNLIBRARYEND_API int16_t get_probe_type(size_t index = 0);
	extern HSNLIBRARYEND_API int getTransducerID();
	extern HSNLIBRARYEND_API std::string getTransducerCode();

	extern HSNLIBRARYEND_API bool getCurrentBoardName(std::string& name);

	//freeze function
	extern HSNLIBRARYEND_API bool freeze();
	extern HSNLIBRARYEND_API bool unfreeze();
	extern HSNLIBRARYEND_API bool isFrozen();

	extern HSNLIBRARYEND_API void set_device_rtc_viewdepth(double value, size_t index = 0);
	extern HSNLIBRARYEND_API double get_device_rtc_viewdepth(size_t index = 0);

	extern HSNLIBRARYEND_API void set_device_tx_power(double value, size_t index = 0);
	extern HSNLIBRARYEND_API double get_device_tx_power(size_t index = 0);
	//1 : 0.5cycle, 2: 1 cycle, 3: 1.5cycle
	extern HSNLIBRARYEND_API void set_device_tx_cycle(int32_t value, size_t index = 0);
	extern HSNLIBRARYEND_API int32_t get_device_tx_cycle(size_t index = 0);

	extern HSNLIBRARYEND_API void set_device_density_factor(int32_t value, size_t index = 0);
	extern HSNLIBRARYEND_API int32_t get_device_density_factor(size_t index = 0);
	extern HSNLIBRARYEND_API void set_device_viewdepth_and_density(double viewdepth, int32_t density);

	extern HSNLIBRARYEND_API int32_t getIpGain(size_t index = 0);
	extern HSNLIBRARYEND_API void setIpGain(int32_t value, size_t index = 0);
	extern HSNLIBRARYEND_API bool getIpFrameAverageEnable(size_t index = 0);
	extern HSNLIBRARYEND_API void setIpFrameAverageEnable(bool value, size_t index = 0);
	extern HSNLIBRARYEND_API float getIpFrameAverageWeight(size_t index = 0);
	extern HSNLIBRARYEND_API void setIpFrameAverageWeight(float value, size_t index = 0);
	extern HSNLIBRARYEND_API float getIpDynamicRangeMax(size_t index = 0);
	extern HSNLIBRARYEND_API void setIpDynamicRangeMax(float value, size_t index = 0);
	extern HSNLIBRARYEND_API float getIpDynamicRangeMin(size_t index = 0);
	extern HSNLIBRARYEND_API void setIpDynamicRangeMin(float value, size_t index = 0);


	extern HSNLIBRARYEND_API float getIpUserTgc(size_t index = 0);
	extern HSNLIBRARYEND_API void setIpUserTgc(float value, size_t index = 0);

	extern HSNLIBRARYEND_API void getIpGrayMapTable(std::vector<float>& table);
	extern HSNLIBRARYEND_API bool listGrayMap(std::vector<std::tuple<int32_t, std::string>>& list);
	extern HSNLIBRARYEND_API int32_t getCurrentGrayMapID();
	extern HSNLIBRARYEND_API bool loadGrayMap(int32_t id);

	extern HSNLIBRARYEND_API bool listSRI(std::vector<std::tuple<int32_t, std::string>>& list);
	extern HSNLIBRARYEND_API int32_t getCurrentSRIID();
	extern HSNLIBRARYEND_API bool loadSRI(int32_t id);
	extern HSNLIBRARYEND_API bool getSRIEnable();
	extern HSNLIBRARYEND_API void setSRIEnable(bool value);

	extern HSNLIBRARYEND_API bool listEdgeEnhance(std::vector<std::tuple<int32_t, std::string>>& list);
	extern HSNLIBRARYEND_API int32_t getCurrentEdgeEnhanceID();
	extern HSNLIBRARYEND_API bool loadEdgeEnhance(int32_t id);
	extern HSNLIBRARYEND_API bool getEdgeEnhanceEnable();
	extern HSNLIBRARYEND_API void setEdgeEnhanceEnable(bool value);


	extern HSNLIBRARYEND_API bool getIpCapsuleIsInnerVisible();
	extern HSNLIBRARYEND_API void setIpCapsuleIsInnerVisible(bool value);
}