using System;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public static class CompareFuncMapper
    {
         public static Compare Map(CompareFunc compareFunc)
         {
             switch (compareFunc)
             {
                 case CompareFunc.Always:
                     return Compare.Always;
                 case CompareFunc.Equal:
                     return Compare.Equal;
                 case CompareFunc.Greater:
                     return Compare.Greater;
                 case CompareFunc.GreaterEqual:
                     return Compare.GreaterEqual;
                 case CompareFunc.Less:
                     return Compare.Less;
                 case CompareFunc.LessEqual:
                     return Compare.LessEqual;
                 case CompareFunc.Never:
                     return Compare.Never;
                 case CompareFunc.NotEqual:
                     return Compare.NotEqual;
                 default:
                     throw new ArgumentOutOfRangeException("compareFunc");
             }
         }
    }
}