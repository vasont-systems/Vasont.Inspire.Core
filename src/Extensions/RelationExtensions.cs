//-----------------------------------------------------------------------
// <copyright file="RelationExtensions.cs" company="Vasont Systems">
//     Copyright (c) 2017 Vasont Systems. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Vasont.Inspire.Core.Extensions
{
    using System.Linq;
    using Configuration.Models;
    using Data;

    /// <summary>
    /// This class contains extension methods used for processing component relations.
    /// </summary>
    public static class RelationExtensions
    {
        /// <summary>
        /// This method is used to load the <see cref="RelationModel"/> from cache if available or from the database.
        /// </summary>
        /// <param name="context">Contains the <see cref="IFrameworkContext{IDataRepository}"/> object used for accessing the Vasont data repository.</param>
        /// <param name="relationId">Contains the unique identity of the relation model to find and return.</param>
        /// <returns>
        /// Returns the <see cref="RelationModel" /> model.
        /// </returns>
        public static RelationModel LoadRelationModelCache(this IFrameworkContext<IDataRepository> context, long relationId)
        {
            string relationCacheKey = $"Tenant{context.Settings.TenantInfo.TenantId}_Relation{relationId}";
            RelationModel model = null;

            // load relation model from the cache if it exists
            if (context.Cache.Contains(relationCacheKey))
            {
                model = context.Cache[relationCacheKey] as RelationModel;
            }
            else
            {
                // build the relation model
                var entity = context.DataRepository.Relations.FirstOrDefault(r => r.RelationId == relationId);

                if (entity != null)
                {
                    model = new RelationModel(entity);

                    // add this to the cache...
                    if (model != null && !context.Cache.Contains(relationCacheKey))
                    {
                        context.Cache.Add(relationCacheKey, model);
                    }
                }
            }

            return model;
        }
    }
}
