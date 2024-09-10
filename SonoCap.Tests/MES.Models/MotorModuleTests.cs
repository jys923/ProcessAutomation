using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SonoCap.MES.Models;
using Xunit;

namespace SonoCap.MES.Models.Tests
{
    public class MotorModuleTests
    {
        [Fact]
        public void MotorModule_ShouldBeValid()
        {
            var module = new MotorModule { Sn = "SN123" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(module);

            bool isValid = Validator.TryValidateObject(module, validationContext, validationResults, true);

            Assert.True(isValid);
        }

        [Fact]
        public void MotorModule_ShouldRequireSn()
        {
            var module = new MotorModule { Sn = null! }; // Sn 필드가 없는 경우
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(module);

            bool isValid = Validator.TryValidateObject(module, validationContext, validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(module.Sn)));
        }
    }
}
