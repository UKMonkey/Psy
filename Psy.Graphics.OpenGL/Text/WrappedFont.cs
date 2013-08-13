using System.Drawing;
using Psy.Graphics.Text;
using QuickFont;
using SlimMath;

namespace Psy.Graphics.OpenGL.Text
{
    public class WrappedFont : IFont 
    {
        private readonly QFont _font;

        public WrappedFont(QFont font)
        {
            _font = font;
        }

        public void DrawString(string text, int x, int y, Color4 color)
        {
            QFont.Begin();

            _font.Options.Colour = color.ToGLColor4();
            _font.Print(text, new OpenTK.Vector2(x, y));

            QFont.End();
        }

        public void DrawString(string text, Rectangle rectangle, TextFormat format, Color4 color)
        {
            QFont.Begin();

            _font.Options.Colour = color.ToGLColor4();
            _font.Print(text, new OpenTK.Vector2(rectangle.X, rectangle.Y));

            QFont.End();
        }

        public Rectangle MeasureString(string text, TextFormat textFormat)
        {
            var result = _font.Measure(text);
            return new Rectangle(0, 0, (int)result.Width, (int)result.Height);
        }
    }
}