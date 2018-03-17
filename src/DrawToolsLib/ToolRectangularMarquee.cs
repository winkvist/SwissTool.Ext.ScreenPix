using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DrawToolsLib
{
    /// <summary>
    /// Rectangular marqueee tool
    /// </summary>
    class ToolRectangularMarquee : ToolRectangleBase
    {
        public ToolRectangularMarquee()
        {
            var stream = new MemoryStream(Properties.Resources.RectangularMarquee);
            ToolCursor = new Cursor(stream);
        }

        /// <summary>
        /// Handle mouse down.
        /// Start moving, resizing or group selection.
        /// </summary>
        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            drawingCanvas.ClearSelection();

            var point = e.GetPosition(drawingCanvas);

            // Group selection. Create selection rectangle.
            var r = new GraphicsAnimatedSelectionRectangle(
                point.X, point.Y,
                point.X + 1, point.Y + 1,
                drawingCanvas.ActualScale);

            r.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));

            drawingCanvas.GraphicsList.Add(r);
            drawingCanvas.CurrentSelection = (GraphicsRectangleBase)r;

            drawingCanvas.CaptureMouse();
        }

        /// <summary>
        /// Handle mouse up.
        /// Return to normal state.
        /// </summary>
        public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (!drawingCanvas.IsMouseCaptured) return;

            var r = (GraphicsAnimatedSelectionRectangle)drawingCanvas[drawingCanvas.Count - 1];
            r.Normalize();
            Rect rect = r.Rectangle;

            // This means no selection
            if (r.Rectangle.Height <= 2 || r.Rectangle.Width <= 2)
            {
                drawingCanvas.GraphicsList.Remove(r);
                drawingCanvas.ClearSelection();
            }

            drawingCanvas.ReleaseMouseCapture();
            drawingCanvas.Cursor = HelperFunctions.DefaultCursor;
        }

        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            if (!drawingCanvas.IsMouseCaptured) return;

            var point = e.GetPosition(drawingCanvas);
            
            // Resize selection rectangle
            drawingCanvas[drawingCanvas.Count - 1].MoveHandleTo(point, 5);
        }
    }
}
