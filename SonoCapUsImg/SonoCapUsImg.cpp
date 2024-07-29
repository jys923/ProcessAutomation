#include "pch.h"
#include "SonoCapUsImg.h"
//#include "HsnBufferCreator.h"
//class HsnBufferCreator;

HsnBufferCreator::HsnBufferCreator(int width, int height) {

	resizeWindow(width, height);
	HMODULE hInstance = GetModuleHandle(NULL);
	handle = CreateWindowEx(
		0,
		_T("static"),
		_T(""),
		WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_POPUP,
		0, // x
		0, // y
		width,
		height,
		NULL,
		NULL, // menu
		hInstance, // hInstance
		0 // param
	);
	if (handle == NULL) {
		//exception
	}
	ShowWindow(handle, SW_HIDE);
}

HsnBufferCreator::~HsnBufferCreator() {
	this->destroy();
	if (buffer != nullptr) {
		delete[] buffer;
		buffer = nullptr;
	}
}

bool HsnBufferCreator::initiate() {

	if (initiated)
		return true;//already initiated
	HDC hdcScreen = GetDC(NULL); // 전체 화면의 디바이스 컨텍스트를 얻음
	device_context = CreateCompatibleDC(hdcScreen);
	//device_context = GetDC((HWND)handle);
	if (!device_context)
	{
		//exception
		return false;
	}

	static PIXELFORMATDESCRIPTOR pfd = {
		sizeof(PIXELFORMATDESCRIPTOR), //nSize
		1, // nVersion
		PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER, // dwFlags
		PFD_TYPE_RGBA, //iPixelType
		32 * 4, // cColorBits (Important)
		0, // cRedBits
		0, // cRedShifts
		0, // cGreenBits
		0, // cGreenShift
		0, // cBlueBits
		0, // cBlueShift
		0, // cAlphaBits
		0, // cAlphaShift
		0, // cAccumBits
		0, // cAccumRedBits
		0, // cAccumGreenBits
		0, // cAccumBlueBits
		0, // cAccumAlphaBits
		0, // cDepthBits (Important)
		0, // cStencilBits (Important)
		0, // cAuxBuffers
		PFD_MAIN_PLANE, // iLayerType
		0, // bReserved
		0, // dwLayerMask
		0, // dwVisibleMask
		0 // dwDamageMask
	};

	GLint pixel_format = 0;
	pixel_format = ChoosePixelFormat(device_context, &pfd);
	if (pixel_format == 0)
	{
		//exception
		return false;
	}
	if (!SetPixelFormat(device_context, pixel_format, &pfd))
	{
		//exception
		return false;
	}
	rendering_context = wglCreateContext(device_context);
	if (rendering_context == nullptr)
	{
		//exception
		return false;
	}
	if (!wglMakeCurrent(device_context, rendering_context))
	{
		//exception
		return false;
	}
	if (!IpInitialize())
	{
		return false;
	}

	{
		int byte_per_sample = 2;
		int max_no_sample = 512;
		int max_scanline = 960;
		if (envdata_buffer != nullptr) {
			delete[] envdata_buffer;
		}
		envdata_buffer_size = max_scanline * max_no_sample * byte_per_sample;
		envdata_buffer = new uint8_t[envdata_buffer_size];
	}
	initiated = true;


	return true;
}

bool HsnBufferCreator::destroy() {

	if (!initiated) {
		return true; //already destroyed
	}
	IpRelease();
	if (handle != nullptr)
	{
		if (!rendering_context)
		{
			wglMakeCurrent(nullptr, nullptr);
			wglDeleteContext(rendering_context);
		}
		if (!device_context)
		{
			ReleaseDC(handle, device_context);
		}
		DestroyWindow(handle);
	}
	if (envdata_buffer != nullptr) {
		delete[] envdata_buffer;
		envdata_buffer = nullptr;
		envdata_buffer_size = 0;
	}
	initiated = false;
	return true;
}

bool SaveBmp(const char* filename, byte* pImage, int width, int height)
{
	// DIB의 형식을 정의한다.
	//BITMAPINFO dib_define;
	//dib_define.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	//dib_define.bmiHeader.biWidth = width;
	//dib_define.bmiHeader.biHeight = height;
	//dib_define.bmiHeader.biPlanes = 1;
	//dib_define.bmiHeader.biBitCount = 24;
	//dib_define.bmiHeader.biCompression = BI_RGB;
	//dib_define.bmiHeader.biSizeImage = (((width * 24 + 31) & ~31) >> 3) * height;
	//dib_define.bmiHeader.biXPelsPerMeter = 0;
	//dib_define.bmiHeader.biYPelsPerMeter = 0;
	//dib_define.bmiHeader.biClrImportant = 0;
	//dib_define.bmiHeader.biClrUsed = 0;
	BITMAPINFO dib_define;
	dib_define.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	dib_define.bmiHeader.biWidth = width;
	dib_define.bmiHeader.biHeight = -height;
	dib_define.bmiHeader.biPlanes = 1;
	dib_define.bmiHeader.biBitCount = 32;
	dib_define.bmiHeader.biCompression = BI_RGB;
	dib_define.bmiHeader.biSizeImage = width * height * 4;
	dib_define.bmiHeader.biXPelsPerMeter = 0;
	dib_define.bmiHeader.biYPelsPerMeter = 0;
	dib_define.bmiHeader.biClrImportant = 0;
	dib_define.bmiHeader.biClrUsed = 0;

	// DIB 파일의 헤더 내용을 구성한다.
	BITMAPFILEHEADER dib_format_layout;
	ZeroMemory(&dib_format_layout, sizeof(BITMAPFILEHEADER));
	dib_format_layout.bfType = *(WORD*)"BM";
	dib_format_layout.bfSize = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);// +dib_define.bmiHeader.biSizeImage;
	dib_format_layout.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);

	// 파일 생성.
	FILE* fp = nullptr;
	fopen_s(&fp, filename, "wb");
	if (nullptr == fp)
	{
		//LOG_OUT_A("fopen() error");
		return false;
	}

	// 생성 후 헤더 및 데이터 쓰기.
	fwrite(&dib_format_layout, 1, sizeof(BITMAPFILEHEADER), fp);
	fwrite(&dib_define, 1, sizeof(BITMAPINFOHEADER), fp);
	fwrite(pImage, 1, dib_define.bmiHeader.biSizeImage, fp);
	fclose(fp);

	return true;
}

bool HsnBufferCreator::renderBitmap() {

	g_mutex.lock();
	bool good = false;
	do {
		if (!IpResize(width, height)) {
			//exception
			break;
		}

		size_t recv_data_length;
		//std::string output_metadata;

		//size_t 
		bool envelope_data_capture_needed = false;
		if (envelope_data_capture_needed) {
			recv_data_length = IpRenderWithCapture(buffer, buffer_size, envdata_buffer, envdata_buffer_size, output_metadata);
		}
		else {
			//it doesn't spend extra cpu resource.
			recv_data_length = IpRenderWithCapture(buffer, buffer_size, nullptr, 0u, output_metadata);
		}
		if (recv_data_length == 0u)
		{
			break;
		}
		//metadata parse
		try {
			//std::string path = "env/env_512img_1.bmp";
			//SaveBmp(path.c_str(), buffer, width, height);

			auto json_data = nlohmann::json::parse(output_metadata);
			auto unit_mm_per_pixel = json_data["unit_mm_per_pixel"].get<float>();
			{
				static float prev_unit_mm_per_pixel = 0.0f;
				if (prev_unit_mm_per_pixel != unit_mm_per_pixel)
				{
					prev_unit_mm_per_pixel = unit_mm_per_pixel;
					if (unit_mm_per_pixel_callback != nullptr)
						unit_mm_per_pixel_callback(unit_mm_per_pixel);
				}
			}

			auto probe_frame_index = json_data["probe_frame_index"].get<int32_t>();

			//{
			//	static int count = 0;
				//	static CString out_str = _T("");
				//	CString value = _T("");
				//	value.Format(_T("%d "), probe_frame_index);
				//	//OutputDebugString(value);
				//	out_str += value;
				//	count++;
				//	if (count == 30) {
					//		OutputDebugString(out_str+_T("\n"));
					//		count = 0;
					//		out_str= _T("");
				//	}
			//}
			auto image_offset_x_in_pixel = json_data["image_offset_x_in_pixel"].get<float>();
			auto image_offset_y_in_pixel = json_data["image_offset_y_in_pixel"].get<float>();
			auto depth_in_cm = json_data["depth_in_cm"].get<double>();
			auto gain = json_data["gain"].get<int32_t>();
			auto dr_min = json_data["dr_min"].get<float>();
			auto dr_max = json_data["dr_max"].get<float>();
			auto tx_power = json_data["tx_power"].get<double>();
			auto density = json_data["density"].get<int32_t>();
			auto frameaverage_enable = json_data["frameaverage_enable"].get<bool>();
			auto frameaverage_weight = json_data["frameaverage_weight"].get<float>();

			if (envelope_data_capture_needed) {
				//envelope data information

				auto env_data_sample_count = json_data["rawdata_sample_no"].get<int32_t>();
				auto env_data_scanline_count = json_data["rawdata_scanline_no"].get<int32_t>();
				auto env_data_byte_per_sample = json_data["rawdata_byte_per_sample"].get<int32_t>();
				size_t total_env_byte = env_data_sample_count * env_data_scanline_count * env_data_byte_per_sample;
				//use envdata_buffer with above informations
				//envdata_buffer

				//data order.
				//each sample is 2byte (uint16_t)
				//sample(s) -> scanline(sc)
				//sc0s0 -> sc0s1 -> sc0s2 -> ...sc0s511 -> sc1s0 -> ....

				//{
				//	////test output
				//	std::ofstream out;
				//	static int out_index = 0;
				//	if (total_env_byte > 0) {
				//		std::string path = "env/env_data_" + std::to_string(out_index) + ".bin";
				//		out.open(path.c_str(), std::ios::binary);
				//		if (out.is_open()) {
				//			out.write((char*)&env_data_sample_count, sizeof(int32_t));
				//			out.write((char*)&env_data_scanline_count, sizeof(int32_t));
				//			out.write((char*)&env_data_byte_per_sample, sizeof(int32_t));
				//			out.write((char*)envdata_buffer, total_env_byte);
				//			out.close();
				//			out_index++;
				//		}
				//	}
				//}

			}

		}
		catch (std::exception& e) {
			OutputDebugStringA(e.what());
		}
		good = true;
	} while (false);
	g_mutex.unlock();
	if (!good)
		return false;
	if (!SwapBuffers(device_context))
	{
		//exception
		return false;
	}

	return true;
}

bool HsnBufferCreator::resizeWindow(int width, int height)
{

	if (this->width != width || this->height != height) {
		g_mutex.lock();
		this->width = width;
		this->height = height;
		this->buffer_size = width * height * 4;
		if (buffer == nullptr) {
			buffer = new uint8_t[buffer_size];
		}
		else {
			delete[] buffer;
			buffer = new uint8_t[buffer_size];
		}
		g_mutex.unlock();
	}

	return true;
}

uint8_t* HsnBufferCreator::getBitmapBuffer(size_t& length)
{
	if (!renderBitmap())
		return nullptr;
	length = this->buffer_size;
	return buffer;
}

//using namespace Hsnlibrary;

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

void HsnBufferCreatorInitialize()
{
	buffer_creator = new HsnBufferCreator(512, 512);
	buffer_creator->initiate();
	//buffer_creator->registerUnitMmPixelCallback(std::bind(&CSampleProjectDlg::UnitMmPerPixelCallback, this, std::placeholders::_1))

}

void HsnBufferCreatorGetBitmapBuffer()
{
	size_t bitmap_size;
	uint8_t* bitmap_buffer = nullptr;
	bitmap_buffer = buffer_creator->getBitmapBuffer(bitmap_size);
}

bool Initialize() {
	return Hsnlibrary::initialize();
}

bool Destroy()
{
	return Hsnlibrary::destroy();
}

bool ListApplication(std::vector<std::tuple<int32_t, std::string>>& application_list)
{
	return Hsnlibrary::listApplication(application_list);
}

bool ListPreset(int32_t application_id, std::vector<std::tuple<int32_t, std::string>>& preset_list)
{
	return Hsnlibrary::listPreset(application_id, preset_list);
}

bool ListSubSetting(const int32_t setting_id, std::vector<std::tuple<int32_t, std::string>>& subsetting_list)
{
	return Hsnlibrary::listSubSetting(setting_id, subsetting_list);
}

bool SaveAsUserSetting(const std::string setting_name, int32_t& setting_id)
{
	return Hsnlibrary::saveAsUserSetting(setting_name, setting_id);
}

bool GetApplicationName(const int32_t application_id, std::string& application_name)
{
	return Hsnlibrary::getApplicationName(application_id, application_name);
}

bool GetSettingName(const int32_t setting_id, std::string& setting_name)
{
	return Hsnlibrary::getSettingName(setting_id, setting_name);
}

bool GetSubSettingName(const int32_t subsetting_id, std::string& subsetting_name)
{
	return Hsnlibrary::getSubSettingName(subsetting_id, subsetting_name);
}

bool GetDefaultApplication(int32_t& application_id)
{
	return Hsnlibrary::getDefaultApplication(application_id);
}

bool SetDefaultApplication(int32_t application_id)
{
	return Hsnlibrary::setDefaultApplication(application_id);
}

bool GetDefaultSetting(const int32_t application_id, int32_t& setting_id)
{
	return Hsnlibrary::getDefaultSetting(application_id, setting_id);
}

bool SetDefaultSetting(const int32_t application_id, int32_t setting_id)
{
	return Hsnlibrary::setDefaultSetting(application_id, setting_id);
}

bool GetDefaultSubSetting(const int32_t setting_id, int32_t& subsetting_id)
{
	return Hsnlibrary::getDefaultSubSetting(setting_id, subsetting_id);
}

bool GetCurrentApplication(int32_t& application_id)
{
	return Hsnlibrary::getCurrentApplication(application_id);
}

bool GetCurrentSetting(int32_t& setting_id)
{
	return Hsnlibrary::getCurrentSetting(setting_id);
}

bool GetCurrentSubSetting(int32_t& subsetting_id)
{
	return Hsnlibrary::getCurrentSubSetting(subsetting_id);
}

bool LoadApplication(int32_t app_id)
{
	return Hsnlibrary::load_Application(app_id);
}

bool LoadSetting(int32_t setting_id)
{
	return Hsnlibrary::load_Setting(setting_id);
}

bool LoadSubSetting(int32_t subsetting_id)
{
	return Hsnlibrary::load_SubSetting(subsetting_id);
}

bool StartProbeDetection()
{
	return Hsnlibrary::startProbeDetection();
}

bool StopProbeDetection()
{
	return Hsnlibrary::stopProbeDetection();
}

bool IsProbeExist()
{
	return Hsnlibrary::isProbeExist();
}

void ActivateProbe()
{
	Hsnlibrary::activateProbe();
}

void DeactivateProbe()
{
	Hsnlibrary::disactivateProbe();
}

bool IpInitialize()
{
	return Hsnlibrary::ipInitialize();
}

bool IpRelease()
{
	return Hsnlibrary::ipRelease();
}

bool IsIpInitialized()
{
	return Hsnlibrary::isIpInitialized();
}

size_t IpRenderWithCapture(void* output_final_image_buf, size_t final_image_buf_length, void* output_raw_data_buf, size_t raw_data_buf_length, std::string& output_metadata)
{
	return Hsnlibrary::ipRenderWithCapture(output_final_image_buf,final_image_buf_length,output_raw_data_buf,raw_data_buf_length,output_metadata);
}

bool IpResize(int width, int height)
{
	return Hsnlibrary::ipResize(width, height);
}

const char* IpGetLastError(int& length)
{
	return Hsnlibrary::ipGetLastError(length);
}

void RegisterCallbackDeviceAttached(DeviceAttached callback)
{
	Hsnlibrary::registerCallback_DeviceAttached(callback);
}

void RegisterCallbackDeviceRemoved(DeviceRemoved callback)
{
	Hsnlibrary::registerCallback_DeviceRemoved(callback);
}

bool RegisterCallbackMotorSpeed(MotorSpeed callback)
{
	return Hsnlibrary::registerCallback_ENDMotorSpeed(callback);
}

bool RegisterCallbackLoading(Loading callback)
{
	return Hsnlibrary::registerCallback_Loading(callback);
}

bool RegisterCallbackProbeState(ProbeState callback)
{
	return Hsnlibrary::registerCallback_ProbeState(callback);
}

bool RegisterCallbackError(Error callback)
{
	return Hsnlibrary::registerCallback_Error(callback);
}

bool GetCurrentDataVersion(std::string& data_version)
{
	return Hsnlibrary::getCurrent_Data_version(data_version);
}

bool GetCurrentPresetVersion(std::string& preset_version)
{
	return Hsnlibrary::getCurrent_Preset_version(preset_version);
}

bool GetCurrentIPVersion(std::string& ip_version)
{
	return Hsnlibrary::getCurrent_IP_version(ip_version);
}

std::string GetProbeTypeName(size_t index)
{
	return Hsnlibrary::get_probe_type_name(index);
}

int16_t GetProbeType(size_t index)
{
	return Hsnlibrary::get_probe_type(index);
}

int GetTransducerID()
{
	return Hsnlibrary::getTransducerID();
}

std::string GetTransducerCode()
{
	return Hsnlibrary::getTransducerCode();
}

bool GetCurrentBoardName(std::string& name)
{
	return Hsnlibrary::getCurrentBoardName(name);
}

bool Freeze()
{
	return Hsnlibrary::freeze();
}

bool Unfreeze()
{
	return Hsnlibrary::unfreeze();
}

bool IsFrozen()
{
	return Hsnlibrary::isFrozen();
}

void SetDeviceRtcViewdepth(double value, size_t index)
{
	Hsnlibrary::set_device_rtc_viewdepth(value, index);
}

double GetDeviceRtcViewdepth(size_t index)
{
	return Hsnlibrary::get_device_rtc_viewdepth(index);
}

void SetDeviceTxPower(double value, size_t index)
{
	Hsnlibrary::set_device_tx_power(value, index);
}

double GetDeviceTxPower(size_t index)
{
	return Hsnlibrary::get_device_tx_power(index);
}

void SetDeviceTxCycle(int32_t value, size_t index)
{
	Hsnlibrary::set_device_tx_cycle(value, index);
}

int32_t GetDeviceTxCycle(size_t index)
{
	return Hsnlibrary::get_device_tx_cycle(index);
}

void SetDeviceDensityFactor(int32_t value, size_t index)
{
	Hsnlibrary::set_device_density_factor(value, index);
}

int32_t GetDeviceDensityFactor(size_t index)
{
	return Hsnlibrary::get_device_density_factor(index);
}

void SetDeviceViewdepthAndDensity(double viewdepth, int32_t density)
{
	Hsnlibrary::set_device_viewdepth_and_density(viewdepth, density);
}

int32_t GetIpGain(size_t index)
{
	return Hsnlibrary::getIpGain(index);
}

void SetIpGain(int32_t value, size_t index)
{
	Hsnlibrary::setIpGain(value, index);
}

bool GetIpFrameAverageEnable(size_t index)
{
	return Hsnlibrary::getIpFrameAverageEnable(index);
}

void SetIpFrameAverageEnable(bool value, size_t index)
{
	Hsnlibrary::setIpFrameAverageEnable(value, index);
}

float GetIpFrameAverageWeight(size_t index)
{
	return Hsnlibrary::getIpFrameAverageWeight(index);
}

void SetIpFrameAverageWeight(float value, size_t index)
{
	Hsnlibrary::setIpFrameAverageWeight(value, index);
}

float GetIpDynamicRangeMax(size_t index)
{
	return Hsnlibrary::getIpDynamicRangeMax(index);
}

void SetIpDynamicRangeMax(float value, size_t index)
{
	Hsnlibrary::setIpDynamicRangeMax(value, index);
}

float GetIpDynamicRangeMin(size_t index)
{
	return Hsnlibrary::getIpDynamicRangeMin(index);
}

void SetIpDynamicRangeMin(float value, size_t index)
{
	Hsnlibrary::setIpDynamicRangeMin(value, index);
}

float GetIpUserTgc(size_t index)
{
	return Hsnlibrary::getIpUserTgc(index);
}

void SetIpUserTgc(float value, size_t index)
{
	Hsnlibrary::setIpUserTgc(value, index);
}

void GetIpGrayMapTable(std::vector<float>& table)
{
	Hsnlibrary::getIpGrayMapTable(table);
}

bool ListGrayMap(std::vector<std::tuple<int32_t, std::string>>& list)
{
	return Hsnlibrary::listGrayMap(list);
}

int32_t GetCurrentGrayMapID()
{
	return Hsnlibrary::getCurrentGrayMapID();
}

bool LoadGrayMap(int32_t id)
{
	return Hsnlibrary::loadGrayMap(id);
}

bool ListSRI(std::vector<std::tuple<int32_t, std::string>>& list)
{
	return Hsnlibrary::listSRI(list);
}

int32_t GetCurrentSRIID()
{
	return Hsnlibrary::getCurrentSRIID();
}

bool LoadSRI(int32_t id)
{
	return Hsnlibrary::loadSRI(id);
}

bool GetSRIEnable()
{
	return Hsnlibrary::getSRIEnable();
}

void SetSRIEnable(bool value)
{
	Hsnlibrary::setSRIEnable(value);
}

bool ListEdgeEnhance(std::vector<std::tuple<int32_t, std::string>>& list)
{
	return Hsnlibrary::listEdgeEnhance(list);
}

int32_t GetCurrentEdgeEnhanceID()
{
	return Hsnlibrary::getCurrentEdgeEnhanceID();
}

bool LoadEdgeEnhance(int32_t id)
{
	return Hsnlibrary::loadEdgeEnhance(id);
}

bool GetEdgeEnhanceEnable()
{
	return Hsnlibrary::getEdgeEnhanceEnable();
}

void SetEdgeEnhanceEnable(bool value)
{
	Hsnlibrary::setEdgeEnhanceEnable(value);
}

bool GetIpCapsuleIsInnerVisible()
{
	return Hsnlibrary::getIpCapsuleIsInnerVisible();
}

void SetIpCapsuleIsInnerVisible(bool value)
{
	Hsnlibrary::setIpCapsuleIsInnerVisible(value);
}