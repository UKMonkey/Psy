using System;
using SlimMath;

namespace Psy.Core
{
    public abstract class TextureArea : IDisposable
    {
        public readonly int TextureId;
        public readonly Vector2 AtlasTopLeft;
        public readonly Vector2 AtlasBottomRight;
        public readonly int Width;
        public readonly int Height;

        public readonly Vector2 AtlasBottomLeft;
        public readonly Vector2 AtlasTopRight;

        public readonly Vector2 Size;

        protected TextureArea(int textureId, int width, int height)
        {
            TextureId = textureId;
            AtlasTopLeft = new Vector2(0.0f, 0.0f);
            AtlasBottomRight = new Vector2(1.0f, 1.0f);

            AtlasBottomLeft = new Vector2(0.0f, 1.0f);
            AtlasTopRight = new Vector2(1.0f, 0.0f);

            Width = width;
            Height = height;

            Size = new Vector2(width, height);
        }

        protected TextureArea(int textureId, TextureAtlasTextureDefinition textureAtlasTextureDefinition)
        {
            TextureId = textureId;
            // todo: use a Rectangle
            AtlasTopLeft = textureAtlasTextureDefinition.TopLeft;
            AtlasBottomRight = textureAtlasTextureDefinition.BottomRight;
            AtlasBottomLeft = new Vector2(textureAtlasTextureDefinition.TopLeft.X, textureAtlasTextureDefinition.BottomRight.Y);
            AtlasTopRight = new Vector2(textureAtlasTextureDefinition.BottomRight.X, textureAtlasTextureDefinition.TopLeft.Y);
            Width = textureAtlasTextureDefinition.Width;
            Height = textureAtlasTextureDefinition.Height;

            Size = new Vector2(Width, Height);
        }

        protected TextureArea(int textureId, Vector2 atlasTopLeft, Vector2 atlasBottomRight, int width, int height)
        {
            TextureId = textureId;
            AtlasTopLeft = atlasTopLeft;
            AtlasBottomRight = atlasBottomRight;
            Width = width;
            Height = height;
        }

        public abstract TextureArea Clone(TextureAtlasTextureDefinition textureAtlasTextureDefinition);
        public abstract void Dispose();
    }
}