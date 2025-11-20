using SonoCap.MES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.UI.Models
{
    public class SnFlowResult
    {
        public bool IsValid { get; set; }

        public Transducer? Transducer { get; set; }
        public TransducerModule? TransducerModule { get; set; }
        public Probe? Probe { get; set; }
        public MotorModule? MotorModule { get; set; }

        public List<Test> TransducerTests { get; set; } = new();
        public List<Test> TransducerModuleTests { get; set; } = new();
        public List<Test> ProbeTests { get; set; } = new();

        public bool TdEnabled { get; set; }
        public bool TdMdEnabled { get; set; }
        public bool ProbeEnabled { get; set; }

        public string? TDMdWatermark { get; set; }
        public string? MotorMdWatermark { get; set; }
        public string? ProbeSnWatermark { get; set; }
    }

}
