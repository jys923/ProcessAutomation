using SonoCap.Tests.MES.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.Tests
{
    public class PapaTests
    {
        [Fact]
        public void Papa_DefaultConstructor_SetsDefaultValues()
        {
            var papa = new Papa();

            Assert.NotNull(papa);
            Assert.Equal("Default Value", papa.RequiredField);
            Assert.Equal(string.Empty, papa.OptionalField);
        }

        [Fact]
        public void Papa_Constructor_SetsProperties()
        {
            var requiredField = "Papa Required";
            var optionalField = "Papa Optional";

            var papa = new Papa(requiredField, optionalField);

            Assert.Equal(requiredField, papa.RequiredField);
            Assert.Equal(optionalField, papa.OptionalField);
        }
    }

    public class SonTests
    {
        [Fact]
        public void Son_DefaultConstructor_SetsDefaultValues()
        {
            var son = new Son();

            Assert.NotNull(son);
            Assert.Equal("Default Value", son.RequiredField);
            Assert.Equal(string.Empty, son.OptionalField);
            Assert.Equal("Default Value", son.SonRequiredField);
            Assert.Equal(string.Empty, son.SonOptionalField);
        }

        //[Fact]
        //public void Son_Constructor_SetsProperties()
        //{
        //    var papaRequiredField = "Papa Required";
        //    var papaOptionalField = "Papa Optional";
        //    var sonRequiredField = "Son Required";
        //    var sonOptionalField = "Son Optional";

        //    var son = new Son(papaRequiredField, papaOptionalField, sonRequiredField, sonOptionalField);

        //    Assert.Equal(papaRequiredField, son.RequiredField);
        //    Assert.Equal(papaOptionalField, son.OptionalField);
        //    Assert.Equal(sonRequiredField, son.SonRequiredField);
        //    Assert.Equal(sonOptionalField, son.SonOptionalField);
        //}

        [Fact]
        public void Son_RequiredField_ThrowsExceptionIfNull()
        {
            var son = new Son
            {
                SonRequiredField = null
            };

            Assert.Throws<ValidationException>(() => Validator.ValidateObject(son, new ValidationContext(son), true));
        }
    }
}
