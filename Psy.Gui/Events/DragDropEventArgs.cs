using Psy.Gui.Components;

namespace Psy.Gui.Events
{
    public class DragDropEventArgs
    {
        public Widget Target { get; set; }
        public Widget Dragged { get; set; }
    }
}