//-------------------------------------------------------------
// <copyright file="LocaleExtensionTests.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using System.Globalization;
    using Extensions;
    using Xunit;

    /// <summary>
    /// Contains tests related to locale extension methods.
    /// </summary>
    public class LocaleExtensionTests
    {
        /// <summary>
        /// This method is used to test locale string encoding conversion.
        /// </summary>
        /// <param name="input">Contains the input value.</param>
        /// <param name="expectedOutput">Contains the expected output value.</param>
        /// <param name="getLocale">Contains a value indicating if the test is to get a locale code.</param>
        [Theory]
        [InlineData("en-US", "en")]
        [InlineData("en", "en-US", true)]
        [InlineData("xxx", "en-US", true)]
        [InlineData("xxx", "en")]
        [InlineData("en-US", "en-US", true)]
        [InlineData("de", "de")]
        [InlineData("de", "de-DE", true)]
        [InlineData("de-", "de-DE", true)]
        [InlineData("de-DE", "de-DE", true)]
        [InlineData("english", "en", false)]
        [InlineData("Chinese", "en", false)]
        public void TestCultureCodes(string input, string expectedOutput, bool getLocale = false)
        {
            Assert.Equal(expectedOutput, getLocale ? input.ToLocalCultureCode() : input.ToNeutralCultureCode());
        }

        /// <summary>
        /// Cultures the information tests.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="successExpected">if set to <c>true</c> [success expected].</param>
        [Theory]
        [InlineData("en", true)]
        [InlineData("xxx", false)]
        public void CultureInfoTests(string input, bool successExpected)
        {
            CultureInfo info = input.ToCultureInfo();

            if (successExpected)
            {
                Assert.NotNull(info);
                Assert.Equal(input, info.TwoLetterISOLanguageName);
            }
            else
            {
                Assert.Null(info);
            }
        }
    }
}
