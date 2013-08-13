using Psy.Core;
using Psy.Graphics;
using Psy.Graphics.VertexDeclarations;
using SlimMath;

namespace Psy.Gui.Renderer
{
    public partial class GuiRenderer
    {
        private class TexturePatch
        {
            public TextureArea TL;
            public TextureArea T;
            public TextureArea TR;
            public TextureArea L;
            public TextureArea C;
            public TextureArea R;
            public TextureArea BL;
            public TextureArea B;
            public TextureArea BR;
        }

        private static TransformedColouredTexturedVertex MakeVertex(Vector2 position, Vector2 textureCoordinate)
        {
            return new TransformedColouredTexturedVertex(
                position.AsVector4(),
                textureCoordinate,
                new Color4(1.0f, 1.0f, 1.0f, 1.0f));
        }

        private static TransformedColouredTexturedVertex MakeVertex(Vector2 position, Vector2 textureCoordinate, 
            float alpha)
        {
            return new TransformedColouredTexturedVertex(
                position.AsVector4(), 
                textureCoordinate, 
                new Color4(alpha, 1.0f, 1.0f, 1.0f));
        }

        private static TransformedColouredTexturedVertex MakeVertex(Vector2 position, Vector2 textureCoordinate,
            Color4 color)
        {
            return new TransformedColouredTexturedVertex(
                position.AsVector4(),
                textureCoordinate,
                color);
        }

        private void PerformUpdate(Vector2 size, TexturePatch textures,
            string overrideTexture = null, float alpha = 1.0f)
        {
            var rectTL =
                new Rectangle
                {
                    TopLeft = new Vector2(0, 0),
                    BottomRight = new Vector2(textures.TL.Width, textures.TL.Height)
                };

            var rectT =
                new Rectangle
                {
                    TopLeft = new Vector2(textures.TL.Width, 0),
                    BottomRight = new Vector2(size.X - rectTL.Width, textures.T.Height)
                };

            var rectTR =
                new Rectangle
                {
                    TopLeft = new Vector2(rectT.Width + rectTL.Width, 0),
                    BottomRight = new Vector2(rectT.Width + rectTL.Width + textures.TR.Width, textures.TR.Height)
                };

            var rectL =
                new Rectangle
                {
                    TopLeft = new Vector2(0, rectTL.Height),
                    BottomRight = new Vector2(textures.L.Width, size.Y - rectTL.Height)
                };

            var rectC =
                new Rectangle
                {
                    TopLeft = new Vector2(textures.TL.Width, textures.TL.Height),
                    BottomRight = new Vector2(size.X - rectTL.Width, size.Y - rectTL.Height)
                };

            var rectR =
                new Rectangle
                {
                    TopLeft = new Vector2(size.X - rectTL.Width, rectTR.Height),
                    BottomRight = new Vector2(size.X, size.Y - rectTL.Height)
                };

            var rectBL =
                new Rectangle
                {
                    TopLeft = new Vector2(0, size.Y - textures.BL.Height),
                    BottomRight = new Vector2(textures.BL.Width, size.Y)
                };

            var rectB =
                new Rectangle
                {
                    TopLeft = new Vector2(textures.BL.Width, size.Y - textures.B.Height),
                    BottomRight = new Vector2(size.X - textures.BR.Width, size.Y)
                };

            var rectBR =
                new Rectangle
                {
                    TopLeft = new Vector2(size.X - textures.BR.Width, size.Y - textures.BR.Height),
                    BottomRight = new Vector2(size.X, size.Y)
                };

            rectTL = rectTL.Translate(_offset);
            rectT = rectT.Translate(_offset);
            rectTR = rectTR.Translate(_offset);
            rectL = rectL.Translate(_offset);
            rectC = rectC.Translate(_offset);
            rectR = rectR.Translate(_offset);
            rectBL = rectBL.Translate(_offset);
            rectB = rectB.Translate(_offset);
            rectBR = rectBR.Translate(_offset);

            var dataStream = _textureRenderer.LockVertexBuffer();

            WriteQuad(textures.TL, rectTL, dataStream, alpha);
            WriteQuad(textures.T, rectT, dataStream, alpha);
            WriteQuad(textures.TR, rectTR, dataStream, alpha);
            WriteQuad(textures.L, rectL, dataStream, alpha);
            WriteQuad(textures.C, rectC, dataStream, alpha);
            WriteQuad(textures.R, rectR, dataStream, alpha);
            WriteQuad(textures.BL, rectBL, dataStream, alpha);
            WriteQuad(textures.B, rectB, dataStream, alpha);
            WriteQuad(textures.BR, rectBR, dataStream, alpha);

            var texture = textures.TL;

            if (!string.IsNullOrEmpty(overrideTexture))
            {
                texture = _graphicsContext.GetTexture(overrideTexture).TextureArea;
            }

            _textureRenderer.UnlockVertexBuffer();
            _textureRenderer.Render(PrimitiveType.TriangleList, 0, 18, texture, alpha, 1);
        }

        public void NinePatch(Vector2 size, NinePatchHandle ninePatchHandle, 
            string overrideTexture = null, float alpha = 1.0f)
        {
            var textures = new TexturePatch
            {
                T = _graphicsContext.GetTexture(ninePatchHandle.Top).TextureArea,
                TR = _graphicsContext.GetTexture(ninePatchHandle.TopRight).TextureArea,
                TL = _graphicsContext.GetTexture(ninePatchHandle.TopLeft).TextureArea,
                BL = _graphicsContext.GetTexture(ninePatchHandle.BottomLeft).TextureArea,
                BR = _graphicsContext.GetTexture(ninePatchHandle.BottomRight).TextureArea,
                B = _graphicsContext.GetTexture(ninePatchHandle.Bottom).TextureArea,
                L = _graphicsContext.GetTexture(ninePatchHandle.Left).TextureArea,
                C = _graphicsContext.GetTexture(ninePatchHandle.Centre).TextureArea,
                R = _graphicsContext.GetTexture(ninePatchHandle.Right).TextureArea
            };

            PerformUpdate(size, textures, overrideTexture, alpha);
        }
    }
}