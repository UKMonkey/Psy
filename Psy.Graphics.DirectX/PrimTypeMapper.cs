using System;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public static class PrimTypeMapper
    {
         public static PrimitiveType MapFrom(PrimType primType)
         {
             switch (primType)
             {
                 case PrimType.PointList:
                     return PrimitiveType.PointList;
                 case PrimType.LineList:
                     return PrimitiveType.LineList;
                 case PrimType.LineStrip:
                     return PrimitiveType.LineStrip;
                 case PrimType.TriangleList:
                     return PrimitiveType.TriangleList;
                 case PrimType.TriangleStrip:
                     return PrimitiveType.TriangleStrip;
                 case PrimType.TriangleFan:
                     return PrimitiveType.TriangleFan;
                 default:
                     throw new ArgumentOutOfRangeException("primType");
             }
         }
    }
}