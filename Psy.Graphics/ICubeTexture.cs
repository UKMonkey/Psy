using System;

namespace Psy.Graphics
{
    public interface ISurface : IDisposable
    {
        string DebugName { get; set; }
    }

    public interface ICubeTexture : IDisposable
    {
        string DebugName { get; set; }
        ISurface[] CubeMapSurface { get; set; }
    }
}