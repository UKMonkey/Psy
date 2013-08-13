using Psy.Core.Input;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Desktop : Widget
    {
        public float Opacity { get; set; }

        internal Desktop(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent)
        {
            AutoSize = AutoSize.Width | AutoSize.Height;
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            guiRenderer.Desktop(this);
        }

        internal override void IntMouseDown(Vector2 position, MouseButton button)
        {
            base.IntMouseDown(position, button);
            Core.Logging.Logger.Write(string.Format("Desktop down {0}", position));
        }
    }
}