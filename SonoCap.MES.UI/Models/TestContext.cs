using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.UI.Models
{
    public class TestContext
    {
        public TestTypes TestType { get; set; }
        public TestCategories TestCategory { get; set; }
        public Tester Tester { get; set; }
        public ICollection<string> ResLogs { get; set; }

        public BitmapSource SnapshotImg { get; set; }
        public BitmapSource ResultImg { get; set; }
        public byte[] TextArray { get; set; }

        // Action 함수들을 컨텍스트에 포함
        public Action<IntPtr, int, int, IntPtr, IntPtr> ProcessFunction { get; set; }
        public Action<IntPtr, int, int, IntPtr, IntPtr, int> InspectionFunction { get; set; }

        public string ResultText { get; set; }
        public string ChangedImgMetadata { get; set; }
        public int ResultScore { get; set; }

        // 10개의 원본 이미지 배열 (데이터 보관)
        public byte[][] InputSnapshots { get; set; }
        // 10개의 결과를 받을 이미지 배열
        public byte[][] ResultSnapshots { get; set; }
        // 10개의 결과를 받을 텍스트 배열 [추가]
        public byte[][] ResultTextArrays { get; set; }

        // P/Invoke를 위해 고정된 10개 원본 이미지 포인터
        public IntPtr[] InputBufferPtrs { get; set; }
        // P/Invoke를 위해 고정된 10개 결과 이미지 포인터
        public IntPtr[] ResultBufferPtrs { get; set; }
        // P/Invoke를 위해 고정된 10개 결과 텍스트 포인터 [추가]
        public IntPtr[] ResultTextBufferPtrs { get; set; }

        // [중요] 30개의 GCHandle (입력 10개 + 출력 이미지 10개 + 출력 텍스트 10개)을 저장
        public List<GCHandle> PinnedHandles { get; set; } = new List<GCHandle>();
    }
}
