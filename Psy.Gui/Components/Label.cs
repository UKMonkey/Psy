using System;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Label : Widget
    {
        private int _maxWidth;
        public string Text { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public string FontFace { get; set; }
        public int FontSize { get; set; }
        public String Colour { get; set; }

    internal Label(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent)
        {
            VerticalAlignment = VerticalAlignment.Top;
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            var verticalOffset = CalculateVerticalOffset();
            Color4? colour = null;

            if (!string.IsNullOrEmpty(Colour))
                colour = guiRenderer.ColourScheme.GetColour(Colour);

            _maxWidth = guiRenderer.Text(Text, verticalOffset, VerticalAlignment, fontFace: FontFace, fontSize: FontSize, colour:colour);
        }

        internal override void Update()
        {
            var width = Size.X;
            if (AutoSize.HasFlag(AutoSize.Width))
            {
                width = _maxWidth;
            }
            var height = Size.Y;
            if (AutoSize.HasFlag(AutoSize.Height))
            {
                height = FontSize > 0 ? FontSize : 16;
            }

            if (AutoSize != AutoSize.None)
            {
                Size = new Vector2(width, height);
            }
        }

        private Vector2 CalculateVerticalOffset()
        {
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    return new Vector2(0, 0);
                case VerticalAlignment.Middle:
                    return new Vector2(0, Size.Y/2);
                case VerticalAlignment.Bottom:
                    return new Vector2(0, Size.Y);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}