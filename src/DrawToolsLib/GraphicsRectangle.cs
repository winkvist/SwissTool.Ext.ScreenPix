using System;
using System.Windows;
using System.Windows.Media;

namespace DrawToolsLib
{
    /// <summary>
    ///  Rectangle graphics object.
    /// </summary>
    public class GraphicsRectangle : GraphicsRectangleBase
    {
        #region Constructors

        public GraphicsRectangle(double left, double top, double right, double bottom,
            double lineWidth, LineStyleType lineStyle, Color objectColor, double actualScale)
        {
            this.rectangleLeft = left;
            this.rectangleTop = top;
            this.rectangleRight = right;
            this.rectangleBottom = bottom;
            this.graphicsLineWidth = lineWidth;
            this.graphicsLineStyle = lineStyle;
            this.graphicsObjectColor = objectColor;
            this.graphicsActualScale = actualScale;
        }

        public GraphicsRectangle()
            :
            this(0.0, 0.0, 100.0, 100.0, 1.0, LineStyleType.Solid, Colors.Black, 1.0)
        {
        }

        #endregion Constructors

        #region Overrides

        /// <summary>
        /// Draw object
        /// </summary>
        public override void Draw(DrawingContext drawingContext)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            var dashStyle = new DashStyle();
            dashStyle.Dashes.Add(5);

            var pen = this.LineStyle == LineStyleType.Dashed
                ? new Pen(new SolidColorBrush(this.ObjectColor), this.ActualLineWidth) { DashStyle = dashStyle }
                : new Pen(new SolidColorBrush(this.ObjectColor), this.ActualLineWidth);

            drawingContext.DrawRectangle(
                null,
                pen,
                this.Rectangle); 

            base.Draw(drawingContext);
        }

        /// <summary>
        /// Test whether object contains point
        /// </summary>
        public override bool Contains(Point point)
        {
            return this.Rectangle.Contains(point);
        }

        /// <summary>
        /// Serialization support
        /// </summary>
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsRectangle(this);
        }


        #endregion Overrides

    }
}
