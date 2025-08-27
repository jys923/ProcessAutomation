using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenRecorderLib;
using Serilog;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Models.Process;
using SonoCap.MES.PreviewUI.ViewModels.Base;
using SonoCap.MES.Services;
using SonoCap.MES.Services.Interfaces;
using SonoCap.MES.Services.Model;
using SonoCap.WpfCommons;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SonoCap.MES.PreviewUI.ViewModels
{
    public partial class PreviewSaveViewModel : ViewModelBase
    {
        private int _frameCounter = 0;
        private const int AnalysisFrameInterval = 10;
        // Fields
        private readonly GlobalModel _model;
        private readonly IMotorService _motorService;
        private USRenderService _usRenderer;
        private double _rotationAngle = 0.0;
        private int _verticalShift = 0;
        //private bool _flipVertical = false;

        // Constructor
        public PreviewSaveViewModel(
            GlobalModel model,
            IMotorService motorService)
        {
            RecorderStatus = RecorderStatus.Idle;
            _model = model;
            _motorService = motorService;

            Title = GetType().Name;
            InitHsn();
            //_ = InitAsync();
        }

        private void InitHsn()
        {
            if (!_model.InitializeLibrary())
            {
                MessageBox.Show("initialize library failure");
                return;
            }

            _model.MotorStateChanged += _motorService.OnMotorStateChanged;
            _model.IpCapsuleIsInnerVisible = true;

            _model.OnInitUI += InitUI;

            //List<Tuple<int, string>> subSettingList = _model.SubSettings;
            //SubSettingList = new ObservableCollection<Tuple<int, string>>(subSettingList);
            //SelectedSubSetting = subSettingList.Find(t => t.Item1 == _model.Subsetting);

            RenderStart();
        }

        private void InitUI()
        {
            ApplicationList = new ObservableCollection<Tuple<int, string>>(_model.Applications);
            SelectedApplication = ApplicationList.FirstOrDefault(x => x.Item1 == _model.Application)
                                 ?? ApplicationList.FirstOrDefault()
                                 ?? new Tuple<int, string>(0, string.Empty);
            Log.Information($"ApplicationList 초기화 완료, SelectedApplication: {SelectedApplication?.Item2 ?? "null"}");

            PresetList = new ObservableCollection<Tuple<int, string>>(_model.Presets);
            SelectedPreset = PresetList.FirstOrDefault(x => x.Item1 == _model.Setting)
                             ?? PresetList.FirstOrDefault()
                             ?? new Tuple<int, string>(0, string.Empty);
            Log.Information($"PresetList 초기화 완료, SelectedPreset: {SelectedPreset?.Item2 ?? "null"}");

            // 허용할 파일명 목록
            var allowList = new HashSet<string>
            {
                "pen.json", "gen.json", "res.json",
                "pen_fh.json", "gen_fh.json", "res_fh.json"
            };

            // 필터링 적용
            var filtered = _model.SubSettings
                .Where(x => allowList.Contains(x.Item2));

            // ObservableCollection 생성
            SubSettingList = new ObservableCollection<Tuple<int, string>>(filtered);
            SelectedSubSetting = SubSettingList.FirstOrDefault(x => x.Item1 == _model.Subsetting)
                                 ?? SubSettingList.FirstOrDefault()
                                 ?? new Tuple<int, string>(0, string.Empty);

            //SrcImg = Utilities.GetFileToImageSource("Resources/usImg.bmp") ?? Utilities.LoadBitmapFromResource("usImg.bmp");

            if (_depthToScanlineMap.TryGetValue(_model.ViewDepthCm, out int scanlineValue))
            {
                _usRenderer?.SetScanline(scanlineValue);
            }

            SelectedViewDepth = _model.ViewDepthCm;
            SelectedLineDensity = _model.LineDensity;
            IPGain = _model.IPGain;
            DRMin = _model.DRMin;
            DRMax = _model.DRMax;
            SelectedPower = _model.TxPower;
        }

        private bool InitMotor()
        {
            //_motorService.CloseViewRequested += CloseViewRequestedHandler;
            if (_motorService.InitializeMotor())
            {
                //_motorService.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);
                Log.Information($"Succ:{nameof(InitMotor)}");
                return true;
            }
            else
            {
                Log.Error($"{nameof(InitMotor)}");
                return false;
            }
        }

        // Observable Properties
        [ObservableProperty] private string _title;
        [ObservableProperty] private string _message;
        [ObservableProperty] private bool _messageIsPopupOpen;
        [ObservableProperty] private int _calibrationOffset;
        [ObservableProperty] private ObservableCollection<Tuple<int, string>> _applicationList;
        [ObservableProperty] private Tuple<int, string> _selectedApplication;
        [ObservableProperty] private ObservableCollection<Tuple<int, string>> _presetList;
        [ObservableProperty] private Tuple<int, string> _selectedPreset;
        [ObservableProperty] private ObservableCollection<Tuple<int, string>> _subSettingList;
        [ObservableProperty] private Tuple<int, string> _selectedSubSetting;
        [ObservableProperty] private bool _isNoiseLogEnabled;
        [ObservableProperty] private bool _isAddLinesEnabled;

        partial void OnIsNoiseLogEnabledChanged(bool value)
        {
            if (!value)
            {
                NoiseLog = string.Empty; // Clear log when disabling
            }
        }

        [ObservableProperty] private string _noiseLog;
        [ObservableProperty] private ImageSource _srcImg;
        [ObservableProperty] private ImageSource _snapshotImg;
        [ObservableProperty] private ImageSource _envImg;

        // SelectedViewDepth 속성이 정의된 클래스 (예: ViewModel 클래스) 내부에 추가
        private static readonly Dictionary<double, int> _depthToScanlineMap = new Dictionary<double, int>
        {
            { 7, 480 },
            { 6, 576 },
            { 5, 720 },
            { 4, 768 },
            { 3, 960 }
        };

        public double SelectedViewDepth
        {
            get => _model.ViewDepthCm;
            set { 
                _model.ViewDepthCm = value;
                if (_depthToScanlineMap.TryGetValue(value, out int scanlineValue))
                {
                    _usRenderer?.SetScanline(scanlineValue);
                }
                else
                {
                    Log.Warning($"Warning: No scanline mapping found for depth: {value}");
                    // _usRenderer?.SetScanline(기본_값_또는_계산된_값);
                }
                OnPropertyChanged(); }
        }

        public int SelectedLineDensity
        {
            get => _model.LineDensity;
            set { _model.LineDensity = value; OnPropertyChanged(); }
        }

        public double SelectedPower
        {
            get => _model.TxPower;
            set { _model.TxPower = value; OnPropertyChanged(); }
        }

        public int IPGain
        {
            get => _model.IPGain;
            set { _model.IPGain = value; OnPropertyChanged(); }
        }

        public float DRMin
        {
            get => _model.DRMin;
            set { if (value <= DRMax - 2) { 
                    _model.DRMin = value;
                    _usRenderer.DRMin = value;
                    OnPropertyChanged(); } }
        }

        public float DRMax
        {
            get => _model.DRMax;
            set { if (value >= DRMin + 2) { 
                    _model.DRMax = value;
                    _usRenderer.DRMax = value;
                    OnPropertyChanged(); } }
        }

        partial void OnSelectedApplicationChanged(Tuple<int, string> value)
        {
            if (value == null) return;
            if (_model.Application != value.Item1)
            {
                _model.Application = value.Item1;
                InitUI();
            }
        }

        partial void OnSelectedPresetChanged(Tuple<int, string> value)
        {
            if (value == null) return;
            if (_model.Setting != value.Item1)
            {
                _model.Setting = value.Item1;
                InitUI();
            }
        }

        // Events
        partial void OnSelectedSubSettingChanged(Tuple<int, string> value)
        {
            if (value == null) return;
            if (_model.Subsetting != value.Item1)
            {
                _model.Subsetting = value.Item1;
                InitUI();
            }
        }

        [RelayCommand]
        public void KeyDown(KeyEventArgs keyEventArgs)
        {
            Key key = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            Log.Information($"{nameof(KeyDown)} key: {key}");

            if (key == Key.Left)
            {
                _rotationAngle = (_rotationAngle - 1 + 360) % 360;
                _usRenderer?.SetRotationAngle(_rotationAngle);
                _usRenderer?.SetVerticalShift((int)_rotationAngle);
                Log.Information($"[Rotate] angle → {_rotationAngle}° (←)");
                keyEventArgs.Handled = true;
            }
            else if (key == Key.Right)
            {
                _rotationAngle = (_rotationAngle + 1) % 360;
                _usRenderer?.SetRotationAngle(_rotationAngle);
                _usRenderer?.SetVerticalShift((int)_rotationAngle);
                Log.Information($"[Rotate] angle → {_rotationAngle}° (→)");
                keyEventArgs.Handled = true;
            }
            else if (key == Key.Up)
            {
                _usRenderer?.SetVerticalFlip(true);
                Log.Information($"[Flip Vertical] → true (↑)");
                keyEventArgs.Handled = true;
            }
            else if (key == Key.Down)
            {
                _usRenderer?.SetVerticalFlip(false);
                Log.Information($"[Flip Vertical] → false (↓)");
                keyEventArgs.Handled = true;
            }
        }

        // Commands
        [RelayCommand]
        private void CalibrateButton()
        {
            //_model.CalibrateScanline(CalibrationOffset);

        }

        [RelayCommand]
        private void Capture()
        {
            // SnapshotImg에 원본 SrcImg를 복사합니다.
            SnapshotImg = Utilities.CopyBitmapSource((BitmapSource)SrcImg);

            string sn = string.IsNullOrWhiteSpace(SerialNumber) ? "NO_SN" : SerialNumber.Trim();
            string prefix = $"{sn}_{DateTime.Now:yyyyMMdd_HHmmss}";

            // 그레이스케일 이미지로 변환하거나 원본 이미지를 사용합니다.
            BitmapSource grayBitmap = (BitmapSource)SnapshotImg;

            if (IsNoiseLogEnabled && !string.IsNullOrEmpty(_formattedResult))
            {
                // 이미지에 텍스트를 추가합니다.
                grayBitmap = AddTextToBitmap(
                    grayBitmap,
                    _formattedResult,
                    new Point(10, 10), // 텍스트 위치 (왼쪽 상단)
                    16,                // 폰트 크기
                    Brushes.White    // 텍스트 색상
                );
            }

            if (IsAddLinesEnabled)
            {
                // 선을 추가하는 새로운 비트맵을 만듭니다.
                grayBitmap = AddLinesToBitmap(grayBitmap);
            }

            string path = Utilities.BuildPath(App.appSettings.Path.ExportImg, prefix, "bmp");
            Utilities.SaveBitmap(grayBitmap, path);

            // 환경 이미지도 저장합니다.
            string envPath = Utilities.BuildPath(App.appSettings.Path.ExportImg, prefix + "_env", "bmp");
            Utilities.SaveBitmap((BitmapSource)EnvImg, envPath);

            ShowSnackbarWithOpen(path);
        }

        public BitmapSource AddTextToBitmap(BitmapSource originalBitmap, string textToAdd, Point position, double fontSize, SolidColorBrush textColor)
        {
            // 이미지 그리기 작업에 사용할 DrawingVisual과 DrawingContext를 생성합니다.
            var drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                // 원본 비트맵을 그립니다.
                dc.DrawImage(originalBitmap, new Rect(0, 0, originalBitmap.PixelWidth, originalBitmap.PixelHeight));

                // 텍스트를 그립니다.
                // FormattedText를 사용하여 텍스트의 서식을 지정합니다.
                FormattedText formattedText = new FormattedText(
                    textToAdd,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), // 폰트 지정 (예: Arial)
                    fontSize,              // 폰트 크기 지정
                    textColor,             // 텍스트 색상 지정
                    VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip // DPI 스케일링
                );

                // 지정된 위치에 텍스트를 그립니다.
                dc.DrawText(formattedText, position);
            }

            // DrawingVisual의 내용을 RenderTargetBitmap으로 렌더링하여 최종 BitmapSource를 생성합니다.
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)originalBitmap.PixelWidth,
                (int)originalBitmap.PixelHeight,
                originalBitmap.DpiX,
                originalBitmap.DpiY,
                PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            return renderTargetBitmap;
        }

        /// <summary>
        /// 주어진 BitmapSource에 대각선, 가로, 세로 선을 추가합니다.
        /// </summary>
        /// <param name="originalBitmap">선을 추가할 원본 BitmapSource입니다.</param>
        /// <returns>선이 그려진 새로운 BitmapSource를 반환합니다.</returns>
        private BitmapSource AddLinesToBitmap(BitmapSource originalBitmap)
        {
            // WriteableBitmap으로 변환하여 수정 가능하게 함
            var writableBitmap = new WriteableBitmap(originalBitmap);

            // 이미지 그리기 작업에 사용할 DrawingVisual과 DrawingContext를 생성합니다.
            var drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawImage(originalBitmap, new Rect(0, 0, originalBitmap.PixelWidth, originalBitmap.PixelHeight));
                // 1픽셀 두께의 빨간색 펜으로 선 스타일을 정의합니다.
                Pen linePen = new Pen(Brushes.Tomato, 1.0);

                // 이미지의 가로/세로 길이를 가져옵니다.
                double width = writableBitmap.Width;
                double height = writableBitmap.Height;

                // 중앙 가로선: 이미지 중앙 (0, height/2) -> (width, height/2)
                dc.DrawLine(linePen, new Point(0, height / 2), new Point(width, height / 2));

                // 중앙 세로선: 이미지 중앙 (width/2, 0) -> (width/2, height)
                dc.DrawLine(linePen, new Point(width / 2, 0), new Point(width / 2, height));

                // 두 개의 대각선
                // 대각선 1: 왼쪽 위 (0, 0) -> 오른쪽 아래 (width, height)
                dc.DrawLine(linePen, new Point(0, 0), new Point(width, height));

                // 대각선 2: 오른쪽 위 (width, 0) -> 왼쪽 아래 (0, height)
                dc.DrawLine(linePen, new Point(width, 0), new Point(0, height));
            }

            // DrawingVisual의 내용을 RenderTargetBitmap으로 렌더링하여 최종 BitmapSource를 생성합니다.
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)writableBitmap.Width,
                (int)writableBitmap.Height,
                writableBitmap.DpiX,
                writableBitmap.DpiY,
                PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            return renderTargetBitmap;
        }

        private Recorder? _rec;
        private Stream? _outputStream;
        private string? _tempVideoPath;
        private string? _finalVideoPath;

        private DispatcherTimer? _recordingTimer;
        private DateTimeOffset? _recordingStartTime;
        private DateTimeOffset? _recordingPauseTime;

        [ObservableProperty]
        private string _serialNumber = "SN00001"; // 기본값

        [ObservableProperty]
        private string _recordingTimeText = "00:00:00";

        [RelayCommand]
        private void Record()
        {
            if (_rec != null && _rec.Status == RecorderStatus.Paused)
            {
                _rec.Resume();
                return;
            }

            if (_rec?.Status == RecorderStatus.Recording)
            {
                // 이미 녹화 중인 경우 일시정지 처리
                _rec.Pause();
                return;
            }

            var hwnd = new WindowInteropHelper(Application.Current.Windows
                .OfType<Window>().FirstOrDefault(w => w.DataContext == this) ?? throw new Exception("녹화할 창을 찾을 수 없습니다.")).Handle;

            var windowSource = Recorder.GetWindows().FirstOrDefault(w => w.Handle == hwnd);
            if (windowSource == null)
            {
                MessageBox.Show("녹화할 창을 찾을 수 없습니다.");
                return;
            }

            var source = new WindowRecordingSource(windowSource);

            // 1. 임시 저장 경로
            string tempDir = Path.Combine(Path.GetTempPath(), "SonoCapRecorderTemp");
            Directory.CreateDirectory(tempDir);
            _tempVideoPath = Path.Combine(tempDir, $"{Guid.NewGuid()}.mp4");

            // 2. 최종 저장 경로
            //_finalVideoPath = Utilities.BuildPath(App.appSettings.Path.ExportVideo, "screen", "mp4");
            //_finalVideoPath = Utilities.BuildPath(App.appSettings.Path.ExportVideo,$"{SerialNumber.Trim()}_{DateTime.Now:yyyyMMdd_HHmmss}","mp4");
            string sn = string.IsNullOrWhiteSpace(SerialNumber) ? "NO_SN" : SerialNumber.Trim();
            string prefix = $"{sn}_{DateTime.Now:yyyyMMdd_HHmmss}";
            _finalVideoPath = Utilities.BuildPath(App.appSettings.Path.ExportVideo, prefix, "mp4");
            //_finalVideoPath = Utilities.BuildPath("./video/", prefix, "mp4");

            // 비디오 인코더 설정
            IVideoEncoder videoEncoder = new H264VideoEncoder
            {
                BitrateMode = H264BitrateControlMode.Quality,
                EncoderProfile = H264Profile.Main
            };

            // RecorderOptions 설정
            var options = RecorderOptions.Default;
            options.AudioOptions.IsAudioEnabled = false;
            options.SourceOptions.RecordingSources = new List<RecordingSourceBase> { source };
            options.OutputOptions.RecorderMode = RecorderMode.Video;
            options.VideoEncoderOptions.Encoder = videoEncoder;

            // Recorder 인스턴스 준비
            if (_rec == null)
            {
                _rec = Recorder.CreateRecorder(options);
                _rec.OnRecordingComplete += Rec_OnRecordingComplete;
                _rec.OnRecordingFailed += Rec_OnRecordingFailed;
                _rec.OnStatusChanged += Rec_OnStatusChanged;
            }
            else
            {
                _rec.SetOptions(options);
            }

            _outputStream = new FileStream(_tempVideoPath, FileMode.Create);
            _rec.Record(_outputStream);

            _recordingStartTime = DateTimeOffset.Now;
            _recordingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _recordingTimer.Tick += (s, e) => UpdateRecordingTime();
            _recordingTimer.Start();

        }

        private void UpdateRecordingTime()
        {
            if (_recordingStartTime == null) return;

            TimeSpan elapsed = DateTimeOffset.Now - _recordingStartTime.Value;
            RecordingTimeText = elapsed.ToString(@"hh\:mm\:ss");
        }

        [RelayCommand]
        private void Stop()
        {
            _rec?.Stop();
            _recordingTimer?.Stop();
            _recordingTimer = null;
            _recordingPauseTime = null;
        }


        // Public Methods
        public void RenderStart()
        {
            _usRenderer = new USRenderService(512, 512);
            _usRenderer.connectRenderToTargetFunction(UpdateImageSource, UpdateEnvImageSource);
            _usRenderer.RenderStart();
        }

        public void RenderEnd()
        {
            _usRenderer?.RenderEnd();
        }

        // Private Helpers
        private void UpdateImageSource(BitmapSource bitmapSource)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                SrcImg = bitmapSource;

                // 10프레임마다 한 번 분석 수행
                _frameCounter++;
                if (
                    IsNoiseLogEnabled && 
                    _frameCounter % AnalysisFrameInterval == 0)
                {
                    try
                    {
                        int width = bitmapSource.PixelWidth;
                        int height = bitmapSource.PixelHeight;

                        int stride = width * 4; // Assume PixelFormat is BGRA32
                        byte[] pixels = new byte[height * stride];
                        bitmapSource.CopyPixels(pixels, stride, 0);

                        // unmanaged 메모리로 복사
                        IntPtr buffer = Marshal.AllocHGlobal(pixels.Length);
                        Marshal.Copy(pixels, 0, buffer, pixels.Length);

                        // 결과를 받을 버퍼
                        IntPtr textBuffer = Marshal.AllocHGlobal(4096); // 충분히 큰 버퍼

                        // 품질 분석 호출
                        MyOpenCVWrapper.OpenCVWrapper.AnalyzeBrightness(buffer, width, height, textBuffer);
                        //MyOpenCVWrapper.OpenCVWrapper.AnalyzeFFT(buffer, width, height, textBuffer);

                        // 결과 문자열로 변환
                        string jsonString = Marshal.PtrToStringAnsi(textBuffer) ?? "";

                        QualityMetrics? metrics = JsonSerializer.Deserialize<QualityMetrics>(jsonString);

                        // metrics.Brightness의 값과 이름을 모두 사용
                        //_formattedResult = $"{nameof(metrics.Brightness)}:{metrics.Brightness:F2}";
                        _formattedResult = $"Noise Br:{metrics.Brightness:F2}";

                        // 로그 갱신
                        NoiseLog = _formattedResult;

                        // 메모리 해제
                        Marshal.FreeHGlobal(buffer);
                        Marshal.FreeHGlobal(textBuffer);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"영상 분석 실패: {ex.Message}");
                    }
                }
            });
        }

        int cnt = 0;

        public void UpdateEnvImageSource(BitmapSource bitmapSource)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                EnvImg = bitmapSource;
            });
        }

        private void Rec_OnRecordingComplete(object? sender, RecordingCompleteEventArgs e)
        {
            try
            {
                _outputStream?.Dispose();

                if (_tempVideoPath != null && _finalVideoPath != null)
                {
                    File.Move(_tempVideoPath, _finalVideoPath, overwrite: true);
                    ShowSnackbarWithOpen(_finalVideoPath);
                }

                RecordingTimeText = "00:00:00"; // ✅ 녹화 시간 초기화
                _recordingStartTime = null;

                CleanupTempDirectory();
            }
            catch (Exception ex)
            {
                ShowSnackbar($"녹화 파일 이동 실패: {ex.Message}");
            }
        }

        private void CleanupTempDirectory()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "SonoCapRecorderTemp");

            if (!Directory.Exists(tempDir))
                return;

            try
            {
                foreach (var file in Directory.GetFiles(tempDir, "*.mp4"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Temp 파일 삭제 실패: {file}, 이유: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"Temp 디렉토리 정리 중 오류: {ex.Message}");
            }
        }

        private void Rec_OnRecordingFailed(object? sender, RecordingFailedEventArgs e)
        {
            Log.Information($"녹화 실패: {e.Error}");
            RecordingTimeText = "00:00:00"; // 실패 시 초기화
            _recordingStartTime = null;
        }

        private void Rec_OnStatusChanged(object? sender, RecordingStatusEventArgs e)
        {
            Log.Information($"녹화 상태: {e.Status}");
            RecorderStatus = e.Status; // ✅ 상태 반영

            if (e.Status == RecorderStatus.Paused)
            {
                _recordingPauseTime = DateTimeOffset.Now;
                _recordingTimer?.Stop();
            }
            else if (e.Status == RecorderStatus.Recording && _recordingPauseTime != null)
            {
                _recordingStartTime = _recordingStartTime?.AddTicks((DateTimeOffset.Now - _recordingPauseTime.Value).Ticks);
                _recordingPauseTime = null;
                _recordingTimer?.Start();
            }
        }

        private void Rec_OnFrameRecorded(object? sender, FrameRecordedEventArgs e)
        {
            Log.Information($"녹화 이미지 상태: {e.FrameNumber}");
        }

        private RecorderStatus _recorderStatus = RecorderStatus.Idle;
        public RecorderStatus RecorderStatus
        {
            get => _recorderStatus;
            set
            {
                SetProperty(ref _recorderStatus, value);
                UpdateRecordingUI();
            }
        }

        [ObservableProperty] private string _recordButtonText = "⏺";
        [ObservableProperty] private bool _isRecordEnabled = true;
        [ObservableProperty] private bool _isStopEnabled = false;
        private string _formattedResult;

        private void UpdateRecordingUI()
        {
            switch (RecorderStatus)
            {
                case RecorderStatus.Idle:
                    RecordButtonText = "⏺";
                    IsRecordEnabled = true;
                    IsStopEnabled = false;
                    break;

                case RecorderStatus.Recording:
                    RecordButtonText = "⏸";
                    IsRecordEnabled = true;
                    IsStopEnabled = true;
                    break;

                case RecorderStatus.Paused:
                    RecordButtonText = "▶";
                    IsRecordEnabled = true;
                    IsStopEnabled = true;
                    break;

                case RecorderStatus.Finishing:
                    RecordButtonText = "⏳";
                    IsRecordEnabled = false;
                    IsStopEnabled = false;
                    break;
            }
        }

        private void CloseWindow()
        {
            //App.Current.MainWindow.Close();
            //Application.Current.Shutdown()
            //Environment.Exit(1);

            Application.Current.Dispatcher.Invoke(() =>
            {
                //Window? focusedWindow = System.Windows.Input.Keyboard.FocusedElement as Window;
                //focusedWindow?.Close();
                //Controls.MessageBox.Show("Get Video Fail", $"Open Video App First");
                var windows = Application.Current.Windows.OfType<Window>();
                var window = windows.FirstOrDefault(w => w.DataContext == this);
                window?.Close();
            });
        }
        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Log.Information($"{nameof(OnWindowLoaded)}");
            //Init();
            //LogIn();

            if (!InitMotor())
            {
                CloseWindow();
            }
            else
            {
                _motorService.StartMotor();
                Task.Delay(100);
            }
        }

        protected override void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            Log.Information($"{nameof(OnWindowClosing)}");
            _motorService.StopMotor();
            Task.Delay(100);
            e.Cancel = true;
            if (sender is Window window)
            {
                window.Hide();
            }
        }

        protected override void OnWindowActivated(object? sender, EventArgs e)
        {
            Log.Information($"{nameof(OnWindowActivated)}");
            _motorService.StartMotor();
            Task.Delay(100);
        }
    }
}