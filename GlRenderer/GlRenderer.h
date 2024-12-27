#pragma once

using namespace System;

namespace GlRenderer {
	public ref class RendererInterface
	{
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
		public:
			static void DrawSquare();
			static void DrawCube();

		private:
			static float angle = 0.0f;
	};
}
