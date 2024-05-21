using SonoCap.MES.UI.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.UI.Models
{
    public class TransducerType : ModelBase
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