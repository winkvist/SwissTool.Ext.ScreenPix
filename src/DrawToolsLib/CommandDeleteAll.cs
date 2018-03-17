using System.Collections.Generic;

namespace DrawToolsLib
{
    /// <summary>
    /// Delete command.
    /// </summary>
    class CommandDeleteAll : CommandBase
    {
        List<PropertiesGraphicsBase> cloneList;

        // Create this command BEFORE applying Delete All function.
        public CommandDeleteAll(DrawingCanvas drawingCanvas)
        {
            cloneList = new List<PropertiesGraphicsBase>();

            // Make clone of the whole list.
            foreach(GraphicsBase g in drawingCanvas.GraphicsList)
            {
                cloneList.Add(g.CreateSerializedObject());
            }
        }

        /// <summary>
        /// Add all deleted objects to GraphicsList
        /// </summary>
        public override void Undo(DrawingCanvas drawingCanvas)
        {
            foreach (PropertiesGraphicsBase o in cloneList)
            {
                drawingCanvas.GraphicsList.Add(o.CreateGraphics());
            }

            drawingCanvas.RefreshClip();
        }

        /// <summary>
        /// Detete All again
        /// </summary>
        public override void Redo(DrawingCanvas drawingCanvas)
        {
            drawingCanvas.GraphicsList.Clear();
        }
    }
}
