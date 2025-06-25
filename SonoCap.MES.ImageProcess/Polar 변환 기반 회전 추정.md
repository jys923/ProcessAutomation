# 🌀 Polar 변환 기반 회전 추정 - 전체 정리

회전된 이미지를 기준 이미지와 정합하여 **회전 각도를 정확히 추정**하는 알고리즘입니다.  
기본 아이디어는 **Polar 변환 + 세로 방향 순환 시프트 + 정규화 상관계수(NCC)**입니다.

---

## 🔧 전체 코드

```cpp
// 세로 방향 순환 시프트
cv::Mat circShiftY(const cv::Mat& src, int shift) {
    int h = src.rows;
    shift = ((shift % h) + h) % h;
    if (h == 0 || shift < 0 || shift >= h) return src.clone();
    if (shift == 0) return src.clone();
    cv::Mat part1 = src.rowRange(shift, h).clone();
    cv::Mat part2 = src.rowRange(0, shift).clone();
    cv::Mat result;
    cv::vconcat(part1, part2, result);
    return result;
}

// 정규화 상관계수 (Normalized Cross-Correlation)
float computeNormalizedCorrelation(const cv::Mat& a, const cv::Mat& b) {
    cv::Mat a32f, b32f;
    a.convertTo(a32f, CV_32F);
    b.convertTo(b32f, CV_32F);
    a32f -= cv::mean(a32f);
    b32f -= cv::mean(b32f);
    double num = cv::sum(a32f.mul(b32f))[0];
    double denom = std::sqrt(cv::sum(a32f.mul(a32f))[0] * cv::sum(b32f.mul(b32f))[0]);
    return (denom == 0) ? 0 : static_cast<float>(num / denom);
}

// 회전 각도 추정
float estimateRotationByCircularShift(const cv::Mat& reference, const cv::Mat& rotated) {
    int radius = std::min(reference.cols, reference.rows) / 2;
    cv::Point2f center(reference.cols / 2.0f, reference.rows / 2.0f);
    int angleResolution = 1024;

    // Polar 변환: 세로(행) = 각도(θ), 가로(열) = 반지름(r)
    cv::Mat refPolar, rotPolar;
    cv::warpPolar(reference, refPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);
    cv::warpPolar(rotated,  rotPolar,  cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);

    std::vector<float> corrList(angleResolution);
    float bestCorr = -1.0f;
    int bestShift = 0;

    // 세로 방향 shift하면서 NCC 계산
    for (int shift = 0; shift < angleResolution; ++shift) {
        cv::Mat shifted = circShiftY(rotPolar, shift);
        float corr = computeNormalizedCorrelation(refPolar, shifted);
        corrList[shift] = corr;
        if (corr > bestCorr) {
            bestCorr = corr;
            bestShift = shift;
        }
    }

    // 서브픽셀 보정 (Parabolic Interpolation)
    float subpixelShift = static_cast<float>(bestShift);
    if (bestShift > 0 && bestShift < angleResolution - 1) {
        float y1 = corrList[bestShift - 1];
        float y2 = corrList[bestShift];
        float y3 = corrList[bestShift + 1];
        float denom = 2 * (2 * y2 - y1 - y3);
        if (denom != 0.0f) {
            float delta = (y1 - y3) / denom;
            subpixelShift += delta;
        }
    }

    // 회전 각도 계산
    float angle = -360.0f * subpixelShift / angleResolution;
    if (angle < 0) angle += 360.0f;
    return -angle;
}
```

---

## 🔍 원리 설명

### 1. 왜 Polar 변환을 쓰나?
- 일반 이미지에서는 회전된 이미지를 정렬하기 어렵다.
- 하지만 이미지를 **Polar 좌표계(r, θ)**로 바꾸면,  
  회전은 단순히 **세로 방향(Y축) 이동**이 된다.

| 일반 이미지 | Polar 변환 후 |
|-------------|----------------|
| 회전 = 픽셀 이동 복잡 | 회전 = 세로 방향 시프트 (간단!) |

---

### 2. 좌표계 이해

```text
Polar 이미지 (cv::Size(radius, angleResolution)):
 - 세로 (rows): 각도(θ) —> 0~360도를 일정 해상도로 분할 (예: 1024)
 - 가로 (cols): 반지름(r) —> 중심부터 외곽까지 거리
```

- 원본 이미지 (직교좌표) → Polar 변환으로 전개
- 중심을 기준으로 펼쳐서 각도를 세로축으로 나타냄

---

### 3. 순환 시프트는 왜 쓰나?
- 이미지가 회전되면 Polar 이미지에서 **세로 방향으로만 이동**됨
- 이걸 `circShiftY()`로 하나씩 이동시키며 상관계수를 측정
- 가장 높은 상관계수를 가지는 시점이 **정렬된 위치**임 → 회전량 추정 가능

---

### 4. 정규화 상관계수(NCC)란?

\[
NCC = \frac{\sum (A - \bar{A})(B - \bar{B})}{\sqrt{\sum (A - \bar{A})^2 \cdot \sum (B - \bar{B})^2}}
\]

- -1 ~ 1 사이의 값
- +1에 가까울수록 유사도 높음
- 밝기 차이를 보정하므로 실제 영상 밝기가 달라도 비교 가능

---

### 5. 서브픽셀 보정이란?

- Shift가 정수 단위(픽셀 단위)면 정확도에 한계 있음
- 최고점 주위 3점(좌, 중, 우)을 이용해 **2차 보간(Parabolic Interpolation)** 수행
- 최대값의 위치를 소수점 단위로 보정

---

### 6. 각도 보정 처리

```cpp
float angle = -360.0f * subpixelShift / angleResolution;
if (angle < 0) angle += 360.0f;
return -angle;
```

- OpenCV 기준: CCW 방향이 음수 (반시계 회전)
- 대부분의 응용에서는 CW를 양수로 쓰므로 `-angle` 반환
- 0~360° 범위로 보정해서 항상 양수 회전값을 사용

---

## ✅ 실험 결과 예시

```text
Estimated angle: -315.0548
→ 실제 회전 각도: 360 - 315.05 = 44.95도
```

- 기준 이미지가 회전된 이미지보다 약 **45도 시계방향** 회전한 경우
- 정확히 근접하게 추정됨

---

## 📌 한계 및 개선 아이디어

| 항목 | 내용 |
|------|------|
| 해상도 의존 | `angleResolution`이 너무 작으면 정확도 저하 |
| 속도 개선 | `cv::phaseCorrelate()` 등 주파수 도메인 활용 가능 |
| 노이즈 민감성 | Pre-blur 또는 에지 강화 후 입력하면 안정성 증가 |

---

## 🧪 테스트 이미지 예시

- `referenceImage.bmp` (기준 이미지)
- `rotatedImage.bmp` (45도 회전)
- 결과: 약 44.95도 추정 → 정확도 우수

---

## 🧠 요약

- Polar 변환은 회전 추정을 쉽게 만든다
- Y축 시프트 + NCC로 간단하고 강력한 정합 가능
- 정수 shift + subpixel 보정으로 정밀도 확보
- 밝기 변화에 강한 NCC 사용

## 정규화 상관계수란?

두 신호 a, b 간의 유사도를 [-1, 1] 사이로 표현하는 방식.
- 1: 완벽히 동일한 방향의 변화
- -1: 완벽히 반대 방향
- 0: 상관 없음

중앙값을 제거하고 분산 기반으로 정규화하여 계산된다.

---

## 느린 이유와 개선 방향

- 현재는 1024단계 전체를 순차 탐색 → 반복 횟수 많아 느림
- 개선 아이디어:
  - FFT 기반 Phase Correlation 적용 (OpenCV `phaseCorrelate`)
  - Polar 변환 후 회전 성분만 따로 FFT
  - 파라볼릭 보간 대신 FFT subpixel peak 보간

## 📌 향후 개선 아이디어

- FFT 기반 Log-Polar 정합 (`phaseCorrelate`)로 속도 향상
- `matchTemplate` 대신 `dft`, `cv::phaseCorrelate()` 도 실험 가능
- 로테이션이 없는 경우 필터링(early exit) 전략

## 개선 아이디어

### 1. FFT 기반 Phase Correlation (추천)
```cpp
cv::phaseCorrelate(patch1, patch2);
```

- 두 이미지 간 subpixel 단위의 이동량을 한 번에 계산
- Polar 변환 후 적용 시 회전량 추정 가능
- `cv::logPolar`과 병행 사용 가능 (회전 + 스케일 정합)

**장점**: 매우 빠르고 서브픽셀 정밀도  
**단점**: 신호 품질이 낮거나 노이즈가 클 경우 오차 발생

---

### 2. Log-Polar 기반 추정

- `cv::logPolar()`로 변환 → Scale + Rotation → X/Y shift
- `phaseCorrelate()` 이용해 동시에 회전/스케일 추정 가능

**활용 시점**: 크기(scale)도 달라질 수 있는 상황

---

### 3. Gradient 기반 회전 추정

- 이미지 gradient 방향 분포(orientation histogram)를 통해 회전각 추정
- ex) Sobel → atan2 → 히스토그램 피크 비교

**장점**: 빠르고 직관적  
**단점**: 정밀도 낮고 부드러운 이미지에 불리함

---

## 결론

| 방식 | 정확도 | 속도 | 정합 범위 | 추천 용도 |
|------|--------|------|------------|------------|
| Polar + shift | 매우 높음 | 느림 | ±180° | 기준 구현 |
| Phase Correlation | 높음 | 매우 빠름 | ±180° | 실시간 추정 |
| Log-Polar + PhaseCorr | 중간~높음 | 중간 | 회전 + 스케일 | 자동 정규화 필요 시 |
| Gradient Histogram | 낮음 | 빠름 | ±90° 제한적 | 대략적 예비 추정 |

---

## 참고

- OpenCV Docs: [warpPolar](https://docs.opencv.org/master/da/d54/group__imgproc__transform.html#ga2e959b6d8bfa6e2980b4f3b25c576549)
- Phase Correlation: [phaseCorrelate()](https://docs.opencv.org/4.x/dc/dff/group__imgproc__motion.html)

# 개선된 고속 회전 추정 코드 (FFT 기반)

## 핵심 아이디어
Polar 변환 후, reference vs rotated 간의 **Phase Correlation**을 적용하여  
순환 Y-방향 shift (즉, 회전량)를 한 번에 추정한다.

---

## 코드 예시

```cpp
#include <opencv2/opencv.hpp>

float estimateRotationByPhaseCorrelation(const cv::Mat& reference, const cv::Mat& rotated) {
    int radius = std::min(reference.cols, reference.rows) / 2;
    cv::Point2f center(reference.cols / 2.0f, reference.rows / 2.0f);

    // 해상도 설정 (각도 방향이 세로, 고정)
    int angleResolution = 1024;

    // Polar 변환
    cv::Mat refPolar, rotPolar;
    cv::warpPolar(reference, refPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);
    cv::warpPolar(rotated, rotPolar, cv::Size(radius, angleResolution), center, radius, cv::WARP_POLAR_LINEAR);

    // 회전 정합: phase correlation (Y 방향 이동량 측정)
    cv::Point2d shift = cv::phaseCorrelate(refPolar, rotPolar);
    double yShift = shift.y;

    // 회전 각도 계산
    float angle = -360.0f * static_cast<float>(yShift) / angleResolution;
    if (angle < 0) angle += 360.0f; // 0~360으로 보정

    return -angle; // 외부 사용 시 CCW 기준 음수 반환
}
```

---

## 성능 비교

| 방식                     | 반복 계산 | 속도      | 정밀도   |
|--------------------------|------------|-----------|----------|
| `for` 루프 + 상관계수     | 1024회     | 느림      | 매우 높음 |
| `phaseCorrelate()` 기반   | 1회        | 매우 빠름 | 충분히 우수 |

---

## 추가 참고
- `warpPolar()`의 해상도는 `(radius, angleResolution)` 순서로 설정
- `phaseCorrelate()`는 내부적으로 FFT를 사용하여 주기적 시프트를 고속 검출


# FFT 기반 세로 Shift 추정 함수 분석

## 📌 함수 개요

```cpp
cv::Mat estimateVerticalShiftByFFT(const cv::Mat& ref, const cv::Mat& target);
```

- 목적: `ref` 이미지와 `target` 이미지 간의 **세로 방향(Y축) shift**를 빠르게 추정
- 사용처: Polar 변환된 이미지에서 각도 방향 차이를 FFT로 정렬
- 특징: 고속 수행, 루프 없음, OpenCV `cv::dft` 기반

## 🔍 작동 원리

### 1. 평균 제거 (DC 성분 제거)

```cpp
ref.convertTo(fRef, CV_32F);
target.convertTo(fTarget, CV_32F);
fRef -= cv::mean(fRef);
fTarget -= cv::mean(fTarget);
```

- DC 성분 제거로 정규화 cross-correlation 정확도 향상

### 2. 세로 방향 DFT 수행

```cpp
cv::dft(fRef, fRef, cv::DFT_ROWS | cv::DFT_COMPLEX_OUTPUT);
cv::dft(fTarget, fTarget, cv::DFT_ROWS | cv::DFT_COMPLEX_OUTPUT);
```

- `cv::DFT_ROWS`: **행별 DFT 수행** (Polar 변환의 Y축 = 각도축)

### 3. Cross Power Spectrum 계산

```cpp
cv::mulSpectrums(fTarget, fRef, conjRef, 0, true);
cv::normalize(conjRef, cps);
```

- \( \text{CPS} = \frac{F_{\text{target}} \cdot F_{\text{ref}}^*}{|F_{\text{target}} \cdot F_{\text{ref}}^*|} \)
- 위상 정보만 보존, shift 추정의 핵심

### 4. 역 DFT → correlation peak 추출

```cpp
cv::dft(cps, corr, cv::DFT_INVERSE | cv::DFT_ROWS | cv::DFT_REAL_OUTPUT | cv::DFT_SCALE);
cv::minMaxLoc(corr, nullptr, &maxVal, nullptr, &maxLoc);
```

- `corr`: correlation 함수 결과
- `maxLoc.y`: 가장 잘 맞는 shift 위치 (최댓값의 y좌표)

## ✅ 장점

- 루프 없이 빠른 수행 (정렬 성능 ↑)
- 정규화된 위상 상관 사용 → 노이즈에 다소 강함
- 서브픽셀 보정도 추가 가능 (parabolic fitting)

## ⚠️ 한계

- Y축 방향 shift만 추정 (Polar 변환된 이미지에 적합)
- 노이즈가 심하거나 반복 패턴이 있는 이미지에는 약할 수 있음
- subpixel 정확도 확보 시 추가 보정 필요

## 📌 요약

| 항목           | 내용                                      |
|----------------|-------------------------------------------|
| 사용 조건      | Polar 변환된 이미지의 세로 방향 shift 추정 |
| 연산 방식      | FFT → Cross Power Spectrum → IFFT         |
| 속도           | 매우 빠름 (루프 없음)                     |
| 정확도         | 정규화된 상관계수로 안정적                 |
| 추가 보정      | subpixel shift 보정 가능                   |
