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

        /// <summary>
        /// This method is used to take in a URI and add a specified suffix to the base URI domain and return.
        /// </summary>
        /// <param name="requestUri">Contains the originating request URI to modify.</param>
        /// <param name="apiSuffix">Contains the optional suffix to add to the base URI sub-domain.</param>
        /// <returns>Returns a new base Uri from the request URI with the specified API suffix appended to the sub-domain of the base domain.</returns>
        public static Uri AddApiSuffixBase(Uri requestUri, string apiSuffix = "-api")
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }
            
            // use it to get the authority left part to build a new URL
            string result = requestUri.GetLeftPart(UriPartial.Authority);

            if (!string.IsNullOrWhiteSpace(apiSuffix))
            {
                string[] hostParts = result.Split('.');

                // if host parts returned and the subdomain does not end with the api suffix...
                if (hostParts != null && hostParts.Length > 0 && !hostParts[0].EndsWith(apiSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    // add the api suffix and rebuild the result
                    hostParts[0] += apiSuffix;
                    result = string.Join(".", hostParts);
                }
            }

            return new Uri(result);
        }

        /// <summary>
        /// This method is used to take in a URI and add a specified suffix to the base URI domain and return.
        /// </summary>
        /// <param name="requestUri">Contains the originating request URI to modify.</param>
        /// <param name="apiSuffix">Contains the optional suffix to add to the base URI sub-domain.</param>
        /// <returns>Returns a new base Uri from the request URI with the specified API suffix appended to the sub-domain of the base domain.</returns>
        public static string AddApiSuffixBase(string requestUri, string apiSuffix = "-api")
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            // use it to get the authority left part to build a new URL
            string result = requestUri;

            if (!string.IsNullOrWhiteSpace(apiSuffix))
            {
                string[] hostParts = result.Split('.');

                // if host parts returned and the subdomain does not end with the api suffix...
                if (hostParts != null && hostParts.Length > 0 && !hostParts[0].EndsWith(apiSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    // add the api suffix and rebuild the result
                    hostParts[0] += apiSuffix;
                    result = string.Join(".", hostParts);
                }
            }

            return result;
        }

        /// <summary>
        /// Strips the -API suffix from a sub domain string.
        /// </summary>
        /// <param name="requestUri">The full host domain.</param>
        /// <param name="apiSuffix">Contains the optional suffix to find and remove from the base URI sub-domain.</param>
        /// <returns>Returns a full host domain without the -api suffix.</returns>
        public static Uri StripApiSuffixBase(Uri requestUri, string apiSuffix = "-api")
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            // use it to get the authority left part to build a new URL
            string result = requestUri.GetLeftPart(UriPartial.Authority);

            if (!string.IsNullOrWhiteSpace(apiSuffix))
            {
                if (result.Contains($"{apiSuffix}."))
                {
                    string[] parts = result.Split('.');

                    if (parts != null && parts.Length > 0 && parts[0].EndsWith(apiSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        parts[0] = parts[0].Replace(apiSuffix, string.Empty);
                        result = string.Join(".", parts);
                    }
                }
            }

            return new Uri(result);
        }

        /// <summary>
        /// Strips the -API suffix from a sub domain string.
        /// </summary>
        /// <param name="requestUri">The full host domain.</param>
        /// <param name="apiSuffix">Contains the optional suffix to find and remove from the base URI sub-domain.</param>
        /// <returns>Returns a full host domain without the -api suffix.</returns>
        public static string StripApiSuffixBase(string requestUri, string apiSuffix = "-api")
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            // use it to get the authority left part to build a new URL
            string result = requestUri;

            if (!string.IsNullOrWhiteSpace(apiSuffix))
            {
                if (result.Contains($"{apiSuffix}."))
                {
                    string[] parts = result.Split('.');

                    if (parts != null && parts.Length > 0 && parts[0].EndsWith(apiSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        parts[0] = parts[0].Replace(apiSuffix, string.Empty);
                        result = string.Join(".", parts);
                    }
                }
            }

            return result;
        }
    }
}
