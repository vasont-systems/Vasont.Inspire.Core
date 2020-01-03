//-----------------------------------------------------------------------
// <copyright file="DateExtensionTests.cs" company="GlobalLink Vasont">
//     Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using Vasont.Inspire.Core.Extensions;
    using Xunit;

    /// <summary>
    /// This class provides tests for API date extensions.
    /// </summary>
    [Trait("Category", "Date Extensions")]
    public class DateExtensionTests
    {
        #region Public Properties
        /// <summary>
        /// Gets test values for the add business days test.
        /// </summary>
        public static IEnumerable<object[]> AddBusinessDaysTestValues => new[]
        {
            new object[] { new DateTime(2014, 12, 23), 3, false, new DateTime(2014, 12, 29) },
            new object[] { new DateTime(2014, 12, 29), 3, false, new DateTime(2015, 1, 2) },
            new object[] { new DateTime(2014, 12, 29), 3, true, new DateTime(2015, 1, 1) }
        };

        /// <summary>
        /// Gets test values for the IsHoliday tests.
        /// </summary>
        public static IEnumerable<object[]> IsHolidayTestValues => new[]
        {
            new object[] { new DateTime(2014, 12, 25), true },
            new object[] { new DateTime(2015, 1, 1), true },
            new object[] { new DateTime(2014, 12, 23), false }
        };

        /// <summary>
        /// Gets test values for the IsHoliday tests.
        /// </summary>
        public static IEnumerable<object[]> IsHolidayCustomTestValues => new[]
        {
            new object[] { new DateTime(2014, 12, 25), true, new List<DateTime> { new DateTime(2014, 10, 12) } },
            new object[] { new DateTime(2014, 10, 12), true, new List<DateTime> { new DateTime(2014, 10, 12) } }
        };

        /// <summary>
        /// Gets test values for the FirstDayOfMonth tests.
        /// </summary>
        public static IEnumerable<object[]> FirstDayOfMonthTestValues => new[]
        {
            new object[] { 2015, 1, DayOfWeek.Monday, new DateTime(2015, 1, 5) },
            new object[] { 2015, 1, DayOfWeek.Thursday, new DateTime(2015, 1, 1) },
            new object[] { 2015, 2, DayOfWeek.Monday, new DateTime(2015, 2, 2) }
        };

        /// <summary>
        /// Gets test values for the FirstDayOfMonth tests.
        /// </summary>
        public static IEnumerable<object[]> LastDayOfMonthTestValues => new[]
        {
            new object[] { 2015, 1, DayOfWeek.Friday, new DateTime(2015, 1, 30) },
            new object[] { 2015, 1, DayOfWeek.Saturday, new DateTime(2015, 1, 31) },
            new object[] { 2015, 2, DayOfWeek.Monday, new DateTime(2015, 2, 23) }
        };

        /// <summary>
        /// Gets test values for the TestLocalizedDateToUTCParsing tests.
        /// </summary>
        public static IEnumerable<object[]> LocalizedDateTimeTestValues => new[]
        {
            new object[] { "20180203T100101-0500", new DateTime(2018, 2, 3, 15, 01, 01, DateTimeKind.Utc) },
            new object[] { "20180203T100101-0000", new DateTime(2018, 2, 3, 10, 01, 01, DateTimeKind.Utc) },
            new object[] { "20180203T100101-05:00", new DateTime(2018, 2, 3, 15, 01, 01, DateTimeKind.Utc) },
            new object[] { "20180206T174921-0500", new DateTime(2018, 2, 6, 22, 49, 21, DateTimeKind.Utc) },
            new object[] { "20180203T100101", DateTime.MinValue }
        };
        #endregion

        /// <summary>
        /// Exercises the To UTC Kind date converter extension.
        /// </summary>
        [Fact]
        public void ToUtcKindDateTests()
        {
            DateTime dateTime = new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Local);
            DateTime result = dateTime.ToUtcKindDate();

            Assert.Equal(DateTimeKind.Utc, result.Kind);
        }

        /// <summary>
        /// This method is used to test adding business days to a specified date.
        /// </summary>
        /// <param name="startDate">Contains a start date to add to.</param>
        /// <param name="businessDaysToAdd">Contains the number of business days to add.</param>
        /// <param name="ignoreHolidays">Contains a value indicating whether holidays are taken into account when adding business days.</param>
        /// <param name="expectedDate">Contains the expected date for testing.</param>
        [Theory]
        [MemberData(nameof(AddBusinessDaysTestValues))]
        public void AddBusinessDaysTest(DateTime startDate, int businessDaysToAdd, bool ignoreHolidays, DateTime expectedDate)
        {
            Assert.Equal(expectedDate, startDate.AddBusinessDays(businessDaysToAdd, ignoreHolidays));
        }

        /// <summary>
        /// This method is used to test the IsHoliday date extension.
        /// </summary>
        /// <param name="testDate">Contains the date to test.</param>
        /// <param name="expectedValue">Contains the expected result.</param>
        [Theory]
        [MemberData(nameof(IsHolidayTestValues))]
        public void IsHolidayTest(DateTime testDate, bool expectedValue)
        {
            Assert.Equal(expectedValue, testDate.IsHoliday());
        }

        /// <summary>
        /// This method is used to test the IsHoliday date extension.
        /// </summary>
        /// <param name="testDate">Contains the date to test.</param>
        /// <param name="expectedValue">Contains the expected result.</param>
        /// <param name="customDates">Contains a list of custom holiday dates.</param>
        [Theory]
        [MemberData(nameof(IsHolidayCustomTestValues))]
        public void IsHolidayCustomTest(DateTime testDate, bool expectedValue, List<DateTime> customDates)
        {
            Assert.Equal(expectedValue, testDate.IsHoliday(customDates));
        }

        /// <summary>
        /// This method is used to test the FirstDayOfMonth extension.
        /// </summary>
        /// <param name="year">Contains the year.</param>
        /// <param name="month">Contains the month.</param>
        /// <param name="dayOfWeek">Contains the first day of the week to get.</param>
        /// <param name="expectedValue">Contains the expected date result.</param>
        [Theory]
        [MemberData(nameof(FirstDayOfMonthTestValues))]
        public void FirstDayOfMonthTest(int year, int month, DayOfWeek dayOfWeek, DateTime expectedValue)
        {
            Assert.Equal(expectedValue, DateExtensions.FirstDayOfMonth(year, month, dayOfWeek));
        }

        /// <summary>
        /// This method is used to test the LastDayOfMonth extension.
        /// </summary>
        /// <param name="year">Contains the year.</param>
        /// <param name="month">Contains the month.</param>
        /// <param name="dayOfWeek">Contains the first day of the week to get.</param>
        /// <param name="expectedValue">Contains the expected date result.</param>
        [Theory]
        [MemberData(nameof(LastDayOfMonthTestValues))]
        public void LastDayOfMonthTest(int year, int month, DayOfWeek dayOfWeek, DateTime expectedValue)
        {
            Assert.Equal(expectedValue, DateExtensions.LastDayOfMonth(year, month, dayOfWeek));
        }

        /// <summary>
        /// This method is used to test the parsing extension for the localized string to UTC extension method.
        /// </summary>
        /// <param name="localizedString">Contains the localized date time string to convert to a UTC kind date time object.</param>
        /// <param name="expectedResult">Contains the expected result.</param>
        [Theory]
        [MemberData(nameof(LocalizedDateTimeTestValues))]
        public void TestLocalizedDateToUtcParsing(string localizedString, DateTime expectedResult)
        {
            DateTime result = localizedString.LocalizedEditorDateToUtc();
            Assert.Equal(expectedResult, result);
        }
    }
}
