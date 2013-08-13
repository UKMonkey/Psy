using SlimMath;

namespace Psy.Core.Console
{
    public class ConsoleLine
    {
        public readonly string Text;
        private Color4 _colour;
        private int _life;

        public ConsoleLine(string text) : this(text, GetDefaultConsoleLineColor()) { }

        public ConsoleLine(string text, Color4 colour)
        {
            Text = text;
            _colour = colour;
            _life = GetDefaultLineLife();
        }

        private static Color4 GetDefaultConsoleLineColor()
        {
            return new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private static int GetDefaultLineLife()
        {
            return 1600;
        }

        public bool EdgeTowardsDeath()
        {
            if (_life <= 0)
            {
                return true;
            }
            _life -= 1;
            return false;
        }

        public Color4 GetCalculatedColor()
        {
            if (_life >= 200)
            {
                return _colour;
            }
            var alpha = _life / 200.0f;
            return new Color4(alpha, _colour.Red, _colour.Green, _colour.Blue);
        }
    }
}
