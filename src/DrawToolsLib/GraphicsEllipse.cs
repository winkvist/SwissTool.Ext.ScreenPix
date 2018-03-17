using System;
using System.Windows;
using System.Windows.Media;

namespace DrawToolsLib
{
    /// <summary>
    ///  Rectangle graphics object.
    /// </summary>
    public class GraphicsEllipse : GraphicsRectangleBase
    {
        #region Constructors

        public GraphicsEllipse(double left, double top, double right, double bottom,
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

            //RefreshDrawng();
        }

        public GraphicsEllipse()
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
            if ( drawingContext == null )
            {
                throw new ArgumentNullException("drawingContext");
            }

            Rect r = Rectangle;

            Point center = new Point(
                (r.Left + r.Right) / 2.0,
                (r.Top + r.Bottom) / 2.0);

            double radiusX = (r.Right - r.Left) / 2.0;
            double radiusY = (r.Bottom - r.Top) / 2.0;

            var dashStyle = new DashStyle();
            dashStyle.Dashes.Add(5);

            var pen = this.LineStyle == LineStyleType.Dashed 
                ? new Pen(new SolidColorBrush(this.ObjectColor), this.ActualLineWidth) { DashStyle = dashStyle } 
                : new Pen(new SolidColorBrush(this.ObjectColor), this.ActualLineWidth);

            drawingContext.DrawEllipse(
                null,
                pen,
                center,
                radiusX,
                radiusY);

            base.Draw(drawingContext);
        }

        /// <summary>
        /// Test whether object contains point
        /// </summary>
        public override bool Contains(Point point)
        {
            if ( IsSelected )
            {
                return this.Rectangle.Contains(point);
            }
            else
            {
                EllipseGeometry g = new EllipseGeometry(Rectangle);

                return g.FillContains(point) || g.StrokeContains(new Pen(Brushes.Black, ActualLineWidth), point);
            }
        }

        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        public override bool IntersectsWith(Rect rectangle)
        {
            RectangleGeometry rg = new RectangleGeometry(rectangle);    // parameter
            EllipseGeometry eg = new EllipseGeometry(Rectangle);        // this object rectangle

            PathGeometry p = Geometry.Combine(rg, eg, GeometryCombineMode.Intersect, null);

            return (!p.IsEmpty());
        }


        /// <summary>
        /// Serialization support
        /// </summary>
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsEllipse(this);
        }


        #endregion Overrides

    }
}
