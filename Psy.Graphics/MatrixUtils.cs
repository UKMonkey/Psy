using System;
using SlimMath;

namespace Psy.Graphics
{
    public static class MatrixUtils
    {
        public static Matrix GetPerspectiveFovLH(GraphicsContext graphicsContext)
        {
            var aspect = (float)graphicsContext.WindowSize.Width / graphicsContext.WindowSize.Height;
            const float halfFovAngle = (float)(Math.PI / 4);

            return
                Matrix.PerspectiveFovLH(
                    halfFovAngle, aspect,
                    ViewFrustum.FovNear, ViewFrustum.FovFar);
        }
    }
}