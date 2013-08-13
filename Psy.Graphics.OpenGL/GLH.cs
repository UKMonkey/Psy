using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Psy.Graphics.OpenGL
{
    public static class GLH
    {
        [Conditional("DEBUG")]
        public static void AssertGLError()
        {
            var errorCode1 = GL.GetError();
            Debug.Assert(errorCode1 == ErrorCode.NoError);
        }
    }
}