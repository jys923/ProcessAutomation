using MES.UI.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace MES.UI.Models
{
    public class Pc : ModelBase
    {
        [Required]
        public required string Name { get; set; }
    }
}
