using System.Drawing;
using SlimMath;

namespace Psy.Core
{
    public static class ColorExtensions
    {
        public static Color4 ToColor4(this Color color)
        {
            return new Color4(color.A, color.R, color.G, color.B);  
        }
    }

    public static class Colours
    {
        public static readonly Color4 Black = new Color4(1.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Color4 DarkGrey = new Color4(1.0f, 0.2f, 0.2f, 0.2f);
        public static readonly Color4 Red = new Color4(1.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Color4 Grey = new Color4(1.0f, 0.6f, 0.6f, 0.6f);
        public static readonly Color4 Green = new Color4(1.0f, 0.0f, 0.8f, 0.0f);
        public static readonly Color4 Blue = new Color4(1.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Color4 White = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Color4 Orange = new Color4(1.0f, 1.0f, 0.5f, 0.0f);
        public static readonly Color4 Yellow = new Color4(1.0f, 1.0f, 239.0f / 255.0f, 0.0f);
        public static readonly Color4 LightBlue = new Color4(173, 216, 230);
        public static readonly Color4 VeryDarkGrey = new Color4(1.0f, 0.1f, 0.1f, 0.1f);

        public static Color4 RandomSolid()
        {
            return new Color4(
                1.0f, 
                (float)StaticRng.Random.NextDouble(),
                (float)StaticRng.Random.NextDouble(),
                (float)StaticRng.Random.NextDouble());
        }
    }
}