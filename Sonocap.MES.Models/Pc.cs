using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    public class Pc : ModelBase
    {
        [Required]
        public required string Name { get; set; }
    }
}
