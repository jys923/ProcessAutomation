# HsnUltrasoundOffScreenView 구조 분석 노트 (확장판)

## ✅ 클래스 개요

`HsnUltrasoundOffScreenView`는 외부 OpenGL 기반 영상 처리 라이브러리(`Hsnlibrary`)를 통해 캡처된 영상을 받아  
WPF 애플리케이션에 `byte[]`로 전달하는 **C++/CLI 래퍼 클래스**이다.

---

## 🔧 구성 요소 역할 요약

| 구성 요소 | 역할 |
|-----------|------|
| `_buffer` | RGBA 프레임 데이터를 담는 고정 크기 배열 |
| `_mutex`, `_buffer_mtx` | 쓰레드 동기화 및 상태 보호 |
| `_render_routine` | 렌더링 루프를 수행하는 백그라운드 스레드 |
| `_handle`, `_device_context`, `_rendering_context` | OpenGL 컨텍스트를 위한 창 및 디바이스 컨텍스트 |
| `_receiveBuffer` | C#에 영상 프레임을 넘겨주는 콜백 함수 |
| `_fps`, `_time_duration_us` | 프레임 타이밍 제어 변수 |

---

## 🔄 전체 흐름 요약

1. `Start()` 호출 시 백그라운드 스레드 생성
2. 내부 루프에서 `Render()`가 FPS 간격으로 호출됨
3. `ipRenderWithCapture()`로 프레임 획득 후 `_buffer`에 저장
4. `_receiveBuffer(_buffer, w, h, len, meta)`를 통해 C# 측에 전달

---

## 🧪 ipRenderWithCapture 바이트 버퍼 흐름

### 함수 호출
```cpp
final_image_length = Hsnlibrary::ipRenderWithCapture(buffer_ptr, _length, 0, 0, output_metadata);
```

### 데이터 흐름

1. `buffer_ptr`는 `_buffer`를 `pin_ptr`로 고정한 native 포인터 (`char*`)
2. `ipRenderWithCapture()`는 내부 OpenGL 렌더링 결과를 `buffer_ptr`에 복사
   - 포맷은 보통 `BGRA` 또는 `RGBA`, 4채널 8bit
   - 크기는 `_width * _height * 4`
3. C++/CLI에서 이 버퍼를 C#의 `BitmapSource.Create()`에 전달하여 화면에 출력

### 특징
- 성능 좋음 (바로 접근 가능)
- 구조 단순함
- **문제점:** 포맷 고정, 해상도 고정, 메타데이터 분리 파싱 필요

---

## ⏱ Thread + sleep_until 구조

### 코드 핵심
```cpp
auto target_point = std::chrono::steady_clock::now();
while (running) {
    target_point += std::chrono::microseconds((int)_time_duration_us);
    Render();
    std::this_thread::sleep_until(target_point);
}
```

### 동작 설명
- 일정 시간마다 정확히 `Render()`를 호출하기 위한 구조
- `sleep_until()`은 초과된 경우를 보정 가능
- 프레임 드롭을 방지하고 안정적인 간격 유지

### 특징
- `DispatcherTimer`나 `Timer`보다 정밀도 높음
- 실시간 영상 처리에서 흔히 사용됨
- 단점: 조정된 지연(slip)이 누적될 수 있음

---

## 📤 ReceiveBuffer 델리게이트 구조

### 선언
```cpp
public delegate void ReceiveBuffer(array<System::Byte>^, int width, int height, int length, MetadataInfo^ meta);
```

### 사용 이유

| 이유 | 설명 |
|------|------|
| **C#과 연동 용이** | .NET 측에서 델리게이트(Action<T>)에 연결하기 쉬움 |
| **비동기 호출 안전** | WPF `Dispatcher.Invoke()` 안에서 안전하게 실행 가능 |
| **핵심 데이터를 하나로 묶음** | 영상 버퍼 + 메타데이터를 한 번에 전달 |

### 장점
- C# ViewModel 또는 Service가 `renderToTarget(BitmapSource)`처럼 자연스럽게 사용 가능
- 멀티 스레드 환경에서도 안정적으로 콜백 전달

### 단점
- 델리게이트를 반드시 `Start()` 전에 연결해야 함
- 구조가 약간 무거움 (단순 콜백보다)

---

## ❌ 구조적 문제점

| 문제 | 설명 |
|------|------|
| 책임 과다 | 렌더링, 버퍼 관리, 쓰레드, 콜백 전달을 하나의 클래스가 모두 처리 |
| 버퍼 강결합 | `_buffer`를 내부에서 직접 관리 → 외부 확장성 제로 |
| 유연성 부족 | 픽셀 포맷, stride 변경 불가 → 확장 어려움 |
| 쓰레드 직접 제어 | 향후 병렬 처리나 GPU 활용 시 확장성 낮음 |

---

## 🧠 개선 아이디어 (역할 분리 방향)

| 분리된 클래스 | 책임 |
|---------------|------|
| `HsnRenderer` | 외부 라이브러리 렌더링만 수행 |
| `BufferManager` | 버퍼 포맷, 리사이징, 핀 관리 |
| `ImageProcessor` | 회전, 필터링 등 후처리 모듈 |
| `ImageDispatcher` | ReceiveBuffer 호출 및 관리 |

---

## ✅ 결론

- **현재 구조는 "데모용 샘플"로는 우수**
- **실제 제품/연구용으로는 분리 및 재구성이 필요**
- 우선 회전 기능은 `Render()` 내 OpenCV 삽입이 가장 효과적
- 장기적으로는 `rotate_image.dll` 같은 별도 처리 모듈화도 고려 가능
- `ReceiveBuffer`는 .NET 연동을 위해 필요한 선택이지만, **핵심 처리는 따로 분리하는 것이 이상적**

---
