# SonoCap.MES

## Release Note
### V 0.9
- 진행 중 ..

## Memo
- db migration
- 패키지 관리자 콘솔 PM 에서 실행
```
add-migration 1st
update-database
remove-migration
```

## Todo List
- [X] show hide
- [X] readonly gray
- [X] blinking
- [X] SN 직접 삽입
- [X] join 성능 개선 SQL (3990ms)
- [X] Linq 정형 dataFlag 먼저 실행 성능 개선 
- [X] Linq, 메소드 방식 비정형
- [X] 레포지토리 패턴
- [X] ioc
- [X] 진짜 UI ms lib? 날코딩? 코드 비하인드 불가
- [X] 로깅 환경 구축 Lib 선택 seridog
- [ ] 모터 모듈 s/n 파일 import
- [ ] TD s/n 파일 import
- [ ] 유저 컨트롤
- [X] db crud
- [ ] 데이터 필터링
- [ ] datagrid 필터링
	- https://stackoverflow.com/questions/6317860/should-i-bind-to-icollectionview-or-observablecollection
- [ ] model, repo 등 dll 로 빼기
- [ ] 한소노 lib에서 영상 직접 받기
- [X] mvvm 성능 측정 샘플 추가
- [ ] datagrid 60만건 속도 개선
	- https://stackoverflow.com/questions/1704512/wpf-toolkit-datagrid-scrolling-performance-problems-why
- [X] AOP 로깅 이름,속도
	- https://blog.arong.info/c%23/2023/02/06/C-Method-Intercept(AOP).html
- [ ] db 조회 null값 대응
- [X] Tester name,pcNo(외래키 pcId) 검색 대응
- [ ] BulkInsertAsync 로그
- [ ] dark theme
- [X] 윈도우 재시작 에러 test만

## Research List
- [ ] linq
- [X] Microsoft.Extensions.Logging vs serilog
	- logging .net 기본 시간 표기 가능,
	- serilog 구조화된 로그 추가 복잡할떄 사용
- [ ] TDD mock 객체 vs UseInMemoryDatabase
- [ ] WPF animation
- [ ] behavior
- [ ] find first asqueryable 용도
- [X] wpf 와 TDD 환경 구축
	- TDD 환경 구축 xunit 프로젝트 설정 바꿔야 됨, test 대상 프로젝트 참조
- [X] EFCore join 시 객체 바인딩 가능?
	- 가능 편리하게 이용