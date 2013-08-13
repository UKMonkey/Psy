using Psy.Core;

namespace Psy.Graphics.OpenGL
{
// ReSharper disable UnusedMember.Global
    public class OpenGLGraphicsContextFactory : IGraphicsContextFactory
// ReSharper restore UnusedMember.Global
    {
        public GraphicsContext Create(WindowAttributes windowAttributes)
        {
            return new OpenGLGraphicsContext(windowAttributes);
        }
    }
}