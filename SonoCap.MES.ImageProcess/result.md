# 검사 결과 JSON 구조 설계

## ✅ 전체 JSON 구조 예시

```json
{
  "Gray": {
    "mean1": 123.4,
    "mean2": 127.8,
    "mean3": 125.1
  },
  "Res": {
    "edgeDensity1": 0.134,
    "edgeDensity2": 0.198,
    "edgeDensity3": 0.105,
    "horizontalDist": 62.0,
    "verticalDist": 59.2
  },
  "Geo": {
    "meanBrightness": 187.6,
    "stdBrightness": 13.2
  }
}
```

---

## 📘 Gray 검사

- **목적**: 원 내부 3개 지점의 밝기 평균을 통해 품질 확인
- **항목**:
  - `mean1`: ROI1의 평균 밝기 (좌상단)
  - `mean2`: ROI2의 평균 밝기 (우상단)
  - `mean3`: ROI3의 평균 밝기 (하단)
- **단위**: 0 ~ 255

---

## 📘 Res 검사 (해상도 / 구분도)

- **목적**: 점 구분도를 에지 밀도 및 ROI 간 거리로 평가
- **항목**:
  - `edgeDensity1`: ROI1 내 에지 픽셀 밀도
  - `edgeDensity2`: ROI2 내 에지 픽셀 밀도
  - `edgeDensity3`: ROI3 내 에지 픽셀 밀도
  - `horizontalDist`: ROI1 ↔ ROI2 중심 간 거리 (수평)
  - `verticalDist`: ROI1 ↔ ROI3 중심 간 거리 (수직)
- **특징**:
  - ROI는 삼각형 형태로 배치
  - 거리는 `cv::norm` 으로 계산

---

## 📘 Geo 검사 (기하학적 왜곡)

- **목적**: 직선성이 보장된 영역의 일관된 밝기 여부로 왜곡 판단
- **항목**:
  - `meanBrightness`: ROI의 평균 밝기
  - `stdBrightness`: ROI의 밝기 표준편차 (작을수록 안정적)

---

## 💡 기타 설계 포인트

- **검사 합격 여부 판단(`pass`)는 포함하지 않음**
  - → 판단은 C# 상위 로직에서 수행
- **확장성 고려**
  - Gray, Res, Geo는 독립적인 구조
  - 각각 개별 저장/분석이 가능하도록 구성
