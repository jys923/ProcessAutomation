using SonoCap.MES.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.UI.Models
{
    public class TestCellDefinition
    {
        public CellPositions Position { get; set; }
        public TestCategories Category { get; set; }
        public TestTypes TestType { get; set; }
        public bool UsesEnvImage { get; set; }
    }

}
