using System.Collections.Generic;
using Psy.Core;
using Psy.Core.Input;
using Psy.Gui.Components;
using SlimMath;

namespace Psy.Gui
{
    public class GuiManager
    {
        private readonly IWindowSize _windowSize;
        public Desktop Desktop { get; private set; }
        internal readonly Dictionary<string, Widget> WidgetNameLookup;
        private Widget FocusWidget { get; set; }
        public Widget HoverWidget { get; private set; }
        public Widget DragWidget { get; private set; }
        public Vector2 LastMousePosition { get; private set; }
        //private Widget DragDestination { get; set; }

        private Widget _mouseDownWidget;
        private bool _dragging;
        private MouseButton _mouseButtonDragging;
        private Widget _lastClickedWidget;
        private double _lastClickedWidgetTime;

        public Widget TooltipWindow;
        private Widget _tooltipParent;
        private float _desktopDragdropPreviousOpacity;
        private bool _desktopDragdropPreviousTransparency;

        public Vector2 ViewportSize
        {
            get
            {
                return new Vector2(_windowSize.Width, _windowSize.Height);
            }
        }

        public GuiManager(IWindowSize windowSize)
        {
            WidgetNameLookup = new Dictionary<string, Widget>(10);
            _windowSize = windowSize;
            FocusWidget = null;
            DragWidget = null;

            CreateDesktop();
        }

        private void CreateDesktop()
        {
            Desktop = 
                new Desktop(this, null)
                {
                    Position = new Vector2(),
                    Size = ViewportSize,
                    Enabled = true
                };
        }

        public Widget GetWidgetByName(string name)
        {
            Widget widget;
            WidgetNameLookup.TryGetValue(name, out widget);
            return widget;
        }

        public T GetWidgetByName<T>(string name) where T : Widget
        {
            return (T)WidgetNameLookup[name];
        }

        internal Tab CreatePanel(Widget parent = null)
        {
            var panel = new Tab(this, parent);
            return panel;

        }

        public Label CreateLabel(string text, Vector2 position, Widget parent = null)
        {
            var widget = 
                new Label(this, parent)
                {
                    Position = position,
                    Enabled = true,
                    Text = text
                };
            return widget;
        }

        public Button CreateButton(string text, Vector2 position, Vector2 size, Widget parent = null)
        {
            var widget = 
                new Button(this, parent)
                {
                    Position = position,
                    Size = size,
                    Label = text,
                    Enabled = true
                };
            return widget;
        }

        internal ToggleButton CreateToggleButton(string label, Vector2 position, Vector2 size, bool toggled, Widget parent = null, string imageName = null)
        {
            var widget = 
                new ToggleButton(this, parent)
                {
                    Position = position,
                    Size = size,
                    Value = toggled
                };

            widget.Label = label;
            widget.ImageName = imageName;

            return widget;
        }

        public void Clear()
        {
            Desktop.RemoveAllChildren();
        }

        public void SetFocus(Widget widget)
        {
            if (FocusWidget != null)
            {
                FocusWidget.IntLoseFocus();
                FocusWidget.HasFocus = false;
            }

            if (widget != null && !widget.CanHaveFocus)
            {
                return;
            }

            FocusWidget = widget;

            if (widget != null)
            {
                widget.IntGainFocus();
                widget.HasFocus = true;
            }
        }

        /// <summary>
        /// Processes a mouse down event and palms it off to the relevant
        /// widget. Returns false if no widget was able to process the event.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool HandleMouseDown(Vector2 position, MouseButton button)
        {
            var result = Desktop.GetWidgetAtPosition(position);
            if (result.Widget == null)
                return false;

            _mouseButtonDragging = button;
            result.Widget.IsMouseDown = true;
            result.Widget.IntMouseDown(result.Position, button);
            
            SetFocus(result.Widget);

            _mouseDownWidget = result.Widget;

            return true;
        }

        public bool HandleMouseUp(Vector2 position, MouseButton button)
        {
            var result = Desktop.GetWidgetAtPosition(position);
            if (result.Widget == null)
                return false;

            result.Widget.IntMouseUp(result.Position, button);

            if (result.Widget == FocusWidget)
            {
                if (result.Widget.IsMouseDown) 
                {
                    if (_lastClickedWidget == result.Widget && (Timer.GetTime() < _lastClickedWidgetTime + 250))
                    {
                        result.Widget.IntMouseDoubleClick(result.Position, button);
                        _lastClickedWidget = null;
                        _lastClickedWidgetTime = 0;
                    }
                    else
                    {
                        result.Widget.IntMouseClick(result.Position, button);
                        _lastClickedWidget = result.Widget;
                        _lastClickedWidgetTime = Timer.GetTime();
                    }

                }
            }

            result.Widget.IsMouseDown = false;

            _mouseDownWidget = null;

            if (DragWidget != null && 
                (result.Widget.IntDragDropAccept(DragWidget) &&
                 DragWidget.IntDragDropEnd(result.Widget)))
            {
                _dragging = false;
                DragWidget = null;
                DragDropEnd();
            }

            return true;
        }

        public bool HandleMouseMove(Vector2 position)
        {
            LastMousePosition = position;

            var result = Desktop.GetWidgetAtPosition(position);

            // begin dragging if we can.
            if (_mouseDownWidget != null && 
                !_dragging && 
                _mouseButtonDragging == MouseButton.Left &&
                result.Widget != null &&
                result.Widget.Draggable && 
                result.Widget.IntDragDropBegin())
            {
                _dragging = true;
                DragWidget = result.Widget;
                DragDropBegin();
                return false;
            }

            // if we're over a widget, tell the widget we've mousemoved.
            if (result.Widget != null)
            {
                result.Widget.IntMouseMove(result.Position);

                if (_dragging)
                {
                    result.Widget.IntDragDropCanAccept(DragWidget);
                }
            }

            var isMouseDown = false;

            if (HoverWidget != null && HoverWidget != result.Widget)
            {
                HoverWidget.IntMouseLeave();
                HoverWidget.IsMouseOver = false;
                HoverWidget.IsMouseDown = false;

                if (TooltipWindow != null)
                {
                    TooltipWindow.Delete();
                    TooltipWindow = null;
                    _tooltipParent = null;
                }
            }

            HoverWidget = result.Widget;

            if (HoverWidget != null)
            {
                isMouseDown = HoverWidget.IsMouseDown;
            }

            if (result.Widget == null)
            {
                return false;
            }

            result.Widget.IsMouseOver = true;
            result.Widget.IntMouseEnter(result.Position);
            result.Widget.IsMouseDown = isMouseDown;

            if (_tooltipParent != result.Widget)
            {
                var tooltipWindow = result.Widget.GetTooltip();
                if (tooltipWindow != null)
                {
                    TooltipWindow = tooltipWindow;
                    _tooltipParent = result.Widget;
                }
            }

            return true;
        }

        private void DragDropEnd()
        {
            Desktop.Transparent = _desktopDragdropPreviousTransparency;
            Desktop.Opacity = _desktopDragdropPreviousOpacity;
        }

        private void DragDropBegin()
        {
            _desktopDragdropPreviousOpacity = Desktop.Opacity;
            _desktopDragdropPreviousTransparency = Desktop.Transparent;

            Desktop.Transparent = false;
            Desktop.Opacity = 0.1f;
        }

        public bool HandleKeyText(char key)
        {
            if (FocusWidget != null)
            {
                FocusWidget.IntKeyText(key);
                return true;
            }
            return false;
        }

        public void Update()
        {
            Desktop.Update();
            if (TooltipWindow != null)
            {
                TooltipWindow.Update();
            }
        }

        public void DeleteWidgetByName(string widgetName)
        {
            var widget = GetWidgetByName(widgetName);
            if (widget != null)
            {
                widget.Delete();
            }
        }

        public Widget GetBasicTooltipFactory(Widget widget)
        {
            return new Label(this, null)
            {
                Text = widget.TooltipText,
                Visible = true,
                AutoSize = AutoSize.Height | AutoSize.Width
            };
        }
    }
}