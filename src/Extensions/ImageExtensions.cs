﻿//-------------------------------------------------------------
// <copyright file="ImageExtensions.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using PdfLibCore;
    using PdfLibCore.Enums;
    using Svg;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Vasont.Inspire.Core.Storage;

    /// <summary>
    /// This class contains extensions to support image binary content within the application.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// This extension method is used to rescale the specified bitmap into the width and height dimensions specified.
        /// </summary>
        /// <param name="sourceImage">Contains the source image to rescale.</param>
        /// <param name="newWidth">Contains the new bitmap width size.</param>
        /// <param name="newHeight">Contains the new bitmap height size.</param>
        /// <returns>Returns a new rescaled bitmap of the source with specified width and height.</returns>
        public static Bitmap Rescale(this Bitmap sourceImage, int newWidth, int newHeight)
        {
            if (sourceImage == null)
            {
                throw new ArgumentNullException(nameof(sourceImage));
            }

            if (sourceImage.Width < newWidth)
            {
                newWidth = sourceImage.Width;
            }

            if (sourceImage.Height < newHeight)
            {
                newHeight = sourceImage.Height;
            }

            // build a new high-quality profile image of allowed size.
            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var outputImage = new Bitmap(newWidth, newHeight);

            outputImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(sourceImage, destRect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return outputImage;
        }

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
                returnValue = Properties.Resources.ImageDefaultIcon.ToByteArray();
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
                returnValue = Properties.Resources.ImageDefaultIcon.ToByteArray();
            }

            return returnValue;
        }

        /// <summary>
        /// This method is used to create a thumbnail for a specified binary file content data using filename for conversion type.
        /// </summary>
        /// <param name="contents">Contains the binary image contents to convert to a thumbnail.</param>
        /// <param name="fileName">Contains the binary image filename.</param>
        /// <param name="thumbnailImageWidth">Contains an optional thumbnail width in pixels.</param>
        /// <param name="thumbnailImageHeight">Contains an optional thumbnail height in pixels.</param>
        /// <returns>Returns the thumbnail image binary contents</returns>
        public static byte[] CreateThumbnailByFilename(this byte[] contents, string fileName, int thumbnailImageWidth = 128, int thumbnailImageHeight = 128)
        {
            byte[] returnValue;
            string fileMimeType = Files.FindMimeContentTypeByExtension(fileName);

            try
            {
                // convert to png first 
                if (fileMimeType.StartsWith("image/svg"))
                {
                    string svgContent = Encoding.UTF8.GetString(contents);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(svgContent);

                    // Open takes an XmlDocument and converts to SvgDocument
                    SvgDocument svgDocument = SvgDocument.Open(doc);
                    var bitmap = svgDocument.Draw();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Now create thumbnail from png data
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        contents = memoryStream.ToArray();
                    }
                }
                else if (fileMimeType.StartsWith("application/pdf"))
                {
                    var dpiX = 300D;
                    var dpiY = 300D;

                    using var pdfDocument = new PdfDocument(contents);
                    using var pdfPage = pdfDocument.Pages[0];
                    var pageWidth = (int)(dpiX * pdfPage.Size.Width / 72);
                    var pageHeight = (int)(dpiY * pdfPage.Size.Height / 72);

                    using var bitmap = new PdfiumBitmap(pageWidth, pageHeight, true);
                    pdfPage.Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);
                    Stream stream = bitmap.AsBmpStream(dpiX, dpiY);

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        contents = memoryStream.ToArray();
                    }
                }

                returnValue = contents.CreateThumbnail();
            }
            catch
            {
                // if image was bad format, catch exception and set thumbnail to default image icon
                returnValue = Properties.Resources.ImageDefaultIcon.ToByteArray();
            }

            return returnValue;
        }

        /// <summary>
        /// This method is used to convert a Bitmap image object to an array of bytes.
        /// </summary>
        /// <param name="image">Contains the bitmap image object to convert.</param>
        /// <returns>Returns a byte array representing the image bitmap.</returns>
        public static byte[] ToByteArray(this Bitmap image)
        {
            byte[] results;

            using (var stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                results = stream.ToArray();
            }

            return results;
        }
    }
}