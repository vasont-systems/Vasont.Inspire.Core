//-------------------------------------------------------------
// <copyright file="ImageExtensions.cs" company="Vasont Systems">
// Copyright (c) Vasont Systems. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// This class contains extensions to support image binary content within the application.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// This extension method is used to properly rescale an image into a smaller thumbnail.
        /// </summary>
        /// <param name="sourceBitmap">Contains the source bitmap image to resize</param>
        /// <param name="newWidth">Contains the new thumbnail image width.</param>
        /// <param name="newHeight">Contains the new thumbnail image height.</param>
        /// <returns>Returns a new <see cref="Bitmap"/> image containing the scaled thumbnail image.</returns>
        public static Bitmap Thumbnail(this Bitmap sourceBitmap, int newWidth, int newHeight)
        {
            if (sourceBitmap == null)
            {
                throw new ArgumentNullException(nameof(sourceBitmap));
            }

            float scale = newWidth > sourceBitmap.Width || newHeight > sourceBitmap.Height ? 1 : Math.Min((float)newWidth / sourceBitmap.Width, (float)newHeight / sourceBitmap.Height);
            int scaledWidth = (int)(sourceBitmap.Width * scale);
            int scaledHeight = (int)(sourceBitmap.Height * scale);
            Rectangle scaledRect = new Rectangle((newWidth - scaledWidth) / 2, (newHeight - scaledHeight) / 2, scaledWidth, scaledHeight);

            // build a new high-quality profile image of allowed size and stream out to a new profile image.
            var destImage = new Bitmap(newWidth, newHeight);

            using (var destGraphic = Graphics.FromImage(destImage))
            {
                // fill the target with a background.
                destGraphic.FillRectangle(Brushes.Transparent, new RectangleF(0, 0, newWidth, newHeight));

                // draw the rescaled image centered inside the new target image dimension bounds
                destGraphic.DrawImage(sourceBitmap, scaledRect);
            }

            return destImage;
        }

        /// <summary>
        /// This extension method is used to properly rescale an image into a smaller thumbnail and write to a specified stream.
        /// </summary>
        /// <param name="outputStream">Contains the stream where the new thumbnail image shall be written to.</param>
        /// <param name="sourceBitmap">Contains the source bitmap image to resize</param>
        /// <param name="newWidth">Contains the new thumbnail image width.</param>
        /// <param name="newHeight">Contains the new thumbnail image height.</param>
        /// <param name="outputFormat">Contains the thumbnail image format.</param>
        public static void WriteThumbnail(this Stream outputStream, Bitmap sourceBitmap, int newWidth, int newHeight, ImageFormat outputFormat)
        {
            using (Bitmap destImage = sourceBitmap.Thumbnail(newWidth, newHeight))
            {
                destImage.Save(outputStream, outputFormat);
            }
        }

        /// <summary>
        /// This method is used to create a thumbnail for a specified binary file content data.
        /// </summary>
        /// <param name="contents">Contains the binary image contents to convert to a thumbnail</param>
        /// <param name="thumbnailImageWidth">Contains an optional thumbnail width in pixels.</param>
        /// <param name="thumbnailImageHeight">Contains an optional thumbnail height in pixels.</param>
        /// <returns>Returns the thumbnail image binary contents</returns>
        public static byte[] CreateThumbnail(this byte[] contents, int thumbnailImageWidth = 128, int thumbnailImageHeight = 128)
        {
            byte[] returnValue;

            try
            {
                // load the image into a bitmap (if bad format, this will throw exception) and rescale to 128x128
                using (MemoryStream imageStream = new MemoryStream(contents))
                using (Bitmap uploadedImage = new Bitmap(imageStream))
                using (MemoryStream thumbnailStream = new MemoryStream())
                {
                    // rescale image and write to the output stream in PNG format
                    thumbnailStream.WriteThumbnail(uploadedImage, thumbnailImageWidth, thumbnailImageHeight, ImageFormat.Png);
                    returnValue = thumbnailStream.ToArray();
                }
            }
            catch
            {
                // if image was bad format, catch exception and set thumbnail to default image icon
                // this will prevent error import from failing and eliminate error in Xeditor
                returnValue = ResourceExtensions.GetEmbeddedResourceImage("ImageDefaultIcon");
            }

            return returnValue;
        }

        /// <summary>
        /// This method is used to create a thumbnail for a specified binary file stream data.
        /// </summary>
        /// <param name="fileStream">Contains the file stream for the binary component image data.</param>
        /// <param name="thumbnailImageWidth">Contains an optional thumbnail width in pixels.</param>
        /// <param name="thumbnailImageHeight">Contains an optional thumbnail height in pixels.</param>
        /// <returns>Returns a byte array.</returns>
        public static byte[] CreateThumbnail(this Stream fileStream, int thumbnailImageWidth = 128, int thumbnailImageHeight = 128)
        {
            byte[] returnValue;

            try
            {
                // load the image into a bitmap (if bad format, this will throw exception) and rescale to 128x128
                using (Bitmap uploadedImage = new Bitmap(fileStream))
                using (MemoryStream thumbnailStream = new MemoryStream())
                {
                    // rescale image and write to the output stream in PNG format
                    thumbnailStream.WriteThumbnail(uploadedImage, thumbnailImageWidth, thumbnailImageHeight, ImageFormat.Png);

                    return thumbnailStream.ToArray();
                }
            }
            catch
            {
                // if image was bad format, catch exception and set thumbnail to default image icon
                // this will prevent error import from failing and eliminate error in Xeditor
                returnValue = ResourceExtensions.GetEmbeddedResourceImage("ImageDefaultIcon");
            }

            return returnValue;
        }
    }
}