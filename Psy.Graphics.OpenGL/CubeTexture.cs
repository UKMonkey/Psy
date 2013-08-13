namespace Psy.Graphics.OpenGL
{
    public class CubeTexture : ICubeTexture
    {
        public void Dispose()
        {
            
        }

        public string DebugName { get; set; }
        public ISurface[] CubeMapSurface { get; set; }

        public ISurface GetSurface(CubeMapFaceEnum face)
        {
            return new Surface();
        }
    }
}