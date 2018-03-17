using System.Windows.Input;

namespace DrawToolsLib
{
    /// <summary>
    /// Base class for all drawing tools
    /// </summary>
    abstract class Tool
    {
        public abstract void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e);

        public abstract void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e);

        public abstract void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e);

        public abstract void OnKeyDown(DrawingCanvas drawingCanvas, KeyboardEventArgs e);
        
        public abstract void OnKeyUp(DrawingCanvas drawingCanvas, KeyboardEventArgs e);

        public abstract void SetCursor(DrawingCanvas drawingCanvas);
    }
}
