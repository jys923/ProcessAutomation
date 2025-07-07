# 공정검사 프로그램 설치 및 초기 설정 가이드

이 문서는 **SonoCap MES 공정검사 프로그램**의 설치 및 실행 절차를 안내합니다.

---

## 📦 1. 요구 사양

- Windows 10 이상
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- MariaDB 10.5 이상
- Git (선택사항, 소스 기반 설치 시)

---

## 🧰 2. 설치 절차

### ✅ 2.1 .NET 런타임 설치

- [.NET 8.0 Runtime 다운로드](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- 설치 후 확인:

```bash
dotnet --info
```

### ✅ 2.2 MariaDB 설치

- [MariaDB 다운로드](https://mariadb.org/download/)
- 설치 시 root 비밀번호는 예: `Endolfin12!@`
- 포트: `3306`

---

## ⚙️ 3. DB 초기 설정

### ✅ 3.1 `appsettings.json` 설정 확인

```json
"ConnectionStrings": {
  "MariaDBConnection": "Server=localhost; Port=3306; Database=sonocap_mes; Uid=root; Pwd=Endolfin12!@;AllowLoadLocalInfile=true; Connection Timeout=5;"
},
"DbSettings": {
  "AutoMigrate": true
}
```

- 프로그램을 **최초 실행 시** `"AutoMigrate": true` 설정으로 DB 테이블이 자동 생성됩니다.
- 테이블이 생성된 후에는 반드시 다음과 같이 변경하세요:

```json
"AutoMigrate": false
```

---

## 🚀 4. 프로그램 실행

- `SonoCap.MES.exe` 실행
- 최초 실행 시 콘솔에 테이블 생성 로그가 출력됩니다.
- 마이그레이션 완료 후 프로그램이 자동 실행됩니다.

---

## 📝 5. 로그 확인

### ✅ 로그 경로
```
logs/log-YYYY-MM-DD.txt
```

### ✅ 실시간 로그 보기

#### Windows PowerShell
```powershell
powershell -Command "Get-Content 'C:\logs\log-2025-07-02.txt' -Tail 20 -Wait"
```

#### Linux / WSL
```bash
tail -f logs/log-2025-07-02.txt
```

- 로그는 하루 단위로 자동 생성되며, 최대 30일치 보관됩니다.
- 개별 파일은 최대 10MB까지 기록됩니다.

---

## 📂 6. 출력 폴더 구조

```plaintext
./report/           → 검사 결과 엑셀
./capture/images/   → 검사 캡처 이미지
./capture/videos/   → 녹화 영상 파일
```

---

## ✅ 마무리 체크리스트

- [ ] .NET 런타임 설치 완료
- [ ] MariaDB 설치 및 포트 확인
- [ ] `AutoMigrate: true`로 설정 후 최초 실행
- [ ] DB 테이블 생성 확인 후 `AutoMigrate: false` 변경
- [ ] 로그 파일 정상 생성 여부 확인



## 할일

## 검사 이미지 저장 구조

### 📁 폴더 구조 (검사 단계별)

| 검사 단계   | 폴더명    |
|-------------|-----------|
| 공정        | 공정      |
| 완제품      | 완제품    |
| 최종        | 최종      |

저장 루트 예: `/InspectionImages/공정/`, `/InspectionImages/완제품/`

---

### 🗂 파일명 규칙

| 검사 단계   | 기준 SN 필드              | 파일명 예시         |
|-------------|----------------------------|----------------------|
| 공정        | Transducer.Sn              | TD567_001.bmp        |
| 완제품      | TransducerModule.Sn        | TDMD789_001.bmp      |
| 최종        | Probe.Sn                   | PB1234_001.bmp       |

- 접미사 `_001`, `_002`는 prefix 기준으로 자동 증가
- 확장자 `.bmp` 고정
- DB에는 **파일명만 저장**하고, 전체 경로는 앱 설정에서 관리

---

### 📌 전체 예시 경로

```plaintext
/InspectionImages/공정/TD567_001.bmp
/InspectionImages/완제품/TDMD789_001.bmp
/InspectionImages/최종/PB1234_001.bmp

# WPF MVVM 창 닫기 구조 정리

## ✅ 기존 구조 요약

- ViewModel: `ViewModelBase`를 상속
- View: `Window` 기반 (`ProbeView.xaml.cs`)
- DataContext 주입: `App.Services.GetRequiredService<ProbeViewModel>()`
- ViewModel에서 `Window` 객체 접근이 필요함

---

## ✅ 기존 방식 1: CloseAction 주입 방식

```csharp
this.Loaded += (_, __) =>
{
    if (DataContext is ProbeViewModel vm)
        vm.CloseAction = this.Close;
};
```

ViewModel에서는:

```csharp
CloseAction?.Invoke();
```

### 장점
- MVVM 원칙 준수 (Window에 직접 접근 안 함)

### 단점
- Delegate 주입 코드가 중복됨
- ViewModel에서 Window의 라이프사이클 제어는 어려움

---

## ✅ 개선 방식 2: Window 참조 구조 (ViewModelBase 기반)

ViewModelBase에 다음 구조 포함:

```csharp
protected Window? Window;
internal void SetWindow(Window window)
{
    Window = window;
    AddLifecycleHander();
}
```

View에서는 다음처럼 연결:

```csharp
this.Loaded += (_, __) =>
{
    if (DataContext is ViewModelBase vm)
        vm.SetWindow(this);
};
```

ViewModel에서는 다음처럼 닫기 가능:

```csharp
Window?.Close();
```

---

## ✅ ViewModelBase 추가 확장 가능성

```csharp
protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e) { }
protected virtual void OnWindowClosing(object? sender, CancelEventArgs e) { }
protected virtual void OnWindowActivated(object? sender, EventArgs e) { }
```

→ ViewModel에서 View 이벤트를 처리할 수 있음

---

## ✅ 결론

| 항목 | CloseAction 방식 | Window 참조 방식 |
|------|------------------|------------------|
| MVVM 순도 | 높음 | 중간 |
| 코드 간결함 | 보통 | ✅ 간결 |
| 유지보수성 | 보통 | ✅ 높음 |
| 현재 구조에 적합 | ❌ 주입 코드 반복 | ✅ 매우 적합 |

> ✅ 현재 구조에서는 `Window?.Close();` 방식이 가장 실용적이고 깔끔함
