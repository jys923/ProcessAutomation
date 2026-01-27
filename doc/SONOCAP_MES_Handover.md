# SONOCAP MES 인수인계서

## 1. 프로젝트 개요
- **프로젝트명**: SONOCAP MES
- **목적**: 초음파 의료기기 SONOCAP의 제조 공정 관리, 검사 자동화, 영상 품질 평가 및 이력 관리
- **적용 대상**: Probe / Transducer / Motor 기반 생산·검사 공정
- **운영 환경**: 생산 라인 검사 PC (오프라인 운용 가능)

---

## 2. 시스템 전체 아키텍처

[장비]
Probe / Transducer / Motor  
↓  
[H/W 연동 Layer]  
- SerialPortWrapper  
- MotorService (RPM / PRF 제어)  
- HsnLibraryCS (초음파 렌더 SDK)  

↓  
[Service Layer]  
- USRenderService (오프스크린 렌더, 60fps)  
- ImageProcess Service (OpenCV)  
- ExcelService (MiniExcel)  

↓  
[Data Layer]  
- Repository Pattern  
- EF Core + MariaDB  
- Raw SQL / Stored Procedure 일부 사용  

↓  
[UI]  
- .NET 8 WPF  
- MVVM 패턴  

---

## 3. 기술 스택
| 구분 | 내용 |
|---|---|
| UI | .NET 8, WPF, MVVM |
| DB | MariaDB, EF Core |
| 영상처리 | OpenCV (C# Wrapper) |
| 초음파 렌더 | HsnLibraryCS (Offscreen View) |
| 통신 | Serial 통신 (Motor 제어) |
| 리포트 | MiniExcel |
| 설정 | appsettings.Dev / Prod.json |

---

## 4. 주요 기능별 인수 포인트

### 4.1 공정 / 장비 식별
- Probe / Transducer / TransducerModule / Motor 단위 식별
- PTRView는 조회 전용 View
- ID 불일치 시 검사 결과 저장 불가

### 4.2 초음파 영상 렌더링
- BGRA 512x512 이미지 사용
- UI Thread 직접 접근 금지

### 4.3 영상 검사(Image Processing)
- ROI 사용
- Gray / Geo / Res 검사 수행
- 기준치 변경 시 전체 재검증 필요

### 4.4 데이터 저장
- EF Core + Raw SQL 혼합
- 마이그레이션 자동 적용 없음
- 운영 DB 변경 시 수동 스크립트 필요

### 4.5 Excel 리포트
- MiniExcel 기반
- 컬럼명 변경 시 템플릿 동기화 필요

---

## 5. 설정 파일
- appsettings.Dev.json
- appsettings.Prod.json

---

## 6. 운영 및 유지보수 주의사항
- SDK 업데이트 시 렌더 콜백 우선 확인
- vcpkg 사용 종속성 관리 주의
- 검사 이력 DB 백업 필수

---

## 7. 미완 / 개선 필요 항목
- 검사 기준치 버전 관리
- Audit Trail 기능
- 장비 연결 자동 복구
- 검사 결과 Trend 분석

---
