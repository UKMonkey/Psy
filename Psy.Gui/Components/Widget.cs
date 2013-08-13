using System;
using System.Collections.Generic;
using Psy.Core;
using Psy.Core.Input;
using Psy.Gui.Events;
using SlimMath;

namespace Psy.Gui.Components
{
    public delegate Widget TooltipFactoryDelegate(Widget parent);

    public class Widget
    {
        private static int _nextWidgetId = 1;
        public TooltipFactoryDelegate TooltipFactory;

        private string _uniqueName;

        protected Widget(GuiManager guiManager, Widget parent)
        {
            GuiManager = guiManager;

            if (parent != null && parent != this)
            {
                parent.AddChild(this);
            }

            Children = new List<Widget>();
            Visible = true;
            AutoSize = AutoSize.None;
            Id = ++_nextWidgetId;

            UniqueName = string.Format("Widget_{0}", Id);
        }

        public virtual bool CanHaveFocus
        {
            get { return true; }
        }

        public string UniqueName
        {
            get { return _uniqueName; }
            set
            {
                if (_uniqueName == value)
                    return;

                if (_uniqueName != null)
                {
                    GuiManager.WidgetNameLookup.Remove(_uniqueName);
                }

                GuiManager.WidgetNameLookup.Add(value, this);
                _uniqueName = value;
            }
        }

        public string Class { get; set; }
        public float Alpha { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Margin { get; set; }
        public PaddingRectangle Padding { get; set; }
        public AutoSize AutoSize { get; set; }
        public Anchor Anchor { get; set; }
        internal GuiManager GuiManager { get; set; }
        public int Tag { get; set; }
        public object Metadata { get; set; }

        /// <summary>
        /// Size of area available for child widgets
        /// </summary>
        public virtual Vector2 ClientSize
        {
            get { return Size - (Margin*2); }
        }

        public bool Draggable { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public bool IsMouseOver { get; internal set; }
        public bool IsMouseDown { get; internal set; }
        public bool HasFocus { get; internal set; }
        public int Id { get; private set; }

        /// <summary>
        /// Transparent windows are not renderered and do not
        /// have events about them raised.
        /// </summary>
        public bool Transparent { get; set; }

        /// <summary>
        /// This widget does not respond to click events.
        /// </summary>
        public bool ClickThrough { get; set; }

        protected internal List<Widget> Children { get; private set; }
        public Widget Parent { get; private set; }

        /// <summary>
        /// Tooltip text. If HasTooltip is true and this property
        /// is null then it is expected that TooltipFactory be used to
        /// construct a custom tooltip.
        /// </summary>
        public string TooltipText { get; set; }

        public event ClickEvent DoubleClick;

        public virtual void OnDoubleClick(ClickEventArgs args)
        {
            var handler = DoubleClick;
            if (handler != null) handler(this, args);
        }

        public event ClickEvent Click;

        protected void OnClick(ClickEventArgs args)
        {
            var handler = Click;
            if (handler != null) handler(this, args);
        }

        public event DragDropEvent DragDrop;

        public void OnDragDropEvent(DragDropEventArgs args)
        {
            var handler = DragDrop;
            if (handler != null) handler(this, args);
        }

        public event MouseEvent MouseDown;

        protected virtual void OnMouseDown(MouseEventArguments args)
        {
            var handler = MouseDown;
            if (handler != null) handler(this, args);
        }

        public event MouseEvent MouseUp;

        protected virtual void OnMouseUp(MouseEventArguments args)
        {
            var handler = MouseUp;
            if (handler != null) handler(this, args);
        }

        public event MouseMoveEvent MouseMove;

        protected virtual void OnMouseMove(MouseMoveEventArgs args)
        {
            var handler = MouseMove;
            if (handler != null) handler(this, args);
        }

        public event MouseLeaveEvent MouseLeave;

        public void OnMouseLeave()
        {
            var handler = MouseLeave;
            if (handler != null) handler(this);
        }

        public event ChangeEvent Change;

        public void OnChange()
        {
            var handler = Change;
            if (handler != null) handler(this);
        }

        public void Delete()
        {
            if (Parent != null)
                Parent.RemoveChild(this);

            RemoveAllChildren();
            Dispose();
        }

        private void Dispose()
        {
            ClearEventHandlers();
            GuiManager.WidgetNameLookup.Remove(UniqueName);
        }

        private void ClearEventHandlers()
        {
            Click = null;
        }

        public void Focus()
        {
            GuiManager.SetFocus(this);
        }

        public void RemoveChild(Widget child)
        {
            Children.Remove(child);
            child.Dispose();
        }

        public void RemoveAllChildren()
        {
            foreach (var child in Children)
            {
                child.RemoveAllChildren();
                child.Dispose();
            }
            Children.Clear();
        }

        public void HandleRender(IGuiRenderer guiRenderer)
        {
            if (!Visible)
                return;

            guiRenderer.IncreaseOffset(Position);
            if (!Transparent)
                Render(guiRenderer);
            guiRenderer.IncreaseOffset(Margin);
            foreach (var child in Children)
            {
                child.HandleRender(guiRenderer);
            }
            guiRenderer.DecreaseOffset(Position + Margin);
        }

        internal Widget GetTooltip()
        {
            if (!string.IsNullOrEmpty(TooltipText))
            {
                return GuiManager.GetBasicTooltipFactory(this);
            }

            if (TooltipFactory == null)
                return null;

            return TooltipFactory(this);
        }

        protected virtual void Render(IGuiRenderer guiRenderer) {}

        internal virtual void AddChild(Widget child)
        {
            if (Children.Contains(child))
            {
                throw new Exception(
                    string.Format("{0} is already a child of this widget", child));
            }
            child.Parent = this;
            Children.Add(child);
        }

        public Widget FindWidget(int widgetId)
        {
            if (Id == widgetId)
                return this;

            foreach (var child in Children)
            {
                var childResult = child.FindWidget(widgetId);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }

        internal GetWidgetResult GetWidgetAtPosition(Vector2 position)
        {
            if (!Visible)
                return new GetWidgetResult();

            var translatedPosition = position - Position - Margin;

            foreach (var child in Children.ReverseInPlace()) 
            {
                var result = child.GetWidgetAtPosition(translatedPosition);
                if (result.Widget != null)
                    return result;
            }

            var extents =
                new Rectangle
                {
                    TopLeft = new Vector2(0, 0),
                    BottomRight = Size
                };

            if (extents.Contains(translatedPosition) && !Transparent && Enabled && !ClickThrough)
            {
                return new GetWidgetResult
                {
                    Widget = this,
                    WindowPosition = position,
                    Position = translatedPosition
                };
            }

            return new GetWidgetResult();
        }

        /// <summary>
        /// Called by GuiManager when this widget loses focus.
        /// </summary>
        internal virtual void IntLoseFocus() {}

        /// <summary>
        /// Called by GuiManager when this widget gains focus.
        /// </summary>
        internal virtual void IntGainFocus() {}

        /// <summary>
        /// Called by GuiManager when a mouse button is pressed over this widget.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="button"></param>
        internal virtual void IntMouseDown(Vector2 position, MouseButton button)
        {
            OnMouseDown(new MouseEventArguments(position, button));
        }

        /// <summary>
        /// Called by GuiManager when this widget is being dragged.
        /// </summary>
        /// <returns>True if the DragDrop behaviour can start.</returns>
        internal virtual bool IntDragDropBegin()
        {
            return true;
        }

        /// <summary>
        /// Called by GuiManager when this widget has finished being dragged.
        /// </summary>
        /// <param name="destination">Can be null. The instance of the widget that recieved the drag</param>
        /// <returns>True if the DragDrop behaviour can end on this widget.</returns>
        internal virtual bool IntDragDropEnd(Widget destination)
        {
            OnDragDropEvent(new DragDropEventArgs {Dragged = this, Target = destination});
            return true;
        }

        /// <summary>
        /// Called by the GuiManager when a widget is dragged over this one.
        /// </summary>
        /// <param name="dragged">The widget being dragged</param>
        /// <returns>Whether this widget accepts the drag</returns>
        internal virtual bool IntDragDropCanAccept(Widget dragged)
        {
            return true;
        }

        /// <summary>
        /// Called by the GuiManager when a widget is dragged over this one and dropped.
        /// </summary>
        /// <param name="dragged">The widget being dragged</param>
        /// <returns>True if the DragDrop behaviur can end on this widget.</returns>
        internal virtual bool IntDragDropAccept(Widget dragged)
        {
            OnDragDropEvent(new DragDropEventArgs {Dragged = dragged, Target = this});
            return true;
        }

        /// <summary>
        /// Called by GuiManager when a mouse button is released over this widget.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="button"></param>
        internal virtual void IntMouseUp(Vector2 position, MouseButton button)
        {
            OnMouseUp(new MouseEventArguments(position, button));
        }

        /// <summary>
        /// Called by GuiManager when a mouse button is clicked over this widget.
        /// A mouse down and mouse up event on the same widget will generate
        /// this event.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="button"></param>
        internal virtual void IntMouseClick(Vector2 position, MouseButton button)
        {
            //IntMouseDown(position, button);
            OnClick(new ClickEventArgs {Button = button});
        }

        public virtual void IntMouseDoubleClick(Vector2 position, MouseButton button)
        {
            OnDoubleClick(new ClickEventArgs {Button = button});
        }

        /// <summary>
        /// Called by GuiManager when the mouse cursor enters this component.
        /// </summary>
        /// <param name="position"></param>
        internal virtual void IntMouseEnter(Vector2 position) {}

        /// <summary>
        /// Called by GuiManager when the mouse cursor leaves this component. This
        /// will also be called if the mouse cursor moves onto a child component.
        /// </summary>
        internal virtual void IntMouseLeave()
        {
            OnMouseLeave();
        }

        /// <summary>
        ///  Called by GuiManager when the mouse cursor moves over the widget.
        /// </summary>
        /// <param name="position"></param>
        internal virtual void IntMouseMove(Vector2 position)
        {
            OnMouseMove(new MouseMoveEventArgs
            {
                Position = position
            });
        }

        /// <summary>
        /// Called by GuiManager when a printable key is pressed. Includes control
        /// keys: delete, tab and enter.
        /// </summary>
        /// <param name="keyInput"></param>
        internal virtual void IntKeyText(char keyInput) {}

        /// <summary>
        /// Called by GuiManager to animate the widget.
        /// </summary>
        internal virtual void Update()
        {
            CalculateAnchor();
            CalculateAutoSize();

            foreach (var child in Children)
            {
                child.Update();
            }
        }

        private void CalculateAnchor()
        {
            if (Anchor == Anchor.None)
                return;

            if (Parent == null)
                return;

            var newX = Position.X;
            var newY = Position.Y;

            var parentSize = this is Desktop ? GuiManager.ViewportSize : Parent.ClientSize;

            if ((Anchor & Anchor.HorizontalMiddle) == Anchor.HorizontalMiddle)
            {
                newX = parentSize.X/2 - Size.X/2;
            }
            else
            {
                if ((Anchor & Anchor.Left) == Anchor.Left)
                {
                    newX = Padding.Left;
                }
                else if ((Anchor & Anchor.Right) == Anchor.Right)
                {
                    newX = (parentSize.X - Size.X) - Padding.Right;
                }
            }

            if ((Anchor & Anchor.VerticalMiddle) == Anchor.VerticalMiddle)
            {
                newY = parentSize.Y/2 - Size.Y/2;
            }
            else
            {
                if ((Anchor & Anchor.Top) == Anchor.Top)
                {
                    newY = Padding.Top;
                }
                else if ((Anchor & Anchor.Bottom) == Anchor.Bottom)
                {
                    newY = (parentSize.Y - Size.Y) - Padding.Bottom;
                }
            }

            Position = new Vector2(newX, newY);
        }

        private void CalculateAutoSize()
        {
            if (AutoSize == AutoSize.None)
                return;

            Vector2 clientSize;

            if (this is Desktop)
            {
                clientSize = GuiManager.ViewportSize;
            }
            else if (Parent != null)
            {
                clientSize = Parent.ClientSize;
            }
            else
            {
                return;
            }

            var newX = ((AutoSize & AutoSize.Width) == AutoSize.Width) ? clientSize.X - Position.X : Size.X;
            var newY = ((AutoSize & AutoSize.Height) == AutoSize.Height) ? clientSize.Y - Position.Y : Size.Y;

            Size = new Vector2(newX, newY);
        }

        public TWidget FindWidgetByClass<TWidget>(string className) where TWidget : Widget
        {
            foreach (var widget in Children)
            {
                if (widget.Class == className)
                {
                    return (TWidget) widget;
                }

                var childSearch = widget.FindWidgetByClass<TWidget>(className);

                if (childSearch != null)
                {
                    return childSearch;
                }
            }

            return null;
        }

        public TWidget FindWidgetByUniqueName<TWidget>(string uniqueName) where TWidget : Widget
        {
            foreach (var widget in Children)
            {
                if (widget.UniqueName == uniqueName)
                {
                    return (TWidget) widget;
                }

                var childSearch = widget.FindWidgetByUniqueName<TWidget>(uniqueName);

                if (childSearch != null)
                {
                    return childSearch;
                }
            }

            return null;
        }

        public Vector2 GetAbsolutePosition()
        {
            if (Parent != null)
            {
                return Parent.GetAbsolutePosition() + Position;
            }
            else
            {
                return Position;
            }
        }

        public void BringToFront()
        {
            Parent.BringToFront(this);
        }

        private void BringToFront(Widget child)
        {
            if (!Children.Contains(child))
            {
                throw new Exception(string.Format("`{0}` is not a child of `{1}`. Cannot bring to front.", child, this));
            }

            Children.Remove(child);
            Children.Add(child);
        }
    }
}