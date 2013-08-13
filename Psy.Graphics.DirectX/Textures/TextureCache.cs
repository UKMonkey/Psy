using System;
using System.Drawing;
using System.Drawing.Imaging;
using Psy.Core;
using Psy.Graphics.Textures;
using SlimDX.Direct3D9;
using Rectangle = System.Drawing.Rectangle;

namespace Psy.Graphics.DirectX.Textures
{
    public class TextureCache : TextureCacheBase
    {
        private Device Device { get; set; }

        public TextureCache(Device device)
        {
            Device = device;
        }

        protected override TextureArea CreateCachedTexture(string name, Bitmap bitmap)
        {
            var texture = LoadFromBitmap(bitmap);

            texture.DebugName = string.Format("CachedTexture[{0}]", name);

            return new CachedTexture(NextTextureId, texture, name, bitmap.Width, bitmap.Height);
        }

        private Texture LoadFromBitmap(Bitmap bitmap)
        {
            var bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    PrecalculateAlpha(bitmap);
                    break;
                case PixelFormat.Format24bppRgb:
                    bitmap = bitmap.Clone(bitmapRect, PixelFormat.Format32bppRgb);
                    break;
                default:
                    throw new Exception(string.Format("Could not load bitmap with pixel format of `{0}`", bitmap.PixelFormat));
            }

            var bmlock = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var newTexture = new Texture(Device, bitmap.Width, bitmap.Height, 0, Usage.None, GetD3DFormat(bitmap.PixelFormat), Pool.Managed);
            var textureLock = newTexture.LockRectangle(0, LockFlags.None);
            textureLock.Data.WriteRange(bmlock.Scan0, bmlock.Height * bmlock.Stride);
            bitmap.UnlockBits(bmlock);
            newTexture.UnlockRectangle(0);

            newTexture.FilterTexture(0, Filter.Default);

            return newTexture;
        }

        /// <summary>
        /// Obtain a SlimDX Format from a System.Drawing PixelFormat
        /// </summary>
        /// <param name="fmt"></param>
        /// <returns></returns>
        protected static Format GetD3DFormat(PixelFormat fmt)
        {
            switch (fmt)
            {
                case PixelFormat.Format16bppRgb555:
                    return Format.X1R5G5B5;
                case PixelFormat.Format16bppRgb565:
                    return Format.R5G6B5;
                case PixelFormat.Format32bppRgb:
                    return Format.X8R8G8B8;
                case PixelFormat.Format24bppRgb:
                    return Format.R8G8B8;
                case PixelFormat.Format32bppArgb:
                    return Format.A8R8G8B8;
                default:
                    return Format.A8R8G8B8;
            }
        }

        protected override TextureArea CreateTextureArea(
            TextureArea originalTextureArea, TextureAtlasTextureDefinition definition)
        {
            return new CachedTexture(
                originalTextureArea.TextureId, definition.TopLeft, 
                definition.BottomRight, definition.Width, definition.Height);
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var textureArea in CacheById)
            {
                textureArea.Value.Dispose();
            }
        }
    }
}
