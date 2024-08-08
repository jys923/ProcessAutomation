using SonoCap.MES.UI.Commons;

namespace SonoCap.Tests.MES.UI.Commons
{
    public class UtilitiesTests : IDisposable
    {
        private readonly string testFolderPath = Path.Combine(Path.GetTempPath(), "TestFolder");

        public UtilitiesTests()
        {
            //AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            //{
            //    if (Directory.Exists(testFolderPath))
            //    {
            //        Directory.Delete(testFolderPath, true);
            //    }
            //};
        }

        [Fact]
        public void EnsureFolderExists_FolderDoesNotExist_ShouldCreateFolder()
        {
            var folderPath = Path.Combine(testFolderPath, "NewFolder");
            var result = Utilities.EnsureFolderExists(folderPath);
            Assert.True(result);
            Assert.True(Directory.Exists(folderPath));
        }

        [Fact]
        public void EnsureFolderExists_FolderAlreadyExists_ShouldReturnTrue()
        {
            Directory.CreateDirectory(testFolderPath);
            var result = Utilities.EnsureFolderExists(testFolderPath);
            Assert.True(result);
        }

        [Fact]
        public void EnsureFolderExists_WhenExceptionOccurs_ShouldReturnFalse()
        {
            var invalidPath = Path.Combine(testFolderPath, "<>:\"/\\|?*");
            var result = Utilities.EnsureFolderExists(invalidPath);
            Assert.False(result);
        }

        public void Dispose()
        {
            if (Directory.Exists(testFolderPath))
            {
                Directory.Delete(testFolderPath, true);
            }
        }
    }
}
