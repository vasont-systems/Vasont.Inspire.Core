//-----------------------------------------------------------------------
// <copyright file="ResourceExtensions.cs" company="GlobalLink Vasont">
//     Copyright (c) GlobalLink Vasont. All rights reserved.
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
        /// This method is used to build the assembly path to a resource.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>Returns the complete resource path string.</returns>
        public static string BuildAssemblyPath(string keyName, string folder = "Resources", string assemblyName = "Vasont.Inspire.Core")
        {
            return string.Join(".", new[] { assemblyName, folder, keyName }.Where(x => !string.IsNullOrEmpty(x)));
        }

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
            string path = BuildAssemblyPath(keyName, folder, assemblyName);
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
    }
}
