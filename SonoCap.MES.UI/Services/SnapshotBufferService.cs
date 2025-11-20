using SonoCap.MES.UI.Models;
using System.Runtime.InteropServices;

namespace SonoCap.MES.UI.Services
{
    public interface ISnapshotBufferService
    {
        void PinSnapshots(byte[][] src, TestContext ctx);
        void FreePinned(TestContext ctx);
    }

    public class SnapshotBufferService : ISnapshotBufferService
    {
        public void PinSnapshots(byte[][] src, TestContext ctx)
        {
            int imgLen = src[0].Length;
            int textLen = 4096;

            ctx.InputSnapshots = src;
            ctx.ResultSnapshots = new byte[10][];
            ctx.ResultTextArrays = new byte[10][];
            ctx.InputBufferPtrs = new IntPtr[10];
            ctx.ResultBufferPtrs = new IntPtr[10];
            ctx.ResultTextBufferPtrs = new IntPtr[10];

            for (int i = 0; i < 10; i++)
            {
                var h1 = GCHandle.Alloc(src[i], GCHandleType.Pinned);
                ctx.PinnedHandles.Add(h1);
                ctx.InputBufferPtrs[i] = h1.AddrOfPinnedObject();

                ctx.ResultSnapshots[i] = new byte[imgLen];
                var h2 = GCHandle.Alloc(ctx.ResultSnapshots[i], GCHandleType.Pinned);
                ctx.PinnedHandles.Add(h2);
                ctx.ResultBufferPtrs[i] = h2.AddrOfPinnedObject();

                ctx.ResultTextArrays[i] = new byte[textLen];
                var h3 = GCHandle.Alloc(ctx.ResultTextArrays[i], GCHandleType.Pinned);
                ctx.PinnedHandles.Add(h3);
                ctx.ResultTextBufferPtrs[i] = h3.AddrOfPinnedObject();
            }
        }

        public void FreePinned(TestContext ctx)
        {
            foreach (var h in ctx.PinnedHandles)
                if (h.IsAllocated)
                    h.Free();
        }
    }
}
