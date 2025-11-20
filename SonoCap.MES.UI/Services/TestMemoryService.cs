using SonoCap.MES.Models.Enums;
using SonoCap.MES.Services;
using SonoCap.MES.UI.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SonoCap.MES.UI.Services
{
    public interface ITestMemoryService
    {
        void Prepare(TestContext ctx, TestTypes type);
        void Release(TestContext ctx);
    }

    public class TestMemoryService : ITestMemoryService
    {
        private readonly ImageBufferService _buffer;

        public TestMemoryService(ImageBufferService bufferService)
        {
            _buffer = bufferService;
        }

        public void Prepare(TestContext ctx, TestTypes type)
        {
            // ★ 1) EnvGeo면 Env 버퍼, 아니면 Src 버퍼 사용
            byte[][] snapshots = (type == TestTypes.EnvGeo)
                ? _buffer.GetEnvSnapshot()
                : _buffer.GetSnapshot();

            if (snapshots == null || snapshots.Length != 10)
                throw new InvalidOperationException("10장 snapshot이 준비되지 않았습니다.");

            ctx.InputSnapshots = snapshots;

            int imageLength = snapshots[0].Length;
            int textLength = 4096;

            // TestContext 내부 버퍼 준비
            ctx.ResultSnapshots = new byte[10][];
            ctx.ResultTextArrays = new byte[10][];
            ctx.InputBufferPtrs = new IntPtr[10];
            ctx.ResultBufferPtrs = new IntPtr[10];
            ctx.ResultTextBufferPtrs = new IntPtr[10];

            if (ctx.PinnedHandles == null)
                ctx.PinnedHandles = new List<GCHandle>();

            // ★ 2) 10장 모두 pin / 포인터 세팅
            for (int i = 0; i < 10; i++)
            {
                // 입력 이미지 pin
                GCHandle srcHandle = GCHandle.Alloc(snapshots[i], GCHandleType.Pinned);
                ctx.PinnedHandles.Add(srcHandle);
                ctx.InputBufferPtrs[i] = srcHandle.AddrOfPinnedObject();

                // 결과 이미지 pin
                ctx.ResultSnapshots[i] = new byte[imageLength];
                GCHandle resImgHandle = GCHandle.Alloc(ctx.ResultSnapshots[i], GCHandleType.Pinned);
                ctx.PinnedHandles.Add(resImgHandle);
                ctx.ResultBufferPtrs[i] = resImgHandle.AddrOfPinnedObject();

                // 결과 텍스트 pin
                ctx.ResultTextArrays[i] = new byte[textLength];
                GCHandle resTxtHandle = GCHandle.Alloc(ctx.ResultTextArrays[i], GCHandleType.Pinned);
                ctx.PinnedHandles.Add(resTxtHandle);
                ctx.ResultTextBufferPtrs[i] = resTxtHandle.AddrOfPinnedObject();
            }
        }

        public void Release(TestContext ctx)
        {
            if (ctx?.PinnedHandles == null)
                return;

            foreach (var h in ctx.PinnedHandles)
            {
                if (h.IsAllocated)
                    h.Free();
            }

            ctx.PinnedHandles.Clear();
        }
    }
}
