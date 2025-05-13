# 🔧 MSVCRTD.lib / abseil_dll.lib 충돌 해결 마크다운 노트

## ⚠️ 발생한 문제

vcpkg로 설치한 OpenCV를 C++/CLI 프로젝트(HsnLibraryCS)에서 사용할 때,  
`MSVCRTD.lib`와 `abseil_dll.lib` 간 심볼 충돌로 인한 **LNK2005 오류** 발생:

```
error LNK2005: std::bad_alloc already defined in abseil_dll.lib
fatal error LNK1169: 하나 이상의 중복된 기호
```

---

## ✅ 원인 요약

| 항목 | 설명 |
|------|------|
| **MSVCRTD.lib** | C++/CLI 프로젝트가 디버그(/MDd)일 때 링크되는 MSVC 디버그 런타임 |
| **abseil_dll.lib** | OpenCV 내부에서 사용하는 라이브러리 (릴리즈 /MD 모드로 빌드됨) |
| **문제** | 동일한 `std::exception`, `std::bad_alloc` 심볼이 양쪽에 중복 정의되어 충돌 |

---

## ✅ 구조적 해결 방법 1: 릴리즈 모드로 빌드

- Visual Studio 상단에서 `Debug` → `Release`로 변경
- 플랫폼: `x64`
- `/MD`로 자동 설정됨 (vcpkg 릴리즈용 OpenCV와 완벽 호환)

### ✔ 장점
- **가장 안정적**
- OpenCV 및 모든 의존 라이브러리와 **정확히 일치**

---

## ✅ 해결 방법 2: 링커 옵션으로 무시

### 설정 방법

```text
프로젝트 → 속성 → 링커 → 명령줄 → 추가 옵션:
/FORCE:MULTIPLE
```

### ✔ 장점
- **즉시 해결**, 빠른 테스트 가능

### ⚠️ 단점
- 충돌된 심볼이 실제 런타임에서 어떤 버전으로 실행될지 **불확실**
- **릴리즈용으로는 부적절**

---

## ✅ 해결 방법 3: vcpkg triplet 수정 (고급)

### 목적
- OpenCV와 모든 의존 라이브러리를 `/MDd` + 디버그 모드로 다시 빌드

### 설정

1. triplet 복제
```bash
cd vcpkg/triplets
copy x64-windows.cmake x64-windows-mddebug.cmake
```

2. triplet 수정 (`x64-windows-mddebug.cmake`)
```cmake
set(VCPKG_TARGET_ARCHITECTURE x64)
set(VCPKG_CRT_LINKAGE dynamic)
set(VCPKG_LIBRARY_LINKAGE dynamic)
set(VCPKG_BUILD_TYPE debug)
```

3. 다시 설치
```bash
vcpkg install opencv4:x64-windows-mddebug
```

4. VS 프로젝트 속성 변경
- 포함 디렉터리: `installed/x64-windows-mddebug/include`
- 라이브러리 디렉터리: `installed/x64-windows-mddebug/lib`
- 추가 종속성: `opencv_world4xxd.lib; abseil_dlld.lib; flatbuffersd.lib`

---

## ✅ 결론

| 상황 | 추천 해결 |
|------|------------|
| 데모, 개발 중 빠른 적용 | `/FORCE:MULTIPLE` 또는 `Release` 모드 |
| 실제 릴리즈, 배포 | **Release 모드로 고정** (/MD) |
| 디버깅까지 완전 일치 필요 | vcpkg triplet 커스터마이징 |

---

**✔ 현재는 되니까 그대로 진행하고, 릴리즈에서는 문제 없음!**
