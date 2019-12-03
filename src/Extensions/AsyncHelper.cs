//-----------------------------------------------------------------------
// <copyright file="AsyncHelper.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains helper methods that allow a developer to run an asynchronous method from within a synchronous method.
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Contains a new task factory.
        /// </summary>
        private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, 
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        /// <summary>
        /// Executes and returns the value of the asynchronous method function specified.
        /// </summary>
        /// <typeparam name="TResult">Contains the type of the result to return.</typeparam>
        /// <param name="func">Contains the function to execute.</param>
        /// <returns>Returns the result of the function.</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the asynchronous method function specified.
        /// </summary>
        /// <param name="func">Contains the function to execute.</param>
        public static void RunSync(Func<Task> func)
        {
            TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
