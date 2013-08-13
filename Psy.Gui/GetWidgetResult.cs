using Psy.Gui.Components;
using SlimMath;

namespace Psy.Gui
{
    internal class GetWidgetResult
    {
        /// <summary>
        /// Resulting widget, can be null.
        /// </summary>
        public Widget Widget { get; set; }
        
        /// <summary>
        /// Position relative to the window top-left.
        /// </summary>
        public Vector2 WindowPosition { get; set; }

        /// <summary>
        /// Position relative to the Widgets absolute top-left.
        /// </summary>
        public Vector2 Position { get; set; }

        public GetWidgetResult()
        {
            Widget = null;
            WindowPosition = new Vector2();
            Position = new Vector2();
        }
    }
}