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
