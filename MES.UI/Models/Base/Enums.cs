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
            Fail,
            Pass,
        }

        public enum DataFlag
        {
            Delete,
            Create,
            //Read,
            //Update,
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
