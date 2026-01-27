# MES 인수인계서 (영상 검사 중심)

## 1. 문서 목적

본 문서는 MES 시스템 중 **영상 기반 검사 로직**에 대한 개발자 인수인계를 목적으로 한다.  
특히 Res 검사와 EnvGeo 검사의 실제 소스 구조와 동작 방식을 기준으로 작성되었으며,  
추측이나 일반론 없이 **현재 구현된 코드 기준**으로 설명한다.

---

## 2. 영상 검사 전체 구조 요약

영상 검사는 다음과 같이 두 개의 독립된 검사 경로로 구성된다.

### Res / Gray 검사 경로

```
Input Image
   ↓
MyOpenCVWrapper
   ↓
RunInspection
   ↓
Res / Gray 결과
```

### EnvGeo 검사 경로

```
Input Image
   ↓
MyOpenCVWrapper
   ↓
EnvGeoInspection
   ↓
EnvGeo 결과
```

- Res / Gray 검사와 EnvGeo 검사는 서로 독립적으로 수행된다.
- 공통 OpenCV 연산은 `MyOpenCVWrapper` 계층을 통해 수행된다.

---

## 3. 주요 파일 역할

| 파일 | 역할 |
|---|---|
| MyOpenCVWrapper.cpp | 공통 OpenCV 연산 및 검사 진입점 |
| RunInspection.cpp | 단일 영상에 대한 Res / Gray 검사 수행 |
| ResInspection.cpp | Res 검사 핵심 로직 |
| EnvGeoInspection.cpp | EnvGeo 검사 핵심 로직 |

---

## 4. EnvGeo 검사 설명

### 4.1 검사 개요

EnvGeo 검사는 환경(Environment) 및 기하(Geometry) 특성을 동시에 평가하는 **단일 검사 항목**이다.  
최종적으로 **EnvGeoResult 1개**를 반환한다.

---

### 4.2 처리 흐름

```
Input Image
   ↓
Histogram 기반 전처리
   ↓
Threshold 후보 생성
   ↓
Threshold × Morphology 조합 반복 검사
   ↓
유효 Trial 수집
   ↓
Best Trial 선택
   ↓
결과 시각화 및 반환
```

---

### 4.3 반복 Trial 구조

- 히스토그램 기반 기준 threshold를 중심으로 여러 threshold 후보를 생성
- 각 threshold에 대해 모든 Morphology 모드를 적용
- `(threshold, morphology)` 조합마다 EnvGeo trial 1회 수행
- 성공한 trial만 후보로 수집

---

### 4.4 최종 결과 선택

- 모든 trial 결과 중 내부 기준에 따라 **가장 적합한 trial 1개 선택**
- 선택된 결과만 외부로 반환
- 중간 trial 결과는 외부에 노출되지 않음

---

## 5. Res 검사 설명

### 5.1 검사 개요

Res 검사는 ROI 내에서 **세 기준 포인트(P1, P2, P3)**를 검출하여  
구조적 거리 관계를 기반으로 해상도 특성을 평가하는 **단일 검사 항목**이다.  
최종적으로 **ResResult 1개**를 반환한다.

---

### 5.2 처리 흐름

```
Input Image
   ↓
Histogram 기반 전처리
   ↓
Threshold 후보 생성
   ↓
Threshold × Morphology 조합 반복 검사
   ↓
Contour 분석
   ↓
P1 / P2 / P3 검출
   ↓
Best Trial 선택
   ↓
결과 시각화 및 반환
```

---

### 5.3 RunResTrial 핵심 처리

- ROI grayscale 영상에 threshold 적용
- MorphMode에 따라 Open / Close / Hybrid morphology 수행
- Contour 추출 후 면적 기준 필터링
- 기준점(영상 우하단) 대비 거리 및 각도 계산
- 세 기준 포인트(P1, P2, P3) 검출 실패 시 trial 무효 처리

---

### 5.4 Best Trial 선택 기준

1. 목표 거리(horizontal / vertical)와의 오차 최소
2. 거리 오차가 유사한 경우
   - 세 포인트의 edgeDensity 합이 큰 trial 선택

---

## 6. 공통 설계 특징 및 인수 포인트

- Res / EnvGeo 검사는 모두 **반복 trial 후 최적 1개 선택 구조**
- Threshold와 Morphology 조합을 통해 영상 조건 편차를 흡수
- ROI, threshold 생성 방식, 선택 기준 변경 시 검사 특성 전체에 영향
- 공통 OpenCV 연산 수정은 모든 검사에 영향

---

## 7. 수정 가이드

| 수정 내용 | 대상 파일 |
|---|---|
| Res 검사 로직 | ResInspection.cpp |
| EnvGeo 검사 로직 | EnvGeoInspection.cpp |
| 검사 흐름 연결 | RunInspection.cpp |
| OpenCV 공통 처리 | MyOpenCVWrapper.cpp |

---

## 8. 인수 핵심 요약

- 영상 검사는 Res / EnvGeo 두 경로로 분리되어 있음
- 두 검사는 독립적으로 수행되며 결과는 각각 1개만 반환
- 내부적으로는 반복 trial 구조를 사용하되, 외부 인터페이스는 단순함
- 실제 검사 특성 변경은 대부분 ResInspection / EnvGeoInspection 수정으로 해결됨
