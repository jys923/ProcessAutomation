using MES.UI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.UI.Models
{
    /// <summary>
    /// 로그인 로그 처럼 사용
    /// </summary>
    public class Tester : ModelBase
    {
        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public int PcId { get; set; }

        [ForeignKey(nameof(PcId))] //lazy 로딩
        public virtual Pc Pc { get; set; } = default!;
    }
}
