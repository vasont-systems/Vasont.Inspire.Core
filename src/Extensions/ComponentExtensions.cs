//------------------------------------------------------------------
// <copyright file="ComponentExtensions.cs" company="Vasont Systems">
//     Copyright (c) 2016 Vasont Systems. All rights reserved.
// </copyright>
// <author>Steve Sargent</author>
//------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// This class contains <see cref="Component"/> related extension methods.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// This method is used to create a component thumbnail for a specified component and binary
        /// file stream data.
        /// </summary>
        /// <param name="dataRepository">
        /// Contains the data repository context used for interacting with the database storage.
        /// </param>
        /// <param name="component">
        /// Contains the related binary component used for creating a new thumbnail.
        /// </param>
        /// <param name="creatingComponent">
        /// Contains a value indicating whether the component is new and therefore an existing
        /// thumbnail does not exist.
        /// </param>
        /// <param name="fileStream">Contains the file stream for the binary component image data.</param>
        /// <param name="thumbnailImageWidth">Contains an optional thumbnail width in pixels.</param>
        /// <param name="thumbnailImageHeight">Contains an optional thumbnail height in pixels.</param>
        public static void CreateComponentThumbnail(this IDataRepository dataRepository, Component component, bool creatingComponent, Stream fileStream, int thumbnailImageWidth = 128, int thumbnailImageHeight = 128)
        {
            try
            {
                // load the image into a bitmap (if bad format, this will throw exception) and
                // rescale to 128x128
                using (Bitmap uploadedImage = new Bitmap(fileStream))
                using (MemoryStream thumbnailStream = new MemoryStream())
                {
                    // rescale image and write to the output stream in PNG format
                    thumbnailStream.WriteThumbnail(uploadedImage, thumbnailImageWidth, thumbnailImageHeight, ImageFormat.Png);
                    BinaryComponentThumbnail binaryThumbnail = null;

                    // if a component was specified and we're not creating it...
                    if (component.ComponentId > 0 && !creatingComponent)
                    {
                        // try and find existing thumbnail if not in navigation property
                        binaryThumbnail = component.Thumbnail ?? dataRepository.BinaryComponentThumbnails.FirstOrDefault(t => t.ComponentId == component.ComponentId);
                    }

                    // if existing thumbnail found...
                    if (binaryThumbnail != null)
                    {
                        // update existing thumbnail entity contents
                        binaryThumbnail.Content = thumbnailStream.ToArray();
                        dataRepository.SetState(binaryThumbnail, EntityState.Modified);
                    }
                    else
                    {
                        // either a new binary component or an existing one that didn't have a
                        // thumbnail create a new binary component thumbnail and add to database
                        binaryThumbnail = new BinaryComponentThumbnail
                        {
                            Component = component,
                            ComponentId = component.ComponentId,
                            UniqueId = Guid.NewGuid(),
                            Content = thumbnailStream.ToArray()
                        };
                        dataRepository.BinaryComponentThumbnails.Add(binaryThumbnail);
                    }

                    // associate thumbnail with component
                    component.Thumbnail = binaryThumbnail;
                }

                // if we can see, seek back to the beginning of the stream
                if (fileStream.CanSeek)
                {
                    // reset back to original beginning of stream
                    fileStream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch
            {
                // if image was bad format, catch exception and set thumbnail to default image icon
                // this will prevent error import from failing and eliminate error in Xeditor
                BinaryComponentThumbnail binaryThumbnail = new BinaryComponentThumbnail
                {
                    Component = component,
                    ComponentId = component.ComponentId,
                    UniqueId = Guid.NewGuid(),
                    Content = Resources.ImageDefaultIcon.ToByteArray()
                };
                dataRepository.BinaryComponentThumbnails.Add(binaryThumbnail);
                component.Thumbnail = binaryThumbnail;
            }
        }

        /// <summary>
        /// This method is used to generate an <see cref="XDocument"/> from the XML content of the
        /// <see cref="Component"/>.
        /// </summary>
        /// <param name="component">
        /// Contains the <see cref="Component"/> that will be used to generate the document.
        /// </param>
        /// <returns>Returns an <see cref="XDocument"/> that is created from the component content.</returns>
        public static XDocument ConvertContentToDocument(this Component component)
        {
            XDocument document = null;

            if (!string.IsNullOrWhiteSpace(component?.XmlComponent?.Content))
            {
                using (TextReader sr = new StringReader(component.XmlComponent.Content))
                {
                    document = XDocument.Load(sr);
                }
            }

            return document;
        }

        /// <summary>
        /// This method is used to determine if a component contains content, which could be either
        /// XML or binary.
        /// </summary>
        /// <param name="component">
        /// Contains the <see cref="Component"/> that will be used to search for content.
        /// </param>
        /// <returns>Returns a value indicating whether the component contains content.</returns>
        public static bool ContainsContent(this Component component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            return (component.BinaryComponent?.Content != null) || (component.XmlComponent?.Content != null);
        }

        /// <summary>
        /// This method is used to determine if a component contains a lock that will prevent the
        /// component from being updated.
        /// </summary>
        /// <param name="component">Contains the <see cref="Component"/> that will be checked.</param>
        /// <param name="userId">Contains the identity of the user attempting to update the component.</param>
        /// <returns>
        /// Returns a value indicating whether the component contains a lock that prevents an update.
        /// </returns>
        public static bool HasLocksThatPreventUpdate(this Component component, long userId)
        {
            bool result = false;

            if (component != null)
            {
                // first check to see component is locked if locked, then these conditions prevent
                // update: 1) locked to another user or 2) has editor content or 3) not a Standard
                // lock (system lock)
                result = component.ComponentLock != null &&
                         (component.ComponentLock.UserId != userId ||
                          component.EditorXmlComponents.Any() ||
                          component.ComponentLock.Type != ComponentLockType.Standard.ToString());
            }

            return result;
        }

        /// <summary>
        /// This method is used to convert an input string into a Component Reference Model object
        /// </summary>
        /// <typeparam name="T">Contains the type of the reference model used for parsing.</typeparam>
        /// <param name="input">Contains the input string to convert.</param>
        /// <param name="defaultFileName">
        /// Contains an optional default file name for empty input file names.
        /// </param>
        /// <returns>Returns a new object of the type specified for the given reference model.</returns>
        public static T ToReferenceModel<T>(this string input, string defaultFileName = "")
        {
            // dynamically create a new reference model of the type passed in. This will technically
            // allow for future reference types to be passed in.
            return (T)Activator.CreateInstance(typeof(T), input, defaultFileName);
        }

        /// <summary>
        /// This method searches for a component element model.
        /// </summary>
        /// <param name="componentConfigModel">Contains the model of the component configuration.</param>
        /// <param name="elementName">Contains the name of the element to search for.</param>
        /// <param name="parentElementName">Contains the name of the parent element.</param>
        /// <returns>Returns a <see cref="ComponentElementModel"/> object.</returns>
        public static ComponentElementModel FindConfigComponentElementModel(this ComponentConfigurationModel componentConfigModel, string elementName, string parentElementName = null)
        {
            ComponentElementModel model = null;

            if (componentConfigModel != null)
            {
                // first determine if this is a valid element
                model = componentConfigModel.Elements.FirstOrDefault(e => e.XmlName == elementName);

                // if the element is contained within a parent element...
                if (!string.IsNullOrWhiteSpace(parentElementName))
                {
                    bool validElement = false;

                    // get the parent element configuration
                    var parentConfigElement = componentConfigModel.Elements.FirstOrDefault(e => e.XmlName == parentElementName && (e.Role == ComponentConfigurationElementModeRole.Container || e.Role == ComponentConfigurationElementModeRole.MixedContainer));

                    // if the parent was found...
                    if (parentConfigElement != null)
                    {
                        // check to see if the element can be contained within the parent...
                        validElement = parentConfigElement.Contains.Any(c => c.ElementId == elementName);
                    }

                    // TODO: Perhaps enhance this validation routine to check for proper sequence and min/max usage.

                    // if the element was not found...
                    if (!validElement)
                    {
                        // element was not found as a valid child of the parent in the configuration
                        // so it is not valid in it's current location
                        model = null;
                    }
                }
            }

            return model;
        }
    }
}