using System.ComponentModel.DataAnnotations;

namespace SonoCap.Tests.MES.Models
{
    public class Son : Papa
    {
        [Required]
        public string SonRequiredField { get; set; } = "Default Value";

        public string SonOptionalField { get; set; } = string.Empty;

        public Son()// : base()
        {
            //SonRequiredField = "Default Value"; // 기본값 설정
        }

        //public Son(string papaRequiredField, string papaOptionalField, string sonRequiredField, string sonOptionalField)
        //    : base(papaRequiredField, papaOptionalField)
        //{
        //    SonRequiredField = sonRequiredField;
        //    SonOptionalField = sonOptionalField;
        //}
    }
}