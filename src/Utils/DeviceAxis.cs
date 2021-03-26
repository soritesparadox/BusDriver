﻿using System.Collections.Generic;

namespace BusDriver.Utils
{
    // enums crash VaM
    //TODO: rename from TCode?
    public static class DeviceAxis
    {
        public const int L0 = 0;
        public const int L1 = 1;
        public const int L2 = 2;
        public const int R0 = 3;
        public const int R1 = 4;
        public const int R2 = 5;

        public static IEnumerable<int> Values
        {
            get
            {
                yield return L0;
                yield return L1;
                yield return L2;
                yield return R0;
                yield return R1;
                yield return R2;
            }
        }

        public static float DefaultValue(int axis)
        {
            switch (axis)
            {
                case L0:
                case L1:
                case L2:
                case R0:
                case R1:
                case R2:
                    return 0.5f;
                default:
                    return float.NaN;
            }
        }

        public static bool Valid(int axis) => axis >= L0 && axis <= R2;

        public static bool TryParse(string axisName, out int axis)
        {
            if (string.CompareOrdinal(axisName, "L0") == 0) axis = L0;
            else if (string.CompareOrdinal(axisName, "L1") == 0) axis = L1;
            else if (string.CompareOrdinal(axisName, "L2") == 0) axis = L2;
            else if (string.CompareOrdinal(axisName, "R0") == 0) axis = R0;
            else if (string.CompareOrdinal(axisName, "R1") == 0) axis = R1;
            else if (string.CompareOrdinal(axisName, "R2") == 0) axis = R2;
            else
            {
                axis = -1;
                return false;
            }

            return true;
        }
    }
}
