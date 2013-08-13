using System;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Tab : Widget
    {
        public bool EnlargeToContainChildren { get; set; }

        internal Tab(GuiManager guiManager, Widget parent)
            : base(guiManager, parent)
        {
            EnlargeToContainChildren = true;
            Margin = new Vector2(10, 10);
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.Rectangle(
                Size, 
                guiRenderer.ColourScheme.WindowBackground, 
                guiRenderer.ColourScheme.ButtonSurround);
        }

        internal override void AddChild(Widget child)
        {
            base.AddChild(child);

            if (EnlargeToContainChildren)
            {
                //CalculateNewSize();
            }
        }

        private void CalculateNewSize()
        {
            float maxWidth = Size.X;
            float maxHeight = Size.Y;

            foreach (var widget in Children)
            {
                maxWidth = Math.Max(maxWidth, widget.Position.X + widget.Size.X + (Margin.X*2));
                maxHeight = Math.Max(maxHeight, widget.Position.Y + widget.Size.Y + (Margin.Y*2));
            }

            Size = new Vector2(maxWidth, maxHeight);
        }
    }
}