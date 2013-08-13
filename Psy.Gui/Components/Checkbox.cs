using System.Xml;
using Psy.Core.Input;
using Psy.Gui.Loader;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Checkbox : Widget
    {
        private const string XmlNodeName = "checkbox";

        public bool Value { get; set; }

        protected Checkbox(GuiManager guiManager, Widget parent)
            : base(guiManager, parent) { }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            guiRenderer.Rectangle(
                new Vector2(0, 0), 
                new Vector2(16, 16), 
                guiRenderer.ColourScheme.ButtonBackground, 
                guiRenderer.ColourScheme.ButtonSurround);

            if (Value)
            {
                guiRenderer.Line(new Vector2(0, 0), new Vector2(16, 16), guiRenderer.ColourScheme.ButtonSurround);
                guiRenderer.Line(new Vector2(16, 0), new Vector2(16, 0), guiRenderer.ColourScheme.ButtonSurround);
            }

            base.Render(guiRenderer);
        }

        internal override void IntMouseClick(Vector2 position, MouseButton button)
        {
            base.IntMouseClick(position, button);

            if (button == MouseButton.Left)
            {
                Value = !Value;
                OnChange();
            }
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var checkbox = new Checkbox(guiManager, parent)
            {
                Value = xmlElement.ReadBooleanAttribute("value")
            };

            checkbox.Size = new Vector2(16, 16);

            return checkbox;
        }
    }
}