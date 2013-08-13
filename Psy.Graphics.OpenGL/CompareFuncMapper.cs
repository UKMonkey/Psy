using System;
using OpenTK.Graphics.OpenGL;

namespace Psy.Graphics.OpenGL
{
    internal static class CompareFuncMapper
    {
        public static AlphaFunction Map(CompareFunc value)
        {
            switch (value)
            {
                case CompareFunc.Always:
                    return AlphaFunction.Always;
                case CompareFunc.Equal:
                    return AlphaFunction.Equal;
                case CompareFunc.Greater:
                    return AlphaFunction.Gequal;
                case CompareFunc.GreaterEqual:
                    return AlphaFunction.Gequal;
                case CompareFunc.Less:
                    return AlphaFunction.Less;
                case CompareFunc.LessEqual:
                    return AlphaFunction.Lequal;
                case CompareFunc.Never:
                    return AlphaFunction.Never;
                case CompareFunc.NotEqual:
                    return AlphaFunction.Notequal;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }
}