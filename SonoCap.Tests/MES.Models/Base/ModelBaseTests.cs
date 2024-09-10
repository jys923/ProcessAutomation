namespace SonoCap.MES.Models.Base.Tests
{
    public class ModelBaseTests
    {
        [Fact]
        public void ModelBase_ShouldInitializeWithDefaultValues()
        {
            // Arrange
            var model = new ModelBase();

            // Act & Assert
            Assert.Equal(1, model.DataFlag);
            Assert.Null(model.Detail);
            Assert.Equal(DateTime.Now.Date, model.CreatedDate.Date);
        }

        [Fact]
        public void ModelBase_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var model = new ModelBase
            {
                DataFlag = 0,
                Detail = "Test Detail",
                CreatedDate = new DateTime(2023, 1, 1)
            };

            // Act & Assert
            Assert.Equal(0, model.DataFlag);
            Assert.Equal("Test Detail", model.Detail);
            Assert.Equal(new DateTime(2023, 1, 1), model.CreatedDate);
        }
    }
}
