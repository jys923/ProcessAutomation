using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
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