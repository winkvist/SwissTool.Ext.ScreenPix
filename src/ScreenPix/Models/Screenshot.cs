// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Screenshot.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the Screenshot type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Models
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The screenshot.
    /// </summary>
    public class Screenshot : IDisposable
    {
        /// <summary>
        /// Gets or sets the working area.
        /// </summary>
        /// <value>
        /// The working area.
        /// </value>
        public Rectangle WorkingArea { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Image?.Dispose();
        }
    }
}
