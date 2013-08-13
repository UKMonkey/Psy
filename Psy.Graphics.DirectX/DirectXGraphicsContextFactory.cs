using Psy.Core;

namespace Psy.Graphics.DirectX
{
// ReSharper disable UnusedMember.Global
    public class DirectXGraphicsContextFactory : IGraphicsContextFactory
// ReSharper restore UnusedMember.Global
    {
        public GraphicsContext Create(WindowAttributes windowAttributes)
        {
            return new DirectXGraphicsContext(windowAttributes);
        }
    }
}