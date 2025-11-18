using SonoCap.MES.UI.Models;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.UI.Services
{
    //public interface ITestPipelineService
    //{
    //    TestContext Prepare(TestCellDefinition cell, BitmapSource snapshot, byte[][] snapshots);
    //    void Finalize(TestContext ctx);
    //}

    //public class TestPipelineService : ITestPipelineService
    //{
    //    private readonly ISnapshotBufferService _snapshots;
    //    private readonly ITestImageFileService _images;

    //    public TestPipelineService(
    //        ISnapshotBufferService snapshots,
    //        ITestImageFileService images)
    //    {
    //        _snapshots = snapshots;
    //        _images = images;
    //    }

    //    public TestContext Prepare(TestCellDefinition cell, BitmapSource snapshot, byte[][] snapshots)
    //    {
    //        var ctx = new TestContext
    //        {
    //            Category = cell.Category,
    //            TestType = cell.Type,
    //            SnapshotImg = snapshot
    //        };

    //        _snapshots.PinSnapshots(snapshots, ctx);
    //        return ctx;
    //    }

    //    public void Finalize(TestContext ctx)
    //    {
    //        _snapshots.FreePinned(ctx);
    //    }
    //}
}
