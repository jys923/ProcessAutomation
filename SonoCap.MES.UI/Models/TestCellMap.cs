using SonoCap.MES.Models.Enums;

namespace SonoCap.MES.UI.Models
{
    public static class TestCellMap
    {
        public static readonly List<TestCellDefinition> Items = new()
    {
        new() { Position = CellPositions.Row1_Column1, Category = TestCategories.Processing, TestType = TestTypes.Gray, UsesEnvImage = false },
        new() { Position = CellPositions.Row1_Column2, Category = TestCategories.Processing, TestType = TestTypes.Res, UsesEnvImage = false },
        new() { Position = CellPositions.Row1_Column3, Category = TestCategories.Processing, TestType = TestTypes.EnvGeo, UsesEnvImage = true },

        new() { Position = CellPositions.Row2_Column1, Category = TestCategories.Process, TestType = TestTypes.Gray, UsesEnvImage = false },
        new() { Position = CellPositions.Row2_Column2, Category = TestCategories.Process, TestType = TestTypes.Res, UsesEnvImage = false },
        new() { Position = CellPositions.Row2_Column3, Category = TestCategories.Process, TestType = TestTypes.EnvGeo, UsesEnvImage = true },

        new() { Position = CellPositions.Row3_Column1, Category = TestCategories.Dispatch, TestType = TestTypes.Gray, UsesEnvImage = false },
        new() { Position = CellPositions.Row3_Column2, Category = TestCategories.Dispatch, TestType = TestTypes.Res, UsesEnvImage = false },
        new() { Position = CellPositions.Row3_Column3, Category = TestCategories.Dispatch, TestType = TestTypes.EnvGeo, UsesEnvImage = true },
    };
    }

}
