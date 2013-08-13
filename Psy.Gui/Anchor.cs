using System;

namespace Psy.Gui
{
    [Flags]
    public enum Anchor
    {
        None = 0,
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8,
        HorizontalMiddle = Left + Right,
        VerticalMiddle = Top + Bottom
    }
}