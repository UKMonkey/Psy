using System;
using System.Collections.Generic;
using SlimMath;

namespace Psy.Gui.ColourScheme
{
    public class Faceless : IColourScheme
    {
        public Color4 ButtonBackground
        {
            get { return new Color4(1.0f, 0.2f, 0.2f, 0.2f); }
        }

        public Color4 ButtonSurround
        {
            get { return new Color4(1.0f, 0.1f, 0.1f, 0.1f); }
        }

        public Color4 ButtonBackgroundHighlight
        {
            get { return new Color4(1.0f, 0.25f, 0.25f, 0.25f); }
        }

        public Color4 ButtonSurroundHighlight
        {
            get { return new Color4(1.0f, 0.15f, 0.15f, 0.15f); }
        }

        public Color4 TextColour
        {
            get { return new Color4(1.0f, 0.8f, 0.8f, 0.8f); }
        }

        public Color4 WindowBackground
        {
            get { return new Color4(1.0f, 0, 0, 0); }
        }

        public Color4 ButtonBackgroundClick
        {
            get { return new Color4(1.0f, 0.15f, 0.15f, 0.15f); }
        }

        public Color4 ButtonSurroundClick
        {
            get { return new Color4(1.0f, 0.15f, 0.15f, 0.15f); }
        }

        public Color4 TextboxBackground
        {
            get { return new Color4(1.0f, 0, 0, 0); }
        }

        public Color4 ToggledButton
        {
            get { return new Color4(1.0f, 0.14f, 0.24f, 0.14f); }
        }

        public List<string> SkinFilenames
        {
            get { return new List<string> {"testguiskin.adf", "gui_2.adf"}; }
        }

        public Color4 GetColour(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "error":
                    return new Color4(1.0f, 0.8f, 0.2f, 0.2f);
                default:
                    throw new Exception("Unable to get colour for " + name);
            }
        }
    }
}