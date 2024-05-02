using System.ComponentModel.DataAnnotations;
using MES.UI.Models.Base;

namespace MES.UI.Models
{
    /// <summary>
    /// 검사중,검사끝,출하전
    /// </summary>
    public class TestCategory : ModelBase
    {
        [Required]
        [StringLength(10)]
        public required string Name { get; set; }
    }
}