//-------------------------------------------------------------
// <copyright file="TypeExtensionTests.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using Vasont.Inspire.Core.Extensions;
    using Xunit;

    /// <summary>
    /// This class is contains tests to support testing type extensions
    /// </summary>
    [Trait("Category", "Misc Tests")]
    public class TypeExtensionTests
    {
        /// <summary>
        /// Contains an enumerated item list for testing description attribute retrieval.
        /// </summary>
        public enum DescriptionTestEnumeration
        {
            /// <summary>
            /// Alpha test
            /// </summary>
            [Description("Value 1")]
            Alpha,

            /// <summary>
            /// Beta test
            /// </summary>
            [Description("Value 2")]
            Beta
        }

        /// <summary>
        /// Gets test values for the ToDecimal tests.
        /// </summary>
        public static IEnumerable<object[]> ToDecimalTestValues => new[]
        {
            new object[] { string.Empty, 0.0, 0 },
            new object[] { "NaN", 0.1, 0.1 },
            new object[] { "1.5", 0.0, 1.5 },
            new object[] { "79,228,162,514,264,337,593,543,950,335", 0, decimal.MaxValue },
            new object[] { "79228162514264337593543950335", 0, decimal.MaxValue },
            new object[] { "-79228162514264337593543950335", 0, decimal.MinValue },
            new object[] { "7922816251426433759354395033579228162514264337593543950335", 0, 0 }
        };

        /// <summary>
        /// Gets test values for the ConvertToString tests.
        /// </summary>
        public static IEnumerable<object[]> ConvertToStringTestValues => new[]
        {
            new object[] { DBNull.Value, "null", "null" },
            new object[] { null, string.Empty, string.Empty },
            new object[] { new string("some value"), string.Empty, "some value" }
        };

        /// <summary>
        /// Gets test values for the ToDateTime tests.
        /// </summary>
        public static IEnumerable<object[]> ToDateTimeTestValues => new[]
        {
            new object[] { "2019-01-01", null, null, DateTimeStyles.None, new DateTime(2019, 1, 1, 0, 0, 0), DateTimeKind.Unspecified },
            new object[] { "2019-01-01", null, CultureInfo.InvariantCulture, DateTimeStyles.None, new DateTime(2019, 1, 1, 0, 0, 0), DateTimeKind.Unspecified },
            new object[] { "01/01/2019 10:00 AM", null, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, new DateTime(2019, 1, 1, 10, 0, 0), DateTimeKind.Unspecified },
            new object[] { "01/01/2019 10:00 AM", null, CultureInfo.CreateSpecificCulture("en-US"),  DateTimeStyles.AssumeLocal, new DateTime(2019, 1, 1, 10, 0, 0), DateTimeKind.Local },
            new object[] { "2019/01/01T10:00:00-5:00", null, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeLocal, new DateTime(2019, 1, 1, 10, 0, 0), DateTimeKind.Local },
            new object[] { "03/01/2009T10:00:00-5:00", null, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeLocal, DateTime.MinValue, DateTimeKind.Unspecified },
        };

        /// <summary>
        /// String to Guid tests.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Theory]
        [InlineData("", "00000000-0000-0000-0000-000000000000")]
        [InlineData("123", "00000000-0000-0000-0000-000000000000")]
        [InlineData("506934FF-F8E4-4F0F-8FAC-C238DDD48910", "506934FF-F8E4-4F0F-8FAC-C238DDD48910")]
        public void ToGuidTests(string input, string expectedResult)
        {
            Assert.Equal(expectedResult, input.ToGuid().ToString().ToUpperInvariant());
        }

        /// <summary>
        /// String to int tests.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="expectedValue">The expected value.</param>
        [Theory]
        [InlineData("", 0, 0)]
        [InlineData("NaN", 0, 0)]
        [InlineData("1.5", 0, 0)]
        [InlineData("2,147,483,647", 0, 0)]
        [InlineData("2147483647", 0, 2147483647)]
        [InlineData("-2147483648", 0, -2147483648)]
        [InlineData("9223372036854775807", 0, 0)]
        public void ToIntTests(string value, int defaultValue, int expectedValue)
        {
            Assert.Equal(expectedValue, value.ToInt(defaultValue));
        }

        /// <summary>
        /// String to long tests.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="expectedValue">The expected value.</param>
        [Theory]
        [InlineData("", 0, 0)]
        [InlineData("NaN", 0, 0)]
        [InlineData("1.5", 0, 0)]
        [InlineData("9,223,372,036,854,775,807", 0, 0)]
        [InlineData("9223372036854775807", 0, 9223372036854775807)]
        [InlineData("-9223372036854775808", 0, -9223372036854775808)]
        [InlineData("79228162514264337593543950335M", 0, 0)]
        public void ToLongTests(string value, long defaultValue, long expectedValue)
        {
            Assert.Equal(expectedValue, value.ToLong(defaultValue));
        }

        /// <summary>
        /// String to Decimal tests.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="expectedValue">The expected value.</param>
        [Theory]
        [MemberData(nameof(ToDecimalTestValues))]
        public void ToDecimalTests(string value, decimal defaultValue, decimal expectedValue)
        {
            Assert.Equal(expectedValue, value.ToDecimal(defaultValue));
        }

        /// <summary>
        /// Tests string conversion tests.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="expectedValue">The expected value.</param>
        [Theory]
        [MemberData(nameof(ConvertToStringTestValues))]
        public void ConvertToStringTests(object value, string defaultValue, string expectedValue)
        {
            Assert.Equal(expectedValue, value.ConvertToString(defaultValue));
        }

        /// <summary>
        /// Tests the to Date time conversion.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="dateFormatter">The date formatter.</param>
        /// <param name="dateTimeStyles">The date time styles.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="expectedKind">The expected kind.</param>
        [Theory]
        [MemberData(nameof(ToDateTimeTestValues))]
        public void ToDateTimeTests(string inputValue, DateTime? defaultValue, IFormatProvider dateFormatter, DateTimeStyles dateTimeStyles, DateTime expectedValue, DateTimeKind expectedKind)
        {
            DateTime result = inputValue.ToDateTime(defaultValue, dateFormatter, dateTimeStyles);
            Assert.Equal(expectedValue, result);
            Assert.Equal(expectedKind, result.Kind);
        }

        /// <summary>
        /// Tests ToEnum conversions.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Theory]
        [InlineData("asc", true, SortDirection.Desc, SortDirection.Asc)]
        [InlineData("asc", false, SortDirection.Desc, SortDirection.Asc)]
        [InlineData("Descending", true, SortDirection.Asc, SortDirection.Asc)]
        [InlineData("", true, SortDirection.Desc, SortDirection.Desc)]
        public void ToEnumTests(string inputValue, bool ignoreCase, SortDirection defaultValue, SortDirection expectedResult)
        {
            Assert.Equal(expectedResult, inputValue.ToEnum(ignoreCase, defaultValue));
        }

        /// <summary>
        /// Converts to boolean tests.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <param name="expectedValue">if set to <c>true</c> [expected value].</param>
        [Theory]
        [InlineData("T", false, true)]
        [InlineData("true", false, true)]
        [InlineData("1", false, true)]
        [InlineData("y", false, true)]
        [InlineData("Yes", false, true)]
        [InlineData("O", false, true)]
        [InlineData("", false, false)]
        [InlineData(" ", false, false)]
        [InlineData("No", false, false)]
        [InlineData("0", true, false)]
        public void ToBooleanTests(string inputValue, bool defaultValue, bool expectedValue)
        {
            Assert.Equal(expectedValue, inputValue.ToBoolean(defaultValue));
        }

        /// <summary>
        /// Tests the retrieval of the enumeration description value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expectedOutput">The expected output.</param>
        [Theory]
        [InlineData(DescriptionTestEnumeration.Alpha, "Value 1")]
        [InlineData(DescriptionTestEnumeration.Beta, "Value 2")]
        public void ToDescriptionTests(DescriptionTestEnumeration input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, input.ToDescription(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// 64-bit integer ZeroToNull tests.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Theory]
        [InlineData(9223372036854775807, 9223372036854775807)]
        [InlineData(0, null)]
        public void ZeroLongToNullTests(long? value, long? expectedResult)
        {
            Assert.Equal(expectedResult, value.ZeroToNull());

            long exactValue = value ?? 0;
            Assert.Equal(expectedResult, exactValue.ZeroToNull());
        }

        /// <summary>
        /// 32-bit integer ZeroToNull tests.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, null)]
        public void ZeroIntToNullTests(int? value, int? expectedResult)
        {
            Assert.Equal(expectedResult, value.ZeroToNull());

            int exactValue = value ?? 0;
            Assert.Equal(expectedResult, exactValue.ZeroToNull());
        }
    }
}