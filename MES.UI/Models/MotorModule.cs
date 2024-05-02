using System.ComponentModel.DataAnnotations;
using MES.UI.Models.Base;

namespace MES.UI.Models
{
    public class MotorModule : ModelBase
    {
        [Required]
        public required string MotorModuleSn { get; set; }
    }
}