using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Entities
{
    public class ProbeType : EntityBase
    {
        //[Key]
        //public Guid ProbeTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }
    }
}