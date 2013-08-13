using SlimDX.Direct3D9;

namespace Psy.Graphics
{
    public static class StaticTextureCache
    {
        public static TextureCache TextureCache;

        public static void Initialize(Device device)
        {
            if (TextureCache == null)
            {
                TextureCache = new TextureCache(device);
            }
        }
    }
}