//-----------------------------------------------------------------------
// <copyright file="StringExtensionTests.cs" company="GlobalLink Vasont">
//     Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using System;
    using System.Linq;
    using Vasont.Inspire.Core.Extensions;
    using Xunit;

    /// <summary>
    /// This class is used to house all the extension method tests for the core library.
    /// </summary>
    [Trait("Category", "Extensions")]
    public class StringExtensionTests
    {
        /// <summary>
        /// Tests the truncate extension method.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="truncateLength">Length of the truncate.</param>
        /// <param name="expectedLength">The expected length.</param>
        [Theory]
        [InlineData("01234567890", 5, 5)]
        [InlineData("01234567890", 11, 10)]
        public void StringTruncateTest(string text, int truncateLength, int expectedLength)
        {
            Assert.Equal(expectedLength, text.Truncate(truncateLength).Length);
        }

        /// <summary>
        /// Tests splitting the string by size.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <param name="expectedWordCount">The expected word count.</param>
        [Theory]
        [InlineData("abcabcabcabcabca", 1, 16)]
        [InlineData("abcabcabcabcabca", 2, 8)]
        [InlineData("abcabcabcabcabca", 3, 6)]
        [InlineData("abcabcabcabcabca", 5, 4)]
        [InlineData("abcabcabcabcabca", 7, 3)]
        [InlineData("abcabcabcabcabca", 8, 2)]
        [InlineData("abcabcabcabcabca", 9, 2)]
        [InlineData("abcabcabcabcabca", 16, 1)]
        [InlineData("abcabcabcabcabca", 17, 1)]
        [InlineData("abcabcabcabcabca", 512, 1)]
        [InlineData(null, 512, 0)]
        [InlineData("", 512, 0)]
        [InlineData("abcabcabcabcabca", -1, 0)]
        public void StringChopBySizeTest(string text, int chunkSize, int expectedWordCount)
        {
            // uses code from https://stackoverflow.com/revisions/8944374/5
            // The method can throw two exception types, so test their throwing.
            try
            {
                var words = text.Chop(chunkSize);
                Assert.Equal(expectedWordCount, words.Count());
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Equal(0, expectedWordCount);
            }
            catch (ArgumentNullException)
            {
                Assert.Equal(0, expectedWordCount);
            }
        }

        /// <summary>
        /// This test method tests file names HasGuid extension method.
        /// </summary>
        /// <param name="input">Contains the input file name string.</param>
        /// <param name="containsGuid">Contains a value indicating whether the file name should have a GUID.</param>
        [Theory]
        [InlineData("filename.xml", false)]
        [InlineData("filename_1234.xml", false)]
        [InlineData("filename_{44935658-48B8-4F73-BC4C-8971570EE160}.xml", true)]
        [InlineData("filename_44935658-48B8-4F73-BC4C-8971570EE160.xml", true)]
        [InlineData("filename_{44935658-48B8-4F73-BC4C-8971570EE160}.xml#_abcd/1234", true)]
        [InlineData("filename_44935658-48B8-4F73-BC4C-8971570EE160.xml#./1234", true)]
        public void TestHasGuid(string input, bool containsGuid)
        {
            Assert.Equal(input.ContainsGuid(), containsGuid);
        }

        /// <summary>
        /// This test method tests file names remove GUID extension method.
        /// </summary>
        /// <param name="input">Contains the input file name string.</param>
        /// <param name="expectedResult">Contains the expected output from Remove GUID extension.</param>
        [Theory]
        [InlineData("filename.xml", "filename.xml")]
        [InlineData("filename_1234.xml", "filename_1234.xml")]
        [InlineData("filename_{44935658-48B8-4F73-BC4C-8971570EE160}.xml", "filename.xml")]
        [InlineData("filename_44935658-48B8-4F73-BC4C-8971570EE160.xml", "filename.xml")]
        [InlineData("filename_{44935658-48B8-4F73-BC4C-8971570EE160}.xml#_abcd/1234", "filename.xml#_abcd/1234")]
        [InlineData("filename_44935658-48B8-4F73-BC4C-8971570EE160.xml#./1234", "filename.xml#./1234")]
        public void TestRemoveGuid(string input, string expectedResult)
        {
            Assert.Equal(input.RemoveGuid(), expectedResult);
        }
    }
}
