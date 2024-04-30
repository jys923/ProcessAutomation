using System.ComponentModel.DataAnnotations;

namespace MES.UI.Models
{
    public class MotorModule : EntityBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}