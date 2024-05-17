using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MES.UI.Models
{
    [NotMapped]
    [Keyless]
    public class TestProbe
    {
        public required int Id { get; set; }
        public required DateTime CreatedDate { get; set; }
        public string? Detail { get; set; }
        public required virtual TestCategory Category { get; set; }
        public required virtual TestType TestType { get; set; }
        public required string Tester { get; set; }
        public required virtual Pc Pc { get; set; }
        public required string OriginalImg { get; set; }
        public required string ChangedImg { get; set; }
        public required string ChangedImgMetadata { get; set; }
        public int Result { get; set; }
        public int Method { get; set; }
        public required virtual TransducerModule TransducerModule { get; set; } = default!;
        public string? ProbeSn { get; set; }
        public virtual MotorModule? MotorModule { get; set; }
    }
    //[Keyless]
    //public class TestProbe
    //{
    //    public required int Id { get; set; }
    //    public required DateTime CreatedDate { get; set; }

    //    [Required]
    //    public required int CategoryId { get; set; }

    //    [ForeignKey(nameof(CategoryId))] //lazy 로딩
    //    public virtual required TestCategory Category { get; set; }

    //    [Required]
    //    public int TestTypeId { get; set; }

    //    [ForeignKey(nameof(TestTypeId))] //lazy 로딩
    //    public virtual required TestType TestType { get; set; }

    //    [Required]
    //    public int TesterId { get; set; }

    //    [ForeignKey(nameof(TesterId))] //lazy 로딩
    //    public virtual required Tester Tester { get; set; }

    //    public required string OriginalImg { get; set; }
    //    public required string ChangedImg { get; set; }
    //    public required string ChangedImgMetadata { get; set; }
    //    public required int Result { get; set; }
    //    public required int Method { get; set; }

    //    [Required]
    //    public int TransducerModuleId { get; set; } = default!;

    //    [ForeignKey(nameof(TransducerModuleId))] //lazy 로딩
    //    public virtual required TransducerModule TransducerModule { get; set; } = default!;

    //    public string? ProbeSn { get; set; }

    //    [Required]
    //    public int MotorModuleId { get; set; }

    //    [ForeignKey(nameof(MotorModuleId))] //lazy 로딩
    //    public virtual MotorModule MotorModule { get; set; } = default!;
    //}
}
