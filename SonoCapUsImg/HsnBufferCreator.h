#pragma once

#include "pch.h"
#include <windows.h>
#include <stdint.h>
#include <exception>
#include <functional>
#include "SonoCapUsImg.h"
#include "glad/glad.h"
#include "nlohmann/json.hpp"

typedef unsigned char byte;

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