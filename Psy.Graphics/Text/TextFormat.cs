using System;

namespace Psy.Graphics.Text
{
    [Flags]
    public enum TextFormat
    {
        Top = 0,
        Left = 1,
        Right = 2,
        Bottom = 8,
        Center = 16,
        VerticalCenter = 32,
        WordBreak = 64,
        SingleLine = 128
    }
}