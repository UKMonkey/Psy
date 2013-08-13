using SlimMath;

namespace Psy.Core
{
    public struct ColourRect
    {
        public Color4 TopLeft;
        public Color4 TopRight;
        public Color4 BottomLeft;
        public Color4 BottomRight;

        public ColourRect(ColourRect cr)
        {
            TopLeft = cr.TopLeft;
            TopRight = cr.TopRight;
            BottomLeft = cr.BottomLeft;
            BottomRight = cr.BottomRight;
        }

        public Color4 BilinearInterp(float fX, float fY)
        {
            var xColourLeft = BottomLeft.Interpolate(TopLeft, fY);
            var xColourRight = BottomRight.Interpolate(TopRight, fY);
            return xColourLeft.Interpolate(xColourRight, fX);
        }

        public ColourRect(Color4 colour)
        {
            TopLeft = colour;
            TopRight = colour;
            BottomLeft = colour;
            BottomRight = colour;
        }

        public ColourRect(Color4 topLeft, Color4 topRight, Color4 bottomLeft, Color4 bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }

        public static ColourRect operator +(ColourRect c1, ColourRect c2)
        {
            return new ColourRect(
                c1.TopLeft + c2.TopLeft,
                c1.TopRight + c2.TopRight,
                c1.BottomLeft + c2.BottomLeft,
                c1.BottomRight + c2.BottomRight
                );

        }

        public ColourRect MultiplyColours(float f)
        {
            return new ColourRect(
                TopLeft * f, TopRight * f, 
                BottomLeft * f, BottomRight * f);
        }
    }
}
