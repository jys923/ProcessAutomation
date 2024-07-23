using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.Models
{
    public class ExportProbe
    {
        public string ProbeSn { get; set; } = default!;

        public string TransducerModuleSn { get; set; } = default!;

        public string TransducerSn { get; set; } = default!;

        public string MotorModuleSn { get; set; } = default!;

        public DateTime Date1 { get; set; }
        public int Score1 { get; set; }
        public DateTime Date2 { get; set; }
        public int Score2 { get; set; }
        public DateTime Date3 { get; set; }
        public int Score3 { get; set; }
        
        public DateTime Date4 { get; set; }
        public int Score4 { get; set; }
        public DateTime Date5 { get; set; }
        public int Score5 { get; set; }
        public DateTime Date6 { get; set; }
        public int Score6 { get; set; }
        
        public DateTime? Date7 { get; set; }
        public int? Score7 { get; set; }
        public DateTime? Date8 { get; set; }
        public int? Score8 { get; set; }
        public DateTime? Date9 { get; set; }
        public int? Score9 { get; set; }
        
    }
}
