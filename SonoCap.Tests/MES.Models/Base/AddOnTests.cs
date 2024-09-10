namespace SonoCap.MES.Models.Base.Tests
{
    public class AddOnTests
    {
        private class TestAddOn : AddOn
        {
            public string Name { get; set; } = "TestName";
            public int Value { get; set; } = 42;
        }

        [Fact]
        public void ToString_ShouldReturnCorrectStringRepresentation()
        {
            // Arrange
            var addOn = new TestAddOn();

            // Act
            var result = addOn.ToString();

            // Assert
            Assert.Contains("Name: TestName", result);
            Assert.Contains("Value: 42", result);
        }
    }
}
