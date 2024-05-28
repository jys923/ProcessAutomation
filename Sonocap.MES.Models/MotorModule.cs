using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    public class MotorModule : ModelBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}