﻿namespace SonoCap.MES.Models.Enums
{
    public enum CellPositions
    {
        Row0_Column0,
        Row0_Column1,
        Row0_Column2,
        Row0_Column3,
        Row0_Column4,
        Row1_Column0,
        Row1_Column1 = 11,
        Row1_Column2 = 12,
        Row1_Column3 = 13,
        Row1_Column4 = 14,
        Row2_Column0,
        Row2_Column1 = 21,
        Row2_Column2 = 22,
        Row2_Column3 = 23,
        Row2_Column4 = 24,
    }

    public enum Commons
    {
        All = -123,
    }

    public enum TestResults
    {
        //All = 0,
        Success = 1,
        Failure = 2,
    }

    public enum DataFlags
    {
        All = 0,
        Create = 1,
        Delete = 2,
        //Update = 3,
        //Read,
    }

    public enum TransducerTypes
    {
        SCP01 = 1,
        SCP02,
        SCP03,
    }

    public enum TestCategories
    {
        Processing = 1,
        Process,
        //Dispatch,
    }

    public enum TestCategoriesKor
    {
        공정용 = 1,
        최종용,
        //출하용,
    }

    public enum TestModes
    {
        Auto,
        Manual,
    }

    public enum TestTypes
    {
        Align = 1,
        Axial,
        Lateral,
    }
}