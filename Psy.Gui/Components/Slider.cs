using Psy.Core;
using Psy.Core.Input;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Slider : Widget
    {
        private readonly NinePatchHandle _ninePatchHandle;
        private const int SliderHeight = 24;
        private const int SliderWidth = 12;
        private const int SliderMargin = SliderWidth;

        public int MinimumValue { get; set; }
        public int MaximumValue { get; set; }
        public int Value { get; set; }

        internal Slider(GuiManager guiManager, Widget parent) : base(guiManager, parent)
        {
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            var positionScale = Value/(float)(MaximumValue - MinimumValue);

            guiRenderer.NinePatch(Size, _ninePatchHandle, "guiskin_down.png");

            var railSize = new Vector2(Size.X - (SliderMargin * 2), 4);
            guiRenderer.Image("slider_rail", railSize, new Vector2(SliderMargin, Size.Y / 2));

            var x = (int)((positionScale * (Size.X - (SliderMargin * 2)))) - (SliderWidth / 2);
            var y = (Size.Y / 2) - (SliderHeight / 2);
            guiRenderer.Image("slider", null, new Vector2(x + SliderMargin, y));
        }

        private void UpdateValue(Vector2 position)
        {
            var positionScale = (position.X - SliderMargin) / (Size.X - (SliderMargin * 2));
            positionScale = positionScale.Clamp(0.0f, 1.0f);

            Value = (int)(MinimumValue + ((MaximumValue - MinimumValue) * positionScale));
            OnChange();
        }

        internal override void IntMouseDown(Vector2 position, MouseButton button)
        {
            base.IntMouseDown(position, button);
            UpdateValue(position);
        }

        internal override void IntMouseMove(Vector2 position)
        {
            base.IntMouseMove(position);
            if (IsMouseDown)
            {
                UpdateValue(position);
            }
        }
    }
}