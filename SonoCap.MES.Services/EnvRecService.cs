using System;
using System.IO;
using Serilog;

namespace SonoCap.MES.Services
{
    public class EnvRecService
    {
        private bool _isRecording = false;
        private FileStream? _recStream;
        private readonly object _lock = new();

        private string? _currentFilePath;
        private long _writtenFrames = 0;

        public bool IsRecording => _isRecording;
        public string? CurrentFilePath => _currentFilePath;
        public long WrittenFrames => _writtenFrames;

        /// <summary>
        /// 녹화를 시작한다. 이미 녹화 중이면 기존 스트림을 닫고 새 파일로 교체한다.
        /// </summary>
        public void Start(string filePath)
        {
            try
            {
                lock (_lock)
                {
                    if (_isRecording)
                        Stop();

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    _recStream = new FileStream(
                        filePath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.Read,
                        bufferSize: 1024 * 1024, // 1MB 버퍼
                        useAsync: false
                    );

                    _isRecording = true;
                    _writtenFrames = 0;
                    _currentFilePath = filePath;

                    Log.Information($"[EnvRecService] Recording started: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[EnvRecService] Failed to start recording");
                _isRecording = false;
            }
        }

        /// <summary>
        /// 16비트 raw buffer (width * height * 2 bytes)를 파일에 이어쓰기
        /// </summary>
        public void AppendRaw(byte[] rawBuffer, int validLength)
        {
            if (!_isRecording || _recStream == null)
                return;

            if (rawBuffer == null || validLength <= 0)
                return;

            try
            {
                lock (_lock)
                {
                    _recStream.Write(rawBuffer, 0, validLength);
                    _writtenFrames++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[EnvRecService] Write failed");
                Stop();
            }
        }

        /// <summary>
        /// 녹화를 종료한다.
        /// </summary>
        public void Stop()
        {
            try
            {
                lock (_lock)
                {
                    if (!_isRecording)
                        return;

                    _recStream?.Flush();
                    _recStream?.Close();
                    _recStream = null;

                    Log.Information($"[EnvRecService] Recording stopped. Frames: {_writtenFrames}, File: {_currentFilePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[EnvRecService] Failed to stop recording");
            }
            finally
            {
                _isRecording = false;
                _currentFilePath = null;
            }
        }
    }
}
