using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SonoCap.MES.UI.ViewModels.Base;
using System.Reflection;
using System.Windows;

namespace SonoCap.MES.UI.ViewModels
{
    public partial class AboutViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _appName = @"sonocap mes";

        [ObservableProperty]
        //private string _versionInfo = $"Version: {Assembly.GetExecutingAssembly().GetName().Version} (빌드: {BuildDate})";
        private string _versionInfo = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";

        [ObservableProperty]
        private string _releaseNotes =
    """
    # SonoCap.MES v1.0.1.0
    
    > 버그 수정 및 개선 (2025.07.15)

    ---

    ## 🔧 주요 기능
    - DB, 파일명 멀티 유저 대응
    - 검사 알고리즘 디파인 파일 분리
    ---

    # SonoCap.MES v1.0.0.0

    > 최초 릴리즈 (2025.07.01)

    ---

    ## 🔧 주요 기능

    - 검사 3종 통합 (Gray / Resolution / Geometry)
    - OpenCV 기반 검사 엔진 연동
    - Full-size 이미지 저장 + ROI 기준 검사 결과 적용
    - 점수 기반 검사 결과 정량화 및 기준 점수 적용
    - 검사 결과 → PTRView 집계 및 모듈 단위 통과 처리
    - 강제 통과(ForcePass) 기능 추가
    - 검사 이력 기반 UI 상태 처리 (검사 여부에 따른 버튼 상태)
    - 검사자/PC 자동 선택 및 저장
    - 시리얼 번호 자동 생성 (Probe, TransducerModule 등)

    ---

    ## 🖼 UI/UX

    - WPF MVVM 기반 구조
    - MaterialDesignThemes v5 적용
      - Snackbar, Dialog, Theme 적용
    - MahApps.Metro 일부 적용 (RangeSlider 등)
    - TestingView: 검사용 메인 화면
      - 실시간 상태 표시, 검사 버튼 활성화, 결과 반영
    - AboutView: 버전/릴리즈 정보 팝업
    - TestListView: 검사 이력 목록
      - 검사 결과 필터링, 검색 기능
      - TestView: 검사 상세 정보
    - ProbeListView: 프로브 목록
      - 검사 결과 필터링, 검색 기능
      - ProbeView: 프로브 상세 정보

    ---

    ## 📁 데이터베이스 구조

    - Tests 테이블 정규화 유지
      - TransducerModuleId / TransducerId / ProbeId 중 하나만 존재
    - PTRView: 속도 최적화를 위한 비정규화 집계 테이블
      - 1 Probe = 최대 9개 테스트 연결 (TestId01 ~ 09)
    - TestType / TestCategory / Tester / Pc 등 마스터 테이블 구성

    ---

    ## 💾 저장 구조

    - Full-size 원본 이미지 저장
    - 검사된 ROI만 내용이 다름
    - 검사 메타데이터 JSON 형식으로 저장 (ChangedImgMetadata)

    ---

    ## 🧪 검사 로직 (Core)

    - 검사 함수: GeoProcess, GrayProcess, ResolutionProcess
    - 검사 결과: JSON → InspectionResult 파싱
    - 기준 점수: PassThreshold.Gray, PassThreshold.Resolution 등 외부 주입
    - 선형 감점 방식 적용 (기준값 오차에 따라 점수 감산)

    ---

    ## ⚙ 기타 구현 사항

    - 앱 설정 JSON (ExportExcel 경로 등)
    - 엑셀 내보내기: 검사 결과 → 파일로 저장
    - ViewModel 단일화: 모든 로직을 MVVM 구조에 맞춰 정리
    - CommunityToolkit.Mvvm 적용
    - Logging: Serilog 기반 로그 출력

    ---

    ## 🎨 디자인 기조

    - Framework: MaterialDesignThemes v5 (with Snackbar, DialogHost, Themes)
    - Color: Primary BlueGray, Accent Amber
    - Typography: Roboto 14pt (기본)
    - Style Guide:
      - 모든 메시지 → Snackbar
      - 모든 팝업 → DialogHost
      - 버튼, 슬라이더 → Material Button / RangeSlider(MahApps 일부 혼용)

    ---

    ## 🗒 향후 개선 예정

    - 슬라이더 MaterialDesign으로 완전 교체
    - 테스트 유형별 상세 옵션 UI 분리
    - 성능 최적화 (쿼리 튜닝 / 로딩 지연 처리)
    - 자동화 배포 프로세스 구성 (릴리즈 빌드 자동화)
    """;


        public string BuildDate => "2025.07.29"; // 또는 자동화 가능

        [RelayCommand]
        private void Close()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }
        public AboutViewModel()
        {
            Title = GetType().Name;
        }
    }
}
