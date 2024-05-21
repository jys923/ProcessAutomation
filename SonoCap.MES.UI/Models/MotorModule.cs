using SonoCap.MES.UI.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.UI.Models
{
    public class MotorModule : ModelBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}