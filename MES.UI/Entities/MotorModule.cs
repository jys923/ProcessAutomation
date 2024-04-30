using System.ComponentModel.DataAnnotations;

namespace MES.UI.Entities
{
    public class MotorModule : EntityBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}