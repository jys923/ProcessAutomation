using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SonoCap.MES.Models.Converts;
using SonoCap.MES.Models;

namespace SonoCap.Tests.MES.Models.Converts
{
    public class PTRViewToProbeTestResultTests
    {
        [Fact]
        public async Task ToListAsync_WithValidInput_ReturnsExpectedResults()
        {
            // Arrange
            var ptrViews = new List<PTRView>
            {
                new PTRView
                {
                    Id = 1,
                    ProbeSn = "ABC123",
                    CreatedDate = DateTime.UtcNow,
                    TransducerModuleSn = "TM123",
                    TransducerSn = "T123",
                    MotorModuleSn = "MM123",
                    TestId01 = 1,
                    TestId02 = 2,
                    TestId03 = 3,
                    TestId04 = 4,
                    TestId05 = 5,
                    TestId06 = 6,
                    Test01 = new Test { TestCategoryId = 1, TestTypeId = 1, CreatedDate = DateTime.UtcNow, Result = 1 },
                    Test02 = new Test { TestCategoryId = 2, TestTypeId = 2, CreatedDate = DateTime.UtcNow, Result = 1 },
                    Test03 = new Test { TestCategoryId = 3, TestTypeId = 3, CreatedDate = DateTime.UtcNow, Result = 1 },
                    Test04 = new Test { TestCategoryId = 4, TestTypeId = 4, CreatedDate = DateTime.UtcNow, Result = 1 },
                    Test05 = new Test { TestCategoryId = 5, TestTypeId = 5, CreatedDate = DateTime.UtcNow, Result = 1 },
                    Test06 = new Test { TestCategoryId = 6, TestTypeId = 6, CreatedDate = DateTime.UtcNow, Result = 1 }
                    // 필요한 경우 다른 Test 속성도 추가
                },
                // 필요한 경우 더 많은 PTRView 객체를 추가
            };

            // Act
            var result = await PTRViewToProbeTestResult.ToListAsync(ptrViews);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ptrViews.Count, result.Count());

            var firstResult = result.First();
            Assert.Equal(ptrViews[0].Id, firstResult.Id);
            Assert.Equal(ptrViews[0].ProbeSn, firstResult.ProbeSn);
            Assert.Equal(ptrViews[0].CreatedDate, firstResult.CreatedDate);
            Assert.Equal(ptrViews[0].TransducerModuleSn, firstResult.TransducerModuleSn);
            Assert.Equal(ptrViews[0].TransducerSn, firstResult.TransducerSn);
            Assert.Equal(ptrViews[0].MotorModuleSn, firstResult.MotorModuleSn);
            Assert.Equal(ptrViews[0].Test01.TestCategoryId, firstResult.TestCategoryId1);
            Assert.Equal(ptrViews[0].Test01.TestTypeId, firstResult.TestTypeId1);
            Assert.Equal(ptrViews[0].Test01.CreatedDate, firstResult.TestCreatedDate1);
            Assert.Equal(ptrViews[0].Test01.Result, firstResult.TestResult1);
        }

        [Fact]
        public async Task ToListAsync_WithNullInput_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => PTRViewToProbeTestResult.ToListAsync(null));
        }
    }

}
