using SlimMath;

namespace Psy.Gui.Components
{
    public class MultilineLabel : Widget
    {
        private const int LineHeight = 12;
        public string Value { get; set; }

        internal MultilineLabel(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent) { }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);

            var y = 0;
            foreach (var line in Value.Split('\n'))
            {
                guiRenderer.Text(line, new Vector2(0, y));
                y += LineHeight;
            }
        }
    }
}