using System;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX.Text
{
    struct FontCacheEntry : IEquatable<FontCacheEntry>
    {
        public int FontSize { get; set; }
        public string FontFace { get; set; }
        public FontWeight FontWeight { get; set; }
        public bool Italic { get; set; }

        public bool Equals(FontCacheEntry other)
        {
            return (

                       (other.FontSize == FontSize) &&
                       (other.FontFace == FontFace) &&
                       (other.FontWeight == FontWeight) &&
                       (other.Italic == Italic)
                   );
        }
    }
}