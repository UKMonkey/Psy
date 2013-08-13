using System;

namespace Psy.Core
{
    public static class PrimitiveExtensions
    {
        public static float Clamp(this float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float ToDegrees(this float value)
        {
            return SlimMath.MathHelper.RadiansToDegrees(value);
        }

        public static float ToRadians(this float value)
        {
            return SlimMath.MathHelper.DegreesToRadians(value);
        }
    }
}