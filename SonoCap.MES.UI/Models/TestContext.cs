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
        public GCHandle ImageHandle { get; set; }
        public IntPtr ImageBufferPtr { get; set; }
        public byte[] ResultImageArray { get; set; }
        public GCHandle ResultHandle { get; set; }
        public IntPtr ResultBufferPtr { get; set; }
        public byte[] TextArray { get; set; }
        public GCHandle TextHandle { get; set; }
        public IntPtr TextBufferPtr { get; set; }

        // Action 함수들을 컨텍스트에 포함
        public Action<IntPtr, int, int, IntPtr, IntPtr> ProcessFunction { get; set; }
        public Action<IntPtr, int, int, IntPtr, IntPtr, int> InspectionFunction { get; set; }

        public string ResultText { get; set; }
        public string ChangedImgMetadata { get; set; }
        public int ResultScore { get; set; }
    }
}
