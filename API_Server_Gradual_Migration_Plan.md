# API_Server_Gradual_Migration_Plan.md

## 1. 전환 개요 및 목표

이 계획은 기존의 **DB 직접 접속(Direct Access) 코드**를 유지하면서, 새로운 **API 서버**를 도입하고 최종적으로 기존 코드를 안전하게 제거하는 **점진적 전환(Strangler Fig Pattern)**을 목표로 합니다.

| 구분 | 내용 |
| :--- | :--- |
| **목표** | 서비스 중단 없이 DB 직접 접속 $\rightarrow$ API 기반 접속으로 안전하게 마이그레이션. |
| **핵심 기법** | Feature Toggle (기능 토글)을 이용한 조건부 로직 적용. |

***

## 2. 3단계 전환 프로세스

### 단계 1: API 서버 구축 및 코드 복제

1.  **새 API 프로젝트 생성:** 독립적인 **ASP.NET Core Web API** 프로젝트를 생성합니다.
2.  **프로젝트 참조:** 다음 기존 프로젝트들을 새 API 서버가 참조하도록 설정합니다.
    * `SonoCap.MES.Models`
    * `SonoCap.MES.Repositories` (DB 접속 로직 포함)
    * `SonoCap.MES.Services` (비즈니스 로직 포함)
3.  **DB 접속 정보 이전:** DB 연결 문자열을 **API 서버 프로젝트**에만 설정하고, 보안을 위해 관리합니다.
4.  **API Controller 구현:** `Services` 계층의 메서드를 호출하는 **API Controller**를 업무 단위로 구현합니다. (예: `/api/pcs`, `/api/tests`)

---

### 단계 2: UI에 스위치 로직 및 API 클라이언트 도입 (병렬 운영)

이 단계는 **기존 코드와 신규 API 호출 코드를 동시에 유지**하는 단계입니다.

1.  **API 클라이언트 프로젝트 생성:** `SonoCap.MES.ApiClients` 프로젝트를 추가하고, API 서버의 엔드포인트를 호출하는 클라이언트 로직을 구현합니다.
2.  **환경 설정 추가:** UI 프로젝트의 설정 파일(예: `appsettings.json`)에 **Feature Toggle** 플래그와 API 서버 주소를 추가합니다.
    * `"FeatureToggles": { "UseApiServer": false }`
    * `"ApiSettings": { "BaseUrl": "http://[API_SERVER_IP]:[PORT]" }`
3.  **Services 계층 수정 (핵심):**
    * 기존 `Services` 클래스에 **`IApiClient`**를 주입받도록 의존성을 추가합니다.
    * 모든 데이터 접근 메서드(예: `GetPcList()`, `InsertTest()`) 내부에 **토글 로직**을 추가합니다.

```csharp
// SonoCap.MES.Services 내부 (예시)
public List<PcModel> GetPcList()
{
    if (Configuration.GetValue<bool>("FeatureToggles:UseApiServer"))
    {
        // 신규: API 클라이언트 호출
        return _apiClient.GetPcList();
    }
    else
    {
        // 기존: Repository를 통한 DB 직접 접속 유지
        return _pcRepository.GetAll().ToList();
    }
}