//-----------------------------------------------------------------------
// <copyright file="TenantExtensions.cs" company="Vasont Systems">
//     Copyright (c) 2018 Vasont Systems. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using Core.Data;
    using Core.Security.Crypto;
    using Models;

    /// <summary>
    /// This class contains tenant related extension methods
    /// </summary>
    public static class TenantExtensions
    {
        /// <summary>
        /// This extension method is used to build a connection string from the specified tenant model object.
        /// </summary>
        /// <param name="tenant">Contains the tenant model object to retrieve connection string values from.</param>
        /// <param name="securityKey">Contains the security key used to decrypt protected tenant information.</param>
        /// <returns>Returns a connection string for the specified tenant object.</returns>
        public static string BuildConnectionString(this TenantInfoModel tenant, string securityKey = "")
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            return ConnectionFactory.BuildConnectionString(
                tenant.DbmsType, 
                tenant.DatabaseSource, 
                tenant.DatabaseName, 
                tenant.DatabaseUserId, 
                tenant.DatabasePassword.Decrypt(securityKey), 
                tenant.ExtendedConnectionString);
        }
    }
}