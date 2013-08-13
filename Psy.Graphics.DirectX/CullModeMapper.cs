using System;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public static class CullModeMapper
    {
        public static CullMode MapFrom(Cull cullMode)
        {
            switch (cullMode)
            {
                case Cull.None:
                    return CullMode.None;
                case Cull.Counterclockwise:
                    return CullMode.CCW;
                case Cull.Clockwise:
                    return CullMode.CW;
                default:
                    throw new ArgumentOutOfRangeException("cullMode");
            }
        }

        public static Cull MapFrom(CullMode cullMode)
        {
            switch (cullMode)
            {
                case CullMode.None:
                    return Cull.None;
                case CullMode.CW:
                    return Cull.Clockwise;
                case CullMode.CCW:
                    return Cull.Counterclockwise;
                default:
                    throw new ArgumentOutOfRangeException("cullMode");
            }
        }
    }
}