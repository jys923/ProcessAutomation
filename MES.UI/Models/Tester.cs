using System.ComponentModel.DataAnnotations;
using MES.UI.Models.Base;

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
        public int PcNo { get; set; }
    }
}
