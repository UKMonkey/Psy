using System.Drawing;
using Psy.Graphics.Text;
using SlimDX.Direct3D9;
using SlimMath;

namespace Psy.Graphics.DirectX.Text
{
    public class Font : SlimDX.Direct3D9.Font, IFont
    {
        public Font(Device device, int height, int width, FontWeight weight, int mipLevels, bool italic, CharacterSet characterSet, 
                           Precision outputPrecision, FontQuality quality, PitchAndFamily pitchAndFamily, string faceName) 
            : base(device, height, width, weight, mipLevels, italic, characterSet, outputPrecision, quality, pitchAndFamily, faceName) { }

        public void DrawString(string text, int x, int y, Color4 color)
        {
            DrawString(null, text, x, y, color.ToColor4());
        }

        public void DrawString(string text, Rectangle rectangle, TextFormat format, int color)
        {
            DrawString(null, text, rectangle, MapTextFormat(format), color);
        }

        public void DrawString(string text, Rectangle rectangle, TextFormat format, Color4 color)
        {
            DrawString(null, text, rectangle, MapTextFormat(format), color.ToColor4());
        }

        public int MeasureString(string text, TextFormat format, Rectangle rectangle)
        {
            return MeasureString(null, text, MapTextFormat(format), ref rectangle);
        }

        private static DrawTextFormat MapTextFormat(TextFormat format)
        {
            var drawTextFormat = DrawTextFormat.Top;

            if (format.HasFlag(TextFormat.Bottom))
            {
                drawTextFormat ^= DrawTextFormat.Bottom;
            }

            if (format.HasFlag(TextFormat.Center))
            {
                drawTextFormat ^= DrawTextFormat.Center;
            }

            if (format.HasFlag(TextFormat.Left))
            {
                drawTextFormat ^= DrawTextFormat.Left;
            }

            if (format.HasFlag(TextFormat.Right))
            {
                drawTextFormat ^= DrawTextFormat.Right;
            }

            if (format.HasFlag(TextFormat.SingleLine))
            {
                drawTextFormat ^= DrawTextFormat.SingleLine;
            }

            if (format.HasFlag(TextFormat.Top))
            {
                drawTextFormat ^= DrawTextFormat.Top;
            }

            if (format.HasFlag(TextFormat.WordBreak))
            {
                drawTextFormat ^= DrawTextFormat.WordBreak;
            }

            if (format.HasFlag(TextFormat.VerticalCenter))
            {
                drawTextFormat ^= DrawTextFormat.VerticalCenter;
            }

            return drawTextFormat;
        }

        private static int _spaceWidth = -1;

        /// <summary>
        /// Measures width of string, correctly taking into account trailing spaces.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textFormat"></param>
        /// <returns></returns>
        public Rectangle MeasureString(string text, TextFormat textFormat)
        {
            if (_spaceWidth == -1)
            {
                CalculateSpaceWidth();
            }
            var size = MeasureString(null, text, MapTextFormat(textFormat));
            var trimStr = text.TrimEnd(' ');
            var spaceCount = text.Length - trimStr.Length;
            size.Width += (_spaceWidth * spaceCount);
            return size;
        }

        private void CalculateSpaceWidth()
        {
            var str1 = MeasureString(null, "| |", DrawTextFormat.Left).Width;
            var str2 = MeasureString(null, "||", DrawTextFormat.Left).Width;
            _spaceWidth = str1 - str2;
        }
    }
}