//-----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Properties;

    /// <summary>
    /// This class contains a number of extension methods for conversion of data values.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// This method is used to convert a string representation of a GUID into a Guid structure.
        /// </summary>
        /// <param name="value">Contains the string value containing a GUID value.</param>
        /// <returns>Returns the parsed string as a Guid structure.</returns>
        public static Guid ToGuid(this string value)
        {
            Guid.TryParse(value, out Guid result);
            return result;
        }

        /// <summary>
        /// This method is used to convert a potentially local date-time to a UTC kind date. This is used in scenarios where all dates are expected to be handled as UTC instead of allowing localization to manipulate dates.
        /// </summary>
        /// <param name="value">Contains the potentially local date-time to convert to UTC kind.</param>
        /// <returns>Returns a new date with the same values as the original date, only formatted as UTC localization.</returns>
        public static DateTime ToUtcKindDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, DateTimeKind.Utc);
        }

        /// <summary>
        /// Truncates a string to a maximum length.
        /// </summary>
        /// <param name="value">Contains the value to truncate.</param>
        /// <param name="maximumLength">Contains the maximum number of bytes to truncate.</param>
        /// <returns>Returns the truncated string.</returns>
        public static string Truncate(this string value, int maximumLength)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maximumLength)
            {
                value = value.Substring(0, maximumLength);
            }

            return value;
        }

        /// <summary>
        /// Converts a string to an integer value returning default value for null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default integer value.</returns>
        public static int ToInt(this string value, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts an object to a long integer value returning default value on null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default long integer value.</returns>
        public static long ToLong(this long? value, long defaultValue = 0)
        {
            return value ?? defaultValue;
        }

        /// <summary>
        /// Converts a string to a long integer value returning default value for null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default long integer value.</returns>
        public static long ToLong(this string value, long defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value) || !long.TryParse(value, out long result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts a string to an decimal value returning default value for null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default decimal value.</returns>
        public static decimal ToDecimal(this string value, decimal defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, out decimal result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts an object to a string value returning default value on null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the object value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <returns>Returns the converted or default string value.</returns>
        public static string ConvertToString(this object value, string defaultValue = "")
        {
            string result = defaultValue;

            if (value != null && value != DBNull.Value)
            {
                result = value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    result = defaultValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a string to an DateTime value returning default value for null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <param name="dateFormatter">Contains the globalization formatter for converting a date.</param>
        /// <param name="dateStyle">Contains the date style the string is conformed to.</param>
        /// <returns>Returns the converted or default DateTime value.</returns>
        public static DateTime ToDateTime(this string value, DateTime? defaultValue = null, IFormatProvider dateFormatter = null, DateTimeStyles dateStyle = DateTimeStyles.None)
        {
            DateTime result = defaultValue ?? DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (dateFormatter == null)
                {
                    dateFormatter = CultureInfo.InvariantCulture;
                }

                if (!DateTime.TryParse(value, dateFormatter, dateStyle, out result))
                {
                    result = defaultValue ?? DateTime.MinValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a string matching an exact string format to a DateTime value returning default value for null or <see cref="DBNull"/> types.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="stringFormat">Contains the exact date format that the value to convert contains.</param>
        /// <param name="defaultValue">Contains the default value to return if value cannot be converted.</param>
        /// <param name="dateFormatter">Contains the globalization formatter for converting a date.</param>
        /// <returns>Returns the converted or default DateTime value.</returns>
        public static DateTime ToExactDateTime(this string value, string stringFormat, DateTime? defaultValue = null, IFormatProvider dateFormatter = null)
        {
            DateTime result = defaultValue ?? DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(stringFormat))
            {
                throw new ArgumentNullException(nameof(stringFormat));
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    if (dateFormatter == null)
                    {
                        dateFormatter = CultureInfo.InvariantCulture;
                    }

                    result = DateTime.ParseExact(value, stringFormat, dateFormatter);
                }
                catch
                {
                    result = defaultValue ?? DateTime.MinValue;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert a string value to an enumerated value of a specified type.
        /// </summary>
        /// <typeparam name="T">Contains the enumeration type.</typeparam>
        /// <param name="value">Contains the string value to convert.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive search; otherwise <c>false</c>.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Returns the enumerated value on success.</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true, T defaultValue = default)
        {
            T result = defaultValue;

            if (!string.IsNullOrWhiteSpace(value))
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert a string value to a boolean.
        /// </summary>
        /// <param name="value">Contains the value to convert.</param>
        /// <param name="defaultValue">Contains a default value to return when the value is invalid.</param>
        /// <returns>Returns the converted value or default value.</returns>
        public static bool ToBoolean(this string value, bool defaultValue = false)
        {
            bool result = defaultValue;
            string[] allowedPositives = { "T", "TRUE", "1", "Y", "YES", "O" };

            if (!string.IsNullOrWhiteSpace(value))
            {
                result = allowedPositives.Contains(value.ToUpperInvariant());
            }

            return result;
        }

        /// <summary>
        /// This method is used to recurse through an exception's inner exceptions and return a combined message string containing all error messages.
        /// </summary>
        /// <param name="ex">Contains the exception object to recurse.</param>
        /// <param name="recursionLevel">Contains the indentation level of the recursive messages.</param>
        /// <returns>Returns a string containing all related exception messages.</returns>
        public static string RecurseMessages(this Exception ex, int recursionLevel = 0)
        {
            string message = string.Empty;
            if (ex != null)
            {
                message = ex.Message + Environment.NewLine;

                if (recursionLevel > 0)
                {
                    message = new string('-', recursionLevel) + ">" + message;
                }

                if (ex.InnerException != null)
                {
                    message += ex.InnerException.RecurseMessages(++recursionLevel);
                }
            }

            return message;
        }

        /// <summary>
        /// This method is used to convert a Bitmap image object to an array of bytes.
        /// </summary>
        /// <param name="image">Contains the bitmap image object to convert.</param>
        /// <returns>Returns a byte array representing the image bitmap.</returns>
        public static byte[] ToByteArray(this Bitmap image)
        {
            byte[] results;

            using (var stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                results = stream.ToArray();
            }

            return results;
        }

        /// <summary>
        /// This method is used to find a <see cref="System.ComponentModel.DescriptionAttribute"/> value specified on an enumeration value.
        /// </summary>
        /// <typeparam name="T">Contains the type of enumeration to retrieve the description value for.</typeparam>
        /// <param name="enumerationValue">Contains the enumeration to search for a description.</param>
        /// <param name="resourceCulture">Contains an optional culture info object used to override the current threads culture info. 
        /// Typically used when an enumerated description must be rendered in a different language than the user's profile preference.</param>
        /// <returns>Returns the enumeration description or the default ToString value.</returns>
        public static string ToDescription<T>(this T enumerationValue, CultureInfo resourceCulture = null) where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
            {
                throw new ArgumentException(nameof(enumerationValue));
            }

            string result = enumerationValue.ToString();
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());

            // if there were any members found...
            if (memberInfo.Length > 0)
            {
                // get the first member that has any custom attributes of the type we're looking for
                var member = memberInfo.FirstOrDefault(mi => mi.CustomAttributes.Any(ca => ca.AttributeType == typeof(DescriptionAttribute) ||
                                        ca.AttributeType == typeof(LocalizedDescriptionAttribute)));

                // if a member was found with either type, get custom attributes
                if (member != null)
                {
                    var localizedDescriptionAttribute = member.GetCustomAttributes<LocalizedDescriptionAttribute>(false).FirstOrDefault();

                    if (localizedDescriptionAttribute != null)
                    {
                        // get the previously set culture if any.
                        var previousCulture = Resources.Culture;

                        // if an override culture was specified...
                        if (resourceCulture != null)
                        {
                            // use the override culture for this instance
                            Resources.Culture = resourceCulture;
                        }

                        // retrieve the resource stored description from the attribute.
                        result = localizedDescriptionAttribute.Description;

                        // reset the previous culture override when done
                        Resources.Culture = previousCulture;
                    }
                    else
                    {
                        var descriptionAttribute = member.GetCustomAttributes<DescriptionAttribute>(false).FirstOrDefault();

                        if (descriptionAttribute != null)
                        {
                            result = descriptionAttribute.Description;
                        }
                    }
                }
            }

            // If we have no description attribute, just return the ToString of the enumeration.
            return result;
        }

        /// <summary>
        /// This method is used to create a dictionary of description to enumeration value mappings for reverse enumeration value lookup.
        /// </summary>
        /// <typeparam name="T">Contains the type of enumeration to retrieve the description mapping for.</typeparam>
        /// <returns>Returns the enumeration character to enumeration mapping dictionary.</returns>
        public static Dictionary<string, T> ToCharMapDictionary<T>() where T : struct
        {
            Dictionary<string, T> resultDictionary = new Dictionary<string, T>();

            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException(nameof(T));
            }

            string[] enumerationNames = type.GetEnumNames();

            if (enumerationNames.Length > 0)
            {
                foreach (string name in enumerationNames)
                {
                    // get all members of the enumeration
                    MemberInfo[] memberInfo = type.GetMember(name);

                    // if there were any members found...
                    if (memberInfo.Length > 0)
                    {
                        // get the first member that has any custom attributes of the type we're looking for
                        var member = memberInfo.FirstOrDefault(mi => mi.CustomAttributes.Any(ca => ca.AttributeType == typeof(DescriptionAttribute)));

                        // if a member was found with either type, get custom attributes
                        if (member != null)
                        {
                            var descriptionAttribute = member.GetCustomAttributes<DescriptionAttribute>(false).FirstOrDefault();

                            if (descriptionAttribute != null &&
                                !resultDictionary.ContainsKey(descriptionAttribute.Description))
                            {
                                resultDictionary.Add(descriptionAttribute.Description, name.ToEnum<T>());
                            }
                        }
                    }
                }
            }

            return resultDictionary;
        }

        /// <summary>
        /// This extension method is used to rescale the specified bitmap into the width and height dimensions specified.
        /// </summary>
        /// <param name="sourceImage">Contains the source image to rescale.</param>
        /// <param name="newWidth">Contains the new bitmap width size.</param>
        /// <param name="newHeight">Contains the new bitmap height size.</param>
        /// <returns>Returns a new rescaled bitmap of the source with specified width and height.</returns>
        public static Bitmap Rescale(this Bitmap sourceImage, int newWidth, int newHeight)
        {
            if (sourceImage == null)
            {
                throw new ArgumentNullException(nameof(sourceImage));
            }

            if (sourceImage.Width < newWidth)
            {
                newWidth = sourceImage.Width;
            }

            if (sourceImage.Height < newHeight)
            {
                newHeight = sourceImage.Height;
            }

            // build a new high-quality profile image of allowed size.
            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var outputImage = new Bitmap(newWidth, newHeight);

            outputImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(sourceImage, destRect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return outputImage;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static long? ZeroToNull(this long? value)
        {
            return value.HasValue ? (value.Value != 0 ? value.Value : (long?)null) : null;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static int? ZeroToNull(this int? value)
        {
            return value.HasValue ? (value.Value != 0 ? value.Value : (int?)null) : null;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static long? ZeroToNull(this long value)
        {
            return value != 0 ? value : (long?)null;
        }

        /// <summary>
        /// This extension method is used to convert a zero value to null.
        /// </summary>
        /// <param name="value">Contains the value to evaluate.</param>
        /// <returns>Returns null if the value is zero.</returns>
        public static int? ZeroToNull(this int value)
        {
            return value != 0 ? value : (int?)null;
        }
    }
}