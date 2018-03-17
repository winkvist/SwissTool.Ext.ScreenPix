using System.Windows.Input;

namespace DrawToolsLib
{
    /// <summary>
    /// Base class for rectangle-based tools
    /// </summary>
    abstract class ToolRectangleBase : ToolObject
    {
        /// <summary>
        /// Set cursor and resize new object.
        /// </summary>
        public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            drawingCanvas.Cursor = ToolCursor;

            if (e.LeftButton == MouseButtonState.Pressed && drawingCanvas.IsMouseCaptured)
            {
                if ( drawingCanvas.Count > 0 )
                {
                    drawingCanvas[drawingCanvas.Count - 1].MoveHandleTo(e.GetPosition(drawingCanvas), 5);
                }
            }
        }

        public override void OnKeyDown(DrawingCanvas drawingCanvas, KeyboardEventArgs e)
        {
            if (drawingCanvas.IsMouseCaptured)
            {
                var rect = drawingCanvas[drawingCanvas.Count - 1] as GraphicsRectangleBase;
                if (rect != null)
                {
                    rect.RefreshDrawingEx();
                }
            }
        }

        public override void OnKeyUp(DrawingCanvas drawingCanvas, KeyboardEventArgs e)
        {
            if (drawingCanvas.IsMouseCaptured)
            {
                var rect = drawingCanvas[drawingCanvas.Count - 1] as GraphicsRectangleBase;
                if (rect != null)
                {
                    rect.RefreshDrawingEx();
                }
            }
        }
    }
}
