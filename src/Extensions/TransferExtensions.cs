//------------------------------------------------------------------------
// <copyright file="TransferExtensions.cs" company="Vasont Systems">
//     Copyright (c) 2015 Vasont Systems. All rights reserved.
// </copyright>
// <author>Steve Sargent</author>
//-----------------------------------------------------------------------

namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration.Models;
    using Data;
    using Data.EF;
    using Properties;
    using Transfers.Models;

    /// <summary>
    /// This class contains extension methods used for processing transfers.
    /// </summary>
    public static class TransferExtensions
    {
        /// <summary>
        /// Gets the Parallel options for Parallel.ForEach usage. Returning a max degree of parallelism to 3/4 the number of available processors.
        /// </summary>
        public static ParallelOptions ParallelForEachOptions => new ParallelOptions
        {
            MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75))
        };

        /// <summary>
        /// This method is used to load the export model from memory if available or from the database.
        /// </summary>
        /// <param name="context">Contains the <see cref="IFrameworkContext{IDataRepository}"/> object used for accessing the Vasont data repository.</param>
        /// <param name="exportId">Contains the identity of the export.</param>
        /// <returns>
        /// Returns the <see cref="ExportModel" /> model.
        /// </returns>
        public static ExportModel LoadExportModelCache(this IFrameworkContext<IDataRepository> context, long exportId)
        {
            string exportCacheKey = $"Tenant_{context.Settings.TenantInfo.DomainKey}_Export{exportId}";
            ExportModel model = null;

            // load XML link type model from the cache if it exists
            if (context.Cache.Contains(exportCacheKey))
            {
                model = context.Cache.Get<ExportModel>(exportCacheKey);
            }
            else
            {
                Export export = context.DataRepository.Exports
                    .AsNoTracking()
                    .FirstOrDefault(e => e.ExportId == exportId);

                if (export != null)
                {
                    model = new ExportModel(export);

                    if (!context.Cache.Contains(exportCacheKey))
                    {
                        context.Cache.Add(exportCacheKey, model);
                    }
                }
                else
                {
                    context.ErrorManager.Warning(Resources.ExportNotFoundText);
                }
            }

            return model;
        }

        /// <summary>
        /// This method is used to ingest property values from a specified entity into the model object.
        /// </summary>
        /// <param name="model">Contains the model to populate.</param>
        /// <param name="entity">Contains the entity to ingest.</param>
        public static void Ingest(this ExportModel model, Export entity)
        {
            if (entity != null)
            {
                model.Description = entity.Description;
                model.ExportId = entity.ExportId;
                model.Name = entity.Name;
                model.Active = entity.Active;
                model.Default = entity.Default;
                model.ExportRelations = entity.ExportRelations.Select(r => new MinimalExportRelationModel(r)).ToList();
                model.ComponentTypes = entity.ComponentTypes.Select(ct => new MinimalComponentTypeModel(ct)).ToList();
            }
        }
    }
}
