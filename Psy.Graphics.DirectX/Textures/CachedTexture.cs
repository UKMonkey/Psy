using Psy.Core;
using SlimDX.Direct3D9;
using SlimMath;

namespace Psy.Graphics.DirectX.Textures
{
    public class CachedTexture : TextureArea
    {
        public Texture Texture { get; private set; }
        public string Filename { get; set; }
        private readonly bool _createdFromAtlas;

        internal CachedTexture(int textureId, Texture texture, string filename, int width, int height) 
            : base(textureId, width, height)
        {
            Texture = texture;
            Filename = filename;
            _createdFromAtlas = false;
        }

        internal CachedTexture(int textureId, Texture texture, string filename, 
            TextureAtlasTextureDefinition textureAtlasTextureDefinition)
            : base(textureId, textureAtlasTextureDefinition)
        {
            Texture = texture;
            Filename = filename;
            _createdFromAtlas = true;
        }

        public CachedTexture(int textureId, Vector2 topLeft, Vector2 bottomRight, int width, int height) 
            : base(textureId, topLeft, bottomRight, width, height) {}

        public override void Dispose()
        {
            if (Texture != null)
                Texture.Dispose();
        }

        public override string ToString()
        {
            return string.Format("Filename:{0} Size:{1}x{2} Flags:[{4}{3}]", 
                Filename, Width, Height, 
                Texture != null ? "L" : "U", 
                _createdFromAtlas ? "A" : "");
        }

        public override TextureArea Clone(TextureAtlasTextureDefinition definition)
        {
            return new CachedTexture(TextureId, Texture, Filename, definition);
        }
    }
}