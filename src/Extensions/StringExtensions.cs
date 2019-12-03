//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Properties;

    /// <summary>
    /// This class contains methods to aid in manipulating strings.
    /// </summary>
    public static class StringExtensions
    {
        #region Private Fields
        /// <summary>
        /// This is the value that will be used when encoding and decoding ampersands.
        /// </summary>
        private const string AmpersandConvertValue = "~VsntAmp~";
        #endregion Private Fields

        /// <summary>
        /// This extension method is used to write a string to the specified stream/
        /// </summary>
        /// <param name="stream">Contains the stream that will be written to.</param>
        /// <param name="content">Contains the string content to write to the stream.</param>
        /// <param name="encoder">Contains the optional encoder used to define the string encoding.</param>
        public static void WriteString(this Stream stream, string content, Encoding encoder = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] outputBytes = content.ToByteArray(encoder);
            stream.Write(outputBytes, 0, outputBytes.Length);
        }

        /// <summary>
        /// This method is used to read and return a string of data from a specified stream.
        /// </summary>
        /// <param name="stream">Contains the stream to read string data from.</param>
        /// <param name="encoder">Contains an optional encoder to use for reading the byte data.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Contains an optional value indicating that byte order marks should be used to determine encoding.</param>
        /// <param name="bufferSize">Contains an optional byte buffer read size.</param>
        /// <returns>Returns stream data as a string.</returns>
        public static string ReadString(this Stream stream, Encoding encoder = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 4096)
        {
            string result;

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (TextReader reader = new StreamReader(stream, encoder ?? Encoding.Default, detectEncodingFromByteOrderMarks, bufferSize))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// This extension method is used to convert a string to an array of bytes using the specified optional encoder.
        /// </summary>
        /// <param name="content">Contains the content string to convert to bytes.</param>
        /// <param name="encoder">Contains the optional encoder to use during the conversion. A default Unicode encoder is used if not specified.</param>
        /// <returns>Returns the specified string as a byte array.</returns>
        public static byte[] ToByteArray(this string content, Encoding encoder = null)
        {
            content = content ?? string.Empty;
            Encoding encoderUsed = encoder ?? Encoding.Default;
            return encoderUsed.GetBytes(content);
        }

        /// <summary>
        /// This extension method is used to convert a byte array to a string using the specified optional encoder.
        /// </summary>
        /// <param name="content">Contains the byte array to convert to a string.</param>
        /// <param name="encoder">Contains the optional encoder to use. Uses the Default encoder if not specified.</param>
        /// <returns>Returns the bytes as a string.</returns>
        public static string ArrayToString(this byte[] content, Encoding encoder = null)
        {
            if (encoder == null)
            {
                encoder = System.Text.Encoding.Default;
            }

            return content != null ? encoder.GetString(content) : string.Empty;
        }

        /// <summary>
        /// This extension method is used to determine if a string contains Base-64 encoded text.
        /// </summary>
        /// <param name="content">Contains the content to analyze.</param>
        /// <returns>Returns a value indicating whether the content string contains Base-64 encoded text.</returns>
        public static bool IsBase64String(this string content)
        {
            content = content.Trim();
            return (content.Length % 4 == 0) && Regex.IsMatch(content, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        /// This extension method is used to convert a byte array into Base-64 encoded string.
        /// </summary>
        /// <param name="content">Contains the byte array to convert to a Base-64 encoded string.</param>
        /// <returns>Returns the Base-64 encoded string.</returns>
        public static string Base64Encode(this byte[] content)
        {
            return Convert.ToBase64String(content);
        }

        /// <summary>
        /// This extension method is used to convert a Base-64 encoded string into a binary byte array.
        /// </summary>
        /// <param name="content">Contains the Base-64 encoded string to convert.</param>
        /// <returns>Returns the decoded string content or the content if not a Base-64 string as a UTF8 byte array.</returns>
        public static byte[] Base64Decode(this string content)
        {
            if (content.IsBase64String())
            {
                return Convert.FromBase64String(content);
            }
            else
            {
                return Encoding.UTF8.GetBytes(content);
            }
        }

        /// <summary>
        /// This method is used to parse out any orphaned inline elements.
        /// </summary>
        /// <param name="text">Contains the text to parse.</param>
        /// <returns>Returns the text with all TGID inline elements removed.</returns>
        public static string StripOrphanedInlineTags(this string text)
        {
            string result = text ?? string.Empty;

            int attributeMarkerStartIndex = result.IndexOf(" TGID=\"", StringComparison.Ordinal);

            // if we find a marker that has an attribute of TGID...
            while (attributeMarkerStartIndex != -1)
            {
                // work our way backwards from the starting point, get the marker starting point
                int markerStartIndex = result.LastIndexOf('<', attributeMarkerStartIndex);

                if (markerStartIndex != -1)
                {
                    int markerEndIndex = result.IndexOf('>', attributeMarkerStartIndex);

                    if (markerEndIndex != -1)
                    {
                        // we've found a marker start and end point, remove the marker from the text
                        result = result.Substring(0, markerStartIndex) + result.Substring(markerEndIndex + 1);
                    }
                    else
                    {
                        // no marker end point found, so break out
                        break;
                    }
                }
                else
                {
                    // no marker start point found, so break out
                    break;
                }

                // get the next inline attribute to work if any
                attributeMarkerStartIndex = result.IndexOf(" TGID=\"", StringComparison.Ordinal);
            }

            return result;
        }

        /// <summary>
        /// This extension method is used to split a large string into an enumerated list of smaller strings of the a length no larger than the 
        /// specified maximum length.
        /// </summary>
        /// <param name="text">Contains the text that is to be split into an enumerable list of strings of the specified maximum length.</param>
        /// <param name="maxLength">Contains the maximum size of each individual split string value returned.</param>
        /// <returns>
        /// Contains an enumerated list of strings that have been split apart from the source string.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><c>maxLength</c> must be greater than 0.</exception>
        public static IEnumerable<string> Chop(this string text, int maxLength)
        {
            if (maxLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), string.Format(CultureInfo.InvariantCulture, Resources.ArgumentMustBeGreaterThanZeroErrorFormat, "maxLength"));
            }

            if (text == null)
            {
                text = string.Empty;
            }

            return text.Select((x, i) => i)
                       .Where(i => i % maxLength == 0)
                       .Select(i => text.Substring(i, text.Length - i >= maxLength ? maxLength : text.Length - i));
        }

        /// <summary>
        /// This method is used to determine if a specified string matches a regular expression.
        /// </summary>
        /// <param name="text">Contains the text to match with the regular expression.</param>
        /// <param name="regularExpression">Contains the regular expression text.</param>
        /// <returns>Returns a value indicating whether the text successfully matches the regular expression.</returns>
        public static bool Match(this string text, string regularExpression)
        {
            Regex reg = new Regex(regularExpression);
            return reg.Match(text).Success;
        }

        /// <summary>
        /// This method is used to determine if a specified string contains a valid e-mail address.
        /// </summary>
        /// <param name="emailAddress">Contains the string to evaluate.</param>
        /// <returns>Returns a value indicating whether the string contains a valid e-mail address.</returns>
        public static bool IsEmail(this string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// This method is used to encode ampersands by replacing all ampersands with a constant converted value.
        /// </summary>
        /// <param name="text">Contains the text that will be replaced.</param>
        /// <returns>Returns text that contains the replaced values.</returns>
        public static string EncodeAmpersands(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return text.Replace("&", AmpersandConvertValue);
        }

        /// <summary>
        /// This method is used to decode ampersands by replacing the constant converted values back to ampersands.
        /// </summary>
        /// <param name="text">Contains the text that will be replaced.</param>
        /// <returns>Returns text that contains the replaced values.</returns>
        public static string DecodeAmpersands(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return text.Replace(AmpersandConvertValue, "&");
        }

        /// <summary>
        /// This method is used to determine if a file name contains a GUID.
        /// </summary>
        /// <param name="fileName">Contains the file name text that will be searched.</param>
        /// <returns>Returns a value indicating whether the file name contains a GUID value.</returns>
        public static bool ContainsGuid(this string fileName)
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(fileName) && fileName.Length >= 36)
            {
                // remove any contents after #
                int anchorCharLocation = fileName.LastIndexOf('#');
                if (anchorCharLocation >= 0)
                {
                    fileName = fileName.Substring(0, anchorCharLocation);
                }

                // its long enough to contain a GUID, so continue...
                string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

                // check to ensure that the string is long enough to contain a guid...
                // potential guid string example: 7C2D53DF-C77E-4E99-B739-6877DDAF165A
                if (!string.IsNullOrWhiteSpace(fileNameOnly) && fileNameOnly.Length >= 36)
                {
                    int delimiterPosition = fileNameOnly.LastIndexOf('_');

                    if (delimiterPosition > 0)
                    {
                        fileNameOnly = fileNameOnly.Substring(delimiterPosition + 1);
                    }

                    result = Guid.TryParse(fileNameOnly, out Guid _);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to remove a GUID from a file name.
        /// </summary>
        /// <param name="fileName">Contains the file name that does not include an extension and ends with a GUID value.</param>
        /// <returns>Returns the value of the file name with the GUID value removed.</returns>
        public static string RemoveGuid(this string fileName)
        {
            var newName = fileName;
            var anchorContents = string.Empty;

            // remove guid if the file name contains one
            if (!string.IsNullOrWhiteSpace(fileName) && fileName.ContainsGuid())
            {
                // store contents after #
                int anchorCharLocation = newName.LastIndexOf('#');
                if (anchorCharLocation >= 0)
                {
                    anchorContents = newName.Substring(anchorCharLocation);
                    newName = newName.Substring(0, anchorCharLocation);
                }

                var path = Path.GetDirectoryName(newName) ?? string.Empty;
                var extension = Path.GetExtension(newName);
                newName = Path.GetFileNameWithoutExtension(newName);

                if (newName.Length > 36)
                {
                    var prefixIndex = newName.LastIndexOf('_');

                    if (prefixIndex >= 0)
                    {
                        newName = newName.Substring(0, prefixIndex);
                    }

                    newName = Path.Combine(path, newName + extension + anchorContents);
                }
                else
                {
                    newName = fileName;
                }
            }

            return newName;
        }

        /// <summary>
        /// This method is used to append a value to a file name before the existing GUID.
        /// </summary>
        /// <param name="filePathName">Contains the file path name that ends with a GUID value.</param>
        /// <param name="appendText">Contains the value of the text to be appended to the file name.</param>
        /// <returns>Returns the value of the file path name with the appended text.</returns>
        public static string AppendFileNameSuffix(this string filePathName, string appendText)
        {
            var newName = filePathName;
            var anchorContents = string.Empty;

            if (!string.IsNullOrWhiteSpace(filePathName))
            {
                // store contents after #
                int anchorCharLocation = newName.LastIndexOf('#');
                if (anchorCharLocation >= 0)
                {
                    anchorContents = newName.Substring(anchorCharLocation);
                    newName = newName.Substring(0, anchorCharLocation);
                }

                var path = Path.GetDirectoryName(newName) ?? string.Empty;
                var fileName = Path.GetFileName(newName);

                if (newName.Length > 36)
                {
                    var prefixIndex = fileName.LastIndexOf('_');

                    if (prefixIndex >= 0)
                    {
                        // append the text after the prefix and include remainder of file name after that
                        newName = Path.Combine(path, $"{fileName.Substring(0, prefixIndex)}_{appendText}{newName.Substring(prefixIndex)}{anchorContents}");
                    }
                    else
                    {
                        // no prefix, place append text at beginning, which would be before the Guid
                        newName = Path.Combine(path, $"{appendText}_{fileName}");
                    }
                }
                else
                {
                    newName = fileName;
                }
            }

            return newName;
        }

        /// <summary>
        /// This method is used to append a value to a an existing string.
        /// </summary>
        /// <param name="text">Contains the current text that will have text appended.</param>
        /// <param name="appendText">Contains the value of the text to be appended to the value.</param>
        /// <returns>Returns the value of the text with the appended text.</returns>
        public static string AppendSuffix(this string text, string appendText)
        {
            var newText = text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                var prefixIndex = text.LastIndexOf('_');

                if (prefixIndex >= 0)
                {
                    // append the text after the prefix and include remainder of file name after that
                    newText = $"{text.Substring(0, prefixIndex)}_{appendText}{text.Substring(prefixIndex)}";
                }
                else
                {
                    // no prefix, place append text at the end
                    newText = $"{text}_{appendText}";
                }
            }

            return newText;
        }

        /// <summary>
        /// This extension method is used to add a specified GUID to the component file name.
        /// </summary>
        /// <param name="fileName">Contains the file name to modify.</param>
        /// <param name="componentGuid">Contains the GUID to include in the file name.</param>
        /// <returns>Returns a GUID with {fileName}_{componentGuid}.extension filename</returns>
        public static string AddGuid(this string fileName, Guid componentGuid)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string componentFileName = Path.GetFileName(fileName);

            // add the GUID to end of the filename if it doesn't already have one
            if (!string.IsNullOrWhiteSpace(componentFileName) && !componentFileName.ContainsGuid())
            {
                // add the GUID to the end of the file name
                string originalFileNameNoExtension = Path.GetFileNameWithoutExtension(componentFileName);
                string originalExtension = Path.GetExtension(componentFileName);

                // build new component file name
                componentFileName = $"{originalFileNameNoExtension}_{componentGuid}{originalExtension}";
            }
            else if (string.IsNullOrWhiteSpace(componentFileName))
            {
                // default to the GUID and default extension .xml
                componentFileName = Path.Combine(componentGuid.ToString(), ".xml");
            }

            return componentFileName;
        }

        /// <summary>
        /// This method is used to retrieve the GUID from a specified file name.
        /// </summary>
        /// <param name="fileName">Contains the filename from which a GUID shall be retrieved.</param>
        /// <returns>Returns the Guid object of the GUID found within the filename.</returns>
        public static Guid ParseGuid(this string fileName)
        {
            Guid result = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(fileName) && fileName.Length >= 36)
            {
                // its long enough to contain a GUID, so continue...
                string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

                // check to ensure that the string is long enough to contain a guid...
                // potential guid string example: 7C2D53DF-C77E-4E99-B739-6877DDAF165A
                if (!string.IsNullOrWhiteSpace(fileNameOnly) && fileNameOnly.Length >= 36)
                {
                    int delimiterPosition = fileNameOnly.LastIndexOf('_');

                    if (delimiterPosition > 0)
                    {
                        fileNameOnly = fileNameOnly.Substring(delimiterPosition + 1);
                    }

                    Guid.TryParse(fileNameOnly, out result);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to strip a GUID from a filename in the format text_{GUID}.ext
        /// </summary>
        /// <param name="fileName">Contains the file name to strip a guid from.</param>
        /// <returns>Returns the filename without a GUID value.</returns>
        public static string StripGuid(this string fileName)
        {
            string originalFileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            string originalExtension = Path.GetExtension(fileName);

            // if the original has a GUID...
            if (originalFileNameNoExtension.ContainsGuid())
            {
                // remove the GUID
                originalFileNameNoExtension = originalFileNameNoExtension.RemoveGuid();
            }

            return originalFileNameNoExtension + originalExtension;
        }

        /// <summary>
        /// This method is used to replace $token$ values within the content string with matching key values from the dictionary specified.
        /// </summary>
        /// <param name="content">Contains the string to parse.</param>
        /// <param name="tokenValues">Contains a dictionary of string values to replace.</param>
        /// <returns>Returns the original content string with all tokens found replaced.</returns>
        public static string ReplaceTokens(this string content, Dictionary<string, string> tokenValues)
        {
            string result = content;

            if (tokenValues != null)
            {
                if (!tokenValues.ContainsKey("DATETIME"))
                {
                    tokenValues.Add("DATETIME", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }

                if (!tokenValues.ContainsKey("DATE"))
                {
                    tokenValues.Add("DATE", DateTime.UtcNow.ToShortDateString());
                }

                if (!tokenValues.ContainsKey("TIME"))
                {
                    tokenValues.Add("TIME", DateTime.UtcNow.ToShortTimeString());
                }

                result = tokenValues.Aggregate(result, (current, item) => current.Replace("$" + item.Key.ToUpperInvariant() + "$", item.Value));
            }

            return result;
        }

        /// <summary>
        /// This method is used to get a substring before a specified character is found within the string.
        /// </summary>
        /// <param name="source">Contains the source string to search within.</param>
        /// <param name="characterToFind">Contains the character to find within the specified source string.</param>
        /// <returns>Returns all characters before the specified character within the source string. 
        /// If the character is not found, the entire source string is returned.
        /// If the character is the only character within the source string, an empty string is returned.</returns>
        public static string Before(this string source, char characterToFind)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.IndexOf(characterToFind);
            return index > -1 ? (index > 0 ? source.Substring(0, index) : string.Empty) : source;
        }

        /// <summary>
        /// This method is used to get a substring after a specified character is found within the string.
        /// </summary>
        /// <param name="source">Contains the source string to search within.</param>
        /// <param name="characterToFind">Contains the character to find within the specified source string.</param>
        /// <returns>Returns all characters after the specified character within a source string. If the character is not found, the entire source string is returned.</returns>
        public static string After(this string source, char characterToFind)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            int index = source.IndexOf(characterToFind) + 1;
            return index > 0 && index < source.Length ? source.Substring(index) : source;
        }

        /// <summary>
        /// This method is used to return a null value when passed an empty string.
        /// </summary>
        /// <param name="source">Contains the source string to evaluate.</param>
        /// <returns>Returns a null value or the original string if not empty.</returns>
        public static string EmptyToNull(this string source)
        {
            return string.IsNullOrWhiteSpace(source) ? null : source;
        }
    }
}
