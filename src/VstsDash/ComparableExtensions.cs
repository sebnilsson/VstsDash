using System;
using System.Collections.Generic;

namespace VstsDash
{
    public static class ComparableExtensions
    {
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            var comparer = Comparer<T>.Default;

            if (comparer == null)
                throw new ArgumentException($"Failed to get default comparer for type '{typeof(T).FullName}'.");

            var isMinGreaterThanMax = comparer.Compare(min, max) > 0;

            if (isMinGreaterThanMax)
                throw new ArgumentOutOfRangeException(nameof(min),
                    "Minimum value cannot be larger than maximum value.");

            var isValueLessThanMin = comparer.Compare(value, min) < 0;
            var isValueGreaterThanMax = !isValueLessThanMin && comparer.Compare(value, max) > 0;

            return isValueLessThanMin ? min : (isValueGreaterThanMax ? max : value);
        }
    }
}