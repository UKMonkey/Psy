using Psy.Core;
using Psy.Graphics;
using SlimMath;

namespace Psy.Gui.Renderer
{
    public partial class GuiRenderer
    {
        public void Image(string imageName, 
            Vector2? size = null, Vector2? topLeft = null, 
            float horizontalCrop = 1.0f, float verticalCrop = 1.0f, 
            Vector2 margin = new Vector2(), float alpha = 1f, float intensity = 1f)
        {
            if (topLeft == null)
            {
                topLeft = new Vector2();
            }

            var texture = _graphicsContext.GetTexture(imageName).TextureArea;

            if (size == null)
            {
                size = texture.Size;
            }

            var dataStream = _textureRenderer.LockVertexBuffer();

            var rect = new Rectangle(margin, size.Value - margin).Translate(_offset + topLeft.Value);

            WriteCroppedQuad(texture, rect, dataStream, horizontalCrop, verticalCrop);

            _textureRenderer.UnlockVertexBuffer();
            _textureRenderer.Render(PrimitiveType.TriangleList, 0, 2, texture, alpha, intensity);
        }

        public Vector2 GetImageSize(string imageName)
        {
            return _graphicsContext.GetTexture(imageName).TextureArea.Size;
        }
    }
}