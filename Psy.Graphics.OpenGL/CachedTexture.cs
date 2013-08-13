using System;
using OpenTK.Graphics.OpenGL;
using Psy.Core;
using SlimMath;

namespace Psy.Graphics.OpenGL
{
    public class CachedTexture : TextureArea, IDisposable
    {
        public CachedTexture(int textureId, int width, int height) : base(textureId, width, height)
        {
        }

        private CachedTexture(int textureId, TextureAtlasTextureDefinition textureAtlasTextureDefinition) 
            : base(textureId, textureAtlasTextureDefinition)
        {
            
        }

        public CachedTexture(int textureId, Vector2 atlasTopLeft, Vector2 atlasBottomRight, int width, int height) 
            : base(textureId, atlasTopLeft, atlasBottomRight, width, height)
        {
            
        }

        public override TextureArea Clone(TextureAtlasTextureDefinition textureAtlasTextureDefinition)
        {
            return new CachedTexture(TextureId, textureAtlasTextureDefinition);
        }

        public override void Dispose()
        {
            GL.DeleteTexture(TextureId);
        }
    }
}