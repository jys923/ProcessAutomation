using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SonoCap.MES.Models
{
    /// <summary>
    /// 1번 검사 2번 검사...
    /// </summary>
    public class TestType : ModelBase
    {
        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public int Threshold { get; set; } = 90;
    }

    //public class TestType : ModelBase
    //{
    //    public string Name { get; set; }
    //    public int Threshold { get; set; }
    //    public string? SubCriteriaJson { get; set; }

    //    public Dictionary<string, double>? GetParsedCriteria()
    //    {
    //        if (string.IsNullOrWhiteSpace(SubCriteriaJson)) return null;
    //        return JsonSerializer.Deserialize<Dictionary<string, double>>(SubCriteriaJson);
    //    }
    //}
}