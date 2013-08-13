using System;
using Psy.Core;
using Psy.Graphics.Text;
using SlimMath;

namespace Psy.Gui.Renderer
{
    public partial class GuiRenderer
    {
        public int Text(string fontface, int size, string text, Color4 colour, Vector2 position, VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.LeftWithMargin, float opacity = 1, bool italic = false)
        {
            TextFormat drawTextFormat;
            var localOffset = new Vector2();

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.LeftAbsolute:
                    drawTextFormat = TextFormat.Left;
                    break;
                case HorizontalAlignment.LeftWithMargin:
                    drawTextFormat = TextFormat.Left;
                    localOffset = new Vector2(Dimensions.ContentMargin, 0);
                    break;
                case HorizontalAlignment.Centre:
                    drawTextFormat = TextFormat.Center;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("horizontalAlignment");
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    drawTextFormat = drawTextFormat | TextFormat.Top;
                    break;
                case VerticalAlignment.Middle:
                    drawTextFormat = drawTextFormat | TextFormat.VerticalCenter;
                    break;
                case VerticalAlignment.Bottom:
                    drawTextFormat = drawTextFormat | TextFormat.Bottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("verticalAlignment");
            }

            var font = _graphicsContext.GetFont(fontface, size, italic: italic);

            var extents = font.MeasureString(text, drawTextFormat);

            font.DrawString(
                text,
                (int)(_offset.X + extents.Left + position.X + localOffset.X),
                (int)(_offset.Y + extents.Top + position.Y + localOffset.Y),
                colour);

            return extents.Width;
        }

        public int Text(string text, Vector2 position, VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalAlignment, float opacity = 1.0f, string fontface = null, int? fontSize = null, Color4? colour = null)
        {
            TextFormat drawTextFormat;
            var localOffset = new Vector2();

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.LeftWithMargin:
                    drawTextFormat = TextFormat.Left;
                    localOffset = new Vector2(Dimensions.ContentMargin, 0);
                    break;
                case HorizontalAlignment.LeftAbsolute:
                    drawTextFormat = TextFormat.Left;
                    localOffset = new Vector2(0, 0);
                    break;
                case HorizontalAlignment.Centre:
                    drawTextFormat = TextFormat.Center;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("horizontalAlignment");
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    drawTextFormat = drawTextFormat | TextFormat.Top;
                    break;
                case VerticalAlignment.Middle:
                    drawTextFormat = drawTextFormat | TextFormat.VerticalCenter;
                    break;
                case VerticalAlignment.Bottom:
                    drawTextFormat = drawTextFormat | TextFormat.Bottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("verticalAlignment");
            }

            var font = _font;

            if (!string.IsNullOrEmpty(fontface)&& fontSize != null)
            {
                font = _graphicsContext.GetFont(fontface, fontSize.Value);
            }

            var extents = font.MeasureString(text, drawTextFormat);

            if (!colour.HasValue)
                colour = ColourScheme.TextColour.MakeTransparent(opacity);

            font.DrawString(
                text,
                (int)(_offset.X + extents.Left + position.X + localOffset.X),
                (int)(_offset.Y + extents.Top + position.Y + localOffset.Y),
                colour.Value);

            return extents.Width;
        }
    }
}