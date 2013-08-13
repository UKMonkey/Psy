using SlimMath;
using Psy.Core;

namespace Psy.Gui.Components
{
    public class GuiWindow : Widget
    {
        private readonly NinePatchHandle _ninePatchHandle;
        public bool RenderBackground { get; set; }

        internal GuiWindow(GuiManager guiManager, Widget parent)
            : base(guiManager, parent)
        {
            Margin = new Vector2(5, 5);
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            if (RenderBackground)
            {
                guiRenderer.NinePatch(Size, _ninePatchHandle, alpha: Alpha);
            }
            base.Render(guiRenderer);
        }
    }
}