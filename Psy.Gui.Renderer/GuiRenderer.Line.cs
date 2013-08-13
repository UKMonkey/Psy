using Psy.Graphics;
using Psy.Graphics.VertexDeclarations;
using SlimMath;

namespace Psy.Gui.Renderer
{
    public partial class GuiRenderer
    {
        public void Line(Vector2 startPosition, Vector2 endPosition, Color4 colour)
        {
            var dataStream = _primitiveRenderer.LockVertexBuffer();

            var startPositionTranslated = startPosition + _offset;
            var endPositionTranslated = endPosition + _offset;

            dataStream.Write(new TransformedColouredVertex(new Vector4(startPositionTranslated, 1.0f, 1.0f), colour));
            dataStream.Write(new TransformedColouredVertex(new Vector4(endPositionTranslated, 1.0f, 1.0f), colour));

            _primitiveRenderer.UnlockVertexBuffer();

            _primitiveRenderer.Render(PrimitiveType.LineList, 0, 2);
        }

        public void Line(Vector2 startPosition, Vector2 endPosition, Color4 start, Color4 end)
        {
            var dataStream = _primitiveRenderer.LockVertexBuffer();

            var startPositionTranslated = startPosition + _offset;
            var endPositionTranslated = endPosition + _offset;

            dataStream.Write(new TransformedColouredVertex(new Vector4(startPositionTranslated, 1.0f, 1.0f), start));
            dataStream.Write(new TransformedColouredVertex(new Vector4(endPositionTranslated, 1.0f, 1.0f), end));

            _primitiveRenderer.UnlockVertexBuffer();

            _primitiveRenderer.Render(PrimitiveType.LineList, 0, 2);
        }
    }
}