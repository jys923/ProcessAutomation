using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreSample.Entities
{
    public class Enums
    {
        public enum TestResult
        {
            Failure,
            //Success,
        }

        public enum DataFlag
        {
            Delete,
            Create,
        }

        public enum TransducerModule
        {
            SCP01 = 1,
            SCP02,
            SCP03,
        }

        public enum TestCategory
        {
            Processing = 1,
            Process,
            Dispatch,
        }

        public enum TestCategoryKor
        {
            공정용 = 1,
            최종용,
            출하용,
        }

        public enum TestMode
        {
            Auto,
            Manual,
        }

        public enum TestType
        {
            Align = 1,
            Axial,
            Lateral,
            Red,
            Green,
            Blue
        }
    }
}
