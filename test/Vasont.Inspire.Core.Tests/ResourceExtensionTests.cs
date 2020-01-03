//-----------------------------------------------------------------------
// <copyright file="ResourceExtensionTests.cs" company="GlobalLink Vasont">
//     Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Vasont.Inspire.Core.Extensions;
    using Xunit;

    /// <summary>
    /// This class provides tests for API resource extensions.
    /// </summary>
    [Trait("Category", "Resource Extensions")]
    public class ResourceExtensionTests
    {
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
        /// This method will retrieve an embedded resource string within the assembly.
        /// </summary>
        /// <param name="keyName">Contains the resource key name.</param>
        /// <param name="folder">Contains an optional resource folder path.</param>
        /// <param name="assemblyName">Contains an optional assembly name.</param>
        /// <returns>Returns the contents found.</returns>
        public static string GetEmbeddedResourceString(string keyName, string folder = "Resources", string assemblyName = "Vasont.Inspire.Core")
        {
            string contents;

            using (var manifestStream = GetEmbeddedResourceStream(keyName, folder, assemblyName))
            using (var reader = new StreamReader(manifestStream ?? throw new InvalidOperationException()))
            {
                contents = reader.ReadToEnd();
            }

            return contents;
        }

        /// <summary>
        /// This method will retrieve an embedded resource stream within the assembly.
        /// </summary>
        /// <param name="keyName">Contains the resource key name.</param>
        /// <param name="folder">Contains an optional resource folder path.</param>
        /// <param name="assemblyName">Contains an optional assembly name.</param>
        /// <returns>Returns the contents found.</returns>
        public static Stream GetEmbeddedResourceStream(string keyName, string folder = "Resources", string assemblyName = "Vasont.Inspire.Core")
        {
            string path = ResourceExtensions.BuildAssemblyPath(keyName, folder, assemblyName);
            var assembly = Assembly.Load(assemblyName);
            string[] manifestResources = assembly.GetManifestResourceNames();

            return manifestResources.Contains(path) ? assembly.GetManifestResourceStream(path) : null;
        }

        /// <summary>
        /// This method will retrieve an embedded resource image within the assembly.
        /// </summary>
        /// <param name="keyName">Contains the resource key name.</param>
        /// <param name="folder">Contains an optional resource folder path.</param>
        /// <param name="assemblyName">Contains an optional assembly name.</param>
        /// <returns>Returns the contents found.</returns>
        public static byte[] GetEmbeddedResourceImage(string keyName, string folder = "Resources", string assemblyName = "Vasont.Inspire.Core")
        {
            byte[] returnValue;

            using (Stream manifestStream = GetEmbeddedResourceStream(keyName, folder, assemblyName))
            {
                if (manifestStream == null)
                {
                    throw new InvalidOperationException();
                }

                returnValue = new byte[manifestStream.Length];
                manifestStream.Read(returnValue, 0, returnValue.Length);
            }

            return returnValue;
        }

        /// <summary>
        /// This method is used to test the build of the assembly path to a resource.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="expectedResult">The expected result.</param>
        [Theory]
        [InlineData("TestStringText", "Resources", "Vasont.Inspire.Core.Tests", "Vasont.Inspire.Core.Tests.Resources.TestStringText")]
        public void BuildAssemblyPathTest(string keyName, string folder, string assemblyName, string expectedResult)
        {
            Assert.Equal(expectedResult, ResourceExtensions.BuildAssemblyPath(keyName, folder, assemblyName));
        }
    }
}
