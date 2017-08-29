using System;

namespace VstsDash
{
    public static class DoubleExtensions
    {
        public static double RoundToHalfs(this double value)
        {
            var floored = Math.Floor(value * 2) / 2;
            return floored;
        }
    }
}