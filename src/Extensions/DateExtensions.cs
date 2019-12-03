//-----------------------------------------------------------------------
// <copyright file="DateExtensions.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// This class contains all date related extension methods.
    /// </summary>
    public static class DateExtensions
    {
        /// <summary>
        /// This method is used to add business days to the target date.
        /// </summary>
        /// <param name="date">Contains the target date to add business days to.</param>
        /// <param name="days">Contains the number of business days to add. Negative values will remove business days from the target date.</param>
        /// <param name="ignoreHolidays"><c>true</c> to ignore holidays; otherwise <c>false</c>.</param>
        /// <param name="holidays">Contains an optional list of holiday dates to compare.</param>
        /// <returns>Returns the new date with the added business days.</returns>
        public static DateTime AddBusinessDays(this DateTime date, int days, bool ignoreHolidays = true, List<DateTime> holidays = null)
        {
            int direction = days < 0 ? -1 : 1;
            DateTime resultValue = date;

            while (days != 0)
            {
                resultValue = resultValue.AddDays(direction);

                if (resultValue.DayOfWeek != DayOfWeek.Saturday && resultValue.DayOfWeek != DayOfWeek.Sunday && (ignoreHolidays || !resultValue.IsHoliday(holidays)))
                {
                    days -= direction;
                }
            }

            return resultValue;
        }

        /// <summary>
        /// This method is used to determine if the specified date is a holiday.
        /// </summary>
        /// <param name="date">Contains the date value to validate.</param>
        /// <param name="holidays">Contains an optional list of holiday dates to check.</param>
        /// <returns>Returns a value indicating whether the date is a valid holiday.</returns>
        public static bool IsHoliday(this DateTime date, List<DateTime> holidays = null)
        {
            // do some basic US holidays
            DateTime[] basicHolidays =
            {
                new DateTime(date.Year, 1, 1).AdjustForWeekendHoliday(),     // New years
                LastDayOfMonth(date.Year, 5, DayOfWeek.Monday),              // Memorial day - last Monday in May
                new DateTime(date.Year, 7, 4).AdjustForWeekendHoliday(),     // Independence day
                FirstDayOfMonth(date.Year, 9, DayOfWeek.Monday),             // Labor day - first Monday in Sept
                new DateTime(date.Year, 11, Enumerable.Range(1, 30)          // Thanksgiving - 4th Thursday of Nov
                    .Where(d => new DateTime(date.Year, 11, d).DayOfWeek == DayOfWeek.Thursday)
                    .ElementAt(3)),
                new DateTime(date.Year, 12, 25).AdjustForWeekendHoliday()    // Christmas
            };

            bool result = basicHolidays.Contains(date.Date);

            if (holidays != null && holidays.Any() && !result)
            {
                result = holidays.Any(d => d.Date == date.Date);
            }

            return result;
        }

        /// <summary>
        /// This method is used to return the date of the first day of the week in the specified month.
        /// </summary>
        /// <param name="year">Contains the date year.</param>
        /// <param name="month">Contains the date month.</param>
        /// <param name="dayOfWeek">Contains the day of the week to find.</param>
        /// <returns>Returns the date of the specified target day.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Year and month values must be valid.</exception>
        public static DateTime FirstDayOfMonth(int year, int month, DayOfWeek dayOfWeek)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month));
            }

            DateTime result = new DateTime(year, month, 1);

            while (result.DayOfWeek != dayOfWeek)
            {
                result = result.AddDays(1);
            }

            return result;
        }

        /// <summary>
        /// This method is used to return the date of the last day of the week in the specified month.
        /// </summary>
        /// <param name="year">Contains the date year.</param>
        /// <param name="month">Contains the date month.</param>
        /// <param name="dayOfWeek">Contains the day of the week to find.</param>
        /// <returns>Returns the date of the specified target day.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Year and month values must be valid.</exception>
        public static DateTime LastDayOfMonth(int year, int month, DayOfWeek dayOfWeek)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month));
            }

            DateTime result = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            while (result.DayOfWeek != dayOfWeek)
            {
                result = result.AddDays(-1);
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert a localized date time string provided by editors into a
        /// UTC date time value.
        /// </summary>
        /// <param name="editorFormattedLocalizedDateTime">Contains the localized date time string to convert to a UTC kind date time object.</param>
        /// <param name="dateTimeFormat">Contains an optional date time format string to parse. The default format is <c>yyyyMMddThhmmsszzz</c></param>
        /// <returns>Returns a UTC date time value for the specified local date time string.</returns>
        public static DateTime LocalizedEditorDateToUtc(this string editorFormattedLocalizedDateTime, string dateTimeFormat = "yyyyMMddTHHmmsszzz")
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime output;
            DateTime.TryParseExact(editorFormattedLocalizedDateTime, dateTimeFormat, provider, DateTimeStyles.AdjustToUniversal, out output);
            return output;
        }

        /// <summary>
        /// This method is used to return UTC date time to ISO 8601 format with zero time zone
        /// offset. XML editor will display user's local time in the browser.
        /// </summary>
        /// <param name="utcDateTime">Contains the UTC date time that will be converted to a string.</param>
        /// <param name="dateTimeFormat">Contains the optional date time format string to parse. The default format is <c>yyyyMMddThhmmss</c></param>
        /// <returns>Returns formatted text for the localized date time.</returns>
        public static string UniversalEditorDateToString(this DateTime utcDateTime, string dateTimeFormat = "yyyyMMddTHHmmss")
        {
            // expected ISO 8601 format: 20180301T110101-0000
            return utcDateTime.ToString(dateTimeFormat, CultureInfo.InvariantCulture) + "+0000";
        }

        #region Private Methods

        /// <summary>
        /// This method is used to adjust holiday dates that occur on a weekend.
        /// </summary>
        /// <param name="holiday">Contains the holiday date.</param>
        /// <returns>Returns the corrected date if holiday falls on a weekend day.</returns>
        private static DateTime AdjustForWeekendHoliday(this DateTime holiday)
        {
            if (holiday.DayOfWeek == DayOfWeek.Saturday)
            {
                holiday = holiday.AddDays(-1);
            }
            else if (holiday.DayOfWeek == DayOfWeek.Sunday)
            {
                holiday = holiday.AddDays(1);
            }

            return holiday;
        }

        #endregion
    }
}