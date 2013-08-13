using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Psy.Core;
using Psy.Core.Input;
using Psy.Gui.Events;
using Psy.Gui.Loader;
using SlimMath;

namespace Psy.Gui.Components
{
    public enum TimelineMarkerShape
    {
        Square,
        Diamond,
        Line
    }

    public class TimelineClickEventArgs
    {
        public float Position { get; private set; }

        public TimelineClickEventArgs(float position)
        {
            Position = position;
        }
    }

    public delegate void TimelineClickEvent(object sender, TimelineClickEventArgs args);

    public class TimelineMarkerEventArgs
    {
        public TimelineMarker Marker { get; set; }

        public TimelineMarkerEventArgs(TimelineMarker marker)
        {
            Marker = marker;
        }
    }

    public delegate void TimelineMarkerClickEvent(object sender, TimelineMarkerEventArgs args);

    public delegate void TimelineMarkerMovedEvent(object sender, TimelineMarkerEventArgs args);

    public class TimelineMarker
    {
        private const float FadeTime = 20.0f;

        private const float HoverMarkerSize = 15.0f;
        private const float NormalMarkerSize = 10.0f;

        public string Id { get; private set; }
        public float Position { get; set; }
        public Color4 Colour { get; set; }
        public TimelineMarkerShape Shape { get; set; }

        private float _hoverFade;
        internal Color4 ColourWithHoverFade
        {
            get 
            {
                var colourAmount = (FadeTime - _hoverFade) / FadeTime;
                var hoverColourAmount = _hoverFade / FadeTime;
                return ((Colour * colourAmount) + (Colour.Multiply(1.6f) * hoverColourAmount)).MakeSolid();
            }
        }

        internal float MarkerSize { get; private set; }

        public TimelineMarker(string id, float position, Color4 colour, TimelineMarkerShape shape)
        {
            Id = id;
            Position = position;
            Colour = colour;
            Shape = shape;
        }

        public static TimelineMarker Create(XmlElement xmlElement)
        {
            var id = xmlElement.GetString("id");
            var position = xmlElement.ReadFloat("position", 0.0f);
            var colour = xmlElement.ReadColor4("colour", Colours.Green);

            TimelineMarkerShape shape;
            Enum.TryParse(xmlElement.GetString("shape"), out shape);

            var timelineMarker = new TimelineMarker(id, position, colour, shape);

            return timelineMarker;
        }

        public void Update()
        {
            if (_hoverFade < 0)
            {
                _hoverFade = 0;
            }
            else
            {
                _hoverFade--;
            }

            if (MarkerSize <= NormalMarkerSize)
            {
                MarkerSize = NormalMarkerSize;
            }
            else
            {
                MarkerSize--;
            }
            
        }

        internal void Hover()
        {
            _hoverFade = FadeTime;
            MarkerSize = HoverMarkerSize;
        }
    }

    public class Timeline : Widget
    {
        private float _selectedMarkerInitialPosition;
        private const string XmlNodeName = "timeline";
        private const float SelectMarkerRadius = 5.0f;
        private const float HorizontalMargin = 15.0f;

        public event TimelineClickEvent TimelineClick;
        public event TimelineMarkerClickEvent MarkerClick;
        public event TimelineMarkerClickEvent MarkerDoubleClick;

        public event TimelineMarkerMovedEvent MarkerMove;

        public string Units { get; set; }
        public float StartValue { get; set; }
        public float EndValue { get; set; }
        public float MajorStep { get; set; }
        public float MinorStep { get; set; }
        public float CursorTimelinePosition { get; private set; }
        public string TimelineGroup { get; set; }
        public TimelineMarker HoverMarker { get; private set; }
        public TimelineMarker SelectedMarker { get; private set; }
        public List<TimelineMarker> Markers { get; set; }

        private float ValueRange
        {
            get { return (EndValue - StartValue); }
        }

        private float TimelineWidth
        {
            get { return (Size.X - (HorizontalMargin * 2)); }
        }

        protected Timeline(GuiManager guiManager, Widget parent): base(guiManager, parent)
        {
            Markers = new List<TimelineMarker>(5);
        }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new Timeline(guiManager, parent)
            {
                Units = xmlElement.GetString("units"),
                TimelineGroup = xmlElement.GetString("timelineGroup"),
                StartValue = xmlElement.ReadFloat("startValue", 0.0f),
                EndValue = xmlElement.ReadFloat("endValue", 1.0f),
                MajorStep = xmlElement.ReadFloat("majorStep", 0.25f),
                MinorStep = xmlElement.ReadFloat("minorStep", 0.1f)
            };

            foreach (XmlLinkedNode node in xmlElement)
            {
                var childNode = node as XmlElement;
                if (childNode == null)
                    continue;

                if (childNode.Name.Equals("markers"))
                {
                    foreach (XmlLinkedNode childChildNode in childNode)
                    {
                        var row = childChildNode as XmlElement;
                        if (row == null)
                            continue;

                        var marker = TimelineMarker.Create(row);

                        widget.Markers.Add(marker);
                    }
                }
            }

            return widget;
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }

        protected virtual void OnTimelineClick(TimelineClickEventArgs args)
        {
            var handler = TimelineClick;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnMarkerClick(TimelineMarkerEventArgs args)
        {
            var handler = MarkerClick;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnMarkerMove(TimelineMarkerEventArgs args)
        {
            var handler = MarkerMove;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnMarkerDoubleClick(TimelineMarkerEventArgs args)
        {
            var handler = MarkerDoubleClick;
            if (handler != null) handler(this, args);
        }

        protected override void OnMouseDown(MouseEventArguments args)
        {
            base.OnMouseDown(args);
            SelectedMarker = HoverMarker;

            if (SelectedMarker != null)
            {
                OnMarkerClick(new TimelineMarkerEventArgs(SelectedMarker));
                _selectedMarkerInitialPosition = SelectedMarker.Position;
            }
            else
            {
                OnTimelineClick(new TimelineClickEventArgs(CursorTimelinePosition));
            }
        }

        protected override void OnMouseUp(MouseEventArguments args)
        {
            base.OnMouseUp(args);

            if (SelectedMarker != null && (Math.Abs(SelectedMarker.Position - _selectedMarkerInitialPosition) >= 0.001))
            {
                OnMarkerMove(new TimelineMarkerEventArgs(SelectedMarker));
            }

            _selectedMarkerInitialPosition = float.NaN;
            SelectedMarker = null;
        }

        internal override void IntMouseLeave()
        {
            base.IntMouseLeave();
            HoverMarker = null;
            SelectedMarker = null;
        }

        public override void IntMouseDoubleClick(Vector2 position, MouseButton button)
        {
            base.IntMouseDoubleClick(position, button);

            if (HoverMarker != null)
            {
                OnMarkerDoubleClick(new TimelineMarkerEventArgs(HoverMarker));
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs args)
        {
            base.OnMouseMove(args);

            UpdateCursorTimelinePosition(args.Position.X);

            if (SelectedMarker == null)
            {
                DetermineHoverMarker();
            }
            else
            {
                MoveSelectedMarker();
            }
        }

        private void MoveSelectedMarker()
        {
            if (SelectedMarker == null)
                return;

            SelectedMarker.Position = CursorTimelinePosition;

            foreach (var timeline in GetTimelinesInGroup())
            {
                var translatedPosition = GetTranslatedPosition(timeline, SelectedMarker.Position);
                timeline.UpdateMarkerPosition(SelectedMarker.Id, translatedPosition);
            }
        }

        /// <summary>
        /// Change the position of a specified marker
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        private void UpdateMarkerPosition(string id, float position)
        {
            var marker = Markers.Single(x => x.Id == id);
            marker.Position = position;
        }

        private void DetermineHoverMarker()
        {
            var maxDist = (ValueRange / TimelineWidth) * SelectMarkerRadius;

            HoverMarker = null;
            foreach (var marker in Markers)
            {
                var dist = Math.Abs(marker.Position - CursorTimelinePosition);
                if (dist > maxDist)
                    continue;

                marker.Hover();
                HoverMarker = marker;
            }
        }

        private void UpdateCursorTimelinePosition(float timelinePixelPosition)
        {
            CursorTimelinePosition = ((timelinePixelPosition - HorizontalMargin) / TimelineWidth) * ValueRange;
            UpdateTimelinesInGroup();
        }

        private void UpdateTimelinesInGroup()
        {
            foreach (var widget in GetTimelinesInGroup())
            {
                widget.CursorTimelinePosition = GetTranslatedPosition(widget, CursorTimelinePosition);
            }
        }

        /// <summary>
        /// Translate a position within this timeline into a position within the referenced timeline.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="timelinePosition"></param>
        /// <returns></returns>
        private float GetTranslatedPosition(Timeline widget, float timelinePosition)
        {
            return widget.ValueRange * (timelinePosition / ValueRange);
        }

        private IEnumerable<Timeline> GetTimelinesInGroup()
        {
            return Parent.Children.OfType<Timeline>().Where(timeline => timeline != this);
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);

            guiRenderer.Rectangle(Size, Colours.Black, border: false);

            RenderCursorMarker(guiRenderer);
            RenderMinorTicks(guiRenderer);
            RenderMajorTicks(guiRenderer);
            RenderMarkers(guiRenderer);
        }

        private void RenderMarkers(IGuiRenderer guiRenderer)
        {
            foreach (var marker in Markers)
            {
                var x = ((marker.Position / ValueRange) * TimelineWidth) + HorizontalMargin;

                RenderLine(guiRenderer, x, Colours.VeryDarkGrey);

                switch (marker.Shape)
                {
                    case TimelineMarkerShape.Square:
                        RenderSquare(guiRenderer, marker, x, marker.ColourWithHoverFade);
                        break;
                    case TimelineMarkerShape.Diamond:
                        RenderDiamond(guiRenderer, marker, x, marker.ColourWithHoverFade);
                        break;
                    case TimelineMarkerShape.Line:
                        RenderLine(guiRenderer, x, marker.ColourWithHoverFade);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void RenderSquare(IGuiRenderer guiRenderer, TimelineMarker marker, float x, Color4 colour)
        {
            var halfMarkerSize = marker.MarkerSize / 2.0f;
            var y = Size.Y / 2.0f;
            var topLeft = new Vector2(x - halfMarkerSize, y - halfMarkerSize);
            var bottomRight = new Vector2(x + halfMarkerSize, y + halfMarkerSize);

            guiRenderer.Line(topLeft, topLeft.Translate(marker.MarkerSize, 0), colour.Multiply(1.4f));
            guiRenderer.Line(topLeft, topLeft.Translate(0, marker.MarkerSize), colour.Multiply(1.4f), colour.Multiply(0.2f));
            guiRenderer.Line(bottomRight.Translate(0, -marker.MarkerSize), bottomRight, colour.Multiply(1.4f), colour.Multiply(0.2f));
            guiRenderer.Line(bottomRight, bottomRight.Translate(-marker.MarkerSize, 0), colour.Multiply(0.2f));
        }

        private void RenderDiamond(IGuiRenderer guiRenderer, TimelineMarker marker, float x, Color4 colour)
        {
            var halfMarkerSize = marker.MarkerSize / 2.0f;
            var y = Size.Y / 2.0f;

            guiRenderer.Line(new Vector2(x, y - halfMarkerSize), new Vector2(x + halfMarkerSize, y), colour.Multiply(1.4f), colour);
            guiRenderer.Line(new Vector2(x, y - halfMarkerSize), new Vector2(x - halfMarkerSize, y), colour.Multiply(1.4f), colour);

            guiRenderer.Line(new Vector2(x - halfMarkerSize, y), new Vector2(x, y + halfMarkerSize), colour, colour.Multiply(0.2f));
            guiRenderer.Line(new Vector2(x + halfMarkerSize, y), new Vector2(x, y + halfMarkerSize), colour, colour.Multiply(0.2f));
        }

        private void RenderLine(IGuiRenderer guiRenderer, float x, Color4 colour)
        {
            guiRenderer.Line(new Vector2(x, 0), new Vector2(x, Size.Y), colour);
        }

        private void RenderCursorMarker(IGuiRenderer guiRenderer)
        {
            var markerColour = new Color4(1.0f, 0.4f, 0.4f, 0.4f);

            var x = ((CursorTimelinePosition / ValueRange) * TimelineWidth) + HorizontalMargin;
            
            guiRenderer.Line(new Vector2(x, 0), new Vector2(x, Size.Y), markerColour);

            var str = string.Format("{0:0.00}{1}", CursorTimelinePosition, Units);
            guiRenderer.Text("Arial", 14, str, markerColour, new Vector2(x + 3, Size.Y),
                             VerticalAlignment.Bottom, HorizontalAlignment.LeftAbsolute);
        }

        private void RenderMinorTicks(IGuiRenderer guiRenderer)
        {
            var stepSize = MinorStep * (TimelineWidth / ValueRange);

            float renderX = 0;
            while (renderX <= TimelineWidth)
            {
                var r1 = new Vector2(renderX + HorizontalMargin, 0);
                var r2 = new Vector2(renderX + HorizontalMargin, 5);

                guiRenderer.Line(r1, r2, Colours.DarkGrey);
                renderX += stepSize;
            }
        }

        private void RenderMajorTicks(IGuiRenderer guiRenderer)
        {
            var stepSize = MajorStep * (TimelineWidth / ValueRange);

            float renderX = 0;
            float val = 0;
            while (renderX <= TimelineWidth)
            {
                var str = string.Format("{0:0.00}{1}", val, Units);
                guiRenderer.Text("Arial", 14, str, Colours.White, new Vector2(renderX + HorizontalMargin, 0),
                                 VerticalAlignment.Top, HorizontalAlignment.Centre);
                renderX += stepSize;
                val += MajorStep;
            }
        }

        internal override void Update()
        {
            base.Update();
            foreach (var marker in Markers)
            {
                marker.Update();

                if (marker == HoverMarker)
                {
                    marker.Hover();
                }
            }
        }
    }
}