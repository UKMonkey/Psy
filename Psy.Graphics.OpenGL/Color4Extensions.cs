using OpenTK.Graphics;

namespace Psy.Graphics.OpenGL
{
    public static class Color4Extensions
    {
        public static Color4 ToGLColor4(this SlimMath.Color4 me)
        {
            return new Color4(me.Red, me.Green, me.Blue, me.Alpha);
        }
    }
}