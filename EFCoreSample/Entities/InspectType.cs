using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Entities
{
    /// <summary>
    /// 1번 검사 2번 검사...
    /// </summary>
    public class InspectType : EntityBase
    {
        //[Key]
        //public Guid InspectTypeId { get; set; }

        [Required]
        [StringLength(10)]
        public required string Name { get; set; }
    }
}