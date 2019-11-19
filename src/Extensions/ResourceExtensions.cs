//-----------------------------------------------------------------------
// <copyright file="ResourceExtensions.cs" company="Vasont Systems">
//     Copyright (c) Vasont Systems. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This class contains numerous extensions to support embedded resources within .NET Core framework.
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// This method will retrieve an embedded resource string within the assembly.
        /// </summary>
        /// <param name="keyName">Contains the resource key name.</param>
        /// <param name="folder">Contains an optional resource folder path.</param>
        /// <param name="assemblyName">Contains an optional assembly name.</param>
        /// <returns>Returns the contents found.</returns>
        public static string GetEmbeddedResourceString(string keyName, string folder = "Resources", string assemblyName = "Vasont.Inspire.Core")
        {
            string contents = null;
            string path = string.Join('.', new[] { assemblyName, folder, keyName }.Where(x => !string.IsNullOrEmpty(x)));
            var assembly = Assembly.Load(assemblyName);
            string[] manifestResources = assembly.GetManifestResourceNames();

            if (manifestResources.Contains(path))
            {
                using (var manifestStream = assembly.GetManifestResourceStream(path))
                using (var reader = new StreamReader(manifestStream ?? throw new InvalidOperationException()))
                {
                    contents = reader.ReadToEnd();
                }
            }

            return contents;
        }

        /// <summary>
        /// This method will retrieve an embedded resource image within the assembly.
        /// </summary>
        /// <param name="keyName">Contains the resource key name.</param>
        /// <param name="subFolderPath">Contains an optional resource folder path.</param>
        /// <returns>Returns the contents found.</returns>
        public static byte[] GetEmbeddedResourceImage(string keyName, string subFolderPath = "")
        {
            byte[] returnValue = null;
            string path = "Vasont.Inspire.Core.Resources." + subFolderPath + keyName;
            var assembly = typeof(Properties.Resources).Assembly;
            string[] manifestResources = assembly.GetManifestResourceNames();

            if (manifestResources.Contains(path))
            {
                using (var manifestStream = assembly.GetManifestResourceStream(path))
                {
                    if (manifestStream == null)
                    {
                        throw new InvalidOperationException();
                    }

                    returnValue = new byte[manifestStream.Length];
                    manifestStream.Read(returnValue, 0, returnValue.Length);
                }
            }

            return returnValue;
        }
    }
}
