using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    public class ProbeSNType : EntityBase
    {
        /*[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProbeSNTypeId { get; set; }*/

        /// <summary>
        /// yyyymmdd
        /// </summary>
        [Required]
        public required string DateTime { get; set; }

        [Required]
        public int PcNo { get; set; }
    }
}