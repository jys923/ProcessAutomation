using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MES.UI.Models.Base;

namespace MES.UI.Models
{
    public class TransducerModuleType : ModelBase
    {
        /// <summary>
        /// SC0P1
        /// </summary>
        [Required]
        [StringLength(10)]
        public required string Code { get; set; }

        /// <summary>
        /// 7.5MHz
        /// </summary>
        [Required]
        [StringLength(10)]
        public required string Type { get; set; }


    }
}