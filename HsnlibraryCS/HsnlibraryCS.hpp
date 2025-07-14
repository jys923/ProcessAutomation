#pragma once

#include <cstdint>

namespace HsnlibraryCS {
	using namespace System;
	using namespace System::Collections::Generic;
	using namespace System::Runtime::InteropServices;

	////device connection handle

	public ref class Callback {

	public:
		delegate void LoadingStateCallback(bool val);
		delegate void ProbeStateCallback(int val);
		delegate void ErrorStateCallback(String^ err_str, int err_num);
		delegate void ENDMotorCallback(int prf_hz, int density);

		static void registerLoadingCallback(Callback::LoadingStateCallback^ callback);
		static void registerProbeStateCallback(Callback::ProbeStateCallback^ callback);
		static void registerErrorStateCallback(Callback::ErrorStateCallback^ callback);
		static void registerENDMotorCallback(Callback::ENDMotorCallback^ callback);
		//bool registerCallback_ENDMotorSpeed(std::function<void(int prf_hz, int density)> callback);
		//bool registerCallback_Loading(std::function<void(bool)> callback);
		//bool registerCallback_ProbeState(std::function<void(int)> callback);
		//bool registerCallback_Error(std::function<void(String^, int)> callback);

	};

	public ref class HsnInterface {
	public:
		static bool initialize();
		static bool destroy();

		static bool startProbeDetection();
		static bool stopProbeDetection();
		static bool isProbeExist();
		static void activateProbe();
		static void disactivateProbe();
		static EventHandler^ DeviceAttached = nullptr;
		static EventHandler^ DeviceDetached = nullptr;


		//get list of setting
		static bool listApplication(List<Tuple<int, String^>^>^% application_list);
		static bool listPreset(int32_t application_id, List<Tuple<int, String^>^>^% preset_list);
		static bool listSubSetting(const int32_t setting_id, List<Tuple<int, String^>^>^% subsetting_list);
		static bool saveAsUserSetting(String^ setting_name, int32_t% setting_id);
		static bool getApplicationName(const int32_t application_id, String^% application_name);
		static bool getSettingName(const int32_t setting_id, String^% setting_name);
		static bool getSubSettingName(const int32_t subsetting_id, String^% subsetting_name);
		static bool getDefaultApplication(int32_t% application_id);
		static bool setDefaultApplication(int32_t application_id);
		static bool getDefaultSetting(const int32_t application_id, int32_t% setting_id);
		static bool setDefaultSetting(const int32_t application_id, int32_t setting_id);
		static bool getDefaultSubSetting(const int32_t setting_id, int32_t% subsetting_id);
		static bool getCurrentApplication(int32_t% application_id);
		static bool getCurrentSetting(int32_t% setting_id);
		static bool getCurrentSubSetting(int32_t% subsetting_id);
		////setting load
		static bool loadApplication(int32_t app_id);
		static bool loadSetting(int32_t setting_id);
		static bool loadSubSetting(int32_t subsetting_id);
		static bool getCurrentDataVersion(String^% data_version);
		static bool getCurrentPresetVersion(String^% preset_version);
		static bool getCurrentIPVersion(String^% ip_version);
		static bool getMainboardSN(String^% value);
		static bool getLibraryVersion(String^% value);
		static String^ getProbeTypeName();
		static int16_t getProbeTypeID();
		static int getTransducerID();
		static String^ getTransducerCode();
		static bool getCurrentBoardName(String^% name);
		////freeze function
		static bool freeze();
		static bool unfreeze();
		static bool isFrozen();
		static void setDeviceRtcViewdepth(double value);
		static double getDeviceRtcViewdepth();
		static void setDeviceTxPower(double value);
		static double getDeviceTxPower();
		////1 : 0.5cycle, 2: 1 cycle, 3: 1.5cycle
		static void setDeviceTxCycle(int32_t value);
		static int32_t getDeviceTxCycle();
		static void setDeviceDensityFactor(int32_t value);
		static int32_t getDeviceDensityFactor();
		static void setDeviceViewdepthDensity(double viewdepth, int32_t density);
		static int32_t getIpGain();
		static void setIpGain(int32_t value);
		static bool getIpFrameAverageEnable();
		static void setIpFrameAverageEnable(bool value);
		static float getIpFrameAverageWeight();
		static void setIpFrameAverageWeight(float value);
		static float getIpDynamicRangeMax();
		static void setIpDynamicRangeMax(float value);
		static float getIpDynamicRangeMin();
		static void setIpDynamicRangeMin(float value);
		static float getIpUserTgc(size_t index);
		static void setIpUserTgc(float value, size_t index);
		static void getIpGrayMapTable(List<float>^% table);
		static bool listGrayMap(List<Tuple<int, String^>^>^% list);
		static int32_t getCurrentGrayMapID();
		static bool loadGrayMap(int32_t id);
		static bool listSRI(List<Tuple<int, String^>^>^% list);
		static int32_t getCurrentSRIID();
		static bool loadSRI(int32_t id);
		static bool getSRIEnable();
		static void setSRIEnable(bool value);
		static bool listEdgeEnhance(List<Tuple<int, String^>^>^% list);
		static int32_t getCurrentEdgeEnhanceID();
		static bool loadEdgeEnhance(int32_t id);
		static bool getEdgeEnhanceEnable();
		static void setEdgeEnhanceEnable(bool value);
		static bool getIpCapsuleIsInnerVisible();
		static void setIpCapsuleIsInnerVisible(bool value);
		static void setCapsuleCalibrateOffset(int value);
		static int getCapsuleCalibrateOffset();
		static void setCapsuleAutoCalibrateTrigCount(int value);
		static int getCapsuleAutoCalibrateTrigCount();
		static int runCapsuleManualCalibration();
		static void setCapsuleAutoCalibrateEnable(bool value);
		static bool getCapsuleAutoCalibrateEnable();
		//r->g->b order
		static bool setIpMonitorColormap(array<float>^ rgb);
		static bool getIpMonitorColormap([Out] array<float>^% rgb);
		static void setIpIsMonitorGammaTableVisible(bool value);
		static bool getIpIsMonitorGammaTableVisible();
	private:
	};
}
