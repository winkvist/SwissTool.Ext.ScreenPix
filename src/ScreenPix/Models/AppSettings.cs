// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.ScreenPix.Models
{
    using System.Windows;
    using System.Windows.Media;

    using DrawToolsLib;

    /// <summary>
    /// The app settings class.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        public AppSettings()
        {
            this.ToolBarLocationX = -1;
            this.ToolBarLocationY = -1;
            this.LineWidth = 2.0;
            this.LineColor = Colors.Black;
            this.Tool = ToolType.Pointer;
            this.FontFamily = new FontFamily("Calibri");
            this.FontSize = 14;
            this.FontWeight = FontWeights.Normal;
            this.FontStyle = FontStyles.Normal;
            this.FontStyleType = "Style";
            this.LineStyle = LineStyleType.Solid;
            this.ImageQuality = 90;
            this.DefaultFileExtension = "JPG";
        }

        /// <summary>
        /// Gets or sets ToolBarLocationX.
        /// </summary>
        public int ToolBarLocationX { get; set; }

        /// <summary>
        /// Gets or sets ToolBarLocationY.
        /// </summary>
        public int ToolBarLocationY { get; set; }

        /// <summary>
        /// Gets or sets LineWidth.
        /// </summary>
        public double LineWidth { get; set; }

        /// <summary>
        /// Gets or sets the tool.
        /// </summary>
        /// <value>
        /// The tool.
        /// </value>
        public ToolType Tool { get; set; }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        /// <value>The line style.</value>
        public LineStyleType LineStyle { get; set; }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>
        /// The color of the line.
        /// </value>
        public Color LineColor { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight { get; set; }
        
        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the type of the font style.
        /// </summary>
        /// <value>The type of the font style.</value>
        public string FontStyleType { get; set; }

        /// <summary>
        /// Gets or sets the default image extension.
        /// </summary>
        /// <value>
        /// The default image extension.
        /// </value>
        public string DefaultFileExtension { get; set; }

        /// <summary>
        /// Gets or sets the image quality.
        /// </summary>
        /// <value>
        /// The image quality.
        /// </value>
        public int ImageQuality { get; set; }
    }
}
