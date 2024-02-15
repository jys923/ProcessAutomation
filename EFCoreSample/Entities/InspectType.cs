using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    /// <summary>
    /// 1번 검사 2번 검사...
    /// </summary>
    public class InspectType : EntityBase
    {
       /* [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InspectTypeId { get; set; }*/

        [Required]
        [StringLength(10)]
        public required string Name { get; set; }
    }
}