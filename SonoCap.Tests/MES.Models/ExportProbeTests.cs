namespace SonoCap.MES.Models.Tests
{
    public class ExportProbeTests
    {
        [Fact]
        public void ExportProbe_Initialization_Test()
        {
            // Arrange
            var exportProbe = new ExportProbe
            {
                ProbeSn = "12345",
                TransducerModuleSn = "67890",
                TransducerSn = "ABCDE",
                MotorModuleSn = "FGHIJ",
                Date1 = DateTime.Now,
                Score1 = 10,
                Date2 = DateTime.Now.AddDays(1),
                Score2 = 20,
                Date3 = DateTime.Now.AddDays(2),
                Score3 = 30,
                Date4 = DateTime.Now.AddDays(3),
                Score4 = 40,
                Date5 = DateTime.Now.AddDays(4),
                Score5 = 50,
                Date6 = DateTime.Now.AddDays(5),
                Score6 = 60,
                Date7 = DateTime.Now.AddDays(6),
                Score7 = 70,
                Date8 = DateTime.Now.AddDays(7),
                Score8 = 80,
                Date9 = DateTime.Now.AddDays(8),
                Score9 = 90
            };

            // Act & Assert
            Assert.NotNull(exportProbe);
            Assert.Equal("12345", exportProbe.ProbeSn);
            Assert.Equal("67890", exportProbe.TransducerModuleSn);
            Assert.Equal("ABCDE", exportProbe.TransducerSn);
            Assert.Equal("FGHIJ", exportProbe.MotorModuleSn);
            Assert.Equal(10, exportProbe.Score1);
            Assert.Equal(20, exportProbe.Score2);
            Assert.Equal(30, exportProbe.Score3);
            Assert.Equal(40, exportProbe.Score4);
            Assert.Equal(50, exportProbe.Score5);
            Assert.Equal(60, exportProbe.Score6);
            Assert.Equal(70, exportProbe.Score7);
            Assert.Equal(80, exportProbe.Score8);
            Assert.Equal(90, exportProbe.Score9);
        }
    }

}
