﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreSample.Entities
{
    public class TransducerModuleView
    {
        [NotMapped]
        public required string TransducerModuleSn { get; set; }
        public required IList<int> Results { get; set; }
    }
}
