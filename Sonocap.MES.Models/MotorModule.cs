using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    [Index(nameof(Sn), IsUnique = true)]
    public class MotorModule : ModelBase, ISn
    {
        [Required]
        public required string Sn { get; set; }
    }
}