using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Entities
{
    public class MotorModule : EntityBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}