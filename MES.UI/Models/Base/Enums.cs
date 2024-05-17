using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.UI.Models.Base
{
    public class Enums
    {
        public enum TestResult
        {
            //All = 0,
            Success = 1,
            Failure = 2,
        }

        public enum DataFlag
        {
            All = 0,
            Create = 1,
            Delete = 2,
            //Update = 3,
            //Read,
        }

        public enum TransducerType
        {
            SCP01 = 1,
            SCP02,
            SCP03,
        }

        public enum TestCategory
        {
            Processing = 1,
            Process,
            //Dispatch,
        }

        public enum TestCategoryKor
        {
            공정용 = 1,
            최종용,
            //출하용,
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
        }
    }
}
