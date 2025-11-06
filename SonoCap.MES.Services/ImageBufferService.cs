using Serilog;
using SonoCap.MES.Models;
using SonoCap.WpfCommons;

namespace SonoCap.MES.Services
{
    public class ImageBufferService// : IImageBufferService
    {
        // 1. 일반(Src) 이미지를 위한 원형 큐 (10장)
        private readonly ThreadSafeCircularBuffer<byte[]> _srcImageBuffer =
            new ThreadSafeCircularBuffer<byte[]>(capacity: 10);

        // 2. Env 이미지를 위한 원형 큐 (Env 데이터도 10장이 필요하다면)
        private readonly ThreadSafeCircularBuffer<byte[]> _envImageBuffer =
            new ThreadSafeCircularBuffer<byte[]>(capacity: 10);

        // --- Src 이미지 메서드 (기존) ---
        public void AddImage(byte[] imageData)
        {
            _srcImageBuffer.Put(imageData);
            //byte[][] currentSnapshot = _srcImageBuffer.ToArrayInOrder();

            //var formattedChecksums = currentSnapshot
            //    .Select(data =>
            //    {
            //        long checksum = Utilities.CalculateChecksum(data); // 기존 체크섬 계산 함수 사용
            //        return checksum.ToString().PadLeft(10, ' ');
            //    })
            //    .ToArray();

            //string logMessage = string.Join(" | ", formattedChecksums);

            //Log.Debug("Src Buffer State: | {LogMessage} | Count: {Count}", logMessage, _srcImageBuffer.Count);
        }

        public byte[][] GetSnapshot()
        {
            return _srcImageBuffer.ToArrayInOrder();
        }

        // --- Env 이미지 메서드 (추가) ---

        /// <summary>
        /// Env 이미지 버퍼에 데이터를 추가합니다.
        /// </summary>
        public void AddEnvImage(byte[] envImageData)
        {
            _envImageBuffer.Put(envImageData);
            //Log.Debug("Env image buffer. Env image count: {Count}", _envImageBuffer.Count);
            byte[][] currentSnapshot = _srcImageBuffer.ToArrayInOrder();

            //var formattedChecksums = currentSnapshot
            //    .Select(data =>
            //    {
            //        long checksum = Utilities.CalculateChecksum(data); // 기존 체크섬 계산 함수 사용
            //        return checksum.ToString().PadLeft(10, ' ');
            //    })
            //    .ToArray();

            //string logMessage = string.Join(" | ", formattedChecksums);

            //// 5. 로그 출력
            //Log.Debug("Env Buffer State: | {LogMessage} | Count: {Count}", logMessage, _srcImageBuffer.Count);
        }

        /// <summary>
        /// Env 이미지 버퍼의 스냅샷(10장 복사본)을 반환합니다.
        /// </summary>
        public byte[][] GetEnvSnapshot()
        {
            return _envImageBuffer.ToArrayInOrder();
        }

        public int GetSrcImageCount()
        {
            return _srcImageBuffer.Count;
        }

        public int GetEnvImageCount()
        {
            return _envImageBuffer.Count;
        }
    }
}