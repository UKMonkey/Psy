using System;
using System.Drawing;

namespace Psy.Graphics.OpenGL.Text
{
    struct FontCacheEntry : IEquatable<FontCacheEntry>
    {
        public int FontSize { get; set; }
        public string FontFace { get; set; }
        public FontStyle FontStyle { get; set; }

        public bool Equals(FontCacheEntry other)
        {
            return (

                       (other.FontSize == FontSize) &&
                       (other.FontFace == FontFace) &&
                       (other.FontStyle == FontStyle)
                   );
        }
    }
}