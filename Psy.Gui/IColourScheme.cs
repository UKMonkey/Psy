using System.Collections.Generic;
using Psy.Core;
using SlimMath;

namespace Psy.Gui
{
    public interface IColourScheme
    {
        Color4 ButtonBackground { get; }
        Color4 ButtonSurround { get; }
        Color4 ButtonBackgroundHighlight { get; }
        Color4 ButtonSurroundHighlight { get; }
        Color4 TextColour { get; }
        Color4 WindowBackground { get; }
        Color4 ButtonBackgroundClick { get; }
        Color4 ButtonSurroundClick { get; }
        Color4 TextboxBackground { get; }
        Color4 ToggledButton { get; }
        List<string> SkinFilenames { get; }

        Color4 GetColour(string name);
    }
}