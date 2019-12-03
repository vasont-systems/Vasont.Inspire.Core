//-----------------------------------------------------------------------
// <copyright file="ILoggerService.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Logging
{
    using System;
    using Errors;

    /// <summary>
    /// Contains the definition of a minimum implementation of a logger within the Inspire API.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Log using the debug level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        void Debug(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// Log using the debug level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="exception">Contains the exception to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="reportExceptionMessages">Contains a value indicating whether the exception messages shall be rendered to the message text.</param>
        void Debug(string message, Exception exception, ErrorCategory category = ErrorCategory.General, bool reportExceptionMessages = false);

        /// <summary>
        /// Log a formatted message string using the debug level message.
        /// </summary>
        /// <param name="format">Contains a string holding zero or more format token items.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="args">Contains a parameter array of zero or more object to inject into the tokenized format string.</param>
        void DebugFormat(string format, ErrorCategory category, params object[] args);

        /// <summary>
        /// Log using the error level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        void Error(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// Log using the error level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="exception">Contains the exception to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="reportExceptionMessages">Contains a value indicating whether the exception messages shall be rendered to the message text.</param>
        void Error(string message, Exception exception, ErrorCategory category = ErrorCategory.General, bool reportExceptionMessages = false);

        /// <summary>
        /// Log a formatted message string using the error level message.
        /// </summary>
        /// <param name="format">Contains a string holding zero or more format token items.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="args">Contains a parameter array of zero or more object to inject into the tokenized format string.</param>
        void ErrorFormat(string format, ErrorCategory category, params object[] args);

        /// <summary>
        /// Log using the fatal level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        void Fatal(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// Log using the fatal level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="exception">Contains the exception to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="reportExceptionMessages">Contains a value indicating whether the exception messages shall be rendered to the message text.</param>
        void Fatal(string message, Exception exception, ErrorCategory category = ErrorCategory.General, bool reportExceptionMessages = false);

        /// <summary>
        /// Log a formatted message string using the fatal level message.
        /// </summary>
        /// <param name="format">Contains a string holding zero or more format token items.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="args">Contains a parameter array of zero or more object to inject into the tokenized format string.</param>
        void FatalFormat(string format, ErrorCategory category, params object[] args);

        /// <summary>
        /// Log using the warning level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        void Warn(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// Log using the warning level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="exception">Contains the exception to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="reportExceptionMessages">Contains a value indicating whether the exception messages shall be rendered to the message text.</param>
        void Warn(string message, Exception exception, ErrorCategory category = ErrorCategory.General, bool reportExceptionMessages = false);

        /// <summary>
        /// Log a formatted message string using the warning level message.
        /// </summary>
        /// <param name="format">Contains a string holding zero or more format token items.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="args">Contains a parameter array of zero or more object to inject into the tokenized format string.</param>
        void WarnFormat(string format, ErrorCategory category, params object[] args);

        /// <summary>
        /// Log using the info level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        void Info(string message, ErrorCategory category = ErrorCategory.General);

        /// <summary>
        /// Log using the info level message.
        /// </summary>
        /// <param name="message">Contains the message object to log.</param>
        /// <param name="exception">Contains the exception to log.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="reportExceptionMessages">Contains a value indicating whether the exception messages shall be rendered to the message text.</param>
        void Info(string message, Exception exception, ErrorCategory category = ErrorCategory.General, bool reportExceptionMessages = false);

        /// <summary>
        /// Log a formatted message string using the info level message.
        /// </summary>
        /// <param name="format">Contains a string holding zero or more format token items.</param>
        /// <param name="category">Contains the category of the log entry.</param>
        /// <param name="args">Contains a parameter array of zero or more object to inject into the tokenized format string.</param>
        void InfoFormat(string format, ErrorCategory category, params object[] args);
    }
}
