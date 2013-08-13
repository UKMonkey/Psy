using System;
using SlimMath;

namespace Psy.Gui.Components
{
    public class Textbox : Widget
    {
        private const string CaretSymbol = "_";

        public override bool CanHaveFocus { get { return true; } }

        public bool Background { get; set; }
        public bool Border { get; set; }

        public string Value { get; set; }
        private float _caretOpacityCycle;

        internal Textbox(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent) {}

        internal override void IntKeyText(char keyInput)
        {
            base.IntKeyText(keyInput);

            _caretOpacityCycle = 0;

            if (keyInput == 8)
            {
                if (Value.Length > 0)
                {
                    Value = Value.Substring(0, Value.Length - 1);
                }
            }
            else if (keyInput == 10 || keyInput == 13)
            {
                OnChange();
            }
            else
            {
                Value += keyInput;
            }
        }

        private float CaretOpacity
        {
            get
            {
                return (float) Math.Abs(Math.Cos(_caretOpacityCycle));
            }
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);

            var horizontalAlignment = Border ? HorizontalAlignment.LeftWithMargin : HorizontalAlignment.LeftAbsolute;

            if (Background)
            {
                guiRenderer.Rectangle(
                    Size,
                    guiRenderer.ColourScheme.ButtonSurround,
                    guiRenderer.ColourScheme.TextboxBackground, Border);
            }

            var textWidth = guiRenderer.Text(
                    Value,
                    new Vector2(0, Size.Y / 2), VerticalAlignment.Middle, horizontalAlignment);

                guiRenderer.Text(
                    (HasFocus ? CaretSymbol : ""),
                    new Vector2(textWidth, Size.Y / 2),
                    VerticalAlignment.Middle,
                    horizontalAlignment, CaretOpacity);
        }

        internal override void Update()
        {
            base.Update();

            _caretOpacityCycle += 0.05f;
            if (_caretOpacityCycle >= (2 * Math.PI))
            {
                _caretOpacityCycle = 0;
            }
        }

        internal override void IntGainFocus()
        {
            base.IntGainFocus();
            _caretOpacityCycle = 0;
        }
    }
}