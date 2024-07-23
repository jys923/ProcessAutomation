using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoCap.MES.Models.Converts
{
    public static class PTRViewToExportProbe
    {
        private static ExportProbe Convert(PTRView ptrView)
        {
            if (ptrView == null)
                throw new ArgumentNullException(nameof(ptrView));

            return new ExportProbe
            {
                ProbeSn = ptrView.ProbeSn,
                TransducerModuleSn = ptrView.TransducerModuleSn,
                TransducerSn = ptrView.TransducerSn,
                MotorModuleSn = ptrView.MotorModuleSn,
                Date1 = ptrView.Test01.CreatedDate,
                Score1 = ptrView.Test01.Result,
                Date2 = ptrView.Test01.CreatedDate,
                Score2 = ptrView.Test01.Result,
                Date3 = ptrView.Test01.CreatedDate,
                Score3 = ptrView.Test01.Result,
                Date4 = ptrView.Test01.CreatedDate,
                Score4 = ptrView.Test01.Result,
                Date5 = ptrView.Test01.CreatedDate,
                Score5 = ptrView.Test01.Result,
                Date6 = ptrView.Test01.CreatedDate,
                Score6 = ptrView.Test01.Result,
                Date7 = ptrView.Test07?.CreatedDate,
                Score7 = ptrView.Test07?.Result,
                Date8 = ptrView.Test08?.CreatedDate,
                Score8 = ptrView.Test08?.Result,
                Date9 = ptrView.Test09?.CreatedDate,
                Score9 = ptrView.Test09?.Result
            };
        }

        public static IList<ExportProbe> ToList(IEnumerable<PTRView> ptrViews)
        {
            if (ptrViews == null)
                return new List<ExportProbe>();

            return ptrViews.Select(ptrView => Convert(ptrView)).ToList();
        }

        private static object PTRViewToAnonymousArray(PTRView ptrView)
        {
            return new
            {
                Column1 = ptrView.ProbeSn,
                Column2 = ptrView.TransducerModuleSn,
                Column3 = ptrView.TransducerSn,
                Column4 = ptrView.MotorModuleSn,
                Column5 = ptrView.Test01.CreatedDate,
                Column6 = ptrView.Test01.Result,
                Column7 = ptrView.Test02.CreatedDate,
                Column8 = ptrView.Test02.Result,
                Column9 = ptrView.Test03.CreatedDate,
                Column10 = ptrView.Test03.Result,
                Column11 = ptrView.Test04.CreatedDate,
                Column12 = ptrView.Test04.Result,
                Column13 = ptrView.Test05.CreatedDate,
                Column14 = ptrView.Test05.Result,
                Column15 = ptrView.Test06.CreatedDate,
                Column16 = ptrView.Test06.Result,
                Column17 = ptrView.Test07?.CreatedDate ?? DateTime.MinValue,
                Column18 = ptrView.Test07?.Result ?? -1,
                Column19 = ptrView.Test08?.CreatedDate ?? DateTime.MinValue,
                Column20 = ptrView.Test08?.Result ?? -1,
                Column21 = ptrView.Test09?.CreatedDate ?? DateTime.MinValue,
                Column22 = ptrView.Test09?.Result ?? -1,
            };
        }

        public static object[] ToAnonymousArray(IEnumerable<PTRView> ptrViews)
        {
            return ptrViews.Select(ptrView => PTRViewToAnonymousArray(ptrView)).Cast<object>().ToArray() ?? Array.Empty<object>();
        }

        public static object[] ToAnonymousArray2(IEnumerable<PTRView> ptrViews)
        {
            if (ptrViews == null)
                return Array.Empty<object>();

            return ptrViews.Select(ptrView => new
            {
                Column1 = ptrView.ProbeSn,
                Column2 = ptrView.TransducerModuleSn,
                Column3 = ptrView.TransducerSn,
                Column4 = ptrView.MotorModuleSn,
                Column5 = ptrView.Test01.CreatedDate,
                Column6 = ptrView.Test01.Result,
                Column7 = ptrView.Test02.CreatedDate,
                Column8 = ptrView.Test02.Result,
                Column9 = ptrView.Test03.CreatedDate,
                Column10 = ptrView.Test03.Result,
                Column11 = ptrView.Test04.CreatedDate,
                Column12 = ptrView.Test04.Result,
                Column13 = ptrView.Test05.CreatedDate,
                Column14 = ptrView.Test05.Result,
                Column15 = ptrView.Test06.CreatedDate,
                Column16 = ptrView.Test06.Result,
                Column17 = ptrView.Test07?.CreatedDate ?? DateTime.MinValue,
                Column18 = ptrView.Test07?.Result ?? -1,
                Column19 = ptrView.Test08?.CreatedDate ?? DateTime.MinValue,
                Column20 = ptrView.Test08?.Result ?? -1,
                Column21 = ptrView.Test09?.CreatedDate ?? DateTime.MinValue,
                Column22 = ptrView.Test09?.Result ?? -1,
            }).Cast<object>().ToArray();
        }

        private static IDictionary<string, object> PTRViewToDictionary(PTRView ptrView)
        {
            var dictionary = new Dictionary<string, object>
            {
                { nameof(ptrView.ProbeSn), ptrView.ProbeSn },
                { nameof(ptrView.TransducerModuleSn), ptrView.TransducerModuleSn },
                { nameof(ptrView.TransducerSn), ptrView.TransducerSn },
                { nameof(ptrView.MotorModuleSn), ptrView.MotorModuleSn },
                { $"{nameof(ptrView.Test01.CreatedDate)}1", ptrView.Test01.CreatedDate },
                { $"{nameof(ptrView.Test01.Result)}1", ptrView.Test01.Result },
                { $"{nameof(ptrView.Test02.CreatedDate)}2", ptrView.Test02.CreatedDate },
                { $"{nameof(ptrView.Test02.Result)}2", ptrView.Test02.Result },
                { $"{nameof(ptrView.Test03.CreatedDate)}3", ptrView.Test03.CreatedDate },
                { $"{nameof(ptrView.Test03.Result)}3", ptrView.Test03.Result },
                { $"{nameof(ptrView.Test04.CreatedDate)}4", ptrView.Test04.CreatedDate },
                { $"{nameof(ptrView.Test04.Result)}4", ptrView.Test04.Result },
                { $"{nameof(ptrView.Test05.CreatedDate)}5", ptrView.Test05.CreatedDate },
                { $"{nameof(ptrView.Test05.Result)}5", ptrView.Test05.Result },
                { $"{nameof(ptrView.Test06.CreatedDate)}6", ptrView.Test06.CreatedDate },
                { $"{nameof(ptrView.Test06.Result)}6", ptrView.Test06.Result },
                { "CreatedDate7", ptrView.Test07?.CreatedDate ?? null },
                { "Result7", ptrView.Test07?.Result ?? null },
                { "CreatedDate8", ptrView.Test08?.CreatedDate ?? null },
                { "Result8", ptrView.Test08?.Result ?? null },
                { "CreatedDate9", ptrView.Test09?.CreatedDate ?? null },
                { "Result9", ptrView.Test09?.Result ?? null },
            };

            //if (ptrView.Test07 != null)
            //{
            //    dictionary.Add("CreatedDate7", ptrView.Test07.CreatedDate);
            //    dictionary.Add("Result7", ptrView.Test07.Result);
            //}

            //if (ptrView.Test08 != null)
            //{
            //    dictionary.Add("CreatedDate8", ptrView.Test08.CreatedDate);
            //    dictionary.Add("Result8", ptrView.Test08.Result);
            //}

            //if (ptrView.Test09 != null)
            //{
            //    dictionary.Add("CreatedDate9", ptrView.Test09.CreatedDate);
            //    dictionary.Add("Result9", ptrView.Test09.Result);
            //}

            return dictionary;
        }

        public static IEnumerable<IDictionary<string, object>> ToDictionaryList(IEnumerable<PTRView> ptrViews)
        {
            if (ptrViews == null)
                return Enumerable.Empty<IDictionary<string, object>>();

            return ptrViews.Select(ptrView => PTRViewToDictionary(ptrView));
        }

        private static IList<Dictionary<string, object>> ToDictionaryList2(IEnumerable<PTRView> ptrViews)
        {
            if (ptrViews == null)
                return new List<Dictionary<string, object>>();

            return ptrViews.Select(ptrView => new Dictionary<string, object>
            {
                { "Column1", ptrView.ProbeSn },
                { "Column2", ptrView.TransducerModuleSn },
                { "Column3", ptrView.TransducerSn },
                { "Column4", ptrView.MotorModuleSn },
                { "Column5", ptrView.Test01.CreatedDate },
                { "Column6", ptrView.Test01.Result },
                { "Column7", ptrView.Test02.CreatedDate },
                { "Column8", ptrView.Test02.Result },
                { "Column9", ptrView.Test03.CreatedDate },
                { "Column10", ptrView.Test03.Result },
                { "Column11", ptrView.Test04.CreatedDate },
                { "Column12", ptrView.Test04.Result },
                { "Column13", ptrView.Test05.CreatedDate },
                { "Column14", ptrView.Test05.Result },
                { "Column15", ptrView.Test06.CreatedDate },
                { "Column16", ptrView.Test06.Result },
                { "Column17", ptrView.Test07?.CreatedDate ?? DateTime.MinValue },
                { "Column18", ptrView.Test07?.Result ?? -1 },
                { "Column19", ptrView.Test08?.CreatedDate ?? DateTime.MinValue },
                { "Column20", ptrView.Test08?.Result ?? -1 },
                { "Column21", ptrView.Test09?.CreatedDate ?? DateTime.MinValue },
                { "Column22", ptrView.Test09?.Result ?? -1 }
            }).ToList();
        }

        private static IList<Dictionary<string, object>> ToDictionaryList3(IEnumerable<PTRView> ptrViews)
        {
            var dictionaryList = new List<Dictionary<string, object>>();

            if (ptrViews == null)
                return dictionaryList;

            foreach (var ptrView in ptrViews)
            {
                var dict = new Dictionary<string, object>
                {
                    { "Column1", ptrView.ProbeSn },
                    { "Column2", ptrView.TransducerModuleSn },
                    { "Column3", ptrView.TransducerSn },
                    { "Column4", ptrView.MotorModuleSn },
                    { "Column5", ptrView.Test01.CreatedDate },
                    { "Column6", ptrView.Test01.Result },
                    { "Column7", ptrView.Test02.CreatedDate },
                    { "Column8", ptrView.Test02.Result },
                    { "Column9", ptrView.Test03.CreatedDate },
                    { "Column10", ptrView.Test03.Result },
                    { "Column11", ptrView.Test04.CreatedDate },
                    { "Column12", ptrView.Test04.Result },
                    { "Column13", ptrView.Test05.CreatedDate },
                    { "Column14", ptrView.Test05.Result },
                    { "Column15", ptrView.Test06.CreatedDate },
                    { "Column16", ptrView.Test06.Result },
                    { "Column17", ptrView.Test07?.CreatedDate ?? DateTime.MinValue },
                    { "Column18", ptrView.Test07?.Result ?? -1 },
                    { "Column19", ptrView.Test08?.CreatedDate ?? DateTime.MinValue },
                    { "Column20", ptrView.Test08?.Result ?? -1 },
                    { "Column21", ptrView.Test09?.CreatedDate ?? DateTime.MinValue },
                    { "Column22", ptrView.Test09?.Result ?? -1 },
                };
                dictionaryList.Add(dict);
            }

            return dictionaryList;
        }

        private static IEnumerable<IDictionary<string, object>> ToDictionaryList<T>(IEnumerable<T> data)
        {
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                var properties = typeof(T).GetProperties();

                var dict = new Dictionary<string, object>();
                foreach (var prop in properties)
                {
                    dict[prop.Name] = prop.GetValue(item);
                }

                dictionaryList.Add(dict);
            }

            return dictionaryList;
        }
    }
}
