﻿//-----------------------------------------------------------------------
// <copyright file="IErrorMessage.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Vasont.Inspire.Core.Errors
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///  Contains an enumerated list of error message types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorType
    {
        /// <summary>
        ///  Errors that have unexpectedly ended the intended process.
        /// </summary>
        Fatal,

        /// <summary>
        ///  Errors that have been expected to end the intended process.
        /// </summary>
        Critical,

        /// <summary>
        ///  Errors that can be handled and allow a process to continue.
        /// </summary>
        Warning,

        /// <summary>
        ///  Errors that occur before a process starts.
        /// </summary>
        Validation
    }

    /// <summary>
    ///  Contains an enumerated list of error message types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorCategory
    {
        /// <summary>
        ///  General errors not categorized.
        /// </summary>
        /// <remarks>All other errors that do not fall within the other categories.</remarks>
        General,

        /// <summary>
        ///  Security related errors.
        /// </summary>
        /// <remarks>
        ///  Errors related to permissions would be good candidates for Application errors.
        /// </remarks>
        Security,

        /// <summary>
        ///  Application related errors not security related.
        /// </summary>
        /// <remarks>
        ///  Errors related to a process failing would be good candidates for Application errors.
        /// </remarks>
        Application,

        /// <summary>
        ///  System related errors that are not security related.
        /// </summary>
        /// <remarks>IO or database related issues would be good candidates for System errors.</remarks>
        System
    }

    /// <summary>
    ///  Represents an error message to be displayed.
    /// </summary>
    public interface IErrorMessage
    {
        /// <summary>
        ///  Gets or sets the message error type.
        /// </summary>
        ErrorType ErrorType { get; set; }

        /// <summary>
        ///  Gets or sets the message error type.
        /// </summary>
        ErrorCategory ErrorCategory { get; set; }

        /// <summary>
        ///  Gets or sets the error message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///  Gets the error stack trace text.
        /// </summary>
        string StackTrace { get; }

        /// <summary>
        ///  Gets or sets the related property name.
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        ///  Gets or sets a suggested error code.
        /// </summary>
        int SuggestedErrorCode { get; set; }

        /// <summary>
        ///  Gets or sets the date time when the error occurred.
        /// </summary>
        DateTime EventDate { get; set; }
    }
}