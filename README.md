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

## Todo
- [ ] openGL research
- [ ] cpp 빌드 과정
- [ ] 한소노 lib에서 영상 직접 받기
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

## Scenario
- [ ] test,testList,probeList 호출시 tester 테이블에 PCId, userName 추가 로그인 개념
	- 셋팅에 pcId 필요(디비 정보)
- [ ] 테스트가 이미 끝난 경우 테스트값을 볼수 있고 재검사 시 수정 되는지 결정

## Sonocap
```
'SonoCapUsImgTest.exe'(Win32): 'C:\Users\USER\source\repos\SonoCap\SonoCapUsImgTest\bin\Debug\net8.0\SonoCapUsImgTest.exe'을(를) 로드했습니다. 
2024-08-07 10:06:35.992 :  HsnInterface : hsn::registerCallback_Loading
2024-08-07 10:06:35.992 :  GLOBAL : Context Initialization : 0
2024-08-07 10:06:35.993 :  HsnInterface : hsn::registerCallback_Error
2024-08-07 10:06:35.996 :  HsnInterface : hsn::initialize
2024-08-07 10:06:35.996 :  HsnInterface : hsn::initialize : Task manager initializing...
2024-08-07 10:06:35.996 :  HsnTask : hsn::TaskManager::initiate
2024-08-07 10:06:35.997 :  HsnTask : hsn::TaskManager::initiate : Activate Looper Section thread
2024-08-07 10:06:35.997 :  HsnTask : hsn::TaskManager::initiate : Activate Task Queue thread
2024-08-07 10:06:35.997 :  HsnInterface : hsn::initialize : Device Context Initializing
2024-08-07 10:06:35.998 :  HsnTask : Start Worker : NO_CONTEXT
2024-08-07 10:06:35.998 :  HsnTask : Start Worker : 
2024-08-07 10:06:35.998 :  HsnInterface : hsn::initialize : USB Connector Constructing...
2024-08-07 10:06:35.998 :  HsnTask : Start Worker : DB
2024-08-07 10:06:35.998 :  HsnTask : Start Worker : UI
2024-08-07 10:06:35.998 :  HsnTask : Start Worker : Device
2024-08-07 10:06:35.998 :  HsnUSBConnector : hsn::USBConnector::initiate
2024-08-07 10:06:35.998 :  HsnInterface : hsn::initialize : Data initializing...
2024-08-07 10:06:35.998 :  HsnAsset : hsn::access::asset::initiate
2024-08-07 10:06:35.998 :  HsnExternal : hsn::access::external::initiate
2024-08-07 10:06:35.999 :  HsnMetadata : hsn::access::metadata::setMetadataPath
2024-08-07 10:06:35.999 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:06:36.000 :  HsnDataVersionController : hsn::data::HsnExternalBackUp::checkExternalBackupExist
2024-08-07 10:06:36.000 :  HsnExternal : fail to open external(external.backup)
2024-08-07 10:06:36.001 :  HsnMetadata : hsn::access::metadata::initiate
2024-08-07 10:06:36.001 :  HsnMetadata : hsn::access::metadata::initiate : cur_version is : v01.03.01
2024-08-07 10:06:36.002 :  HsnDataVersionController : hsn::data::HsnDataVersionController::initiate : data version (v01.03.01)
2024-08-07 10:06:36.002 :  HsnDataVersionController : hsn::data::HsnDataVersionController::initiate : preset version (hsn_v01.00.01)
2024-08-07 10:06:36.002 :  HsnDataVersionController : hsn::data::HsnDataVersionController::initiate : preset out version (hsn_v01.00.01)
2024-08-07 10:06:36.015 :  HsnData : hsn::data::Data::activate
2024-08-07 10:06:36.017 :  HsnConfig : hsn::data::Config::initiate
2024-08-07 10:06:36.017 :  HsnDeviceConfig : hsn::data::DeviceConfig::initiate
2024-08-07 10:06:36.017 :  HsnDeviceConfig : hsn::data::DeviceConfig::loadFPGAList
2024-08-07 10:06:36.022 :  HsnProperty : hsn::data::Property::construct
2024-08-07 10:06:36.040 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.040 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.040 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.041 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.041 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.041 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.041 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.042 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.045 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.048 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.049 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.049 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:06:36.050 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:06:36.061 :  HsnInterface : hsn::initialize : Load Setting from Default Application...
2024-08-07 10:06:36.068 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::synchronize
2024-08-07 10:06:36.080 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::find_record
2024-08-07 10:06:36.081 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::find_record
2024-08-07 10:06:36.081 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:06:36.081 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:06:36.089 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:06:36.090 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:06:36.106 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::find_record
2024-08-07 10:06:36.106 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:06:36.106 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:06:36.222 :  HsnInterface : hsn::initialize : Device Information Constructing...
2024-08-07 10:06:36.224 :  HsnInterface : hsn::initialize : Connect Property to IP...
2024-08-07 10:06:36.236 :  HsnInterface : hsn::initialize : Connect Property to Device...
2024-08-07 10:06:36.239 :  HsnDeviceProperty : hsn::DeviceProperty::registerCallback
2024-08-07 10:06:36.239 :  DEVICE_STATE : define_device_state : Make Device State Machine
2024-08-07 10:06:36.239 :  ULTRASOUND_STATE : define_ultrasound_state : Make UltraSound State Machine
2024-08-07 10:06:36.239 :  HsnTask : Start Worker : Device State
2024-08-07 10:06:36.239 :  HsnTask : Start Worker : Ultrasound State
2024-08-07 10:06:36.240 :  NGS_STATE : define_ngs_state : Make NGS State Machine
2024-08-07 10:06:36.240 :  HsnInterface : hsn::initialize : Buffer Constructing...
2024-08-07 10:06:36.240 :  HsnTask : Start Worker : NGS State
2024-08-07 10:06:36.240 :  Buffer : hsn::make_buffer : Making Buffer instance
2024-08-07 10:06:36.240 :  Buffer : hsn::make_buffer : Making Buffer instance
2024-08-07 10:06:36.240 :  HsnInterface : hsn::initialize : Device Manager Constructing...
2024-08-07 10:06:36.240 :  HsnUSBManager : hsn::USBManager::initiate
2024-08-07 10:06:36.240 :  HsnInterface : hsn::initialize : Device Field Upgrader Constructing...
2024-08-07 10:06:36.240 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::initiate
2024-08-07 10:06:36.240 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::initiate : Getting Manager
2024-08-07 10:06:36.240 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::initiate : Getting DeviceInformation
2024-08-07 10:06:36.240 :  HsnFileterCoefficients : hsn::FilterCoefficients::initiate
2024-08-07 10:06:36.247 :  HsnGlobalConfig : encrypted path : g0038.dat
2024-08-07 10:06:36.247 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:06:36.247 :  HsnInterface : hsn::initialize : Device Transfer Constructing...
2024-08-07 10:06:36.247 :  HsnDeviceTransfer : hsn::DeviceTransfer::initiate
2024-08-07 10:06:36.247 :  HsnDeviceTransfer : hsn::DeviceTransfer::initiate : Getting Property
2024-08-07 10:06:36.247 :  HsnDeviceTransfer : hsn::DeviceTransfer::initiate : Initialize Device Sendor
2024-08-07 10:06:36.247 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_SendorController::initiate
2024-08-07 10:06:36.247 :  HsnDeviceTransfer : hsn::DeviceTransfer::initiate : Initialize Device Receiver
2024-08-07 10:06:36.247 :  DeviceTransfer_Receiver : hsn::DeviceTransfer_Receiver::initiate
2024-08-07 10:06:36.247 :  HsnInterface : hsn::initialize : Device Motion Controller Constructing...
2024-08-07 10:06:36.248 :  DeviceMotionControl : hsn::DeviceMotionControl::initiate
2024-08-07 10:06:36.248 :  DeviceMotionControl : hsn::DeviceMotionControl::initiate : Getting Manager
2024-08-07 10:06:36.248 :  DataATGC : hsn::DataATGC::initiate
2024-08-07 10:06:36.249 :  HsnStateMachine : hsn::StepStateMachine::set
2024-08-07 10:06:36.419 :  DEVICE_STATE : device_detect
2024-08-07 10:06:36.419 :  HsnUSBManager : hsn::USBManager::detect
2024-08-07 10:06:36.419 :  HsnUSBManager : hsn::USBManager::detect : find a device
2024-08-07 10:06:36.419 :  HsnUSBConnector : hsn::USBConnector::startDetection
2024-08-07 10:06:36.670 :  HsnInterface : hsn::probeConnect (fd number - 0)
2024-08-07 10:06:36.670 :  HsnStateMachine : hsn::StepStateMachine::set
2024-08-07 10:06:36.670 :  DEVICE_STATE : device_connect
2024-08-07 10:06:36.670 :  HsnUSBManager : hsn::USBManager::connect(0)
2024-08-07 10:06:36.670 :  HsnUSBManager : Get File Descriptor
2024-08-07 10:06:36.671 :  HsnUSBConnector : hsn::USBConnector::activate
2024-08-07 10:06:48.530 :  HsnUSBManager : hsn::USBManager::connect : Mode initialization
2024-08-07 10:06:48.564 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::checkMCU
2024-08-07 10:06:48.564 :  HsnUSBManager : hsn::USBManager::readMCUVersion
2024-08-07 10:06:48.591 :  HsnUSBManager : hsn::USBManager::readMCUVersion : return(12)
2024-08-07 10:06:48.592 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::checkSystem
2024-08-07 10:06:48.617 :  HsnUSBManager : hsn::USBManager::updateMCUStatus
2024-08-07 10:06:48.667 :  HsnUSBManager : hsn::USBManager::updateMCUStatus : return(9)
2024-08-07 10:06:48.667 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::readSC1data_from_MCUEEPROM
2024-08-07 10:06:54.336 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::readSC1data_from_MCUEEPROM : read data is not valid
2024-08-07 10:06:54.364 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::readSC1data_from_MCUEEPROM
2024-08-07 10:06:54.400 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::readSC1data_from_MCUEEPROM
2024-08-07 10:06:55.123 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::readSC1data_from_MCUEEPROM : read data is not valid
2024-08-07 10:06:55.123 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::readSC1data_from_MCUEEPROM
2024-08-07 10:06:56.454 :  HsnUSBManager : hsn::USBManager::updateMCUStatus
2024-08-07 10:06:56.466 :  HsnUSBManager : hsn::USBManager::updateMCUStatus : return(9)
2024-08-07 10:06:56.466 :  DEVICE_STATE : device_connect : MAINBOARD INFO : I201M10232000006
2024-08-07 10:06:56.466 :  DEVICE_STATE : device_connect : MCU INFO :UNC3101010000
2024-08-07 10:06:56.466 :  DEVICE_STATE : device_connect : hsn_git : 26ce405f2cffbbd6777c27b0de86d8dc0836aa44 2024-02-22 16:03:26 +0900

2024-08-07 10:06:56.501 :  DEVICE_STATE : device_activate
2024-08-07 10:06:56.501 :  COMMON_EVENT_DEVICE : hsn::common::device_activate
2024-08-07 10:06:56.501 :  HsnUSBManager : hsn::USBManager::activate
2024-08-07 10:06:56.501 :  HsnUSBManager : hsn::USBManager::offPower
2024-08-07 10:06:56.502 :  HsnUSBManager : hsn::USBManager::offPower : good off power
2024-08-07 10:06:56.529 :  HsnUSBManager : hsn::USBManager::offPower : return(0)
2024-08-07 10:06:56.529 :  HsnUSBManager : hsn::USBManager::onPower
2024-08-07 10:06:58.988 :  HsnUSBManager : hsn::USBManager::onPower : return(0)
2024-08-07 10:06:58.989 :  HsnUSBManager : hsn::USBManager::onFPGA
2024-08-07 10:06:59.015 :  HsnUSBManager : hsn::USBManager::onFPGA : return(1)
2024-08-07 10:07:00.679 :  HsnGlobalConfig : encrypted path : g0017.dat
2024-08-07 10:07:00.679 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:07:00.731 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::downloadFPGA : writing FPGA

2024-08-07 10:07:10.276 :  HsnGraphicsInformation : Renderer: NVIDIA GeForce RTX 4060 Ti/PCIe/SSE2

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : OpenGL version supported: 4.6.0 NVIDIA 555.85

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : Number of texture units: 32

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : Maximum number of color attachments: 8

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : Maximum number of fragment shader outputs: 8

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : Maximum work group invocations: 1024

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : Maximum work group count: 2147483647 65535 65535

2024-08-07 10:07:10.277 :  HsnGraphicsInformation : Maximum work group size: 1024 1024 64

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : GL_MAX_UNIFORM_BUFFER_BINDINGS : 84

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : GL_MAX_UNIFORM_BLOCK_SIZE : 65536

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : Maximum uniform locations : 65536

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : GL_MAX_VERTEX_UNIFORM_BLOCKS : 14

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : GL_MAX_GEOMETRY_UNIFORM_BLOCKS : 14

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : GL_MAX_FRAGMENT_UNIFORM_BLOCKS : 14

2024-08-07 10:07:10.278 :  HsnGraphicsInformation : hsnlibrary ip module version : 3.0.7 [2024.2.21] - r2
2024-08-07 10:07:10.278 :  HsnInterfaceIP : ip_data init() start
2024-08-07 10:07:10.278 :  HsnInterfaceIP : ip_app init() start
2024-08-07 10:07:10.278 :  HsnInterfaceIP : ip_buffer init() start
2024-08-07 10:07:10.279 :  HSN_IP_BUFFER_CONTAINER : ip createbuffers result - success
2024-08-07 10:07:10.279 :  HsnInterfaceIP : ip_layer init() start
2024-08-07 10:07:31.673 :  HsnUSBManager : hsn::USBManager::offFPGA
2024-08-07 10:07:31.685 :  HsnUSBManager : hsn::USBManager::offFPGA : return(1)
2024-08-07 10:07:31.742 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9ff3(40947))
2024-08-07 10:07:31.753 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(304e)
2024-08-07 10:07:31.754 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9ff2(40946))
2024-08-07 10:07:31.765 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(3031)
2024-08-07 10:07:31.765 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9ff1(40945))
2024-08-07 10:07:31.776 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(3030)
2024-08-07 10:07:31.776 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9ff0(40944))
2024-08-07 10:07:31.788 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(3031)
2024-08-07 10:07:31.788 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::downloadFPGA : FPGA Boot success
2024-08-07 10:07:31.788 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9f01(40705))
2024-08-07 10:07:31.799 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(3)
2024-08-07 10:07:31.799 :  HsnData : hsn::data::Data::activate
2024-08-07 10:07:31.800 :  HsnConfig : hsn::data::Config::initiate
2024-08-07 10:07:31.800 :  HsnDeviceConfig : hsn::data::DeviceConfig::initiate
2024-08-07 10:07:31.800 :  HsnDeviceConfig : hsn::data::DeviceConfig::loadFPGAList
2024-08-07 10:07:31.800 :  HsnProperty : hsn::data::Property::construct
2024-08-07 10:07:31.808 :  HsnProperty : hsn::data::Property::construct::<lambda_1>::operator () : clear previous parameter
2024-08-07 10:07:31.808 :  HsnProperty : hsn::data::Property::construct::<lambda_1>::operator () : clear previous parameter
2024-08-07 10:07:31.826 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.826 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.826 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.827 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.827 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.827 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.828 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.828 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.831 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.834 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.835 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.836 :  HsnPreview : hsn::data::Preview::construct
2024-08-07 10:07:31.836 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:07:31.847 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::find_record
2024-08-07 10:07:31.847 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::find_record
2024-08-07 10:07:31.847 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:07:31.847 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:07:31.856 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:07:31.856 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:07:31.866 :  HsnDBCache : hsn::db::Cache<class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> >,class std::basic_string<char,struct std::char_traits<char>,class std::allocator<char> > >::find_record
2024-08-07 10:07:31.866 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:07:31.867 :  HsnProperty : hsn::data::Property::scan
2024-08-07 10:07:31.979 :  HsnUIConnector : hsn::UIConnectors::disconnect
2024-08-07 10:07:31.979 :  COMMON_EVENT_DEVICE : hsn::common::device_activate : Connect Propoerty to UI...
2024-08-07 10:07:31.979 :  HsnUIConnector : hsn::UIConnectors::connect
2024-08-07 10:07:31.980 :  COMMON_EVENT_DEVICE : hsn::common::device_activate : Connect Propoerty to Device...
2024-08-07 10:07:31.981 :  HsnDeviceProperty : hsn::DeviceProperty::registerCallback
2024-08-07 10:07:31.982 :  HsnGlobalConfig : encrypted path : g0038.dat
2024-08-07 10:07:31.982 :  HsnAsset : hsn::access::asset::Asset::Asset
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_SendorController::initiateSendors
2024-08-07 10:07:31.982 :  I3_ENDDeviceSendor : hsn::I3_END::I3_END_DeviceSendor_Linker::addSendors
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.982 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.983 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_Sendor::initiate
2024-08-07 10:07:31.984 :  COMMON_EVENT_DEVICE : hsn::common::device_activate : Connect Propoerty to IP...
2024-08-07 10:07:31.997 :  COMMON_EVENT_DEVICE : hsn::common::device_activate : Connect Property to ATGC...
2024-08-07 10:07:31.997 :  DataATGC : hsn::DataATGC::initiate
2024-08-07 10:07:31.997 :  DEVICE_STATE : device_boot
2024-08-07 10:07:31.997 :  COMMON_EVENT_DEVICE : hsn::common::device_boot
2024-08-07 10:07:31.998 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 1 return : 0)
2024-08-07 10:07:31.998 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 0 return : 0)
2024-08-07 10:07:32.023 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8110(33040))
2024-08-07 10:07:32.034 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(a)
2024-08-07 10:07:32.034 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 10 return : 0)
2024-08-07 10:07:32.035 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 0 return : 0)
2024-08-07 10:07:32.054 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : 87d2 return : 0)
2024-08-07 10:07:32.054 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 200 return : 0)
2024-08-07 10:07:32.054 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.065 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(a0)
2024-08-07 10:07:32.066 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 201 return : 0)
2024-08-07 10:07:32.066 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.077 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(41)
2024-08-07 10:07:32.077 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 202 return : 0)
2024-08-07 10:07:32.077 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.089 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(58)
2024-08-07 10:07:32.089 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 203 return : 0)
2024-08-07 10:07:32.089 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.100 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(91)
2024-08-07 10:07:32.101 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 204 return : 0)
2024-08-07 10:07:32.101 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.112 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(1)
2024-08-07 10:07:32.113 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 205 return : 0)
2024-08-07 10:07:32.113 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.124 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:32.124 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 206 return : 0)
2024-08-07 10:07:32.124 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.136 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:32.136 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 207 return : 0)
2024-08-07 10:07:32.136 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.147 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(54)
2024-08-07 10:07:32.148 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8011(32785) src : 1700 return : 0)
2024-08-07 10:07:32.148 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 8 return : 0)
2024-08-07 10:07:32.148 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 0 return : 0)
2024-08-07 10:07:32.162 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : 87d2 return : 0)
2024-08-07 10:07:32.163 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 0 return : 0)
2024-08-07 10:07:32.163 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.174 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(aa)
2024-08-07 10:07:32.174 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1 return : 0)
2024-08-07 10:07:32.174 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.186 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(85)
2024-08-07 10:07:32.186 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2 return : 0)
2024-08-07 10:07:32.186 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.197 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(ee)
2024-08-07 10:07:32.198 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 3 return : 0)
2024-08-07 10:07:32.198 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.209 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(71)
2024-08-07 10:07:32.210 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 4 return : 0)
2024-08-07 10:07:32.210 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.221 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(b3)
2024-08-07 10:07:32.221 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 5 return : 0)
2024-08-07 10:07:32.221 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.233 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(61)
2024-08-07 10:07:32.233 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 6 return : 0)
2024-08-07 10:07:32.233 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.244 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(dd)
2024-08-07 10:07:32.245 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 7 return : 0)
2024-08-07 10:07:32.245 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.256 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(14)
2024-08-07 10:07:32.256 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 8 return : 0)
2024-08-07 10:07:32.256 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.268 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(e9)
2024-08-07 10:07:32.268 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 9 return : 0)
2024-08-07 10:07:32.268 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.279 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(d7)
2024-08-07 10:07:32.280 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : a return : 0)
2024-08-07 10:07:32.280 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.291 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(a2)
2024-08-07 10:07:32.291 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : b return : 0)
2024-08-07 10:07:32.291 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.303 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(d0)
2024-08-07 10:07:32.303 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : c return : 0)
2024-08-07 10:07:32.303 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.314 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(e2)
2024-08-07 10:07:32.315 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : d return : 0)
2024-08-07 10:07:32.315 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.326 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(71)
2024-08-07 10:07:32.327 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : e return : 0)
2024-08-07 10:07:32.327 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.338 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(83)
2024-08-07 10:07:32.338 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : f return : 0)
2024-08-07 10:07:32.338 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.349 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(93)
2024-08-07 10:07:32.350 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 10 return : 0)
2024-08-07 10:07:32.350 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.361 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(87)
2024-08-07 10:07:32.362 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 11 return : 0)
2024-08-07 10:07:32.362 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.373 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(8e)
2024-08-07 10:07:32.373 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 12 return : 0)
2024-08-07 10:07:32.373 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.385 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(97)
2024-08-07 10:07:32.385 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 13 return : 0)
2024-08-07 10:07:32.385 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.396 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(d)
2024-08-07 10:07:32.397 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 14 return : 0)
2024-08-07 10:07:32.397 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.408 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(22)
2024-08-07 10:07:32.408 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 15 return : 0)
2024-08-07 10:07:32.408 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.420 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(3c)
2024-08-07 10:07:32.420 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 16 return : 0)
2024-08-07 10:07:32.420 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.431 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(5a)
2024-08-07 10:07:32.432 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 17 return : 0)
2024-08-07 10:07:32.432 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.443 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(67)
2024-08-07 10:07:32.443 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 18 return : 0)
2024-08-07 10:07:32.443 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.455 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:32.455 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 20 return : 0)
2024-08-07 10:07:32.455 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 0 return : 0)
2024-08-07 10:07:32.476 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8110(33040))
2024-08-07 10:07:32.488 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(b)
2024-08-07 10:07:32.488 :  HsnUSBManager :  hsn::USBManager::burstWrite (addr : 2004, offset : 0, length : 4096)
2024-08-07 10:07:32.488 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : 7d4 return : 0)
2024-08-07 10:07:32.488 :  HsnUSBManager : hsn::USBManager::gotoBurstWriteMode
2024-08-07 10:07:32.489 :  HsnUSBManager :  hsn::USBManager::burstWrite (return : 4096)
2024-08-07 10:07:32.508 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : 87d4 return : 0)
2024-08-07 10:07:32.509 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 0 return : 0)
2024-08-07 10:07:32.509 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.521 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(56)
2024-08-07 10:07:32.521 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1 return : 0)
2024-08-07 10:07:32.521 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.532 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(30)
2024-08-07 10:07:32.533 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2 return : 0)
2024-08-07 10:07:32.533 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.544 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(30)
2024-08-07 10:07:32.544 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 3 return : 0)
2024-08-07 10:07:32.544 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.556 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(32)
2024-08-07 10:07:32.556 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 4 return : 0)
2024-08-07 10:07:32.556 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.567 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(32)
2024-08-07 10:07:32.568 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 5 return : 0)
2024-08-07 10:07:32.568 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.579 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(39)
2024-08-07 10:07:32.579 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 6 return : 0)
2024-08-07 10:07:32.579 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.591 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(30)
2024-08-07 10:07:32.591 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 7 return : 0)
2024-08-07 10:07:32.591 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.602 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(35)
2024-08-07 10:07:32.603 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 8 return : 0)
2024-08-07 10:07:32.603 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.614 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(45)
2024-08-07 10:07:32.614 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 9 return : 0)
2024-08-07 10:07:32.614 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.626 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(4e)
2024-08-07 10:07:32.626 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : a return : 0)
2024-08-07 10:07:32.626 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.637 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(44)
2024-08-07 10:07:32.638 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : b return : 0)
2024-08-07 10:07:32.638 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.649 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(4f)
2024-08-07 10:07:32.649 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : c return : 0)
2024-08-07 10:07:32.649 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.661 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(4c)
2024-08-07 10:07:32.661 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : d return : 0)
2024-08-07 10:07:32.661 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.672 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(46)
2024-08-07 10:07:32.673 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : e return : 0)
2024-08-07 10:07:32.673 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.684 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(49)
2024-08-07 10:07:32.684 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : f return : 0)
2024-08-07 10:07:32.684 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.695 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(92)
2024-08-07 10:07:32.696 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 10 return : 0)
2024-08-07 10:07:32.696 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.707 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(f2)
2024-08-07 10:07:32.707 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 11 return : 0)
2024-08-07 10:07:32.707 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.719 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(a2)
2024-08-07 10:07:32.719 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 12 return : 0)
2024-08-07 10:07:32.719 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.730 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(91)
2024-08-07 10:07:32.731 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 13 return : 0)
2024-08-07 10:07:32.731 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.742 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(35)
2024-08-07 10:07:32.742 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 14 return : 0)
2024-08-07 10:07:32.743 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.754 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(bc)
2024-08-07 10:07:32.754 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 15 return : 0)
2024-08-07 10:07:32.754 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.765 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(61)
2024-08-07 10:07:32.766 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 16 return : 0)
2024-08-07 10:07:32.766 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.777 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(dc)
2024-08-07 10:07:32.777 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 17 return : 0)
2024-08-07 10:07:32.777 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.789 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(74)
2024-08-07 10:07:32.789 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 18 return : 0)
2024-08-07 10:07:32.789 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.800 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(fa)
2024-08-07 10:07:32.801 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 19 return : 0)
2024-08-07 10:07:32.801 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.812 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(43)
2024-08-07 10:07:32.812 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1a return : 0)
2024-08-07 10:07:32.812 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.824 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(11)
2024-08-07 10:07:32.824 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1b return : 0)
2024-08-07 10:07:32.824 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.835 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(52)
2024-08-07 10:07:32.836 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1c return : 0)
2024-08-07 10:07:32.836 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.847 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(fc)
2024-08-07 10:07:32.847 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1d return : 0)
2024-08-07 10:07:32.847 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.859 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(70)
2024-08-07 10:07:32.859 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1e return : 0)
2024-08-07 10:07:32.859 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.870 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(ab)
2024-08-07 10:07:32.871 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 1f return : 0)
2024-08-07 10:07:32.871 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.882 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(1f)
2024-08-07 10:07:32.882 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 20 return : 0)
2024-08-07 10:07:32.882 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.894 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(5)
2024-08-07 10:07:32.894 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 21 return : 0)
2024-08-07 10:07:32.894 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.905 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(a9)
2024-08-07 10:07:32.906 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 22 return : 0)
2024-08-07 10:07:32.906 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.917 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(e)
2024-08-07 10:07:32.917 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 23 return : 0)
2024-08-07 10:07:32.917 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.929 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(34)
2024-08-07 10:07:32.929 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 24 return : 0)
2024-08-07 10:07:32.929 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.940 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(3e)
2024-08-07 10:07:32.941 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 25 return : 0)
2024-08-07 10:07:32.941 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.952 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(24)
2024-08-07 10:07:32.953 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 26 return : 0)
2024-08-07 10:07:32.953 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.964 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(53)
2024-08-07 10:07:32.964 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 27 return : 0)
2024-08-07 10:07:32.964 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.976 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(47)
2024-08-07 10:07:32.976 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 28 return : 0)
2024-08-07 10:07:32.976 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.987 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:32.988 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 29 return : 0)
2024-08-07 10:07:32.988 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:32.999 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:32.999 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2a return : 0)
2024-08-07 10:07:33.000 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:33.011 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.011 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2b return : 0)
2024-08-07 10:07:33.011 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:33.023 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.023 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2c return : 0)
2024-08-07 10:07:33.023 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:33.034 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.035 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2d return : 0)
2024-08-07 10:07:33.035 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:33.046 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.046 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2e return : 0)
2024-08-07 10:07:33.046 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:33.058 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.058 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7201(29185) src : 2f return : 0)
2024-08-07 10:07:33.058 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8113(33043))
2024-08-07 10:07:33.069 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.070 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 40 return : 0)
2024-08-07 10:07:33.070 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 8010(32784) src : 0 return : 0)
2024-08-07 10:07:33.090 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 8110(33040))
2024-08-07 10:07:33.101 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(4b)
2024-08-07 10:07:33.102 :  HsnDeviceFieldUpgader : hsn::DeviceFieldUpgrader::getMCUVersionCompare
2024-08-07 10:07:33.107 :  HsnDeviceFieldUpgader : current firmware version is equal with Library firmware version
2024-08-07 10:07:33.108 :  DeviceTransfer_Receiver : hsn::DeviceTransfer_Receiver::initialize_receiver
2024-08-07 10:07:33.108 :  DeviceTransfer_Receiver : hsn::DeviceTransfer_Receiver::make_receiver
2024-08-07 10:07:33.108 :  DeviceTransfer_Receiver : hsn::DeviceTransfer_Receiver::make_receiver_capsule
2024-08-07 10:07:33.109 :  Buffer : length : 4194304, x_length : 1048576, y_length : 1, z_length : 4
2024-08-07 10:07:33.109 :  I3_ENDDeviceSendor : hsn::I3_END::I3_END_DeviceSendor_Linker::registerSendor
2024-08-07 10:07:33.109 :  COMMON_EVENT_DEVICE : USB Start BMode
2024-08-07 10:07:33.117 :  COMMON_EVENT_DEVICE : Do atomic start bmode
2024-08-07 10:07:33.117 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_SendorController::startBMode
2024-08-07 10:07:33.117 :  HsnDeviceTransfer_Sendor : hsn::DeviceTransfer_SendorController::send_sync
2024-08-07 10:07:33.117 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::SystemReset::SystemResetTransfer_Sendor::send
2024-08-07 10:07:33.117 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0400(1024) src : 0 return : 0)
2024-08-07 10:07:33.118 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0401(1025) src : 1 return : 0)
2024-08-07 10:07:33.118 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0401(1025) src : 0 return : 0)
2024-08-07 10:07:33.169 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::CommonControl::CommonControlTransfer_Sendor::send
2024-08-07 10:07:33.171 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Fan_Control::FanSpeedTransfer_Sendor::send
2024-08-07 10:07:33.172 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::ActivationTransfer_Sendor::send
2024-08-07 10:07:33.172 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0000(0) src : 1 return : 0)
2024-08-07 10:07:33.172 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0001(1) src : 1 return : 0)
2024-08-07 10:07:33.172 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::PowerTransfer_Sendor::send
2024-08-07 10:07:33.172 :  HsnUSBManager : hsn::USBManager::onPower
2024-08-07 10:07:33.173 :  HsnUSBManager : hsn::USBManager::onPower : return(0)
2024-08-07 10:07:33.173 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0400(1024) src : 8d0 return : 0)
2024-08-07 10:07:33.173 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0401(1025) src : 1 return : 0)
2024-08-07 10:07:33.174 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0401(1025) src : 0 return : 0)
2024-08-07 10:07:33.782 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::ApertureTransfer_Sendor::send
2024-08-07 10:07:33.782 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::FocusTransfer_Sendor::send
2024-08-07 10:07:33.782 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::BlendingTransfer_Sendor::send
2024-08-07 10:07:33.782 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::FrequencyTransfer_Sendor::send
2024-08-07 10:07:33.783 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0004(4) src : 504 return : 0)
2024-08-07 10:07:33.783 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0009(9) src : 605 return : 0)
2024-08-07 10:07:33.783 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 000c(12) src : 504 return : 0)
2024-08-07 10:07:33.784 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 000d(13) src : 605 return : 0)
2024-08-07 10:07:33.784 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 000a(10) src : a return : 0)
2024-08-07 10:07:33.784 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::CycleTransfer_Sendor::send
2024-08-07 10:07:33.784 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7034(28724) src : 0 return : 0)
2024-08-07 10:07:33.785 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0005(5) src : 1 return : 0)
2024-08-07 10:07:33.785 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Tx::DelayTransfer_Sendor::send
2024-08-07 10:07:33.785 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::AFE_InitialTransfer_Sendor::send
2024-08-07 10:07:33.785 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0200(512) src : 1 return : 0)
2024-08-07 10:07:33.798 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0206(518) src : 4 return : 0)
2024-08-07 10:07:33.798 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.799 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4000 return : 0)
2024-08-07 10:07:33.799 :  I3_ENDDeviceSendor : afe addr : 0, data : 0
2024-08-07 10:07:33.799 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 10 return : 0)
2024-08-07 10:07:33.799 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4003 return : 0)
2024-08-07 10:07:33.799 :  I3_ENDDeviceSendor : afe addr : 3, data : 16
2024-08-07 10:07:33.800 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 8000 return : 0)
2024-08-07 10:07:33.800 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4041 return : 0)
2024-08-07 10:07:33.800 :  I3_ENDDeviceSendor : afe addr : 65, data : 32768
2024-08-07 10:07:33.800 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 8000 return : 0)
2024-08-07 10:07:33.800 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4042 return : 0)
2024-08-07 10:07:33.801 :  I3_ENDDeviceSendor : afe addr : 66, data : 32768
2024-08-07 10:07:33.801 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.801 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4041 return : 0)
2024-08-07 10:07:33.801 :  I3_ENDDeviceSendor : afe addr : 65, data : 0
2024-08-07 10:07:33.801 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.802 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4042 return : 0)
2024-08-07 10:07:33.802 :  I3_ENDDeviceSendor : afe addr : 66, data : 0
2024-08-07 10:07:33.802 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0201(513) src : 2 return : 0)
2024-08-07 10:07:33.802 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::AFE_InitialTransfer_Sendor::send : LVDS CONFIGURATION ADD
2024-08-07 10:07:33.802 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0207(519) src : a0 return : 0)
2024-08-07 10:07:33.803 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::LVDS_SETTINGTransfer_Sendor::send
2024-08-07 10:07:33.803 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 180 return : 0)
2024-08-07 10:07:33.803 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4002 return : 0)
2024-08-07 10:07:33.803 :  DeviceLVDSControl : afe addr : 2, data : 180
2024-08-07 10:07:33.804 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 10 return : 0)
2024-08-07 10:07:33.804 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4003 return : 0)
2024-08-07 10:07:33.804 :  DeviceLVDSControl : afe addr : 3, data : 10
2024-08-07 10:07:33.804 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 10 return : 0)
2024-08-07 10:07:33.804 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4004 return : 0)
2024-08-07 10:07:33.805 :  DeviceLVDSControl : afe addr : 4, data : 10
2024-08-07 10:07:33.805 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 7050 return : 0)
2024-08-07 10:07:33.805 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4005 return : 0)
2024-08-07 10:07:33.805 :  DeviceLVDSControl : afe addr : 5, data : 7050
2024-08-07 10:07:33.805 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0301(769) src : a20 return : 0)
2024-08-07 10:07:33.806 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0303(771) src : 705 return : 0)
2024-08-07 10:07:33.806 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0302(770) src : 3 return : 0)
2024-08-07 10:07:33.806 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0300(768) src : 1 return : 0)
2024-08-07 10:07:33.814 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9300(37632))
2024-08-07 10:07:33.825 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(1)
2024-08-07 10:07:33.825 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9302(37634))
2024-08-07 10:07:33.836 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:33.836 :  DeviceLVDSControl : AFE LVDS good
2024-08-07 10:07:33.837 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0206(518) src : 0 return : 0)
2024-08-07 10:07:33.837 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::TR_ENTransfer_Sendor::send
2024-08-07 10:07:33.837 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0205(517) src : 32 return : 0)
2024-08-07 10:07:33.837 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.838 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40ce return : 0)
2024-08-07 10:07:33.838 :  I3_ENDDeviceSendor : afe addr : 206, data : 0
2024-08-07 10:07:33.838 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 2 return : 0)
2024-08-07 10:07:33.838 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40ca return : 0)
2024-08-07 10:07:33.838 :  I3_ENDDeviceSendor : afe addr : 202, data : 2
2024-08-07 10:07:33.838 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::Offset_CorrectionTransfer_Sendor::send
2024-08-07 10:07:33.839 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.839 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4002 return : 0)
2024-08-07 10:07:33.839 :  I3_ENDDeviceSendor : afe addr : 2, data : 0
2024-08-07 10:07:33.839 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 110 return : 0)
2024-08-07 10:07:33.840 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4003 return : 0)
2024-08-07 10:07:33.840 :  I3_ENDDeviceSendor : afe addr : 3, data : 272
2024-08-07 10:07:33.840 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : c210 return : 0)
2024-08-07 10:07:33.840 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4004 return : 0)
2024-08-07 10:07:33.840 :  I3_ENDDeviceSendor : afe addr : 4, data : 49680
2024-08-07 10:07:33.841 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0201(513) src : 2 return : 0)
2024-08-07 10:07:33.841 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0305(773) src : 0 return : 0)
2024-08-07 10:07:33.841 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::Digital_HPFTransfer_Sendor::send
2024-08-07 10:07:33.841 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 25 return : 0)
2024-08-07 10:07:33.841 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4015 return : 0)
2024-08-07 10:07:33.842 :  I3_ENDDeviceSendor : afe addr : 21, data : 37
2024-08-07 10:07:33.842 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 5 return : 0)
2024-08-07 10:07:33.842 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4021 return : 0)
2024-08-07 10:07:33.842 :  I3_ENDDeviceSendor : afe addr : 33, data : 5
2024-08-07 10:07:33.842 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 25 return : 0)
2024-08-07 10:07:33.843 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 402d return : 0)
2024-08-07 10:07:33.843 :  I3_ENDDeviceSendor : afe addr : 45, data : 37
2024-08-07 10:07:33.843 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 5 return : 0)
2024-08-07 10:07:33.843 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4039 return : 0)
2024-08-07 10:07:33.844 :  I3_ENDDeviceSendor : afe addr : 57, data : 5
2024-08-07 10:07:33.844 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 400 return : 0)
2024-08-07 10:07:33.844 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40c7 return : 0)
2024-08-07 10:07:33.844 :  I3_ENDDeviceSendor : afe addr : 199, data : 1024
2024-08-07 10:07:33.845 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.845 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40c8 return : 0)
2024-08-07 10:07:33.845 :  I3_ENDDeviceSendor : afe addr : 200, data : 0
2024-08-07 10:07:33.845 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.845 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40cc return : 0)
2024-08-07 10:07:33.846 :  I3_ENDDeviceSendor : afe addr : 204, data : 0
2024-08-07 10:07:33.846 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.846 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40cd return : 0)
2024-08-07 10:07:33.847 :  I3_ENDDeviceSendor : afe addr : 205, data : 0
2024-08-07 10:07:33.848 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.848 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40d0 return : 0)
2024-08-07 10:07:33.848 :  I3_ENDDeviceSendor : afe addr : 208, data : 0
2024-08-07 10:07:33.849 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.849 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40d8 return : 0)
2024-08-07 10:07:33.849 :  I3_ENDDeviceSendor : afe addr : 216, data : 0
2024-08-07 10:07:33.849 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.849 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40db return : 0)
2024-08-07 10:07:33.850 :  I3_ENDDeviceSendor : afe addr : 219, data : 0
2024-08-07 10:07:33.850 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.850 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40e5 return : 0)
2024-08-07 10:07:33.850 :  I3_ENDDeviceSendor : afe addr : 229, data : 0
2024-08-07 10:07:33.850 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::BACK_TO_ATGCTransfer_Sendor::send
2024-08-07 10:07:33.851 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 10 return : 0)
2024-08-07 10:07:33.851 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4000 return : 0)
2024-08-07 10:07:33.851 :  I3_ENDDeviceSendor : afe addr : 0, data : 16
2024-08-07 10:07:33.851 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::Inp_Resistor_SelectTransfer_Sendor::send
2024-08-07 10:07:33.851 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 32 return : 0)
2024-08-07 10:07:33.851 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40b5 return : 0)
2024-08-07 10:07:33.852 :  I3_ENDDeviceSendor : afe addr : 181, data : 50
2024-08-07 10:07:33.852 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : e80 return : 0)
2024-08-07 10:07:33.852 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40b6 return : 0)
2024-08-07 10:07:33.852 :  I3_ENDDeviceSendor : afe addr : 182, data : 3712
2024-08-07 10:07:33.852 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::AttenuatorTransfer_Sendor::send
2024-08-07 10:07:33.852 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 0 return : 0)
2024-08-07 10:07:33.853 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40b9 return : 0)
2024-08-07 10:07:33.853 :  I3_ENDDeviceSendor : afe addr : 185, data : 0
2024-08-07 10:07:33.853 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::ATGCTransfer_Sendor::send
2024-08-07 10:07:33.853 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 9f return : 0)
2024-08-07 10:07:33.853 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40a1 return : 0)
2024-08-07 10:07:33.854 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : ff return : 0)
2024-08-07 10:07:33.854 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 40a2 return : 0)
2024-08-07 10:07:33.854 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0207(519) src : 200 return : 0)
2024-08-07 10:07:33.855 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0208(520) src : 20 return : 0)
2024-08-07 10:07:33.855 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::AFE::BACK_TO_ATGCTransfer_Sendor::send
2024-08-07 10:07:33.855 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0203(515) src : 10 return : 0)
2024-08-07 10:07:33.855 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0202(514) src : 4000 return : 0)
2024-08-07 10:07:33.855 :  I3_ENDDeviceSendor : afe addr : 0, data : 16
2024-08-07 10:07:33.855 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Rx::ApodizationTransfer_Sendor::send
2024-08-07 10:07:33.855 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Rx::BeamformingOffsetTransfer_Sendor::send
2024-08-07 10:07:33.855 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Rx::ApertureTransfer_Sendor::send
2024-08-07 10:07:33.855 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Rx::DelayTransfer_Sendor::send
2024-08-07 10:07:33.856 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7039(28729) src : 96 return : 0)
2024-08-07 10:07:33.856 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7038(28728) src : 0 return : 0)
2024-08-07 10:07:33.856 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Rx::InitialDelayTransfer_Sendor::send
2024-08-07 10:07:33.856 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Mid::DcCancelTransfer_Sendor::send
2024-08-07 10:07:33.857 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6000(24576) src : fea0 return : 0)
2024-08-07 10:07:33.857 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6001(24577) src : fe91 return : 0)
2024-08-07 10:07:33.857 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6002(24578) src : fe71 return : 0)
2024-08-07 10:07:33.858 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6003(24579) src : fe3c return : 0)
2024-08-07 10:07:33.858 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6004(24580) src : fdee return : 0)
2024-08-07 10:07:33.858 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6005(24581) src : fd84 return : 0)
2024-08-07 10:07:33.858 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6006(24582) src : fd00 return : 0)
2024-08-07 10:07:33.859 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6007(24583) src : fc66 return : 0)
2024-08-07 10:07:33.859 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6008(24584) src : fbbb return : 0)
2024-08-07 10:07:33.859 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6009(24585) src : fb09 return : 0)
2024-08-07 10:07:33.859 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 600a(24586) src : fa58 return : 0)
2024-08-07 10:07:33.860 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 600b(24587) src : f9b5 return : 0)
2024-08-07 10:07:33.860 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 600c(24588) src : f929 return : 0)
2024-08-07 10:07:33.860 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 600d(24589) src : f8bd return : 0)
2024-08-07 10:07:33.860 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 600e(24590) src : f87a return : 0)
2024-08-07 10:07:33.861 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 600f(24591) src : 783e return : 0)
2024-08-07 10:07:33.861 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Mid::QDMTransfer_Sendor::send
2024-08-07 10:07:33.861 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6104(24836) src : 0 return : 0)
2024-08-07 10:07:33.862 :  HsnUSBManager :  hsn::USBManager::burstWrite (addr : 212, offset : 0, length : 4096)
2024-08-07 10:07:33.863 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : d4 return : 0)
2024-08-07 10:07:33.863 :  HsnUSBManager : hsn::USBManager::gotoBurstWriteMode
2024-08-07 10:07:33.863 :  HsnUSBManager :  hsn::USBManager::burstWrite (return : 4096)
2024-08-07 10:07:33.863 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Mid::LPFTransfer_Sendor::send
2024-08-07 10:07:33.864 :  HsnUSBManager :  hsn::USBManager::burstWrite (addr : 220, offset : 0, length : 4096)
2024-08-07 10:07:33.865 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : dc return : 0)
2024-08-07 10:07:33.865 :  HsnUSBManager : hsn::USBManager::gotoBurstWriteMode
2024-08-07 10:07:33.865 :  HsnUSBManager :  hsn::USBManager::burstWrite (return : 4096)
2024-08-07 10:07:33.865 :  HsnUSBManager :  hsn::USBManager::burstWrite (addr : 221, offset : 0, length : 4096)
2024-08-07 10:07:33.865 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : dd return : 0)
2024-08-07 10:07:33.865 :  HsnUSBManager : hsn::USBManager::gotoBurstWriteMode
2024-08-07 10:07:33.866 :  HsnUSBManager :  hsn::USBManager::burstWrite (return : 4096)
2024-08-07 10:07:33.866 :  HsnUSBManager :  hsn::USBManager::burstWrite (addr : 222, offset : 0, length : 4096)
2024-08-07 10:07:33.866 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7202(29186) src : de return : 0)
2024-08-07 10:07:33.866 :  HsnUSBManager : hsn::USBManager::gotoBurstWriteMode
2024-08-07 10:07:33.867 :  HsnUSBManager :  hsn::USBManager::burstWrite (return : 4096)
2024-08-07 10:07:33.867 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 610b(24843) src : 0 return : 0)
2024-08-07 10:07:33.867 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 610c(24844) src : ffff return : 0)
2024-08-07 10:07:33.867 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 610d(24845) src : 0 return : 0)
2024-08-07 10:07:33.867 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Mid::TruncationTransfer_Sendor::send
2024-08-07 10:07:33.868 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 610f(24847) src : 0 return : 0)
2024-08-07 10:07:33.868 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::PRFTransfer_Sendor::send
2024-08-07 10:07:33.868 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7035(28725) src : f9d return : 0)
2024-08-07 10:07:33.868 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7036(28726) src : 176f return : 0)
2024-08-07 10:07:33.869 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7037(28727) src : 1 return : 0)
2024-08-07 10:07:33.869 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::DEBUGTransfer_Sendor::send
2024-08-07 10:07:33.869 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7005(28677) src : 20 return : 0)
2024-08-07 10:07:33.869 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 0006(6) src : 0 return : 0)
2024-08-07 10:07:33.869 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::ExtendedApertureTransfer_Sendor::send
2024-08-07 10:07:33.870 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::SpatialCompoundingTransfer_Sendor::send
2024-08-07 10:07:33.870 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::DensityTransfer_Sendor::send
2024-08-07 10:07:33.870 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7031(28721) src : 1df return : 0)
2024-08-07 10:07:33.870 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::ViewDepthTransfer_Sendor::send
2024-08-07 10:07:33.870 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 6101(24833) src : 700 return : 0)
2024-08-07 10:07:33.870 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::ModeSelectTransfer_Sendor::send
2024-08-07 10:07:33.870 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::LoadPARAMTransfer_Sendor::send
2024-08-07 10:07:33.871 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7030(28720) src : 1 return : 0)
2024-08-07 10:07:33.871 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7030(28720) src : 0 return : 0)
2024-08-07 10:07:33.873 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::RTC::TIMICalculateTransfer_Sendor::send
2024-08-07 10:07:33.873 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::WAIT::WaitTransfer_Sendor::send
2024-08-07 10:07:33.873 :  I3_ENDDeviceSendor : hsn::I3_END::SENDOR::Buffer_Flush::FlushBufferTransfer_Sendor::send
2024-08-07 10:07:33.873 :  DEVICE_STATE : device_streaming
2024-08-07 10:07:33.873 :  COMMON_EVENT_DEVICE : hsn::common::device_streaming
2024-08-07 10:07:33.873 :  HsnStateMachine : hsn::GeneralStateMachine::set
2024-08-07 10:07:33.874 :  ULTRASOUND_STATE : startTransfer
2024-08-07 10:07:33.874 :  HsnUSBManager : hsn::USBManager::singleWrite (addr : 7003(28675) src : 1 return : 0)
2024-08-07 10:07:33.874 :  HsnUSBManager : hsn::USBManager::gotoBurstReadMode
2024-08-07 10:07:33.874 :  HsnStateMachine : hsn::GeneralStateMachine::set
2024-08-07 10:07:33.874 :  DeviceTransfer_Receiver : hsn::DeviceTransfer_Receiver::startReceiver
2024-08-07 10:07:33.874 :  HsnTask : hsn::Looper::start
2024-08-07 10:07:33.875 :  HsnTask : hsn::Looper::add_to_loop
2024-08-07 10:07:34.885 :  DeviceTransfer_Receiver : SYSTEM INFO LOOPER IS RUNNING!
2024-08-07 10:07:34.885 :  DeviceTransfer_Receiver : hsn::DeviceTransfer_Receiver::run_receive_system_info PCB FIELD(0x31)
2024-08-07 10:07:34.885 :  HsnUSBManager : hsn::USBManager::singleRead (addr : 9308(37640))
2024-08-07 10:07:34.886 :  HsnUSBManager : hsn::USBManager::singleRead : return(0) dat(0)
2024-08-07 10:07:34.886 :  HsnTask : hsn::Looper::start
2024-08-07 10:07:34.886 :  HsnTask : hsn::Looper::add_to_loop

2024-08-07 10:07:36.117 :  HSN_IP_BUFFER_CONTAINER : error code (0x0502) - GL_INVALID_OPERATION
```