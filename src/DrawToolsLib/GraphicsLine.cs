using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DrawToolsLib
{
    /// <summary>
    ///  Line graphics object.
    /// </summary>
    public class GraphicsLine : GraphicsBase
    {
        #region Class Members

        protected Point lineStart;
        protected Point lineEnd;
        protected Point currPos;

        #endregion Class Members

        #region Constructors

        public GraphicsLine(Point start, Point end, double lineWidth, LineStyleType lineStyle, Color objectColor, double actualScale)
        {
            this.lineStart = start;
            this.lineEnd = end;
            this.graphicsLineWidth = lineWidth;
            this.graphicsLineStyle = lineStyle;
            this.graphicsObjectColor = objectColor;
            this.graphicsActualScale = actualScale;
        }

        public GraphicsLine()
            :
            this(new Point(0.0, 0.0), new Point(100.0, 100.0), 1.0, LineStyleType.Solid, Colors.Black, 1.0)
        {
        }

        #endregion Constructors

        #region Properties

        public Point Start
        {
            get { return lineStart; }
            set { lineStart = value; }
        }

        public Point End
        {
            get { return lineEnd; }
            set { lineEnd = value; }
        }

        #endregion Properties

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

            var dashStyle = new DashStyle();
            dashStyle.Dashes.Add(5);

            var pen = this.LineStyle == LineStyleType.Dashed
                ? new Pen(new SolidColorBrush(this.ObjectColor), this.ActualLineWidth) { DashStyle = dashStyle }
                : new Pen(new SolidColorBrush(this.ObjectColor), this.ActualLineWidth);

            drawingContext.DrawLine(
                pen,
                lineStart,
                this.lineEnd);

            base.Draw(drawingContext);
        }

        /// <summary>
        /// Test whether object contains point
        /// </summary>
        public override bool Contains(Point point)
        {
            var g = new LineGeometry(lineStart, lineEnd);

            return g.StrokeContains(new Pen(Brushes.Black, LineHitTestWidth), point);
        }

        /// <summary>
        /// XML serialization support
        /// </summary>
        /// <returns></returns>
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsLine(this);
        }

        /// <summary>
        /// Get number of handles
        /// </summary>
        public override int HandleCount
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        public override Point GetHandle(int handleNumber)
        {
            return handleNumber == 1 ? this.lineStart : this.lineEnd;
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        public override int MakeHitTest(Point point)
        {
            if (IsSelected)
            {
                for (var i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (Contains(point))
                return 0;

            return -1;
        }


        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        public override bool IntersectsWith(Rect rectangle)
        {
            var rg = new RectangleGeometry(rectangle);

            var lg = new LineGeometry(lineStart, lineEnd);
            var widen = lg.GetWidenedPathGeometry(new Pen(Brushes.Black, LineHitTestWidth));

            var p = Geometry.Combine(rg, widen, GeometryCombineMode.Intersect, null);

            return (!p.IsEmpty());
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                case 2:
                    return Cursors.SizeAll;
                default:
                    return HelperFunctions.DefaultCursor;
            }
        }

        /// <summary>
        /// Move handle to new point (resizing)
        /// </summary>
        public override void MoveHandleTo(Point point, int handleNumber)
        {
            this.currPos = new Point(point.X, point.Y);

            if (handleNumber == 1)
            {
                lineStart = point;
            }
            else
            {
                lineEnd = point;
            }

            RefreshDrawingEx();
        }

        /// <summary>
        /// An extension of RefreshDrawing that takes modifier keys into account.
        /// </summary>
        public void RefreshDrawingEx()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // Draw straight lines when shift is pressed.
                var diffX = Math.Abs(lineStart.X - lineEnd.X);
                var diffY = Math.Abs(lineStart.Y - lineEnd.Y);

                if (diffX > diffY)
                {
                    lineEnd.Y = lineStart.Y;
                }
                else
                {
                    lineEnd.X = lineStart.X;
                }
            }
            else
            {
                this.lineEnd.X = this.currPos.X;
                this.lineEnd.Y = this.currPos.Y;
            }

            this.RefreshDrawing();
        }

        /// <summary>
        /// Move object
        /// </summary>
        public override void Move(double deltaX, double deltaY)
        {
            lineStart.X += deltaX;
            lineStart.Y += deltaY;

            lineEnd.X += deltaX;
            lineEnd.Y += deltaY;

            RefreshDrawing();
        }


        #endregion Overrides
    }
}
