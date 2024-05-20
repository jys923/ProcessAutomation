using MES.UI.Models.Base;
using System.ComponentModel.DataAnnotations;

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