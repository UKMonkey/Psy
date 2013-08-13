using System;
using Psy.Core;
using SlimMath;

namespace Psy.Gui.Components
{
    public class TextListRow
    {
        public string Text { get; set; }
        public Color4 Colour { get; set; }
        public string FontFace { get; set; }
        public int FontSize { get; set; }
        public bool Italic { get; set; }

        public TextListRow(string text, Color4 colour, string fontFace, int fontSize, bool italic)
        {
            Text = text;
            Colour = colour;
            FontFace = fontFace;
            FontSize = fontSize;
            Italic = italic;
        }
    }

    public class TextList : Widget
    {
        private const string DefaultFontFace = "Arial";
        private const int DefaultFontSize = 16;

        private FixedLengthList<TextListRow> Items { get; set; }
        public TextListDirection Direction { get; set; }
        public bool ResizeParent { get; set; }

        public int MaxLength { 
            get { return Items.MaxCount; }
            set { Items.MaxCount = value; }
        }

        internal TextList(GuiManager guiManager, Widget parent = null) : base(guiManager, parent)
        {
            Items = new FixedLengthList<TextListRow>();
        }

        public void AddLine(string text, Color4 colour, string fontFace = null, int fontSize = 0, bool italic = false)
        {
            if (string.IsNullOrEmpty(fontFace))
            {
                fontFace = DefaultFontFace;
            }

            if (fontSize == 0)
            {
                fontSize = DefaultFontSize;
            }

            Items.Add(new TextListRow(text, colour, fontFace, fontSize, italic));
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);

            var height = 0;
            var width = 0;

            if (Direction == TextListDirection.BottomUp)
            {
                var textY = Size.Y;

                for (var index = Items.Count - 1; index >= 0; index--)
                {
                    var row = Items[index];
                    textY -= row.FontSize;
                    height += row.FontSize;
                    var thisWidth = guiRenderer.Text(row.FontFace, row.FontSize, row.Text, row.Colour, 
                        new Vector2(0, textY), horizontalAlignment: HorizontalAlignment.LeftAbsolute);
                    width = Math.Max(width, thisWidth);
                }
            }
            else
            {
                var textY = 0;
                foreach (var row in Items)
                {
                    var thisWidth = guiRenderer.Text(row.FontFace, row.FontSize, row.Text, row.Colour, 
                        new Vector2(0, textY), horizontalAlignment: HorizontalAlignment.LeftAbsolute);
                    textY += row.FontSize;
                    height += row.FontSize;
                    width = Math.Max(width, thisWidth);
                }
            }

            if (ResizeParent)
            {
                Parent.Size = new Vector2(
                    width + (Parent.Margin.X * 2),
                    height + (Parent.Margin.Y * 2));
            }
        }
    }
}