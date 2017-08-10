using System;
using System.Collections.Generic;
using System.Linq;

namespace VstsDash
{
    public static class WorkDatesUtility
    {
        public static IEnumerable<DateTime> GetWorkDates(IEnumerable<DateTime> dates)
        {
            var workDays = from date in dates
                           let dayOfWeek = (int)date.DayOfWeek
                           let isWeekDay = dayOfWeek >= 1 && dayOfWeek <= 5
                           where isWeekDay
                           select date;
            return workDays;
        }
    }
}