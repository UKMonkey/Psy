using Psy.Core;
using Psy.Core.Input;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Button : Widget
    {
        private readonly NinePatchHandle _ninePatchHandle;

        internal Button(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent)
        {
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        public string Label { get; set; }
        public string ImageName { get; set; }
        private float Highlight { get; set; }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            if (IsMouseDown)
            {
                guiRenderer.Rectangle(
                    Size,
                    guiRenderer.ColourScheme.ButtonBackgroundClick,
                    guiRenderer.ColourScheme.ButtonSurroundClick
                    );
            }
            else
            {
                var backgroundColour =
                    guiRenderer.ColourScheme.ButtonBackground.Interpolate(
                        guiRenderer.ColourScheme.ButtonBackgroundHighlight, Highlight);

                var surroundColour =
                    guiRenderer.ColourScheme.ButtonSurround.Interpolate(
                        guiRenderer.ColourScheme.ButtonSurroundHighlight, Highlight);

                guiRenderer.Rectangle(
                    Size,
                    backgroundColour,
                    surroundColour
                    );
            }

            string textureName;

            if (IsMouseDown)
            {
                textureName = "guiskin_down.png";
            }
            else if (IsMouseOver)
            {
                textureName = "guiskin_sel.png";
            }
            else
            {
                textureName = null;
            }

            guiRenderer.NinePatch(Size, _ninePatchHandle, textureName);

            guiRenderer.Text(
                    Label, 
                    Size / 2, 
                    VerticalAlignment.Middle,
                    HorizontalAlignment.Centre);

            if (!string.IsNullOrEmpty(ImageName))
            {
                guiRenderer.Image(ImageName, Size);
            }
        }

        internal override void Update()
        {
            base.Update();

            if (IsMouseOver)
            {
                if (Highlight <= 1.0f)
                {
                    Highlight += 0.33f;
                }
            }
            else
            {
                if (Highlight > 0.0f)
                {
                    Highlight -= 0.1f;
                }
            }
        }
    }
}