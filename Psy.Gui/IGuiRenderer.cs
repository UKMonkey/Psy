using Psy.Core;
using Psy.Gui.Components;
using SlimMath;

namespace Psy.Gui
{
    public interface IGuiRenderer
    {
        RenderMode RenderMode { get; }

        void Rectangle(Vector2 bottomRight, Color4 backgroundColour, Color4 borderColour = default(Color4), bool border = true);
        
        /// <summary>
        /// Render rectangle with no background
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="borderColour"></param>
        void Rectangle(Vector2 topLeft, Vector2 bottomRight, Color4 borderColour);

        void Rectangle(Vector2 topLeft, Vector2 bottomRight, Color4 backgroundColour, Color4 borderColour, bool border = true);

        /// <summary>
        /// Render text, returns the width of the text rendered.
        /// </summary>
        /// <param name="text">Text to render</param>
        /// <param name="position"></param>
        /// <param name="verticalAlignment"> </param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="opacity"></param>
        /// <param name="fontFace"> </param>
        /// <param name="fontSize"> </param>
        /// <param name="colour"> </param>
        /// <returns></returns>
        int Text(string text, Vector2 position, VerticalAlignment verticalAlignment = VerticalAlignment.Top,
                 HorizontalAlignment horizontalAlignment = HorizontalAlignment.LeftWithMargin, float opacity = 1.0f, string fontFace = null, int? fontSize = null,
                 Color4? colour=null);

        /// <summary>
        /// Render text, returns the width of the text rendered.
        /// </summary>
        /// <param name="font">Name of the font face</param>
        /// <param name="size">Font face size</param>
        /// <param name="text">Text to render</param>
        /// <param name="colour"></param>
        /// <param name="position"></param>
        /// <param name="verticalAlignment"> </param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="opacity"></param>
        /// <param name="italic"> </param>
        /// <returns></returns>
        int Text(string font, int size, string text, Color4 colour, Vector2 position, 
            VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.LeftWithMargin, float opacity = 1.0f, bool italic = false);

        void Desktop(Desktop widget);

        void IncreaseOffset(Vector2 offset);

        void DecreaseOffset(Vector2 offset);

        IColourScheme ColourScheme { get; }

        float TextHeight { get; }

        void NinePatch(Vector2 size, NinePatchHandle ninePatchHandle, string overrideTexture = null, float alpha = 1.0f);

        void Image(string imageName, Vector2? size = null, Vector2? topLeft = null, 
            float horizontalCrop = 1.0f, float verticalCrop = 1.0f, Vector2 margin = new Vector2(), float alpha = 1.0f, float intensity = 1.0f);

        Vector2 GetImageSize(string imageName);

        void Line(Vector2 startPosition, Vector2 endPosition, Color4 colour);
        void Line(Vector2 startPosition, Vector2 endPosition, Color4 start, Color4 end);
    }
}