namespace Psy.Graphics.OpenGL
{
    public class Texture : ITexture 
    {
        public void Dispose()
        {
            
        }

        public ISurface Surface { get; private set; }
        public string DebugName { get; set; }
    }
}