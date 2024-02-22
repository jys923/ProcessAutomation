using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample.Entities
{
    [NotMapped]
    public class ProbeView// : EntityBase
    {
        public required string ProbeSN { get; set; }
        public int Result1 { get; set; }
        public int Result2 { get; set; }
        public int Result3 { get; set; }
        public int Result4 { get; set; }
        public int Result5 { get; set; }
    }
}
