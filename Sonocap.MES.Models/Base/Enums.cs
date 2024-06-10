﻿namespace SonoCap.MES.Models.Enums
{
    public static class CommonValues
    {
        public static readonly int All = 0;
        public static readonly int None = -1;
    }

    public enum SnType
    {
        Probe,
        TransducerModule,
        Transducer,
        MotorModule,
    }

    public enum CellPositions
    {
        Row0_Column0,
        Row0_Column1 = 1,
        Row0_Column2 = 2,
        Row0_Column3 = 3,
        Row0_Column4 = 4,
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
        Row3_Column0,
        Row3_Column1 = 31,
        Row3_Column2 = 32,
        Row3_Column3 = 33,
        Row3_Column4 = 34,
    }

    public enum Commons
    {
        All = -123,
        None = -1,
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
        All = 0,
        Processing = 1,
        Process,
        Dispatch,
    }

    public enum TestCategoriesKor
    {
        공정용 = 1,
        최종용,
        출하용,
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
