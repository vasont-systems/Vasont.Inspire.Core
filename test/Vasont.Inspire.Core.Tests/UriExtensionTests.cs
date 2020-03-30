//-------------------------------------------------------------
// <copyright file="UriExtensionTests.cs" company="GlobalLink Vasont">
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
    /// This class is contains tests to support testing URI extensions
    /// </summary>
    [Trait("Category", "Misc Tests")]
    public class UriExtensionTests
    {
        [Theory]
        [InlineData("https://inspire.vasont.com", "-api", "https://inspire-api.vasont.com/")]
        [InlineData("https://inspire-api.vasont.com", "-api", "https://inspire-api.vasont.com/")]
        [InlineData("https://inspire.vasont.com", "", "https://inspire.vasont.com/")]
        public void TestAddApiSuffixBaseUri(string input, string suffix, string expectedResult)
        {
            Uri baseUri = new Uri(input);
            Uri result = UriExtensions.AddApiSuffixBase(baseUri, suffix);
            Assert.Equal(result.ToString(), expectedResult);
        }

        [Theory]
        [InlineData("https://inspire.vasont.com", "-api", "https://inspire-api.vasont.com/")]
        [InlineData("https://inspire-api.vasont.com", "-api", "https://inspire-api.vasont.com/")]
        [InlineData("https://inspire.vasont.com", "", "https://inspire.vasont.com/")]
        public void TestAddApiSuffixBaseString(string input, string suffix, string expectedResult)
        {
            string result = UriExtensions.AddApiSuffixBase(input, suffix);
            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData("https://inspire-api.vasont.com", "-api", "https://inspire.vasont.com/")]
        [InlineData("https://inspire.vasont.com", "-api", "https://inspire.vasont.com/")]
        [InlineData("https://inspire-api.vasont.com", "", "https://inspire-api.vasont.com/")]
        public void TestStripApiSuffixBaseUri(string input, string suffix, string expectedResult)
        {
            Uri baseUri = new Uri(input);
            Uri result = UriExtensions.StripApiSuffixBase(baseUri, suffix);
            Assert.Equal(result.ToString(), expectedResult);
        }

        [Theory]
        [InlineData("https://inspire-api.vasont.com", "-api", "https://inspire.vasont.com/")]
        [InlineData("https://inspire.vasont.com", "-api", "https://inspire.vasont.com/")]
        [InlineData("https://inspire-api.vasont.com", "", "https://inspire-api.vasont.com/")]
        public void TestStripApiSuffixBaseString(string input, string suffix, string expectedResult)
        {
            string result = UriExtensions.StripApiSuffixBase(input, suffix);
            Assert.Equal(result, expectedResult);
        }
    }
}
