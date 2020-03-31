//-------------------------------------------------------------
// <copyright file="ImageExtensionTests.cs" company="GlobalLink Vasont">
// Copyright (c) GlobalLink Vasont. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Vasont.Inspire.Core.Tests
{
    using System.Collections.Generic;
    using System.Drawing;
    using Vasont.Inspire.Core.Extensions;
    using Xunit;

    /// <summary>
    /// This class is contains tests for the Image extensions
    /// </summary>
    [Trait("Category", "Image Extension Tests")]
    public class ImageExtensionTests
    {
        /// <summary>
        /// Gets the create thumbnail test values.
        /// </summary>
        /// <value>
        /// The create thumbnail test values.
        /// </value>
        public static IEnumerable<object[]> CreateThumbnailTestValues => new[]
        {
            new object[] 
            { 
                Properties.Resources.ImageDefaultIcon,
                32, 
                32 
            }
        };

        /// <summary>
        /// Gets the create thumbnail from bytes test values.
        /// </summary>
        /// <value>
        /// The create thumbnail from bytes test values.
        /// </value>
        public static IEnumerable<object[]> CreateThumbnailFromBytesTestValues => new[]
        {
            new object[] 
            {
                Properties.Resources.ImageDefaultIcon,
                32, 
                32 
            }
        };

        /// <summary>
        /// Rescales the image test.
        /// </summary>
        /// <param name="sourceBitmap">The source image.</param>
        /// <param name="targetWidth">Width of the target.</param>
        /// <param name="targetHeight">Height of the target.</param>
        [Theory]
        [MemberData(nameof(CreateThumbnailTestValues))]
        public void RescaleImageTest(Bitmap sourceBitmap, int targetWidth, int targetHeight)
        {
            Assert.NotNull(sourceBitmap);

            using (Bitmap result = sourceBitmap.Rescale(targetWidth, targetHeight))
            {
                ThumbnailResultAssertions(sourceBitmap, result, targetWidth, targetHeight);
            }
        }

        /// <summary>
        /// Thumbnail creation test.
        /// </summary>
        /// <param name="sourceBitmap">The source image.</param>
        /// <param name="targetWidth">Width of the target.</param>
        /// <param name="targetHeight">Height of the target.</param>
        [Theory]
        [MemberData(nameof(CreateThumbnailTestValues))]
        public void CreateThumbnailTest(Bitmap sourceBitmap, int targetWidth, int targetHeight)
        {
            Assert.NotNull(sourceBitmap);

            using (Bitmap result = sourceBitmap.Thumbnail(targetWidth, targetHeight))
            {
                ThumbnailResultAssertions(sourceBitmap, result, targetWidth, targetHeight);
            }
        }

        /// <summary>
        /// Executes the test result assertions.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="result">The result.</param>
        /// <param name="targetWidth">Width of the target.</param>
        /// <param name="targetHeight">Height of the target.</param>
        private static void ThumbnailResultAssertions(Image sourceImage, Image result, int targetWidth, int targetHeight)
        {
            int originalWidth = sourceImage.Width;
            int originalHeight = sourceImage.Height;
            Assert.NotNull(result);
            Assert.NotEqual(originalWidth, result.Width);
            Assert.NotEqual(originalHeight, result.Height);
            Assert.Equal(targetWidth, result.Width);
            Assert.Equal(targetHeight, result.Height);
        }
    }
}
