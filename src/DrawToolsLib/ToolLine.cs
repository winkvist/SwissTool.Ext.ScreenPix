using System.Windows;
using System.Windows.Input;
using System.IO;

namespace DrawToolsLib
{
    /// <summary>
    /// Line tool
    /// </summary>
    class ToolLine : ToolObject
    {
        public ToolLine()
        {
            var stream = new MemoryStream(Properties.Resources.Line);
            ToolCursor = new Cursor(stream);
        }

        /// <summary>
        /// Create new object
        /// </summary>
        public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(drawingCanvas);

            AddNewObject(drawingCanvas,
                new GraphicsLine(
                p,
                new Point(p.X + 1, p.Y + 1),
                drawingCanvas.LineWidth,
                drawingCanvas.LineStyle,
                drawingCanvas.ObjectColor,
                drawingCanvas.ActualScale));
        }

        /// <summary>
        /// Set cursor and resize new object.
        /// </summary>
        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            drawingCanvas.Cursor = ToolCursor;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var line = drawingCanvas[drawingCanvas.Count - 1] as GraphicsLine;
                if (line != null)
                {
                    line.MoveHandleTo(e.GetPosition(drawingCanvas), 2);
                }
            }
        }

        public override void OnKeyDown(DrawingCanvas drawingCanvas, KeyboardEventArgs e)
        {
            if (drawingCanvas.IsMouseCaptured)
            {
                var line = drawingCanvas[drawingCanvas.Count - 1] as GraphicsLine;
                if (line != null)
                {
                    line.RefreshDrawingEx();
                }
            }
        }

        public override void OnKeyUp(DrawingCanvas drawingCanvas, KeyboardEventArgs e)
        {
            if (drawingCanvas.IsMouseCaptured)
            {
                var line = drawingCanvas[drawingCanvas.Count - 1] as GraphicsLine;
                if (line != null)
                {
                    line.RefreshDrawingEx();
                }
            }
        }
    }
}
