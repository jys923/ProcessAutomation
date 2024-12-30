using System;
using System.Linq;
using Xunit;
using SonoCap.Commons;

namespace SonoCap.Tests.Commons
{
    public class ExtensionMethodsTests
    {
        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        [Fact]
        public void ToFormattedString_ShouldReturnFormattedString()
        {
            // Arrange
            var testObject = new TestClass
            {
                Id = 1,
                Name = "Test Object",
                CreatedAt = new DateTime(2021, 1, 1)
            };

            // Act
            string result = testObject.ToFormattedString();

            // Assert
            Assert.Equal("Id: 1, Name: Test Object, CreatedAt: 2021-01-01 오전 12:00:00", result);
        }

        [Fact]
        public void ToJson_ShouldReturnJsonString()
        {
            // Arrange
            var testObject = new TestClass
            {
                Id = 1,
                Name = "Test Object",
                CreatedAt = new DateTime(2021, 1, 1)
            };

            // Act
            string result = testObject.ToJson();

            // Assert
            Assert.Contains("\"Id\":1", result);
            Assert.Contains("\"Name\":\"Test Object\"", result);
            Assert.Contains("\"CreatedAt\":\"2021-01-01T00:00:00\"", result);
        }
    }
}
