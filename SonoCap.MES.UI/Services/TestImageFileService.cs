using SonoCap.WpfCommons;
using System.IO;
using System.Windows.Media.Imaging;

namespace SonoCap.MES.UI.Services
{
    public interface ITestImageFileService
    {
        string SaveOriginal(BitmapSource src, string tempDir);
        string SaveResult(BitmapSource result, string tempDir);
        void MoveToExport(string tempFile, string exportDir, string newName);
    }

    public class TestImageFileService : ITestImageFileService
    {
        public string SaveOriginal(BitmapSource src, string tempDir)
        {
            long epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            string path = Path.Combine(tempDir, $"{epoch}_ori.bmp");
            Utilities.SaveBitmap(src, path);
            return path;
        }

        public string SaveResult(BitmapSource src, string tempDir)
        {
            long epoch = Utilities.GetCurrentUnixTimestampMilliseconds();
            string path = Path.Combine(tempDir, $"{epoch}_det.png");
            Utilities.SavePng(src, path);
            return path;
        }

        public void MoveToExport(string tempFile, string exportDir, string newName)
        {
            Utilities.MoveTempImageToExport(
                Path.GetFileName(tempFile),
                Path.GetDirectoryName(tempFile)!,
                exportDir,
                newName
            );
        }
    }
}
