//-----------------------------------------------------------------------
// <copyright file="SortDirection.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated selection list of query direction values.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDirection
    {
        /// <summary>
        /// Queries are made in ascending order.
        /// </summary>
        Asc,

        /// <summary>
        /// Queries are made in descending order.
        /// </summary>
        Desc
    }
}
