using System;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public static class PrimitiveTypeMapper
    {
        public static SlimDX.Direct3D9.PrimitiveType MapFrom(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    return SlimDX.Direct3D9.PrimitiveType.PointList;
                case PrimitiveType.LineList:
                    return SlimDX.Direct3D9.PrimitiveType.LineList;
                case PrimitiveType.LineStrip:
                    return SlimDX.Direct3D9.PrimitiveType.LineStrip;
                case PrimitiveType.TriangleList:
                    return SlimDX.Direct3D9.PrimitiveType.TriangleList;
                case PrimitiveType.TriangleStrip:
                    return SlimDX.Direct3D9.PrimitiveType.TriangleStrip;
                case PrimitiveType.TriangleFan:
                    return SlimDX.Direct3D9.PrimitiveType.TriangleFan;
                default:
                    throw new ArgumentOutOfRangeException("primitiveType");
            }
        }
    }
}