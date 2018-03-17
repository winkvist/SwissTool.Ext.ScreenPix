// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationManager.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The application manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Linq;

    using SwissTool.Ext.ScreenPix.Models;
    using SwissTool.Framework.Infrastructure;

    /// <summary>
    /// The application manager.
    /// </summary>
    public static class ApplicationManager
    {
        /// <summary>
        /// Gets the supported image types.
        /// </summary>
        /// <returns>The image type.</returns>
        internal static readonly List<string> SupportedImageTypes = new List<string> { "BMP", "GIF", "JPG", "JPEG", "JPE", "JFIF", "PNG", "TIF", "TIFF" };

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        internal static AppSettings Settings { get; private set; }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        internal static ApplicationBase Application { get; private set; }

        /// <summary>
        /// Gets the supported image decoders.
        /// </summary>
        /// <returns>The supported image decoders.</returns>
        internal static List<ImageCodecInfo> GetSupportedImageDecoders()
        {
            var imageDecoders = ImageCodecInfo.GetImageDecoders();

            return imageDecoders.Where(d => SupportedImageTypes.Contains(d.FormatDescription, StringComparer.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        internal static void SaveSettings()
        {
            Application.SaveConfiguration(Settings);
        }

        /// <summary>
        /// Setups the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="settings">The settings.</param>
        internal static void Setup(ApplicationBase application, AppSettings settings)
        {
            Application = application;
            Settings = settings;
        }
    }
}
