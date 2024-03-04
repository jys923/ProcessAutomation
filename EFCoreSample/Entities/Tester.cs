using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Entities
{
    /// <summary>
    /// 로그인 로그 처럼 사용
    /// </summary>
    public class Tester : EntityBase
    {
        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public int PcNo { get; set; }
    }
}
