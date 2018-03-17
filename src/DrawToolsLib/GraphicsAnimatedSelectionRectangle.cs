using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DrawToolsLib
{
    class GraphicsAnimatedSelectionRectangle : GraphicsRectangleBase
    {
        #region Constructors

        public GraphicsAnimatedSelectionRectangle(double left, double top, double right, double bottom, double actualScale)
        {
            this.rectangleLeft = left;
            this.rectangleTop = top;
            this.rectangleRight = right;
            this.rectangleBottom = bottom;
            this.graphicsLineWidth = 1.0;
            this.graphicsActualScale = actualScale;
        }

        public GraphicsAnimatedSelectionRectangle()
            :
            this(0.0, 0.0, 100.0, 100.0, 1.0)
        {
        }

        #endregion Constructors

        #region Overrides

        /// <summary>
        /// Draw graphics object
        /// </summary>
        public override void Draw(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(
               null,
               new Pen(Brushes.White, ActualLineWidth),
               Rectangle);

            var dashStyle = new DashStyle();
            dashStyle.Dashes.Add(5);

            var dashedPen = new Pen(Brushes.Black, ActualLineWidth) { DashStyle = dashStyle };

            var animation = new DoubleAnimation
            {
                BeginTime = new TimeSpan(0, 0, 0),
                RepeatBehavior =RepeatBehavior.Forever,
                From = 0,
                To = 20
            };

            dashStyle.BeginAnimation(DashStyle.OffsetProperty, animation);

            drawingContext.DrawRectangle(
                null,
                dashedPen,
                Rectangle);
        }

        public override bool Contains(Point point)
        {
            return this.Rectangle.Contains(point);
        }

        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return null;        // not used
        }

        #endregion Overrides
    }
}
