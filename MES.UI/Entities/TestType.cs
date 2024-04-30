using System.ComponentModel.DataAnnotations;

namespace MES.UI.Entities
{
    /// <summary>
    /// 1번 검사 2번 검사...
    /// </summary>
    public class TestType : EntityBase
    {
        /*[Required]
        public int CategoryId { get; set; } = (int)Enums.TestCategory.Processing;*/

        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public int Threshold { get; set; } = new Random().Next(70,100);
    }
}