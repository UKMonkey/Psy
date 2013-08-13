using Psy.Core;
using Psy.Graphics;
using SlimMath;

namespace Psy.Gui.Renderer
{
    public partial class GuiRenderer
    {
        public void Rectangle(Vector2 topLeft, Vector2 bottomRight, Color4 borderColour)
        {
            // todo: optimize this

            var topRight = new Vector2(bottomRight.X, topLeft.Y);
            var bottomLeft = new Vector2(topLeft.X, bottomRight.Y);

            Line(topLeft, topRight, borderColour);
            Line(topLeft, bottomLeft, borderColour);
            Line(topRight, bottomRight, borderColour);
            Line(bottomLeft, bottomRight, borderColour);
        }

        /// <summary>
        /// Renders a Rectangle from the top-left of the control (Position) to the bottom right (bottomRight)
        /// </summary>
        /// <param name="bottomRight"></param>
        /// <param name="backgroundColour"></param>
        /// <param name="borderColour"></param>
        /// <param name="border"></param>
        public void Rectangle(Vector2 bottomRight, Color4 backgroundColour, Color4 borderColour, bool border = true)
        {
            Rectangle(new Vector2(), bottomRight, backgroundColour, borderColour, border);
        }

        public void Rectangle(Vector2 topLeft, Vector2 bottomRight, Color4 backgroundColour, Color4 borderColour, bool border = true)
        {
            var dataStream = _primitiveRenderer.LockVertexBuffer();

            var primitiveCount = 0;

            var tl = topLeft + _offset;
            var br = bottomRight + _offset;

            if (border)
            {
                WriteRectangle(
                    new Rectangle { TopLeft = tl, BottomRight = br },
                    borderColour,
                    dataStream);
                primitiveCount = 2;

                tl = tl + new Vector2(Dimensions.WidgetBorder, Dimensions.WidgetBorder);
                br = br + new Vector2(-Dimensions.WidgetBorder, -Dimensions.WidgetBorder);
            }

            WriteRectangle(
                new Rectangle { TopLeft = tl, BottomRight = br },
                backgroundColour, 
                dataStream);
            primitiveCount += 2;

            _primitiveRenderer.UnlockVertexBuffer();

            _primitiveRenderer.Render(PrimitiveType.TriangleList, 0, primitiveCount);
        }
    }
}