// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageUtilities.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageUtilities type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Utilities
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using SwissTool.Ext.ScreenPix.Models;

    using Clipboard = System.Windows.Clipboard;
    using PixelFormat = System.Drawing.Imaging.PixelFormat;
    using Size = System.Drawing.Size;

    /// <summary>
    /// The screen capture mode.
    /// </summary>
    public enum ScreenCaptureMode
    {
        /// <summary>
        /// Capture a single screen
        /// </summary>
        Single,

        /// <summary>
        /// Capture all screens
        /// </summary>
        All
    }

    /// <summary>
    /// An image utilities class.
    /// </summary>
    public static class ImageUtilities
    {
        /// <summary>
        /// FxCop requires all Marshalled functions to be in a class called NativeMethods.
        /// </summary>
        private static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }

        /// <summary>
        /// Captures a full screen image of the screen.
        /// </summary>
        /// <param name="mode">The screen capture mode.</param>
        /// <returns>A screenshot object.</returns>
        public static Screenshot CaptureScreen(ScreenCaptureMode mode)
        {
            var screenshots = (from screen in Screen.AllScreens
                               where mode != ScreenCaptureMode.Single || screen.Primary
                               select CaptureScreenInternal(screen.Bounds)).ToList();

            if (mode == ScreenCaptureMode.Single)
            {
                return screenshots.FirstOrDefault();
            }

            var totalWidth = screenshots.Sum(b => b.WorkingArea.Width);
            var totalHeight = screenshots.Select(b => b.WorkingArea.Height).FirstOrDefault();
            var startX = screenshots.Select(b => b.WorkingArea.X).FirstOrDefault();
            var startY = screenshots.Select(b => b.WorkingArea.Y).FirstOrDefault();

            var bmp = new Bitmap(totalWidth, totalHeight, PixelFormat.Format32bppArgb);
            using (var gfx = Graphics.FromImage(bmp))
            {
                foreach (var screenshot in screenshots)
                {
                    gfx.DrawImage(screenshot.Image, screenshot.WorkingArea);
                }
            }

            // Dispose the old temporary screenshots);
            foreach (var screenshot in screenshots)
            {
                screenshot?.Dispose();
            }

            return new Screenshot
                {
                    Image = bmp, 
                    WorkingArea = new Rectangle(startX, startY, totalWidth, totalHeight)
                };
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap" /> into a WPF <see cref="BitmapSource" />.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// A BitmapSource
        /// </returns>
        /// <remarks>
        /// Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        public static BitmapSource BitmapToBitmapSource(Bitmap image)
        {
            BitmapSource bitSrc = null;

            var hBitmap = image.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }
        
        /// <summary>
        /// Saves the screen image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void SaveScreenImage(UIElement source, double scale, int quality, string filename, Rectangle rectangle)
        {
            var imageFormat = GetImageFormat(filename);
            var encoder = GetBitmapEncoder(filename);

            using (var stream = GetScreenImageStream(source, scale, quality, encoder))
            {
                var img = (Bitmap)Image.FromStream(stream);

                if (!rectangle.IsEmpty)
                {
                    img = img.Clone(rectangle.Contract(1), img.PixelFormat);
                }

                var enc = GetEncoder(imageFormat);
                var encParams = new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, quality) } };

                img.Save(filename, enc, encParams);
            }
        }

        /// <summary>
        /// Copies the screen image to clipboard.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="encoder">The encoder.</param>
        public static void CopyScreenImageToClipboard(UIElement source, double scale, int quality, Rectangle rectangle, BitmapEncoder encoder = null)
        {
            using (var stream = GetScreenImageStream(source, scale, quality, encoder ?? new PngBitmapEncoder()))
            {
                var img = (Bitmap)Image.FromStream(stream);

                if (!rectangle.IsEmpty)
                {
                    img = img.Clone(rectangle.Contract(1), img.PixelFormat);
                }

                var bmpSource = BitmapToBitmapSource(img);
                Clipboard.SetImage(bmpSource);
            }
        }

        /// <summary>
        /// Prints the screen image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void PrintScreenImage(UIElement source, double scale, Rectangle rectangle)
        {
            var dialog = new System.Windows.Controls.PrintDialog();
            
            var dialogResult = dialog.ShowDialog() ?? false;
            if (!dialogResult)
            {
                return;
            }

            using (var stream = GetScreenImageStream(source, scale, 100, new PngBitmapEncoder()))
            {
                var img = (Bitmap)Image.FromStream(stream);

                if (!rectangle.IsEmpty)
                {
                    img = img.Clone(rectangle.Contract(1), img.PixelFormat);
                }

                var bmpSource = BitmapToBitmapSource(img);

                var visual = new DrawingVisual();
                using (var dc = visual.RenderOpen())
                {
                    dc.DrawImage(bmpSource, new Rect { Width = img.Width, Height = img.Height });    
                }

                var filename = $"screenshot_{DateTime.Now.ToString("yyyyMMddHHmm")}";

                dialog.PrintVisual(visual, filename);
            }
        }

        /// <summary>
        /// Gets the current dpi.
        /// </summary>
        /// <returns>The size.</returns>
        private static Size GetCurrentDpi()
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                return new Size { Width = (int)g.DpiX, Height = (int)g.DpiY };
            }
        }

        /// <summary>
        /// Converts the Rect into a Rectangle.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A rectangle object.</returns>
        public static Rectangle ToRectangle(this Rect value)
        {
            return value == Rect.Empty
                       ? Rectangle.Empty
                       : new Rectangle
                       {
                           X = (int)value.X,
                           Y = (int)value.Y,
                           Width = (int)value.Width,
                           Height = (int)value.Height
                       };
        }

        /// <summary>
        /// Converts the Rectangle into a Rect.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A rect object.</returns>
        public static Rect ToRect(this Rectangle value)
        {
            return value == Rectangle.Empty
                       ? Rect.Empty
                       : new Rect { X = value.X, Y = value.Y, Width = value.Width, Height = value.Height };
        }

        /// <summary>
        /// Contracts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns>A contracted rectangle.</returns>
        public static Rectangle Contract(this Rectangle source, int value)
        {
            return new Rectangle
            {
                X = source.X + value,
                Y = source.Y + value,
                Width = source.Width - (value * 2),
                Height = source.Height - (value * 2)
            };
        }

        /// <summary>
        /// Internal method to capture just one screen.
        /// </summary>
        /// <param name="workingArea">The working area.</param>
        /// <returns>A screenshot object.</returns>
        private static Screenshot CaptureScreenInternal(Rectangle workingArea)
        {
            var dpi = GetCurrentDpi();

            var bmp = new Bitmap(workingArea.Width, workingArea.Height, PixelFormat.Format32bppArgb);

            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.CopyFromScreen(workingArea.X, workingArea.Y, 0, 0, workingArea.Size, CopyPixelOperation.SourceCopy);
            }

            return new Screenshot { Image = bmp, WorkingArea = workingArea };
        }

        /// <summary>
        /// Gets the screen image stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="encoder">The encoder.</param>
        /// <returns>An image stream.</returns>
        private static Stream GetScreenImageStream(UIElement source, double scale, int quality, BitmapEncoder encoder)
        {
            var actualHeight = source.RenderSize.Height;
            var actualWidth = source.RenderSize.Width;

            var renderHeight = actualHeight * scale;
            var renderWidth = actualWidth * scale;

            var dpi = GetCurrentDpi(); // TODO Change 96 to DPI and fix marquee selection

            var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);

            source.Measure(source.RenderSize);
            source.Arrange(new Rect(source.RenderSize));
            renderTarget.Render(source);

            var outputStream = new MemoryStream();

            if (encoder is JpegBitmapEncoder)
            {
                var jpegEncoder = encoder as JpegBitmapEncoder;
                jpegEncoder.QualityLevel = quality;
            }

            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            encoder.Save(outputStream);

            return outputStream;
        }

        /// <summary>
        /// Gets the image format from a filename.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The image format.</returns>
        /// <exception cref="System.ArgumentException">Argument exception.</exception>
        /// <exception cref="System.NotImplementedException">Not implemented exception.</exception>
        private static ImageFormat GetImageFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"Unable to determine file extension for filename: {fileName}");
            }

            switch (extension.ToLower())
            {
                case ".bmp":
                    return ImageFormat.Bmp;

                case ".gif":
                    return ImageFormat.Gif;

                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".jfif":
                    return ImageFormat.Jpeg;

                case ".png":
                    return ImageFormat.Png;

                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the bitmap encoder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The bitmap encoder.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public static BitmapEncoder GetBitmapEncoder(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"Unable to determine file extension for filename: {fileName}");
            }

            switch (extension.ToLower())
            {
                case ".bmp":
                    return new BmpBitmapEncoder();

                case ".gif":
                    return new GifBitmapEncoder();

                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".jfif":
                    return new JpegBitmapEncoder();

                case ".png":
                    return new PngBitmapEncoder();

                case ".tif":
                case ".tiff":
                    return new TiffBitmapEncoder();

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the encoder.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>The encoder.</returns>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }
}
