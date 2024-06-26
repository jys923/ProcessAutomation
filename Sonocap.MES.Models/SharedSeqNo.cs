using Microsoft.EntityFrameworkCore;
using SonoCap.MES.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SonoCap.MES.Models
{
    [Index(nameof(Date), IsUnique = true)]
    public class SharedSeqNo : ModelBase
    {
        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        public int TDMdNo { get; set; } = 1;

        [Required]
        public int ProbeNo { get; set; } = 1;
    }
}
