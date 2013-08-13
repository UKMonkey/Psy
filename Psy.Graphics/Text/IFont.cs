using System.Drawing;
using SlimMath;

namespace Psy.Graphics.Text
{
    public interface IFont {
        void DrawString(string text, int x, int y, Color4 color);
        void DrawString(string text, Rectangle rectangle, TextFormat format, Color4 color);
        Rectangle MeasureString(string text, TextFormat textFormat);
    }
}