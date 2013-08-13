using System.Collections.Generic;
using System.Xml;
using Psy.Core;
using Psy.Core.Input;
using Psy.Gui.Events;
using Psy.Gui.Loader;
using SlimMath;

namespace Psy.Gui.Components
{
    public class DropdownListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public DropdownListItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public static DropdownListItem Create(XmlElement row)
        {
            var text = row.GetString("text");
            var value = row.GetString("value");

            return new DropdownListItem(text, value);
        }
    }

    public class DropdownList : Widget
    {
        private class DropdownListWindow : Widget
        {
            private readonly DropdownList _dropdownList;
            private int _hoverRowIndex;

            private IEnumerable<DropdownListItem> Items
            {
                get { return _dropdownList.Items; }
            }

            protected internal DropdownListWindow(GuiManager guiManager, Widget parent, DropdownList dropdownList)
                : base(guiManager, parent)
            {
                _dropdownList = dropdownList;
                Visible = false;
                Enabled = true;
            }

            private float GetDropdownHeight()
            {
                return RowHeight * _dropdownList.Items.Count;
            }

            protected internal void Show()
            {
                Size = new Vector2(_dropdownList.Size.X, GetDropdownHeight());
                // todo: move to above the dropdown list if there isn't enough room on the screen

                Position = _dropdownList.GetAbsolutePosition() + new Vector2(0, _dropdownList.Size.Y);

                if (Position.Y + Size.Y > GuiManager.ViewportSize.Y)
                {
                    Position = Position.Translate(0, -(Size.Y + _dropdownList.Size.Y));
                }

                Visible = true;
                BringToFront();
                
            }

            protected internal void Hide()
            {
                Visible = false;
            }

            protected override void Render(IGuiRenderer guiRenderer)
            {
                base.Render(guiRenderer);

                var y = 0;

                foreach (var item in Items.IndexOver())
                {
                    var buttonBackground = _hoverRowIndex == item.Index ? Colours.LightBlue : guiRenderer.ColourScheme.ButtonBackground;
                    guiRenderer.Rectangle(
                        new Vector2(0, y), 
                        new Vector2(Size.X, y + RowHeight), 
                        guiRenderer.ColourScheme.ButtonSurround, 
                        buttonBackground);

                    guiRenderer.Text(
                        "Arial", 
                        RowHeight - 1, 
                        item.Value.Text, 
                        guiRenderer.ColourScheme.TextColour,
                        new Vector2(0, y));

                    y += RowHeight;
                }
            }

            protected override void OnMouseMove(MouseMoveEventArgs args)
            {
                base.OnMouseMove(args);

                var rowIndex = (int)(args.Position.Y / RowHeight);
                _hoverRowIndex = rowIndex;
            }

            internal override void IntMouseDown(Vector2 position, MouseButton button)
            {
                base.IntMouseDown(position, button);

                var rowIndex = (int)(position.Y / RowHeight);
                _dropdownList.SelectRow(rowIndex);
            }
        }

        private void SelectRow(int rowIndex)
        {
            SelectedItemIndex = rowIndex;
            _selectionWindow.Hide();
            OnChange();
        }

        private const int RowHeight = 16;
        private const string XmlNodeName = "dropdownlist";

        public List<DropdownListItem> Items { get; private set; }
        private readonly DropdownListWindow _selectionWindow;
        public int? SelectedItemIndex { get; set; }

        public string SelectedItemText
        {
            get
            {
                if (!SelectedItemIndex.HasValue || Items.Count < SelectedItemIndex.Value)
                {
                    return "";
                }

                return Items[SelectedItemIndex.Value].Text;
            }
            set
            { 
                var indexOf = Items.FindIndex(x => x.Text == value);
                SelectedItemIndex = indexOf;
            }
        }

        protected DropdownList(GuiManager guiManager, Widget parent)
            : base(guiManager, parent)
        {
            Items = new List<DropdownListItem>();
            _selectionWindow = new DropdownListWindow(GuiManager, GuiManager.Desktop, this);
        }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new DropdownList(guiManager, parent)
            {
                SelectedItemIndex = xmlElement.ReadInteger("selectedItemIndex", 0)
            };

            foreach (XmlLinkedNode node in xmlElement)
            {
                var childNode = node as XmlElement;
                if (childNode == null)
                    continue;

                if (!childNode.Name.Equals("items"))
                    continue;

                foreach (XmlLinkedNode childChildNode in childNode)
                {
                    var row = childChildNode as XmlElement;
                    if (row == null)
                        continue;

                    var item = DropdownListItem.Create(row);

                    widget.Items.Add(item);
                }
            }

            return widget;
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }

        internal override void IntMouseClick(Vector2 position, MouseButton button)
        {
            base.IntMouseClick(position, button);

            if (_selectionWindow.Visible)
            {
                _selectionWindow.Hide();
            }
            else
            {
                _selectionWindow.Show();    
            }
        }

        internal override void IntLoseFocus()
        {
            base.IntLoseFocus();
            _selectionWindow.Hide();
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);

            guiRenderer.Rectangle(
                Size, 
                guiRenderer.ColourScheme.TextboxBackground, 
                guiRenderer.ColourScheme.ButtonSurround);

            guiRenderer.Text(
                    SelectedItemText,
                    new Vector2(0, Size.Y / 2), 
                    VerticalAlignment.Middle, 
                    HorizontalAlignment.LeftAbsolute);
        }
    }
}