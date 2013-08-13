using System;
using SlimMath;

namespace Psy.Core
{
    public static class Color4Extensions
    {
        public static Color4 Multiply(this Color4 me, float amount)
        {
            return new Color4(me.Alpha * amount, me.Red * amount, me.Green * amount, me.Blue * amount);
        }

        public static Color4 PremultiplyAlpha(this Color4 me)
        {
            return new Color4(me.Alpha, me.Red * me.Alpha, me.Green * me.Alpha, me.Blue * me.Alpha);
        }

        public static Color4 MakeSolid(this Color4 me)
        {
            return new Color4(1.0f, me.Red, me.Green, me.Blue);
        }

        public static Color4 Interpolate(this Color4 me, Color4 to, float amount)
        {
            var a = me.Alpha + ((to.Alpha - me.Alpha) * amount);
            var r = me.Red + ((to.Red - me.Red) * amount);
            var g = me.Green + ((to.Green - me.Green) * amount);
            var b = me.Blue + ((to.Blue - me.Blue) * amount);
            return new Color4(a, r, g, b);
        }

        public static Color4 MakeTransparent(this Color4 me, float amount)
        {
            return new Color4(
                me.Alpha * amount,
                me.Red * amount,
                me.Green * amount,
                me.Blue * amount);
        }
    }

    public static class Color4StringExtensions
    {
        public static Color4 ParseColor4(this string str)
        {
            var parts = str.Split(',');

            if (parts.Length != 4)
            {
                throw new Exception(string.Format("Cannot parse `{0}` as a Color4.", str));
            }

            var a = float.Parse(parts[0]);
            var r = float.Parse(parts[1]);
            var g = float.Parse(parts[2]);
            var b = float.Parse(parts[3]);

            return new Color4(a, r, g, b);
        }
    }
}