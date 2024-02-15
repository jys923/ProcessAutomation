using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    public class ProbeType : EntityBase
    {
        /*[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProbeTypeId { get; set; }*/

        /// <summary>
        /// SCP01
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