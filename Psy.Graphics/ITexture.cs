using System;

namespace Psy.Graphics
{
    public interface ITexture : IDisposable
    {
        ISurface Surface { get; }
        string DebugName { get; set; }
    }
}