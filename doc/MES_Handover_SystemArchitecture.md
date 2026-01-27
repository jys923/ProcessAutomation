# MES 인수인계서 (시스템 아키텍처 / 영상 처리 제외)

## 1. 문서 목적

본 문서는 MES 시스템 중 **영상 처리 로직을 제외한 전체 소프트웨어 구조**에 대해  
신규 개발자가 빠르게 이해하고 유지보수할 수 있도록 하기 위한 **개발자 인수인계 문서**이다.

본 문서는 실제 구현 기준으로 작성되며,  
MVVM, Service, Repository, Entity, ORM 구조를 중심으로 설명한다.

---

## 2. 전체 아키텍처 개요

MES는 **단일 PC 기반 Client Application** 구조이며,  
UI–비즈니스 로직–데이터 접근 계층을 명확히 분리하여 구성되어 있다.

```
UI (WPF / MVVM)
   ↓
Service Layer
   ↓
Repository Layer
   ↓
ORM (EF Core / Raw SQL)
   ↓
Database (MariaDB)
```

---

## 3. UI 계층 (WPF / MVVM)

### 3.1 설계 원칙

- MVVM 패턴 적용
- View는 UI 표현만 담당
- 모든 로직은 ViewModel 또는 Service로 위임
- Code-behind 최소화

---

### 3.2 View

- XAML 기반 UI 정의
- 비즈니스 로직 없음
- Binding / Command만 사용

예:
- MainView
- InspectionView
- ResultView
- SettingsView

---

### 3.3 ViewModel

- UI 상태 관리
- Command 처리
- Service 호출 담당

특징:
- 계산 로직 최소화
- 장시간 작업은 Service로 위임
- Dispatcher 사용하여 UI Thread 보호

---

## 4. Service Layer

### 4.1 역할

Service 계층은 **MES의 핵심 비즈니스 로직 계층**이다.

- 검사 시나리오 제어
- 장비 제어 연계
- 데이터 저장 트리거
- 트랜잭션 단위 제어

UI는 Service를 직접 호출하며,  
Repository에는 직접 접근하지 않는다.

---

### 4.2 주요 Service 예시

| Service | 역할 |
|---|---|
| InspectionService | 검사 시나리오 제어 |
| MotorService | 장비 회전 / 속도 제어 |
| ExcelService | 검사 결과 리포트 생성 |
| SerialPortService | 장비 통신 |
| ConfigService | 설정 관리 |

---

## 5. Repository Layer

### 5.1 역할

Repository는 **DB 접근 전용 계층**으로,  
비즈니스 로직과 DB 구현을 분리하기 위해 사용된다.

- CRUD 캡슐화
- Query 로직 집중
- DB 변경 영향 최소화

---

### 5.2 Repository 사용 원칙

- Service만 Repository 호출
- UI / ViewModel에서 직접 접근 금지
- 트랜잭션 단위는 Service에서 관리

---

### 5.3 주요 Repository 예시

| Repository | 대상 |
|---|---|
| ProbeRepository | Probe 정보 |
| TransducerRepository | 장비 정보 |
| TestResultRepository | 검사 결과 |
| UserRepository | 사용자 정보 |

---

## 6. Entity / ORM 구조

### 6.1 ORM 개요

- EF Core 사용
- MariaDB / MySQL 기반
- 일부 성능 민감 쿼리는 Raw SQL 또는 View 사용

---

### 6.2 Entity

- DB 테이블 1:1 매핑
- 비즈니스 로직 없음
- 순수 데이터 구조

예:
- Probe
- Transducer
- TestResult
- Tester

---

### 6.3 DbContext

- 전체 Entity 관리
- Connection / Transaction 관리
- Migration 관리

---

## 7. 데이터 흐름 예시 (검사 결과 저장)

```
View
 ↓
ViewModel
 ↓
InspectionService
 ↓
TestResultRepository
 ↓
DbContext (EF Core)
 ↓
Database
```

---

## 8. 설정 관리

- appsettings.json 기반
- 환경 분리
  - Development
  - Production

설정 변경 시 재빌드 없이 적용 가능하도록 설계됨.

---

## 9. 로그 구조

- 시스템 로그
- 검사 로그
- 통신 로그

날짜별 폴더 구조로 관리되며,  
장애 발생 시 로그 우선 확인을 원칙으로 한다.

---

## 10. 유지보수 가이드 (핵심)

### 10.1 이런 수정은 ViewModel

- UI 상태 변경
- 버튼 동작 변경

### 10.2 이런 수정은 Service

- 검사 흐름 변경
- 저장 시점 변경
- 장비 연계 로직

### 10.3 이런 수정은 Repository

- DB 쿼리 변경
- 조회 성능 개선

### 10.4 이런 수정은 Entity / DbContext

- DB 스키마 변경
- 신규 테이블 추가

---

## 11. 인수 핵심 요약

- UI는 얇고, 로직은 Service에 집중됨
- Repository는 DB 접근 전용
- Entity는 데이터 구조체 역할만 수행
- 영상 처리 외 MES 로직은 이 구조를 따른다
