using SlimDX.Direct3D9;

namespace Psy.Graphics
{
    public class CachedTexture
    {
        public int TextureId { get; set; }
        public Texture Texture { get; private set; }
        public string Filename { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        internal CachedTexture(int textureId, Texture texture, string filename, int width, int height)
        {
            TextureId = textureId;
            Texture = texture;
            Filename = filename;
            Width = width;
            Height = height;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}