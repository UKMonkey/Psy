using System;
using System.Collections.Generic;
using Psy.Graphics.Text;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Text
{
    public class FontCache : IFontCache
    {
        private readonly Device _device;
        private readonly Dictionary<FontCacheEntry, Font> _fontCache;

        internal FontCache(Device device)
        {
            _device = device;
            _fontCache = new Dictionary<FontCacheEntry, Font>();
        }

        public void DevicePreReset()
        {
            foreach (var font in _fontCache)
            {
                font.Value.OnLostDevice();
            }
        }

        public void DevicePostReset()
        {
            foreach (var font in _fontCache)
            {
                font.Value.OnResetDevice();
            }
        }

        public IFont GetFont(string fontFace="Arial", int fontSize=16, Weight weight=Weight.Normal, bool italic=false)
        {
            var fontWeight = MapWeight(weight);

            var testEntry = new FontCacheEntry
                                {
                                    FontFace = fontFace,
                                    FontSize = fontSize,
                                    FontWeight = fontWeight,
                                    Italic = italic
                                };

            if (!_fontCache.ContainsKey(testEntry))
            {
                CreateFontCacheEntry(testEntry);
            }

            return _fontCache[testEntry];
        }

        private static FontWeight MapWeight(Weight weight)
        {
            switch (weight)
            {
                case Weight.Normal:
                    return FontWeight.Normal;
                case Weight.Bold:
                    return FontWeight.Bold;
                default:
                    throw new ArgumentOutOfRangeException("weight");
            }
        }

        private void CreateFontCacheEntry(FontCacheEntry fontCacheEntry)
        {
            var font = new Font(_device, fontCacheEntry.FontSize, 0, fontCacheEntry.FontWeight, 1,
                                fontCacheEntry.Italic, CharacterSet.Default, Precision.Device, FontQuality.ClearTypeNatural,
                                PitchAndFamily.DontCare, fontCacheEntry.FontFace);
            _fontCache[fontCacheEntry] = font;
        }
    }
}
