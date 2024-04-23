#pragma once

#include <cstdint>
#include <vcclr.h> // for PtrToStringChars
#include <stdio.h> // for wprintf



using namespace System;
using namespace System::Collections::Generic;

typedef unsigned char byte;

namespace VILib {
	public ref class VILibWrapper
	{
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	public:
		VILibWrapper();
		~VILibWrapper();

		int Load(int mode);
		int DoInspection(System::String^ sModel, array<byte>^ src, int img_w, int img_h, int img_ch, array<byte>^ dest, VIRes^% res, int opt);
		int Unload(int mode);
		int MakeMaskImg(System::String^ sModel, System::String^ full_path, int nw, int nh, int cen_x, int cen_y, int radius);

	private:
		int DoInspection(System::String^ sModel, array<byte>^ src, array<byte>^ dest, VIRes^% res, int opt);
		
		
	private:
	
	};
}
