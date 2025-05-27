#include "HsnUSOffScreenView.hpp"
#include <Hsnlibrary.hpp>
#include <GL/gl.h>
#include <opencv2/opencv.hpp>

#include <chrono>
#include <exception>
//#include <atomic>
#include <thread>
//#include <functional>
#include <msclr/lock.h>
//#include <atlcoll.h>
#include <json.hpp>


using namespace System;
using namespace System::Collections::Generic;
//using namespace System::Diagnostics;
//using namespace System::Runtime::InteropServices;
//using namespace System::Threading;
static String^ getLastErrorMessage()
{
	auto error_value = GetLastError();
	TCHAR* message = nullptr;

	FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER,
		NULL,
		error_value,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		message,
		0,
		NULL);

	String^ ret = gcnew String(message);
	LocalFree(message);
	return ret;
}

HsnLibraryCS::HsnUltrasoundOffScreenView::HsnUltrasoundOffScreenView(int initial_width, int initial_height)
{
	_width = initial_width;
	_height = initial_height;
	_length = _width * _height * 4;
	_buffer = gcnew array<System::Byte>(_length);
	setTargetIPFrameRate(MAX_FPS);
	if (_buffer == nullptr)
	{
		throw gcnew Exception(getLastErrorMessage());
	}

	HMODULE hInstance = GetModuleHandle(NULL);
	_handle = CreateWindowEx(
		0,
		L"static",
		L"",
		WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_POPUP,
		0, // x
		0, // y
		_width,
		_height,
		NULL,
		NULL, // menu
		hInstance, // hInstance
		0 // param
	);

	if (_handle == NULL)
	{
		throw gcnew Exception(getLastErrorMessage());
	}

	ShowWindow(_handle, SW_HIDE);

}

System::Void HsnLibraryCS::HsnUltrasoundOffScreenView::Start(ReceiveBuffer^ receiveBuffer)
{
	_receiveBuffer = receiveBuffer;
	{
		msclr::lock lock{ _mutex };
		_is_running = true;
	}
	_render_routine = gcnew Thread(gcnew ThreadStart(this, &HsnUltrasoundOffScreenView::RenderRoutine));
	_render_routine->Name = "OffScreenView_RenderRoutine";
	_render_routine->Start();
}

System::Void HsnLibraryCS::HsnUltrasoundOffScreenView::End()
{
	{
		msclr::lock lock{ _mutex };
		_is_running = false;
	}
	_render_routine->Join();
	_receiveBuffer = nullptr;
}

System::Void HsnLibraryCS::HsnUltrasoundOffScreenView::Resize(int new_width, int new_height)
{
	if (new_width != _width || new_height != _height)
	{
		msclr::lock lock{ _buffer_mtx };
		_width = new_width;
		_height = new_height;
		_length = _width * _height * 4;
		_buffer = gcnew array<System::Byte>(_length);
	}
}

System::Boolean HsnLibraryCS::HsnUltrasoundOffScreenView::setTargetIPFrameRate(double fps)
{
	if (!(fps >= 1.0 && fps <= MAX_FPS))
	{
		return false;
	}
	this->_fps = fps;
	setDuration(_fps);
	return true;
}

System::Double HsnLibraryCS::HsnUltrasoundOffScreenView::getTargetIPFrameRate()
{
	return _fps;
}

void HsnLibraryCS::HsnUltrasoundOffScreenView::SetRotationAngle(double angleDeg)
{
	msclr::lock lock{ _mutex };
	_rotation_angle = angleDeg;
}

void HsnLibraryCS::HsnUltrasoundOffScreenView::SetVerticalFlip(bool enable)
{
	msclr::lock lock{ _mutex };
	_flip_vertical = enable;
}

void HsnLibraryCS::HsnUltrasoundOffScreenView::setDuration(double fps)
{
	_time_duration_us = (double)1e6 / fps;
}

bool HsnLibraryCS::HsnUltrasoundOffScreenView::Initialize()
{
	_device_context = GetDC((HWND)_handle);
	if (!_device_context)
	{
		throw gcnew Exception(getLastErrorMessage());
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
	pixel_format = ChoosePixelFormat(_device_context, &pfd);
	if (pixel_format == 0)
	{
		throw gcnew Exception(getLastErrorMessage());
		return false;
	}
	if (!SetPixelFormat(_device_context, pixel_format, &pfd))
	{
		throw gcnew Exception(getLastErrorMessage());
		return false;
	}
	_rendering_context = wglCreateContext(_device_context);
	if (_rendering_context == nullptr)
	{
		throw gcnew Exception(getLastErrorMessage());
		return false;
	}
	if (!wglMakeCurrent(_device_context, _rendering_context))
	{
		throw gcnew Exception(getLastErrorMessage());
		return false;
	}
	if (!Hsnlibrary::ipInitialize())
	{
		return false;
	}

	return true;
}

void HsnLibraryCS::HsnUltrasoundOffScreenView::Release()
{
	Hsnlibrary::ipRelease();
	if (_handle!=NULL)
	{
		if (!_rendering_context)
		{
			wglMakeCurrent(nullptr, nullptr);
			wglDeleteContext(_rendering_context);
		}
		if (!_device_context)
		{
			ReleaseDC(_handle, _device_context);
		}
		if (_handle != NULL) 
		{ 
			DestroyWindow(_handle); 
			_handle = NULL; 
		}
	}
}

void HsnLibraryCS::HsnUltrasoundOffScreenView::Render()
{
	do{
		msclr::lock lock{ _buffer_mtx };
		pin_ptr<System::Byte> charPointer = &_buffer[0];
		char* buffer_ptr = reinterpret_cast<char*>(charPointer);
		if (!Hsnlibrary::ipResize(_width, _height))
		{
			throw gcnew Exception(getLastErrorMessage());
		}
		
		size_t final_image_length;
		std::string output_metadata;

		final_image_length = Hsnlibrary::ipRenderWithCapture(buffer_ptr, _length, 0, 0, output_metadata);
		if (final_image_length == 0u)
		{
			break;
		}
		// OpenCV 기반 회전 처리
		try {
			cv::Mat src(_height, _width, CV_8UC4, buffer_ptr);
			cv::Mat flipped;
			cv::Mat dst;

			bool do_flip = false;
			double angle = 0.0;
			{
				msclr::lock lock{ _mutex };
				angle = _rotation_angle;
				do_flip = _flip_vertical;
			}

			if (do_flip) {
				cv::flip(src, flipped, 0); // flipCode = 0 → 상하 반전
			}
			else {
				flipped = src; // 복사 아님, 얕은 참조
			}

			if (std::abs(angle) >= 1.0)
			{
				cv::Point2f center(_width / 2.0f, _height / 2.0f);
				cv::Mat rotMat = cv::getRotationMatrix2D(center, -angle, 1.0);
				cv::warpAffine(flipped, dst, rotMat, src.size(), cv::INTER_LINEAR, cv::BORDER_CONSTANT, cv::Scalar(0, 0, 0, 255));
			}
			else
			{
				dst = flipped;
			}

			std::memcpy(buffer_ptr, dst.data, _width * _height * 4);
		}
		catch (const std::exception& ex) {
			// 회전 실패 시 무시
		}

		//metadata parse
		try {
			auto json_data = nlohmann::json::parse(output_metadata);
			auto unit_mm_per_pixel = json_data["unit_mm_per_pixel"].get<float>();
			auto probe_frame_index = json_data["probe_frame_index"].get<int32_t>();
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
			MetadataInfo^ metadata = gcnew MetadataInfo();
			metadata->unitMmPerPixel = unit_mm_per_pixel;
			_receiveBuffer(_buffer, _width, _height, _length, metadata);
		}
		catch (std::exception e) {

		}
	} while (false);
	if (!SwapBuffers(_device_context))
	{
		throw gcnew Exception(getLastErrorMessage());
	}
}

void HsnLibraryCS::HsnUltrasoundOffScreenView::RenderRoutine()
{
	if (Initialize())
	{
		auto target_point = std::chrono::steady_clock::now();
		while (1)
		{
			target_point = target_point + std::chrono::microseconds((int)_time_duration_us);

			bool local_is_running{ false };
			{
				msclr::lock lock{ _mutex };
				local_is_running = _is_running;
			}
			if (!local_is_running)
				break;
			Render();

			if (std::chrono::steady_clock::now() > target_point) {
				std::this_thread::sleep_for(std::chrono::microseconds(10));
				target_point = std::chrono::steady_clock::now();
			}
			else {
				std::this_thread::sleep_until(target_point);
			}
		}
		Release();
	}
}
