using System.ComponentModel.DataAnnotations;

namespace MES.UI.Entities
{
    /// <summary>
    /// 검사중,검사끝,출하전
    /// </summary>
    public class TestCategory : EntityBase
    {
        [Required]
        [StringLength(10)]
        public required string Name { get; set; }
    }
}