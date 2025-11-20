using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.UI.Models;
using System.Windows.Media;

namespace SonoCap.MES.UI.Services
{
    public interface ICellStatusService
    {
        void SetCellPassFail(Test test, Action<CellPositions, Brush> applyBrush);
        void SetCategoryDefault(TestCategories category, Action<CellPositions, Brush> applyBrush);
    }

    public class CellStatusService : ICellStatusService
    {
        public void SetCellPassFail(Test test, Action<CellPositions, Brush> applyBrush)
        {
            CellPositions cell = (CellPositions)(test.TestCategoryId * 10 + test.TestTypeId);
            Brush color = IsPass(test) ? Brushes.LightGreen : Brushes.Tomato;
            applyBrush(cell, color);
        }

        public void SetCategoryDefault(TestCategories category, Action<CellPositions, Brush> applyBrush)
        {
            var targets = category switch
            {
                TestCategories.Processing => new[] { CellPositions.Row1_Column1, CellPositions.Row1_Column2, CellPositions.Row1_Column3 },
                TestCategories.Process => new[] { CellPositions.Row2_Column1, CellPositions.Row2_Column2, CellPositions.Row2_Column3 },
                TestCategories.Dispatch => new[] { CellPositions.Row3_Column1, CellPositions.Row3_Column2, CellPositions.Row3_Column3 },
                TestCategories.All => Enum.GetValues(typeof(CellPositions)).Cast<CellPositions>(),
                _ => Array.Empty<CellPositions>()
            };

            foreach (var cell in targets)
                applyBrush(cell, Brushes.LightGray);
        }

        private bool IsPass(Test test)
        {
            int key = test.TestCategoryId * 10 + test.TestTypeId;
            if (!App.TestThresholdDict.TryGetValue(key, out int threshold))
                return false;

            return test.Result >= threshold;
        }
    }
}
