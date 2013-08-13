using System;
using System.Linq;
using Psy.Core.Input;
using SlimMath;

namespace Psy.Gui.Components
{
    public delegate void ButtonToggledEvent(object sender);

    public class ToggleButton : Widget
    {

        /// <summary>
        /// If this button is grouped then any other ToggleButton siblings
        /// will have their Value set to false if the value of this widget
        /// is set to true
        /// </summary>
        public bool IsGrouped { get; set; }

        public string Label { get; set; }

        public string ImageName { get; set; }

        public string GroupName { get; set; }

        private bool _value;
        private readonly NinePatchHandle _ninePatchHandle;

        public bool Value
        {
            get { return _value; }
            set
            {
                if (value && Parent != null)
                {
                    foreach (var toggleButton in Parent.Children
                        .OfType<ToggleButton>()
                        .Where(x => x.GroupName.Equals(GroupName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        toggleButton.Value = false;
                    }
                }

                _value = value;
            }
        }

        public event ButtonToggledEvent Toggled;

        public ToggleButton(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent)
        {
            _value = false;
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        private void OnToggled()
        {
            var handler = Toggled;
            if (handler != null) handler(this);
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            string textureName;

            if (Value && IsMouseOver)
            {
                textureName = "guiskin_sel_down.png";
            }
            else if (Value)
            {
                textureName = "guiskin_down.png";
            }
            else if (IsMouseOver)
            {
                textureName = "guiskin_sel.png";
            }
            else
            {
                textureName = null;
            }

            guiRenderer.NinePatch(Size, _ninePatchHandle, textureName);

            guiRenderer.Text(
                    Label,
                    Size / 2,
                    VerticalAlignment.Middle,
                    HorizontalAlignment.Centre);

            if (!string.IsNullOrEmpty(ImageName))
            {
                guiRenderer.Image(ImageName, Size);
            }

        }

        internal override void IntMouseClick(Vector2 position, MouseButton button)
        {
            base.IntMouseDown(position, button);
            OnClick(null);
        }

        internal override void IntMouseUp(Vector2 position, MouseButton button)
        {
            base.IntMouseUp(position, button);
            Value = !Value;
            OnToggled();
        }

    }
}