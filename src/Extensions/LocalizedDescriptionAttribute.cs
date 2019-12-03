//-------------------------------------------------------------
// <copyright file="LocalizedDescriptionAttribute.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.ComponentModel;
    using Vasont.Inspire.Core.Properties;

    /// <summary>
    ///  This class allows for the specification of a resource string key to use for an enumeration description.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        #region Private Fields

        /// <summary>
        ///  Contains the localized description key for the attribute.
        /// </summary>
        private readonly string localizedDescriptionKey;

        #endregion

        /// <summary>
        ///  Initializes a new instance of the <see cref="LocalizedDescriptionAttribute" /> class.
        /// </summary>
        /// <param name="localizedDescriptionKey">
        ///  Contains the resource key name to load into the attribute description property.
        /// </param>
        public LocalizedDescriptionAttribute(string localizedDescriptionKey)
        {
            this.localizedDescriptionKey = localizedDescriptionKey;
        }

        /// <summary>
        ///  Gets the description of the localized description from resource.
        /// </summary>
        public override string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(this.localizedDescriptionKey))
                {
                    this.DescriptionValue = Resources.ResourceManager.GetString(this.localizedDescriptionKey, Resources.Culture) ?? this.localizedDescriptionKey;
                }

                return this.DescriptionValue;
            }
        }
    }
}