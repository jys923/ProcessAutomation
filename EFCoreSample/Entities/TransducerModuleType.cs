using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    public class TransducerModuleType : EntityBase
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