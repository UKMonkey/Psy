using System;
using Psy.Core;
using Psy.Graphics;
using Psy.Graphics.Text;
using Psy.Graphics.VertexDeclarations;
using Psy.Gui.Components;
using SlimMath;

namespace Psy.Gui.Renderer
{
    public partial class GuiRenderer : IGuiRenderer, IDisposable
    {
        public const string DefaultFontName = "Arial";
        public const int DefautltFontSize = 16;

        private readonly GraphicsContext _graphicsContext;
        private readonly IVertexRenderer<TransformedColouredVertex> _primitiveRenderer;
        private readonly IVertexRenderer<TransformedColouredTexturedVertex> _textureRenderer;
        private readonly IFont _font;
        public IColourScheme ColourScheme { get; private set; }
        public float TextHeight { get { return _font.MeasureString("l", TextFormat.SingleLine).Height; }}
        private Vector2 _offset;
        public RenderMode RenderMode { get; private set; }

        public GuiRenderer(GraphicsContext graphicsContext, IColourScheme colourScheme)
        {
            ColourScheme = colourScheme;
            _graphicsContext = graphicsContext;
            _primitiveRenderer = _graphicsContext.CreateVertexRenderer<TransformedColouredVertex>(18);
            _textureRenderer = _graphicsContext.CreateVertexRenderer<TransformedColouredTexturedVertex>(9*2*3);

            _font = graphicsContext.GetFont(DefaultFontName, DefautltFontSize, Weight.Bold);

            PrecacheTextures(colourScheme);
        }

        private static void WriteQuad(TextureArea texture, Rectangle rect, IDataStream<TransformedColouredTexturedVertex> dataStream, float alpha)
        {
            rect = rect.Translate(-0.5f, -0.5f);

            dataStream.WriteRange(
                new[]
                {
                    MakeVertex(rect.TopLeft, texture.AtlasTopLeft, alpha),
                    MakeVertex(rect.BottomRight, texture.AtlasBottomRight, alpha),
                    MakeVertex(rect.BottomLeft, texture.AtlasBottomLeft, alpha),
                    MakeVertex(rect.TopLeft, texture.AtlasTopLeft, alpha),
                    MakeVertex(rect.TopRight, texture.AtlasTopRight, alpha),
                    MakeVertex(rect.BottomRight, texture.AtlasBottomRight, alpha)
                });
        }

        private static void WriteCroppedQuad(TextureArea texture, Rectangle rect, 
            IDataStream<TransformedColouredTexturedVertex> dataStream, float horizontalCrop, float verticalCrop)
        {
            rect = rect.Translate(-0.5f, -0.5f).Scale(horizontalCrop, verticalCrop);

            var atlasRect = new Rectangle(texture.AtlasTopLeft, texture.AtlasBottomRight);
            atlasRect = atlasRect.Scale(horizontalCrop, verticalCrop);

            dataStream.WriteRange(
                new[]
                {
                    MakeVertex(rect.TopLeft, atlasRect.TopLeft),
                    MakeVertex(rect.BottomRight, atlasRect.BottomRight),
                    MakeVertex(rect.BottomLeft, atlasRect.BottomLeft),
                    MakeVertex(rect.TopLeft, atlasRect.TopLeft),
                    MakeVertex(rect.TopRight, atlasRect.TopRight),
                    MakeVertex(rect.BottomRight, atlasRect.BottomRight)
                });

        }

        private void PrecacheTextures(IColourScheme colourScheme)
        {
            foreach (var skinFilename in colourScheme.SkinFilenames)
            {
                _graphicsContext.LoadTextureAtlas(skinFilename);
            }
        }

        public void Dispose()
        {
            if (_primitiveRenderer != null) _primitiveRenderer.Dispose();
            if (_textureRenderer != null) _textureRenderer.Dispose();
        }

        private void Begin()
        {
            _graphicsContext.ZBufferEnabled = false;
            _offset = new Vector2(0.0f, 0.0f);
        }

        public void IncreaseOffset(Vector2 offset)
        {
            _offset += offset;
        }

        public void DecreaseOffset(Vector2 offset)
        {
            _offset -= offset;
        }

        private void End()
        {
            _graphicsContext.ZBufferEnabled = true;
        }

        public void Render(GuiManager guiManager)
        {
            _graphicsContext.FillMode = FillMode.Solid;
            RenderMode = RenderMode.Normal;
            Begin();
            guiManager.Desktop.HandleRender(this);
            RenderDragBehaviour(guiManager);
            RenderTooltip(guiManager);

            End();
        }

        private void RenderTooltip(GuiManager guiManager)
        {
            if (guiManager.TooltipWindow == null)
                return;

            // Don't render the tooltip if we're dragging
            var dragWidget = guiManager.DragWidget;
            if (dragWidget != null)
                return;

            var oldOffset = _offset;
            _offset = GetTooltipPosition(guiManager);
            guiManager.TooltipWindow.HandleRender(this);
            _offset = oldOffset;
        }

        private Vector2 GetTooltipPosition(GuiManager guiManager)
        {
            var x = guiManager.LastMousePosition.X + 16;
            var y = guiManager.LastMousePosition.Y + 16;

            var maxX = guiManager.Desktop.Size.X - guiManager.TooltipWindow.Size.X;
            var maxY = guiManager.Desktop.Size.Y - guiManager.TooltipWindow.Size.Y;

            x = Math.Min(x, maxX);
            
            if (y > maxY)
            {
                y -= 16;
                y -= guiManager.TooltipWindow.Size.Y;
            }

            return new Vector2(x - 16, y);
        }

        private void RenderDragBehaviour(GuiManager guiManager)
        {
            RenderMode = RenderMode.Dragging;

            var dragWidget = guiManager.DragWidget;
            if (dragWidget == null)
                return;

            var oldOffset = _offset;
            _offset = guiManager.LastMousePosition - dragWidget.Position - (dragWidget.Size / 2);
            dragWidget.HandleRender(this);
            _offset = oldOffset;
        }

        public void Desktop(Desktop widget)
        {
            var dataStream = _primitiveRenderer.LockVertexBuffer();

            var colour = ColourScheme.WindowBackground;
            colour.Alpha = widget.Opacity;

            WriteRectangle(
                new Rectangle
                    {
                        BottomRight = widget.Position + widget.Size,
                        TopLeft = widget.Position
                    },
                colour, 
                dataStream);

            _primitiveRenderer.UnlockVertexBuffer();

            _primitiveRenderer.Render(PrimitiveType.TriangleList, 0, 2);
        }

        private static void WriteRectangle(Rectangle extents, Color4 colour, IDataStream<TransformedColouredVertex> dataStream)
        {
            dataStream.WriteRange(
                new[]
                {
                    new TransformedColouredVertex
                        {
                            Colour = colour,
                            Position = new Vector4(extents.BottomLeft.X, extents.BottomLeft.Y, 1.0f, 1.0f)
                        },
                    new TransformedColouredVertex
                        {
                            Colour = colour,
                            Position = new Vector4(extents.TopLeft.X, extents.TopLeft.Y, 1.0f, 1.0f)
                        },
                    new TransformedColouredVertex
                        {
                            Colour = colour,
                            Position = new Vector4(extents.TopRight.X, extents.TopRight.Y, 1.0f, 1.0f)
                        },

                    new TransformedColouredVertex
                        {
                            Colour = colour,
                            Position = new Vector4(extents.BottomLeft.X, extents.BottomLeft.Y, 1.0f, 1.0f)
                        },
                    new TransformedColouredVertex
                        {
                            Colour = colour,
                            Position = new Vector4(extents.TopRight.X, extents.TopRight.Y, 1.0f, 1.0f)
                        },
                    new TransformedColouredVertex
                        {
                            Colour = colour,
                            Position = new Vector4(extents.BottomRight.X, extents.BottomRight.Y, 1.0f, 1.0f)
                        }
                });
        }
    }
}