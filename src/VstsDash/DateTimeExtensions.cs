using System;
using System.Collections.Generic;
using System.Linq;

namespace VstsDash
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> GetDatesUntil(this DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (startDate > endDate)
                throw new ArgumentOutOfRangeException(nameof(startDate), "Start-date cannot be after end-date.");

            var dayDifferenceCount = (int)(endDate - startDate).TotalDays;

            var dates = Enumerable.Range(0, dayDifferenceCount + 1).Select(i => startDate.AddDays(i));
            return dates;
        }

        public static IEnumerable<DateTime> GetWorkDatesUntil(this DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentOutOfRangeException(nameof(startDate), "Start-date cannot be after end-date.");

            var dates = GetDatesUntil(startDate, endDate);

            var workDays = WorkDatesUtility.GetWorkDates(dates);
            return workDays;
        }

        public static double ToEpoch(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}