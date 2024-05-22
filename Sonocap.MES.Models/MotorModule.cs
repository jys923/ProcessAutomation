using SonoCap.MES.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    public class MotorModule : ModelBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}