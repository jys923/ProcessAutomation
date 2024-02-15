using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample.Entities
{
    public class Enums
    {
        public enum InspectResult
        {
            Failure,
            Success,
        }

        public enum DataFlag
        {
            Delete,
            Create,
        }

        public enum Method
        {
            Auto,
            Manual,
        }
    }
}
