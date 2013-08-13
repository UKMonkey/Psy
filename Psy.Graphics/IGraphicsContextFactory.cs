using Psy.Core;

namespace Psy.Graphics
{
    public interface IGraphicsContextFactory
    {
        GraphicsContext Create(WindowAttributes windowAttributes);
    }
}