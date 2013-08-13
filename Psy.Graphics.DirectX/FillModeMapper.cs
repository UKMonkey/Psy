using System;

namespace Psy.Graphics.DirectX
{
    public static class FillModeMapper
    {
        public static FillMode MapFrom(SlimDX.Direct3D9.FillMode fillMode)
        {
            switch (fillMode)
            {
                case SlimDX.Direct3D9.FillMode.Solid:
                    return FillMode.Solid;
                case SlimDX.Direct3D9.FillMode.Wireframe:
                    return FillMode.Wireframe;
                case SlimDX.Direct3D9.FillMode.Point:
                    return FillMode.Point;
                default:
                    throw new ArgumentOutOfRangeException("fillMode");
            }
        }

        public static SlimDX.Direct3D9.FillMode MapFrom(FillMode fillMode)
        {
            switch (fillMode)
            {
                case FillMode.Solid:
                    return SlimDX.Direct3D9.FillMode.Solid;
                case FillMode.Wireframe:
                    return SlimDX.Direct3D9.FillMode.Wireframe;
                case FillMode.Point:
                    return SlimDX.Direct3D9.FillMode.Point;
                default:
                    throw new ArgumentOutOfRangeException("fillMode");
            }
        }
    }
}