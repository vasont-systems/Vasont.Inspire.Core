//-----------------------------------------------------------------------
// <copyright file="StorageExtensionTests.cs" company="GlobalLink Vasont">
//     Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using Vasont.Inspire.Core.Storage;
    using Xunit;

    /// <summary>
    /// This class is used to house all the extension method tests for the core library.
    /// </summary>
    [Trait("Category", "Extensions")]
    // ReSharper disable once InconsistentNaming
    public class StorageExtensionTests
    {
        /// <summary>
        /// This method is used to test the path add separator extensions.
        /// </summary>
        /// <param name="folderPath">Contains the source folder path.</param>
        /// <param name="expectedResult">Contains the expected result path.</param>
        [Theory]
        [InlineData(@"C:\MyDir\MySubDir", @"C:\MyDir\MySubDir\")]
        [InlineData(@"C:\", @"C:\")]
        public void AddPathSeparatorTest(string folderPath, string expectedResult)
        {
            Assert.Equal(expectedResult, Paths.AddPathSeparator(folderPath));
        }

        /// <summary>
        /// This method is used to test the path remove separator extensions.
        /// </summary>
        /// <param name="folderPath">Contains the source folder path.</param>
        /// <param name="expectedResult">Contains the expected result path.</param>
        [Theory]
        [InlineData(@"C:\MyDir\MySubDir\", @"C:\MyDir\MySubDir")]
        [InlineData(@"C:\", @"C:\")]
        public void RemovePathSeparatorTest(string folderPath, string expectedResult)
        {
            Assert.Equal(expectedResult, Paths.RemovePathSeparator(folderPath));
        }

        /// <summary>
        /// This method is used to test the return parent directory extension.
        /// </summary>
        /// <param name="folderPath">Contains the source folder path.</param>
        /// <param name="expectedResult">Contains the expected result path.</param>
        [Theory]
        [InlineData(@"C:\MyDir\MySubDir\", @"C:\MyDir")]
        [InlineData(@"C:\MyDir\MySubDir", @"C:\MyDir")]
        [InlineData(@"C:\MyDir\MySubDir\AnotherSubDir", @"C:\MyDir\MySubDir")]
        [InlineData(@"C:\MyDir", @"C:\")]
        [InlineData(@"C:\MyDir\", @"C:\")]
        [InlineData(@"C:\", @"C:\")]
        public void ParentDirectoryTest(string folderPath, string expectedResult)
        {
            Assert.Equal(expectedResult, Paths.ParentDirectory(folderPath));
        }

        /// <summary>
        /// This test is used to determine if a specified content type returns true for is image method.
        /// </summary>
        /// <param name="contentType">Contains the content type to test.</param>
        /// <param name="expectedResult">Contains a value indicating the expected result.</param>
        [Theory]
        [InlineData("image/png", true)]
        [InlineData("image/GIF", true)]
        [InlineData("image/randomValue", false)]
        [InlineData("application/xml", false)]
        public void IsImageTest(string contentType, bool expectedResult)
        {
            Assert.Equal(expectedResult, Files.IsImageContent(contentType));
        }

        /// <summary>
        /// This test is used to find the MIME content type by extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Theory]
        [InlineData("noextension", "application/octet-stream")]
        [InlineData("image.jpg", "image/jpeg")]
        [InlineData("image.png", "image/png")]
        [InlineData("image.gif", "image/gif")]
        [InlineData("image.xml", "application/xml")]
        [InlineData("tranform.xsl", "application/xml")]
        [InlineData("tranform.xslt", "application/xslt+xml")]
        public void FindMimeContentTypeByExtensionTest(string fileName, string expectedResult)
        {
            Assert.Equal(expectedResult, Files.FindMimeContentTypeByExtension(fileName));
        }

        /// <summary>
        /// This test is used to determine if a specified content type returns true for is image method.
        /// </summary>
        /// <param name="contentType">Contains the content type to test.</param>
        /// <param name="expectedResult">Contains a value indicating the expected result.</param>
        [Theory]
        [InlineData("application/xml", true)]
        [InlineData("text/Html", true)]
        [InlineData("text/randomValue", false)]
        [InlineData("text/png", false)]
        public void IsXmlTest(string contentType, bool expectedResult)
        {
            Assert.Equal(Files.IsXmlContent(contentType), expectedResult);
        }

        /// <summary>
        /// This test ensures that invalid file name characters are removed from a file name string
        /// </summary>
        /// <param name="fileName">Contains the file name string test data</param>
        /// <param name="result">Contains the expected result of the test</param>
        [Theory]
        [InlineData("\"<>|\bfile.xml", "file.xml")]
        public void CleanFileNameTest(string fileName, string result)
        {
            Assert.Equal(result, Files.CleanFileName(fileName));
        }

        /// <summary>
        /// This test ensures that invalid path characters are removed from a path string
        /// </summary>
        /// <param name="folderPath">Contains the file name string test data</param>
        /// <param name="result">Contains the expected result of the test</param>
        [Theory]
        [InlineData(@"C:|\temp\", @"C:\temp\")]
        public void CleanPathNameTest(string folderPath, string result)
        {
            Assert.Equal(result, Paths.CleanPath(folderPath));
        }

        /// <summary>
        /// This test ensures that the GetFileName method can retrieve file names from paths and URI correctly.
        /// </summary>
        /// <param name="fileUri">Contains the file URI value to retrieve a file name from.</param>
        [Theory]
        [InlineData("C:\\temp\\testing.html")]
        [InlineData("http://somewhere.com/testing.html")]
        [InlineData("https://somewhere.com/testing.html?param=1234")]
        [InlineData("file://C:/temp/testing.html")]
        public void FilesGetFileNameText(string fileUri)
        {
            Assert.Equal("testing.html", Paths.GetFileName(fileUri));
        }
    }
}
