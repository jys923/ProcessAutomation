#include "HsnlibraryCS.hpp"
#include <Hsnlibrary.hpp>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::Generic;

static String^ toCSharpString(const std::string& str)
{
	return gcnew String(str.c_str());
}

static std::string toCppString(String^ str)
{
	const char* buffer = (const char*)(Marshal::StringToHGlobalAnsi(str)).ToPointer();
	auto ret = std::string(buffer);
	return ret;
}

static void convertListTupleToCSharp(std::vector<std::tuple<int32_t, std::string>> cpp_item,
	List<Tuple<int, String^>^>^% csharp_item) {
	csharp_item->Clear();
	for (auto output : cpp_item)
	{
		int id = std::get<0>(output);
		std::string temp_name = std::get<1>(output);
		String^ name = toCSharpString(temp_name);

		auto tuple = gcnew Tuple<int, String^>(id, name);
		csharp_item->Add(tuple);
	}
}

static void runDeviceAttached() {
	if (HsnlibraryCS::HsnInterface::DeviceAttached != nullptr) {
		HsnlibraryCS::HsnInterface::DeviceAttached(gcnew System::Object(), gcnew System::EventArgs());
	}
}
static void runDeviceDetached() {
	if (HsnlibraryCS::HsnInterface::DeviceDetached != nullptr) {
		HsnlibraryCS::HsnInterface::DeviceDetached(gcnew System::Object(), gcnew System::EventArgs());
	}
}

bool HsnlibraryCS::HsnInterface::initialize()
{
	bool ret = Hsnlibrary::initialize();
	//register handle
	Hsnlibrary::registerCallback_DeviceAttached(runDeviceAttached);
	Hsnlibrary::registerCallback_DeviceRemoved(runDeviceDetached);
	return ret;
}

bool HsnlibraryCS::HsnInterface::destroy()
{
	Hsnlibrary::registerCallback_DeviceAttached(static_cast<std::function<void()>>(nullptr));
	Hsnlibrary::registerCallback_DeviceRemoved(static_cast<std::function<void()>>(nullptr));
	return Hsnlibrary::destroy();
}

bool HsnlibraryCS::HsnInterface::startProbeDetection()
{
	return Hsnlibrary::startProbeDetection();
}

bool HsnlibraryCS::HsnInterface::stopProbeDetection()
{
	return Hsnlibrary::stopProbeDetection();
}

bool HsnlibraryCS::HsnInterface::isProbeExist()
{
	return Hsnlibrary::isProbeExist();
}

void HsnlibraryCS::HsnInterface::activateProbe()
{
	return Hsnlibrary::activateProbe();
}

void HsnlibraryCS::HsnInterface::disactivateProbe()
{
	return Hsnlibrary::disactivateProbe();
}

bool HsnlibraryCS::HsnInterface::listApplication(List<Tuple<int, String^>^>^% application_list)
{
	std::vector<std::tuple<int32_t, std::string>> cpp_items;
	bool ret = Hsnlibrary::listApplication(cpp_items);
	if (!ret)
		return false;
	convertListTupleToCSharp(cpp_items, application_list);
	return true;
}

bool HsnlibraryCS::HsnInterface::listPreset(int32_t application_id, List<Tuple<int, String^>^>^% preset_list)
{
	std::vector<std::tuple<int32_t, std::string>> cpp_items;
	bool ret = Hsnlibrary::listPreset(application_id, cpp_items);
	if (!ret)
		return false;
	convertListTupleToCSharp(cpp_items, preset_list);
	return true;
}

bool HsnlibraryCS::HsnInterface::listSubSetting(const int32_t setting_id, List<Tuple<int, String^>^>^% subsetting_list)
{
	std::vector<std::tuple<int32_t, std::string>> cpp_items;
	bool ret = Hsnlibrary::listSubSetting(setting_id, cpp_items);
	if (!ret)
		return false;
	convertListTupleToCSharp(cpp_items, subsetting_list);
	return true;
}

bool HsnlibraryCS::HsnInterface::saveAsUserSetting(String^ setting_name, int32_t% setting_id)
{
	auto cpp_str = toCppString(setting_name);
	int cpp_int = 0;
	auto ret = Hsnlibrary::saveAsUserSetting(cpp_str, cpp_int);
	setting_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::getApplicationName(const int32_t application_id, String^% application_name)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getApplicationName(application_id, cpp_str);
	application_name = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getSettingName(const int32_t setting_id, String^% setting_name)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getSettingName(setting_id, cpp_str);
	setting_name = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getSubSettingName(const int32_t subsetting_id, String^% subsetting_name)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getSubSettingName(subsetting_id, cpp_str);
	subsetting_name = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getDefaultApplication(int32_t% application_id)
{
	int cpp_int = 0;
	auto ret = Hsnlibrary::getDefaultApplication(cpp_int);
	application_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::setDefaultApplication(int32_t application_id)
{
	return Hsnlibrary::setDefaultApplication(application_id);
}

bool HsnlibraryCS::HsnInterface::getDefaultSetting(const int32_t application_id, int32_t% setting_id)
{
	int cpp_int = 0;
	auto ret = Hsnlibrary::getDefaultSetting(application_id, cpp_int);
	setting_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::setDefaultSetting(const int32_t application_id, int32_t setting_id)
{
	return Hsnlibrary::setDefaultSetting(application_id, setting_id);
}

bool HsnlibraryCS::HsnInterface::getDefaultSubSetting(const int32_t setting_id, int32_t% subsetting_id)
{
	int cpp_int = 0;
	auto ret = Hsnlibrary::getDefaultSubSetting(setting_id, cpp_int);
	subsetting_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::getCurrentApplication(int32_t% application_id)
{
	int cpp_int = 0;
	auto ret = Hsnlibrary::getCurrentApplication(cpp_int);
	application_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::getCurrentSetting(int32_t% setting_id)
{
	int cpp_int = 0;
	auto ret = Hsnlibrary::getCurrentSetting(cpp_int);
	setting_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::getCurrentSubSetting(int32_t% subsetting_id)
{
	int cpp_int = 0;
	auto ret = Hsnlibrary::getCurrentSubSetting(cpp_int);
	subsetting_id = cpp_int;
	return ret;
}

bool HsnlibraryCS::HsnInterface::loadApplication(int32_t app_id)
{
	return Hsnlibrary::load_Application(app_id);
}

bool HsnlibraryCS::HsnInterface::loadSetting(int32_t setting_id)
{
	return Hsnlibrary::load_Setting(setting_id);
}

bool HsnlibraryCS::HsnInterface::loadSubSetting(int32_t subsetting_id)
{
	return Hsnlibrary::load_SubSetting(subsetting_id);
}

bool HsnlibraryCS::HsnInterface::getCurrentDataVersion(String^% data_version)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getCurrent_Data_version(cpp_str);
	data_version = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getCurrentPresetVersion(String^% preset_version)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getCurrent_Preset_version(cpp_str);
	preset_version = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getCurrentIPVersion(String^% ip_version)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getCurrent_IP_version(cpp_str);
	ip_version = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getMainboardSN(String^% value)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getMainboardSN(cpp_str);
	value = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::getLibraryVersion(String^% value)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getLibraryVersion(cpp_str);
	value = toCSharpString(cpp_str);
	return ret;
}

String^ HsnlibraryCS::HsnInterface::getProbeTypeName()
{
	auto ret = toCSharpString(Hsnlibrary::get_probe_type_name());
	return ret;
}

int16_t HsnlibraryCS::HsnInterface::getProbeTypeID()
{
	auto ret = Hsnlibrary::get_probe_type();
	return ret;
}

int HsnlibraryCS::HsnInterface::getTransducerID()
{
	return Hsnlibrary::getTransducerID();
}

String^ HsnlibraryCS::HsnInterface::getTransducerCode()
{
	auto ret = toCSharpString(Hsnlibrary::getTransducerCode());
	return ret;
}

bool HsnlibraryCS::HsnInterface::getCurrentBoardName(String^% name)
{
	std::string cpp_str = "";
	auto ret = Hsnlibrary::getCurrentBoardName(cpp_str);
	name = toCSharpString(cpp_str);
	return ret;
}

bool HsnlibraryCS::HsnInterface::freeze()
{
	return Hsnlibrary::freeze();
}

bool HsnlibraryCS::HsnInterface::unfreeze()
{
	return Hsnlibrary::unfreeze();
}

bool HsnlibraryCS::HsnInterface::isFrozen()
{
	return Hsnlibrary::isFrozen();
}

void HsnlibraryCS::HsnInterface::setDeviceRtcViewdepth(double value)
{
	return Hsnlibrary::set_device_rtc_viewdepth(value);
}

double HsnlibraryCS::HsnInterface::getDeviceRtcViewdepth()
{
	return Hsnlibrary::get_device_rtc_viewdepth();
}

void HsnlibraryCS::HsnInterface::setDeviceTxPower(double value)
{
	return Hsnlibrary::set_device_tx_power(value);
}

double HsnlibraryCS::HsnInterface::getDeviceTxPower()
{
	return Hsnlibrary::get_device_tx_power();
}

void HsnlibraryCS::HsnInterface::setDeviceTxCycle(int32_t value)
{
	return Hsnlibrary::set_device_tx_cycle(value);
}

int32_t HsnlibraryCS::HsnInterface::getDeviceTxCycle()
{
	return Hsnlibrary::get_device_tx_cycle();
}

void HsnlibraryCS::HsnInterface::setDeviceDensityFactor(int32_t value)
{
	return Hsnlibrary::set_device_density_factor(value);
}

int32_t HsnlibraryCS::HsnInterface::getDeviceDensityFactor()
{
	return Hsnlibrary::get_device_density_factor();
}

void HsnlibraryCS::HsnInterface::setDeviceViewdepthDensity(double viewdepth, int32_t density)
{
	return Hsnlibrary::set_device_viewdepth_and_density(viewdepth, density);
}

int32_t HsnlibraryCS::HsnInterface::getIpGain()
{
	return Hsnlibrary::getIpGain();
}

void HsnlibraryCS::HsnInterface::setIpGain(int32_t value)
{
	return Hsnlibrary::setIpGain(value);
}

bool HsnlibraryCS::HsnInterface::getIpFrameAverageEnable()
{
	return Hsnlibrary::getIpFrameAverageEnable();
}

void HsnlibraryCS::HsnInterface::setIpFrameAverageEnable(bool value)
{
	return Hsnlibrary::setIpFrameAverageEnable(value);
}

float HsnlibraryCS::HsnInterface::getIpFrameAverageWeight()
{
	return Hsnlibrary::getIpFrameAverageWeight();
}

void HsnlibraryCS::HsnInterface::setIpFrameAverageWeight(float value)
{
	return Hsnlibrary::setIpFrameAverageWeight(value);
}

float HsnlibraryCS::HsnInterface::getIpDynamicRangeMax()
{
	return Hsnlibrary::getIpDynamicRangeMax();
}

void HsnlibraryCS::HsnInterface::setIpDynamicRangeMax(float value)
{
	return Hsnlibrary::setIpDynamicRangeMax(value);
}

float HsnlibraryCS::HsnInterface::getIpDynamicRangeMin()
{
	return Hsnlibrary::getIpDynamicRangeMin();
}

void HsnlibraryCS::HsnInterface::setIpDynamicRangeMin(float value)
{
	return Hsnlibrary::setIpDynamicRangeMin(value);
}

float HsnlibraryCS::HsnInterface::getIpUserTgc(size_t index)
{
	return Hsnlibrary::getIpUserTgc(index);
}

void HsnlibraryCS::HsnInterface::setIpUserTgc(float value, size_t index)
{
	return Hsnlibrary::setIpUserTgc(value, index);
}

void HsnlibraryCS::HsnInterface::getIpGrayMapTable(List<float>^% table)
{
	std::vector<float> cpp_items;
	Hsnlibrary::getIpGrayMapTable(cpp_items);
	table->Clear();
	for (auto output : cpp_items)
	{
		table->Add(output);
	}
}

bool HsnlibraryCS::HsnInterface::listGrayMap(List<Tuple<int, String^>^>^% list)
{
	std::vector<std::tuple<int32_t, std::string>> cpp_items;
	bool ret = Hsnlibrary::listGrayMap(cpp_items);
	if (!ret)
		return false;
	convertListTupleToCSharp(cpp_items, list);
	return true;
}

int32_t HsnlibraryCS::HsnInterface::getCurrentGrayMapID()
{
	return Hsnlibrary::getCurrentGrayMapID();
}

bool HsnlibraryCS::HsnInterface::loadGrayMap(int32_t id)
{
	return Hsnlibrary::loadGrayMap(id);
}

bool HsnlibraryCS::HsnInterface::listSRI(List<Tuple<int, String^>^>^% list)
{
	std::vector<std::tuple<int32_t, std::string>> cpp_items;
	bool ret = Hsnlibrary::listSRI(cpp_items);
	if (!ret)
		return false;
	convertListTupleToCSharp(cpp_items, list);
	return true;
}

int32_t HsnlibraryCS::HsnInterface::getCurrentSRIID()
{
	return Hsnlibrary::getCurrentSRIID();
}

bool HsnlibraryCS::HsnInterface::loadSRI(int32_t id)
{
	return Hsnlibrary::loadSRI(id);
}

bool HsnlibraryCS::HsnInterface::getSRIEnable()
{
	return Hsnlibrary::getSRIEnable();
}

void HsnlibraryCS::HsnInterface::setSRIEnable(bool value)
{
	return Hsnlibrary::setSRIEnable(value);
}

bool HsnlibraryCS::HsnInterface::listEdgeEnhance(List<Tuple<int, String^>^>^% list)
{
	std::vector<std::tuple<int32_t, std::string>> cpp_items;
	bool ret = Hsnlibrary::listEdgeEnhance(cpp_items);
	if (!ret)
		return false;
	convertListTupleToCSharp(cpp_items, list);
	return true;
}

int32_t HsnlibraryCS::HsnInterface::getCurrentEdgeEnhanceID()
{
	return Hsnlibrary::getCurrentEdgeEnhanceID();
}

bool HsnlibraryCS::HsnInterface::loadEdgeEnhance(int32_t id)
{
	return Hsnlibrary::loadEdgeEnhance(id);
}

bool HsnlibraryCS::HsnInterface::getEdgeEnhanceEnable()
{
	return Hsnlibrary::getEdgeEnhanceEnable();
}

void HsnlibraryCS::HsnInterface::setEdgeEnhanceEnable(bool value)
{
	return Hsnlibrary::setEdgeEnhanceEnable(value);
}

bool HsnlibraryCS::HsnInterface::getIpCapsuleIsInnerVisible()
{
	return Hsnlibrary::getIpCapsuleIsInnerVisible();
}

void HsnlibraryCS::HsnInterface::setIpCapsuleIsInnerVisible(bool value)
{
	return Hsnlibrary::setIpCapsuleIsInnerVisible(value);
}



void HsnlibraryCS::HsnInterface::setCapsuleCalibrateOffset(int value) {
	return Hsnlibrary::setCapsuleCalibrateOffset(value);
}
int HsnlibraryCS::HsnInterface::getCapsuleCalibrateOffset() {
	return Hsnlibrary::getCapsuleCalibrateOffset();
}
void HsnlibraryCS::HsnInterface::setCapsuleAutoCalibrateTrigCount(int value) {
	return Hsnlibrary::setCapsuleAutoCalibrateTrigCount(value);
}
int HsnlibraryCS::HsnInterface::getCapsuleAutoCalibrateTrigCount() {
	return Hsnlibrary::getCapsuleAutoCalibrateTrigCount();
}
int HsnlibraryCS::HsnInterface::runCapsuleManualCalibration() {
	return Hsnlibrary::runCapsuleManualCalibration();
}
void HsnlibraryCS::HsnInterface::setCapsuleAutoCalibrateEnable(bool value) {
	return Hsnlibrary::setCapsuleAutoCalibrateEnable(value);
}
bool HsnlibraryCS::HsnInterface::getCapsuleAutoCalibrateEnable() {
	return Hsnlibrary::getCapsuleAutoCalibrateEnable();
}

void HsnlibraryCS::Callback::registerLoadingCallback(Callback::LoadingStateCallback^ callback)
{
	if (callback != nullptr) {
		GCHandle gch = GCHandle::Alloc(callback);
		IntPtr ip = Marshal::GetFunctionPointerForDelegate(callback);
		void(__stdcall * cb)(bool) = static_cast<void(__stdcall*)(bool)>(ip.ToPointer());
		Hsnlibrary::registerCallback_Loading(cb);
		GC::KeepAlive(gch);
		//GC::Collect();
		//gch.Free();
	}
	else {
		auto cb = static_cast<void(__stdcall*)(bool)>(nullptr);
		Hsnlibrary::registerCallback_Loading(cb);
	}
}

void HsnlibraryCS::Callback::registerProbeStateCallback(Callback::ProbeStateCallback^ callback)
{
	if (callback != nullptr) {
		GCHandle gch = GCHandle::Alloc(callback);
		IntPtr ip = Marshal::GetFunctionPointerForDelegate(callback);
		void(__stdcall * cb)(int) = static_cast<void(__stdcall*)(int)>(ip.ToPointer());
		Hsnlibrary::registerCallback_ProbeState(cb);
		GC::KeepAlive(gch);
		//GC::Collect();
		//gch.Free();
	}
	else {
		auto cb = static_cast<void(__stdcall*)(int)>(nullptr);
		Hsnlibrary::registerCallback_ProbeState(cb);
	}
}

static void(__stdcall* errorCallback)(String^, int) = nullptr;
static void executeErrorCallback(std::string err_str, int err_num)
{
	auto converted_str = toCSharpString(err_str);
	if (errorCallback)
	{
		errorCallback(converted_str, err_num);
	}
}
void HsnlibraryCS::Callback::registerErrorStateCallback(Callback::ErrorStateCallback^ callback)
{
	if (callback != nullptr) {
		GCHandle gch = GCHandle::Alloc(callback);
		IntPtr ip = Marshal::GetFunctionPointerForDelegate(callback);
		errorCallback = static_cast<void(__stdcall*)(String^, int)>(ip.ToPointer());
		Hsnlibrary::registerCallback_Error(executeErrorCallback);
		GC::KeepAlive(gch);
		//GC::Collect();
		//gch.Free();
	}
	else {
		errorCallback = nullptr;
		auto cb = static_cast<void(__stdcall*)(std::string, int)>(nullptr);
		Hsnlibrary::registerCallback_Error(cb);
	}
}

void HsnlibraryCS::Callback::registerENDMotorCallback(Callback::ENDMotorCallback^ callback)
{
	if (callback != nullptr) {
		GCHandle gch = GCHandle::Alloc(callback);
		IntPtr ip = Marshal::GetFunctionPointerForDelegate(callback);
		void(__stdcall * cb)(int, int) = static_cast<void(__stdcall*)(int, int)>(ip.ToPointer());
		Hsnlibrary::registerCallback_ENDMotorSpeed(cb);
		GC::KeepAlive(gch);
		//GC::Collect();
		//gch.Free();
	}
	else {
		auto cb = static_cast<void(__stdcall*)(int, int)>(nullptr);
		Hsnlibrary::registerCallback_ENDMotorSpeed(cb);
	}
}


bool HsnlibraryCS::HsnInterface::setIpMonitorColormap(array<float>^ rgb)
{
	if (rgb->Length != 256 * 3)
		return false;
	std::vector<float> r(256);
	std::vector<float> g(256);
	std::vector<float> b(256);
	for (int i = 0; i < 256; i++)
	{
		r[i] = rgb[i + 0];
		g[i] = rgb[i + 256];
		b[i] = rgb[i + 256 * 2];
	}
	return Hsnlibrary::setIpMonitorColormap(r, g, b);
}

bool HsnlibraryCS::HsnInterface::getIpMonitorColormap([Out] array<float>^% rgb)
{
	auto arr = gcnew array<float>(256 * 3);
	std::vector<float> r;
	std::vector<float> g;
	std::vector<float> b;
	Hsnlibrary::getIpMonitorColormap(r, g, b);
	for (int c = 0; c < 3; c++)
	{
		float* pbuf;
		switch (c)
		{
		case 0: pbuf = r.data();
			break;
		case 1: pbuf = g.data();
			break;
		case 2: pbuf = b.data();
			break;
		}
		for (int i = 0; i < 256; i++)
		{
			arr[i + c * 256] = pbuf[i];
		}
	}
	rgb = arr;
	return true;
}

void HsnlibraryCS::HsnInterface::setIpIsMonitorGammaTableVisible(bool value)
{
	Hsnlibrary::setIpIsMonitorGammaTableVisible(value);
}

bool HsnlibraryCS::HsnInterface::getIpIsMonitorGammaTableVisible()
{
	return Hsnlibrary::getIpIsMonitorGammaTableVisible();
}