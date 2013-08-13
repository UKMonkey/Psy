using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Psy.Core;
using Psy.Core.FileSystem;
using SlimDX.Direct3D9;
using Rectangle = Psy.Core.Rectangle;

namespace Psy.Graphics
{
    public unsafe class TextureCache : IDisposable
    {
        private readonly Device _device;
        private readonly Dictionary<string, CachedTexture> _cache;
        private readonly Dictionary<int, CachedTexture> _textureCache;
        private int _nextTextureId;

        public TextureCache(Device device)
        {
            _device = device;
            _cache = new Dictionary<string, CachedTexture>();
            _textureCache = new Dictionary<int, CachedTexture>();
            _nextTextureId = 1;
        }

        public void SetTexture(int sampler, int textureId)
        {
            _device.SetTexture(sampler, _textureCache[textureId].Texture);
        }

        /// <summary>
        /// Obtain a SlimDX Format from a System.Drawing PixelFormat
        /// </summary>
        /// <param name="fmt"></param>
        /// <returns></returns>
        private static Format GetD3DFormat(PixelFormat fmt)
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

        private Texture LoadFromBitmap(Bitmap bitmap)
        {

            var bmlock = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                         ImageLockMode.ReadWrite, bitmap.PixelFormat);

            Debug.Assert(bmlock.PixelFormat == PixelFormat.Format32bppArgb, "Invalid pixel format");
            Debug.Assert(bmlock.Stride == bmlock.Width * sizeof(RgbaColor), "Invalid pixel stride");

            var ptr = (byte*) bmlock.Scan0.ToPointer();

            for (var y = 0; y < bmlock.Height; y++)
            {
                for (var x = 0; x < bmlock.Width; x++)
                {
                    // premultiply!
                    var color = (RgbaColor*)(ptr + (y * bmlock.Stride) + (x * sizeof(RgbaColor)));

                    var alphaFloat = (*color).Alpha/255.0f;

                    (*color).Red = Convert.ToByte(alphaFloat * (*color).Red);
                    (*color).Green = Convert.ToByte(alphaFloat * (*color).Green);
                    (*color).Blue = Convert.ToByte(alphaFloat * (*color).Blue);
                }
            }

            var newTexture = new Texture(_device, bitmap.Width, bitmap.Height, 0,
                Usage.None, GetD3DFormat(bitmap.PixelFormat), Pool.Managed);
            var textureLock = newTexture.LockRectangle(0, LockFlags.None);
            textureLock.Data.WriteRange(bmlock.Scan0, bmlock.Height * bmlock.Stride);
            bitmap.UnlockBits(bmlock);
            newTexture.UnlockRectangle(0);

            newTexture.FilterTexture(0, Filter.Default);

            return newTexture;
        }

        private CachedTexture GetTextureFromCache(string filename)
        {
            // do we already have this file?
            var fullFileName = Lookup.GetAssetPath(filename);

            if (_cache.ContainsKey(fullFileName))
            {
                return _cache[fullFileName];
            }

            // cache miss.
            var bitmap = new Bitmap(fullFileName);
            var texture = LoadFromBitmap(bitmap);
            var cachedTexture = new CachedTexture(_nextTextureId, texture, filename, bitmap.Width, bitmap.Height);
            _textureCache[_nextTextureId] = cachedTexture;
            _nextTextureId++;
            bitmap.Dispose();
            _cache[fullFileName] = cachedTexture;

            return cachedTexture;

        }

        /// <summary>
        /// Load a texture. This texture may be loaded into a texture atlas.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public TextureArea GetTexture(string filename)
        {
            var cachedTexture = GetTextureFromCache(filename);
            var newTextureArea = new TextureArea(cachedTexture.TextureId);
            return newTextureArea;

        }

        public TextureArea GetTexture(string filename, Rectangle rectangle)
        {
            var newTexture = GetTextureFromCache(filename);

            // Determine pixel coordinates
            var topLeft = new Vector
            {
                X = rectangle.TopLeft.X / newTexture.Width,
                Y = rectangle.TopLeft.Y / newTexture.Height
            };

            var bottomRight = new Vector
            {
                X = rectangle.BottomRight.X / newTexture.Width,
                Y = rectangle.BottomRight.Y / newTexture.Height
            };

            var newTextureArea = new TextureArea(newTexture.TextureId, new Vector(topLeft.X, topLeft.Y), new Vector(bottomRight.X, bottomRight.Y));

            return newTextureArea;
        }

        /// <summary>
        /// Load a texture atlas.
        /// </summary>
        /// <param name="filename">Name of the atlas definition file</param>
        /// <returns></returns>
        public List<TextureArea> LoadAtlas(string filename)
        {
            var textureFilename = "";
            var definitions = new List<TextureAtlasTextureDefinition>();

            using (var stream = new StreamReader(Lookup.GetAssetPath(filename)))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    if (line.StartsWith(";"))
                        continue;

                    var parts = line.Split('=');
                    var key = parts[0];
                    var value = parts[1];

                    switch (key)
                    {
                        case "filename":
                            textureFilename = value;
                            break;

                        case "texture":
                            var definition = TextureAtlasTextureDefinition.Parse(value);
                            definitions.Add(definition);
                            break;
                    }
                }
            }

            if (String.IsNullOrEmpty(textureFilename))
            {
                throw new ArgumentException(String.Format("No texture filename for atlas {0}", filename));
            }

            var cachedTexture = GetTextureFromCache(textureFilename);

            foreach (var definition in definitions)
            {
                definition.NormalizeTextureCoordinates(cachedTexture.Width, cachedTexture.Height);
            }

            return definitions.Select(definition => new TextureArea(cachedTexture.TextureId, definition.TopLeft, definition.BottomRight)).ToList();
        }

        public void Dispose()
        {
            foreach (var texture in _cache.Values)
            {
                texture.Dispose();
            }
        }
    }
}
