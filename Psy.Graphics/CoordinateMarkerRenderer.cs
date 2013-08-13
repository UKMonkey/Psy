using Psy.Core;
using Psy.Graphics.Effects;
using Psy.Graphics.VertexDeclarations;
using SlimMath;

namespace Psy.Graphics
{
    public class CoordinateMarkerRenderer
    {
        private readonly IVertexRenderer<TexturedColouredVertex4> _vertexRenderer;
        private readonly TextureAreaHolder _noTexture;

        public CoordinateMarkerRenderer(GraphicsContext graphicsContext)
        {
            _vertexRenderer = graphicsContext.CreateVertexRenderer<TexturedColouredVertex4>(6);
            _noTexture = graphicsContext.GetTexture("noTexture.png");

            PopululateBuffer();
        }

        private void PopululateBuffer()
        {
            var stream = _vertexRenderer.LockVertexBuffer();

            stream.WriteRange(new []
            {
                new TexturedColouredVertex4 { Position = new Vector4(0, 0, 0, 1), Colour = Colours.Red, TextureCoordinate = new Vector2(0, 0) },
                new TexturedColouredVertex4 { Position = new Vector4(1, 0, 0, 1), Colour = Colours.Red, TextureCoordinate = new Vector2(0, 0) },

                new TexturedColouredVertex4 { Position = new Vector4(0, 0, 0, 1), Colour = Colours.Green, TextureCoordinate = new Vector2(0, 0) },
                new TexturedColouredVertex4 { Position = new Vector4(0, 1, 0, 1), Colour = Colours.Green, TextureCoordinate = new Vector2(0, 0) },

                new TexturedColouredVertex4 { Position = new Vector4(0, 0, 0, 1), Colour = Colours.Blue, TextureCoordinate = new Vector2(0, 0) },
                new TexturedColouredVertex4 { Position = new Vector4(0, 0, 1, 1), Colour = Colours.Blue, TextureCoordinate = new Vector2(0, 0) }
            });

            _vertexRenderer.UnlockVertexBuffer();
        }

        public void Render(IEffect effect)
        {
            effect.SetTexture("tex0", _noTexture);
            effect.CommitChanges();

            _vertexRenderer.RenderForShader(PrimitiveType.LineList, 0, 3);
        }
    }
}