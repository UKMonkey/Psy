using System;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public static class ZCompareFunctionMapper
    {
        public static ZCompareFunction MapFrom(Compare compare)
        {
            switch (compare)
            {
                case Compare.Always:
                    return ZCompareFunction.Always;
                case Compare.GreaterEqual:
                    return ZCompareFunction.GreaterEqual;
                case Compare.NotEqual:
                    return ZCompareFunction.NotEqual;
                case Compare.Greater:
                    return ZCompareFunction.Greater;
                case Compare.LessEqual:
                    return ZCompareFunction.LessEqual;
                case Compare.Equal:
                    return ZCompareFunction.Equal;
                case Compare.Less:
                    return ZCompareFunction.Less;
                case Compare.Never:
                    return ZCompareFunction.Never;
                default:
                    throw new ArgumentOutOfRangeException("compare");
            }
        }

        public static Compare MapFrom(ZCompareFunction compareFunction)
        {
            switch (compareFunction)
            {
                case ZCompareFunction.Never:
                    return Compare.Never;
                case ZCompareFunction.Less:
                    return Compare.Less;
                case ZCompareFunction.Equal:
                    return Compare.Equal;
                case ZCompareFunction.LessEqual:
                    return Compare.LessEqual;
                case ZCompareFunction.Greater:
                    return Compare.Greater;
                case ZCompareFunction.NotEqual:
                    return Compare.NotEqual;
                case ZCompareFunction.GreaterEqual:
                    return Compare.GreaterEqual;
                case ZCompareFunction.Always:
                    return Compare.Always;
                default:
                    throw new ArgumentOutOfRangeException("compareFunction");
            }
        }
    }
}