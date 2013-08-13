using SlimDX;

namespace Psy.Graphics.DirectX
{
    public static class Color4Extensions
    {
        public static Color4 ToColor4(this SlimMath.Color4 colour)
        {
            return new Color4(colour.Alpha, colour.Red, colour.Green, colour.Blue);
        }
    }
}
