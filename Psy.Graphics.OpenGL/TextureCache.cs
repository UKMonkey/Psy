using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using Psy.Core;
using Psy.Graphics.Textures;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace Psy.Graphics.OpenGL
{
    public class TextureCache : TextureCacheBase
    {
        public TextureCache()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        protected override TextureArea CreateCachedTexture(string name, Bitmap bitmap)
        {
            return LoadTexture(bitmap);
        }

        protected override TextureArea CreateTextureArea(TextureArea originalTextureArea, 
            TextureAtlasTextureDefinition definition)
        {
            return new CachedTexture(
                originalTextureArea.TextureId, definition.TopLeft,
                definition.BottomRight, definition.Width, definition.Height);
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }

        private static CachedTexture LoadTexture(Bitmap bitmap)
        {
            var textureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, textureId);

            var bitmapBytes = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                                           PixelFormat.Format32bppArgb);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapBytes.Width, bitmapBytes.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapBytes.Scan0);

            // todo: premultiplied alpha.

            return new CachedTexture(textureId, bitmap.Width, bitmap.Height);
        }
    }
}