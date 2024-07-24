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
- [X] 유저 컨트롤
	- [ ] messagebox
		- https://arong.info/Archive/ContentsView/28
		- https://github.com/pierre01/MessageBox
		- https://stackoverflow.com/questions/5644459/show-dialog-with-mvvm-light-toolkit
		- https://www.codeproject.com/Articles/5332442/Csharp-MVVM-Toolkit-Demo
		- https://github.com/FantasticFiasco/mvvm-dialogs-integrated-into-windows-community-toolkit/tree/main/src
	- [ ] RoutedEventHandler
	- [ ] Action
	- [ ] https://forum.dotnetdev.kr/t/mvvm-sample-for-wpf-usercontrol/8919
	- [ ] UserControl https://kaki104.tistory.com/851
	- [ ] UserControl https://narup.tistory.com/68
- [ ] WPF - DataTemplate로 UserControl 변경하기 https://www.youtube.com/watch?v=h_OOFnqCjLw
- [ ] animation manager 추가 해서 깜빡이 코드 중복 제거 https://www.youtube.com/playlist?list=PLlrfTSXS0LLK7V03CY3VouBPOn6cL3YAu
- [ ] 한소노 lib에서 영상 직접 받기
- [X] 이벤트 핸들러, 콜백 쓰는 방법
- [X] mfc, wpf 통신 방법 소켓
	- <a href="https://petra.tistory.com/613">cpp 소켓 프로그래밍</a>
	- <a href="https://learn.microsoft.com/ko-kr/windows/win32/winsock/finished-server-and-client-code">cpp 소켓 프로그래밍</a>
	- <a href="https://velog.io/@blanca/C-winsock2%EB%A5%BC-%EC%9D%B4%EC%9A%A9%ED%95%9C-%EC%86%8C%EC%BC%93-%ED%86%B5%EC%8B%A0-%EA%B5%AC%ED%98%84">cpp 소켓 프로그래밍</a>
	- <a href="https://www.youtube.com/playlist?list=PLlrfTSXS0LLL8dHVmURJiaf1ggZSFZV6u">cs 소켓 프로그래밍</a>
	- :star: <a href="https://zadd.tistory.com/32">mfc 소켓 프로그래밍</a>
- [X] 결과 이미지에 원그리기
- [X] winform bmp -> wpf bmp 바꾸기
- [X] WPF에서 bmp 파일 저장.
- [X] excel import
- [X] excel export
- [X] WPF -> MFC : save 명령 req , MFC : img 저장 , MFC -> WPF : 파일패스 res
- [ ] 전역 스타일 지정 https://www.youtube.com/watch?v=WWvYYxamemY, Dark theme
- [X] 모달 https://www.youtube.com/watch?v=uBMMvPG7zn4 
- [X] 윈도우 생명주기
	- https://www.youtube.com/watch?v=SJIhnpb0rM0
	- 화면 최상단 일때 사진 로드
- [X] 뷰모델간 데이터 전달 https://www.youtube.com/watch?v=_KXr5dGGS3s
- [X] 윈도우 간 파라미터 전달
	- https://www.youtube.com/watch?v=zDNFFJKW-KE 이거씀 이미 생성되어 있는데 전달
	- https://www.youtube.com/watch?v=_KXr5dGGS3s 생성시 주입
	- https://kaki104.tistory.com/869
- [X] 자동 완성, 예측 텍스트
- [X] seqNo 삭제
- [X] 서버에서 seqNo 내리기
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
- [X] 처음에 마우스 키보드로 선택하고 sn 입력 후 F9는 시험 F10은 저장 test결과는 자동화. 마우스 키보드 안만지고 발판 2개 버튼으로 시험 종료
- [X] 실패 점수 기록
- [X] test 삭제까지 보이게
- [X] test 갯수 가변
- [X] 성공 조건 가변
- [X] 윈도우 생성시 tester 객체에 데이터 삽입 널 체크
- [X] 검색시 td 객체에 데이터 삽입 널 체크
- [X] dataflag 0 조회 확인
- [X] testView 시나리오 확립
- [ ] datagrid click 세부 내용
- [X] 기본 포커스 위치?
	- why 빨간줄 나옴
- [X] 모터 모듈 s/n 파일 import
- [X] TD s/n 파일 import
- [X] 모터 모듈 s/n -> Lot 라고 바꾸기
- [X] text placeholder hint
	- https://www.youtube.com/watch?v=QUx2gh0PaEc
- [X] 벨리데이션
	- 직접 구현 디비 조회는 이 방법이 :crown: :+1:
	- https://kaki104.tistory.com/829
	- https://kaki104.tistory.com/863
	- https://stackoverflow.com/questions/75561916/validation-with-observablevalidator-error-on-geterrors-net-maui
	- [X] https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3750
	- [ ] https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/observablevalidator
- [X] 시리얼 체크
	- [X] 없으면 밑에 없다고 출력
- [ ] Datagrid 필터링
	- https://stackoverflow.com/questions/6317860/should-i-bind-to-icollectionview-or-observablecollection
- [ ] datagrid 60만건 속도 개선
	- https://stackoverflow.com/questions/1704512/wpf-toolkit-datagrid-scrolling-performance-problems-why
- [X] db crud
- [X] db 만건 1초 튜닝
	- [X] 뷰 테이블 생성
	:x: lazy 로딩
	:x: join 튜닝
- [X] model, repo 등 dll 로 빼기
- [X] mvvm 성능 측정 샘플 추가
- [X] AOP 로깅 이름,속도
	- https://blog.arong.info/c%23/2023/02/06/C-Method-Intercept(AOP).html
- [X] db 조회 null값 대응-자동
- [X] Tester name,pcNo(외래키 pcId) 검색 대응
- [X] BulkInsertAsync 원래 로그 못봄
- [X] 윈도우 재시작 에러
- [X] IsEnabled = false 일때 style null
- :x: test 카테고리 정리
- :x: enum에 all 추가
- [X] ListView 자동 마지막 스크롤 
	:x: ScrollToEndBehavior 사용시 같은 데이터 계속 넣으면 스크롤 안됨 cnt 넣어주기!
## Research List
- [X] C# 8.0 "switch expression"
- [ ] Mini Excel study 특히 자료형
- [ ] linq
- [ ] Dependency Property와 Attached Property
	- https://kaki104.tistory.com/563
- [X] Microsoft.Extensions.Logging vs :star: :sparkles: :crown: Serilog
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
- [ ] async await 정리
- [ ] Property 
	- https://dh-0501.tistory.com/138

## Scenario List
- [ ] test,testList,probeList 호출시 tester 테이블에 PCId, userName 추가 로그인 개념
	- 셋팅에 pcId 필요(디비 정보)
- [ ] 테스트가 이미 끝난 경우 테스트값을 볼수 있고 재검사 시 수정 되는지 결정