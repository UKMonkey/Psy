using System;
using System.Collections.Generic;
using System.Drawing;
using Psy.Graphics.Text;
using QuickFont;

namespace Psy.Graphics.OpenGL.Text
{
    public class FontCache
    {
        private readonly Dictionary<FontCacheEntry, QFont> _fontCache;

        public FontCache()
        {
            _fontCache = new Dictionary<FontCacheEntry, QFont>(5);
        }

        public IFont GetFont(string name, int size, Weight weight, bool italic)
        {
            var fontStyle = GetFontStyle(weight, italic);

            var fce = new FontCacheEntry
            {
                FontFace = name,
                FontSize = size,
                FontStyle = fontStyle
            };

            QFont font;

            var config = new QFontBuilderConfiguration(false)
            {
                TextGenerationRenderHint = GetFontFaceRenderHintingPreference(name),
                GlyphMargin = 2,
                TransformToCurrentOrthogProjection = true,
                KerningConfig = 
                {
                    alphaEmptyPixelTolerance = 40,
                },
            };

            if (!_fontCache.TryGetValue(fce, out font))
            {
                var sysFont = GetFontBestSizeFit(name, size, fontStyle);
                
                font = new QFont(sysFont, config);

                _fontCache[fce] = font;                
            }

            return new WrappedFont(font);
        }

        private static TextGenerationRenderHint GetFontFaceRenderHintingPreference(string fontFaceName)
        {
            return TextGenerationRenderHint.ClearTypeGridFit;
            
            if (fontFaceName.Equals("consolas", StringComparison.InvariantCultureIgnoreCase))
            {
                return TextGenerationRenderHint.ClearTypeGridFit;
            }
            return TextGenerationRenderHint.AntiAlias;
        }

        private static Font GetFontBestSizeFit(string name, int size, FontStyle fontStyle)
        {
            float testSize = size;

            while (true)
            {
                //fontStyle = FontStyle.Regular;
                var font = new Font(name, testSize, fontStyle);

                if (font.Height <= size)
                {
                    return font;
                }

                testSize -= 0.5f;
            }
        }

        private static FontStyle GetFontStyle(Weight weight, bool italic)
        {
            var fontStyle = FontStyle.Regular;

            if (italic)
            {
                fontStyle |= FontStyle.Italic;
            }

            if (weight == Weight.Bold)
            {
                fontStyle |= FontStyle.Bold;   
            }

            return fontStyle;
        }
    }
}