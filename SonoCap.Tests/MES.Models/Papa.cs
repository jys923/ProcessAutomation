using System.ComponentModel.DataAnnotations;

namespace SonoCap.Tests.MES.Models {
    public class Papa
    {
        [Required]
        public string RequiredField { get; set; }// = "Default Value";

        public string OptionalField { get; set; }// = string.Empty;

        public Papa()
        {
            RequiredField = "Default Value";
            OptionalField = string.Empty;
        }

        public Papa(string requiredField, string optionalField)
        {
            RequiredField = requiredField;
            OptionalField = optionalField;
        }
    }
}

