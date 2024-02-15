using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample.Entities
{
    /// <summary>
    /// 로그인 로그 처럼 사용
    /// </summary>
    public class InspectorType : EntityBase
    {
        /*[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InspectorTypeId { get; set; }*/

        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public int PcNo { get; set; }
    }
}
