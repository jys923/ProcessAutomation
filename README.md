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
- [X] null 이면 전부 검색으로 바꾸자! all 일떄 항상 0 인게 문제. all을 항상 -1 해서 -1 이면 무시
	- if (dataFlagProbe != null && dataFlagProbe != 0)
            {
                query = query.Where(tp => tp.DataFlagProbe == dataFlagProbe);
            }
- [X] enums 추가 네임스페이스만 필요. 클래스 필요없음
- [ ] 윈도우 생명주기 필요! 화면 최상단 일때 사진 로드
- [ ] 윈도우 간 파라미터 전달
- [X] 실패 점수 기록
- [X] test 삭제까지 보이게
- [X] test 갯수 가변
- [ ] 성공 조건 가변
- [X] dataflag 0 조회 확인
- [X] testView 시나리오 확립
- [ ] datagrid click 세부 내용
- [ ] 한소노 lib에서 영상 직접 받기
- [ ] Dark theme
- [ ] 모터 모듈 s/n 파일 import
- [ ] 모터 모듈 s/n -> Lot 라고 바꾸기
- [ ] winform bmp -> wpf bmp 바꾸기
- [ ] 벨리데이션
	- https://kaki104.tistory.com/829
- [ ] 시리얼 검색 버튼
	- [ ] 없으면 밑에 없다고 출력
- [ ] TD s/n 파일 import
- [ ] 유저 컨트롤
	- [ ] messagebox
		- https://arong.info/Archive/ContentsView/28
		- https://github.com/pierre01/MessageBox
		- https://stackoverflow.com/questions/5644459/show-dialog-with-mvvm-light-toolkit
		- https://www.codeproject.com/Articles/5332442/Csharp-MVVM-Toolkit-Demo
		- https://github.com/FantasticFiasco/mvvm-dialogs-integrated-into-windows-community-toolkit/tree/main/src
	- [ ] RoutedEventHandler
	- [ ] Action
	- [ ] 
- [ ] Datagrid 필터링
	- https://stackoverflow.com/questions/6317860/should-i-bind-to-icollectionview-or-observablecollection
- [ ] datagrid 60만건 속도 개선
	- https://stackoverflow.com/questions/1704512/wpf-toolkit-datagrid-scrolling-performance-problems-why
- [X] db crud
- [X] db 만건 1초 튜닝
	- [ ] lazy 로딩 끄기 
	- [ ] join 튜닝, 안쓰기
	- [X] 뷰 테이블 생성 
- [X] model, repo 등 dll 로 빼기
- [X] mvvm 성능 측정 샘플 추가
- [X] AOP 로깅 이름,속도
	- https://blog.arong.info/c%23/2023/02/06/C-Method-Intercept(AOP).html
- [X] db 조회 null값 대응-자동
- [X] Tester name,pcNo(외래키 pcId) 검색 대응
- [X] BulkInsertAsync 원래 로그 못봄
- [X] 윈도우 재시작 에러
## Research List
- [ ] linq
- [X] Microsoft.Extensions.Logging vs <font color='green'>Serilog</font>
	- logging .net 기본 시간 표기 가능,
	- serilog 구조화된 로그 추가 복잡할 때 사용
- [ ] TDD mock 객체 vs UseInMemoryDatabase
- [ ] WPF animation
- [ ] behavior
- [ ] find first asqueryable 용도
- [X] wpf 와 TDD 환경 구축
	- TDD 환경 구축 xunit 프로젝트 설정 바꿔야 됨, test 대상 프로젝트 참조
- [X] EFCore join 시 객체 바인딩 가능?
	- 가능 편리하게 이용