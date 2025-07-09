#pragma once

#include <windows.h>
#include <fstream>

namespace HsnLibraryCS
{
	using namespace System;
	using namespace System::Threading;
	using namespace System::Runtime::InteropServices;
	using namespace System::Diagnostics;

	public ref class MetadataInfo{
	public:
		float unitMmPerPixel;
		/*int probeFrameIndex;
		float imageOffsetXInPixel;
		float imageOffsetYInPixel;
		double depthInCm;
		int gain;
		float drMin;
		float drMax;
		double txPower;
		int density;
		bool frameaverageEnable;
		float frameaverageWeight;*/
	};
	public ref class HsnUltrasoundOffScreenView {
	public:
		explicit HsnUltrasoundOffScreenView(int initial_width, int initial_height);
		virtual ~HsnUltrasoundOffScreenView() {
			Release();
		}

		delegate void ReceiveEnvBuffer(array<System::Byte>^ buffer, int width, int height, int length, MetadataInfo^ metadata);
		delegate void ReceiveBuffer(array<System::Byte>^ buffer, int width, int height, int length, MetadataInfo^ metadata);

		System::Void Start(ReceiveBuffer^ receiveBuffer, ReceiveEnvBuffer^ receiveEnvBuffer);
		System::Void End();
		System::Void Resize(int new_width, int new_height);
		System::Boolean setTargetIPFrameRate(double fps);
		System::Double getTargetIPFrameRate();

		const double MAX_FPS = 60.0;

		void SetRotationAngle(double angleDeg);
		void SetVerticalFlip(bool enable);
		void SetScanline(int envdata_height);
	private:
		ReceiveEnvBuffer^ _receiveEnvBuffer;
		array<System::Byte>^ _envdata_buffer;

		int _envdata_width = 512;
		int _envdata_height = 480;
		int _envdata_buffer_size = _envdata_width * _envdata_height * 2;

		ReceiveBuffer^ _receiveBuffer;
		array<System::Byte>^ _buffer;

		int _width;
		int _height;
		int _length;
		double _fps;
		double _time_duration_us;

		HWND _handle;
		HDC _device_context;
		HGLRC _rendering_context;

		Object^ _buffer_mtx = gcnew Object;
		Object^ _mutex = gcnew Object;
		bool _is_running;
		Thread^ _render_routine;

		void setDuration(double fps);
		bool Initialize();
		void Release();
		void Render();
		void RenderRoutine();

		double _rotation_angle = 0.0;
		bool _flip_vertical = false;
	};
}