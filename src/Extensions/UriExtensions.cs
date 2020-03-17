//-----------------------------------------------------------------------
// <copyright file="UriExtensions.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class contains extension and helper methods related to URI objects.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// This method is used to create a fully qualified URL to a specified API application URL
        /// </summary>
        /// <param name="requestUri">Contains the request URI made to the API endpoint.</param>
        /// <param name="routePath">Contains the route used within the API call.</param>
        /// <param name="parameters">Contains an optional dictionary of query parameters to add to the UI URL.</param>
        /// <returns>Returns a fully qualified URL to a UI module within the application.</returns>
        public static string CreateApiUrl(Uri requestUri, string routePath, Dictionary<string, object> parameters = null)
        {
            Uri requestedUri = requestUri ?? new Uri("https://localhost");

            // use it to get the authority left part to build a new URL
            string result = requestedUri.GetLeftPart(UriPartial.Authority);
            string queryString = string.Empty;

            // if any query parameters were specified...
            if (parameters != null)
            {
                // build a new query string
                queryString = string.Join("&", parameters.Select((x) => x.Key + "=" + x.Value.ConvertToString()));

                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    queryString = "?" + queryString;
                }
            }

            // finally build the final URL string
            result += routePath + queryString;

            return result;
        }

        /// <summary>
        /// This method is used to create a fully qualified URL to a specified UI application URL
        /// </summary>
        /// <param name="requestUri">Contains the request URI made to the API endpoint.</param>
        /// <param name="moduleKey">Contains the module key used within the application display.</param>
        /// <param name="parameters">Contains an optional dictionary of query parameters to add to the UI URL.</param>
        /// <returns>Returns a fully qualified URL to a UI module within the application.</returns>
        public static string CreateUiUrl(Uri requestUri, string moduleKey, Dictionary<string, object> parameters = null)
        {
            var requestedUri = requestUri ?? new Uri("https://localhost");

            // use it to get the authority left part to build a new URL
            string result = requestedUri.GetLeftPart(UriPartial.Authority);
            string queryString = string.Empty;

            // if any query parameters were specified...
            if (parameters != null)
            {
                // build a new query string
                queryString = string.Join("&", parameters.Select((x) => x.Key + "=" + x.Value.ConvertToString()));

                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    queryString = "?" + queryString;
                }
            }

            // finally build the final URL string
            result += "/index.html#" + moduleKey + queryString;

            return result;
        }
    }
}
