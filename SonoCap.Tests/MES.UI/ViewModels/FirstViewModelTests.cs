using System.Threading.Tasks;
using Moq;
using Xunit;
using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Interfaces;
using SonoCap.MES.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Enums;
using System.Windows.Media.Imaging;
using SonoCap.MES.UI.Services.Interfaces;

namespace SonoCap.MES.UI.ViewModels.Tests
{
    public class FirstViewModelTests : IDisposable
    {
        private readonly Mock<IExcelService> _mockExcelService;
        private readonly Mock<IViewService> _mockViewService;
        private readonly Mock<IMotorModuleRepository> _mockMotorModuleRepository;
        private readonly Mock<IPcRepository> _mockPcRepository;
        private readonly Mock<ITestCategoryRepository> _mockTestCategoryRepository;
        private readonly Mock<ITestTypeRepository> _mockTestTypeRepository;
        private readonly Mock<ITransducerRepository> _mockTransducerRepository;
        private readonly Mock<ITransducerTypeRepository> _mockTransducerTypeRepository;
        private readonly FirstViewModel _viewModel;

        public FirstViewModelTests()
        {
            _mockExcelService = new Mock<IExcelService>();
            _mockViewService = new Mock<IViewService>();
            _mockMotorModuleRepository = new Mock<IMotorModuleRepository>();
            _mockPcRepository = new Mock<IPcRepository>();
            _mockTestCategoryRepository = new Mock<ITestCategoryRepository>();
            _mockTestTypeRepository = new Mock<ITestTypeRepository>();
            _mockTransducerRepository = new Mock<ITransducerRepository>();
            _mockTransducerTypeRepository = new Mock<ITransducerTypeRepository>();

            _viewModel = new FirstViewModel(
                _mockExcelService.Object,
                _mockViewService.Object,
                _mockMotorModuleRepository.Object,
                _mockPcRepository.Object,
                _mockTestCategoryRepository.Object,
                _mockTestTypeRepository.Object,
                _mockTransducerRepository.Object,
                _mockTransducerTypeRepository.Object
            );
        }

        public void Dispose()
        {
            // 자원 해제 관련 코드
        }


        [Fact]
        public void TitleProperty_ShouldRaisePropertyChanged()
        {
            bool propertyChanged = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Title))
                {
                    propertyChanged = true;
                }
            };

            _viewModel.Title = "New Title";
            Assert.True(propertyChanged);
        }

        [Fact]
        public void LogoProperty_ShouldSetDefaultLogo()
        {
            BitmapImage expectedLogo = new BitmapImage();
            _viewModel.Logo = expectedLogo;
            Assert.Equal(expectedLogo, _viewModel.Logo);
        }

        [Fact]
        public void GoTestingView_ShouldCallViewService()
        {
            _viewModel.GoTestingViewCommand.Execute(null);
            _mockViewService.Verify(v => v.ShowTestingView(It.IsAny<SubData>()), Times.Once);
        }

        [Fact]
        public void GoTestListView_ShouldCallViewService()
        {
            _viewModel.GoTestListViewCommand.Execute(null);
            _mockViewService.Verify(v => v.ShowTestListView(), Times.Once);
        }

        [Fact]
        public void GoProbeListView_ShouldCallViewService()
        {
            _viewModel.GoProbeListViewCommand.Execute(null);
            _mockViewService.Verify(v => v.ShowProbeListView(), Times.Once);
        }

        [Fact]
        public async Task AddMasterDataAsync_ShouldInsertData()
        {
            // Act
            await _viewModel.AddMasterDataCommand.ExecuteAsync(null);

            // Assert
            _mockPcRepository.Verify(r => r.InsertAsync(It.Is<Pc>(pc => pc.Name == "left")), Times.Once);
            _mockPcRepository.Verify(r => r.InsertAsync(It.Is<Pc>(pc => pc.Name == "middle")), Times.Once);
            _mockPcRepository.Verify(r => r.InsertAsync(It.Is<Pc>(pc => pc.Name == "right")), Times.Once);
            _mockTestCategoryRepository.Verify(r => r.InsertAsync(It.Is<TestCategory>(tc => tc.Name == TestCategoriesKor.공정용.ToString())), Times.Once);
            _mockTestCategoryRepository.Verify(r => r.InsertAsync(It.Is<TestCategory>(tc => tc.Name == TestCategoriesKor.최종용.ToString())), Times.Once);
            _mockTestCategoryRepository.Verify(r => r.InsertAsync(It.Is<TestCategory>(tc => tc.Name == TestCategoriesKor.출하용.ToString())), Times.Once);
            _mockTestTypeRepository.Verify(r => r.InsertAsync(It.Is<TestType>(tt => tt.Name == "Align")), Times.Once);
            _mockTestTypeRepository.Verify(r => r.InsertAsync(It.Is<TestType>(tt => tt.Name == "Axial")), Times.Once);
            _mockTestTypeRepository.Verify(r => r.InsertAsync(It.Is<TestType>(tt => tt.Name == "Lateral")), Times.Once);
            _mockTransducerTypeRepository.Verify(r => r.InsertAsync(It.Is<TransducerType>(tt => tt.Code == TransducerTypes.SCP01.ToString() && tt.Type == "5Mhz")), Times.Once);
            _mockTransducerTypeRepository.Verify(r => r.InsertAsync(It.Is<TransducerType>(tt => tt.Code == TransducerTypes.SCP02.ToString() && tt.Type == "7.5Mhz")), Times.Once);
        }

        //[Fact]
        public async Task ImportTDExcelAsync_ShouldImportData()
        {
            // Arrange
            var transducers = new List<Transducer>
            {
                new Transducer { Sn = "123" },
                new Transducer { Sn = "456" }
            };

            _mockTransducerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(transducers);

            var fileData = new Dictionary<string, List<SnDate>>
            {
                { "TDSn", new List<SnDate> { new SnDate { Sn = "789" }, new SnDate { Sn = "123" } } }
            };

            _mockExcelService.Setup(s => s.ReadColumnsDataByHeaders(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(fileData);

            // Act
            await _viewModel.ImportTDExcelCommand.ExecuteAsync(null);

            // Assert
            _mockTransducerRepository.Verify(r => r.BulkInsertAsync(It.Is<List<Transducer>>(t => t.Count == 1 && t[0].Sn == "789")), Times.Once);
        }

        //[Fact]
        public async Task ImportMTExcelAsync_ShouldImportData()
        {
            // Arrange
            var motorModules = new List<MotorModule>
            {
                new MotorModule { Sn = "123" },
                new MotorModule { Sn = "456" }
            };

            _mockMotorModuleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(motorModules);

            var fileData = new Dictionary<string, List<SnDate>>
            {
                { "MTLot", new List<SnDate> { new SnDate { Sn = "789" }, new SnDate { Sn = "123" } } }
            };

            _mockExcelService.Setup(s => s.ReadColumnsDataByHeaders(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(fileData);

            // Act
            await _viewModel.ImportMTExcelCommand.ExecuteAsync(null);
            //await _viewModel.ImportMTExcelAsync();

            // Assert
            _mockMotorModuleRepository.Verify(r => r.BulkInsertAsync(It.Is<List<MotorModule>>(m => m.Count == 1 && m[0].Sn == "789")), Times.Once);
        }
    }

}