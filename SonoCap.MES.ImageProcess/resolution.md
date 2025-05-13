# 초음파 영상 해상도 검사 설계 노트

## ✅ 목적
- 초음파 영상 상에서 **2mm 간격의 3개 핀**이 구분되는지를 검사
- 해상도 유지 여부를 **객관적인 수치(Edge Density)**로 판별

---

## ✅ 입력 조건
- **입력 이미지 크기**: `512 × 512` (BGRA)
- **핀 위치**: 중앙에 삼각형 형태로 고정되어 있음
- **해상도 검사 영역**: 전체 이미지의 중심 `1/4` (`256 × 256` 영역)

---

## ✅ 검사 핵심 로직

1. **Grayscale 변환**
    - BGRA → GRAY

2. **Edge Detection**
    - `Sobel` 또는 `Canny` 필터로 고주파 성분 추출
    - Canny 예시:
      ```cpp
      cv::Canny(grayROI, edgeImage, 100, 200);
      ```

3. **Edge Density 계산**
    - `(엣지 픽셀 수) / (ROI 전체 픽셀 수)`
    - 수치가 **기준치 이상이면**, 3개의 핀이 구분되는 것으로 판단

4. **시각화**
    - 검사 ROI를 노란 사각형으로 표시
    - 결과는 JSON으로 출력:
      ```json
      {"EdgeRatio": 0.184}
      ```

---

## ✅ ROI 설정

```cpp
int roiX = image.cols / 4;
int roiY = image.rows / 4;
int roiW = image.cols / 2;
int roiH = image.rows / 2;
cv::Rect centerROI(roiX, roiY, roiW, roiH);
```

---

## ✅ 검사 예시 코드

```cpp
void MyOpenCVWrapper::ResInspection(cv::Mat& roiImage, std::string& resultText)
{
    if (roiImage.empty() || roiImage.channels() != 4) {
        resultText = R"({"error": "invalid input"})";
        return;
    }

    cv::Mat grayImage;
    cv::cvtColor(roiImage, grayImage, cv::COLOR_BGRA2GRAY);

    int roiX = grayImage.cols / 4;
    int roiY = grayImage.rows / 4;
    int roiW = grayImage.cols / 2;
    int roiH = grayImage.rows / 2;
    cv::Rect roi(roiX, roiY, roiW, roiH);
    cv::Mat grayROI = grayImage(roi);

    cv::Mat edgeImage;
    cv::Canny(grayROI, edgeImage, 100, 200);

    double edgePixels = cv::countNonZero(edgeImage);
    double totalPixels = roiW * roiH;
    double edgeRatio = edgePixels / totalPixels;

    cv::rectangle(roiImage, roi, YellowA, 2);

    std::ostringstream oss;
    oss << R"({"EdgeRatio": )" << edgeRatio << "}";
    resultText = oss.str();
}
```

---

## ✅ 판단 기준
- `EdgeRatio > 0.15` → OK (핀 3개 구분 가능)
- `EdgeRatio <= 0.15` → FAIL (해상도 부족)

※ 기준값은 데이터 기반으로 튜닝 필요

---

## ✅ 확장 방향
- 핀 위치 자동 검출 알고리즘 추가
- 엣지 수치 외에 frequency analysis 기반 접근
- FFT 기반 고주파 성분 면적 분석으로 보강

---
