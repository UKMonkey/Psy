using System.Collections.Generic;
using System.Linq;
using Psy.Core;
using Psy.Core.Input;
using SlimMath;

namespace Psy.Gui.Components
{
    public delegate void ListboxRowSelected(Listbox listbox, int rowNumber);

    public class Listbox : Widget
    {
        private int _selectedRowIndex;
        private int _highlightedRowIndex;
        private const int HeaderHeight = 18;
        private const int RowHeight = 14;
        
        private List<Widget> ColumnHeaderWidgets { get; set; }
        private List<List<string>> Data { get; set; }

        /// <summary>
        /// Row change resets this to zero, increases to MouseOverFadeMax
        /// </summary>
        private float _mouseOverFade;
        private int _previousHighlightedRowIndex = -1;
        private NinePatchHandle _ninePatchHandle;
        private const float MouseOverFadeMax = 0.6f;
        private const int MouseOverFadeSteps = 6;

        public List<ListboxColumn> Columns { get; set; }
        public event ListboxRowSelected RowSelected;

        private void OnRowSelected(int rowIndex)
        {
            var handler = RowSelected;
            if (handler != null) 
                handler(this, rowIndex);
        }

        internal Listbox(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent)
        {
            Columns = new List<ListboxColumn>(4);
            ColumnHeaderWidgets = new List<Widget>(4);
            Data = new List<List<string>>(10);

            _selectedRowIndex = -1;
            _highlightedRowIndex = -1;
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        public void Populate(IEnumerable<IEnumerable<RowData>> rowData)
        {
            Data.Clear();

            foreach (var rows in rowData)
            {
                var newRow = new List<string>(Columns.Count);
                newRow.AddRange(Columns.Select(t => ""));

                foreach (var column in rows)
                {
                    var idx = Columns.FindIndex(c => c.Header == column.Header);
                    newRow[idx] = column.Data;
                }
                Data.Add(newRow);
            }
        }

        /// <summary>
        /// Specify 0 width to auto-size.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        public void AddColumn(string title, int width = 0)
        {
            Columns.Add(new ListboxColumn(title, width));
            CalculateColumnWidths();
        }

        private void CreateColumnLabels()
        {
            RemoveColumnHeaderLabelWidgets();

            var x = 0;
            foreach (var column in Columns)
            {
                var columnWidget = GuiManager.CreateLabel(column.Header, new Vector2(x, 0), this);
                ColumnHeaderWidgets.Add(columnWidget);
                x += column.Width;
            }
        }

        private void CalculateColumnWidths()
        {
            var fixedWidthTotal = Columns.Where(c => !c.AutoSize).Sum(c => c.Width);
            var remainingWidth = Size.X - fixedWidthTotal;
            if (remainingWidth <= 0)
                return;

            var numAutoSizeColumns = Columns.Count(c => c.AutoSize);
            var autoSizeColumnWidth = (int)(remainingWidth/numAutoSizeColumns);

            foreach (var column in Columns.Where(c => c.AutoSize))
            {
                column.Width = autoSizeColumnWidth;
            }

            CreateColumnLabels();
        }

        private void RemoveColumnHeaderLabelWidgets()
        {
            foreach (var columnHeader in ColumnHeaderWidgets)
            {
                columnHeader.Delete();
            }
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            guiRenderer.NinePatch(Size, _ninePatchHandle);
            guiRenderer.NinePatch(new Vector2(Size.X, HeaderHeight), _ninePatchHandle);

            base.Render(guiRenderer);

            if (_selectedRowIndex != -1)
            {
                RenderRowBox(_selectedRowIndex, guiRenderer, guiRenderer.ColourScheme.ToggledButton);    
            }
            if (_highlightedRowIndex != -1)
            {
                RenderRowBox(
                    _highlightedRowIndex,
                    guiRenderer,
                    guiRenderer
                        .ColourScheme
                        .ToggledButton
                        .MakeTransparent(_mouseOverFade));
            }

            RenderRowText(guiRenderer);
        }

        private void RenderRowBox(int rowIndex, IGuiRenderer guiRenderer, Color4 colour)
        {
            if (rowIndex == -1)
                return;

            var pin = 4;

            var rowY = (rowIndex * RowHeight) + HeaderHeight;
            guiRenderer.Rectangle(
                new Vector2(pin, rowY),
                new Vector2(Size.X - pin, rowY + RowHeight),
                colour,
                colour,
                false);
        }

        private void RenderRowText(IGuiRenderer guiRenderer)
        {
            var y = HeaderHeight; // accomodate for header.
            foreach (var row in Data)
            {
                var i = 0;
                var x = 0;
                foreach (var column in Columns)
                {
                    guiRenderer.Text(row[i], new Vector2(x, y));
                    i++;
                    x += column.Width;
                }
                y += RowHeight;
            }
        }

        internal override void Update()
        {
            base.Update();

            if (_mouseOverFade < MouseOverFadeMax)
            {
                _mouseOverFade += MouseOverFadeMax / MouseOverFadeSteps;    
            }
        }

        internal override void IntMouseMove(Vector2 position)
        {
            base.IntMouseMove(position);
            var rowIndex = GetRowIndex(position);

            if (rowIndex != _previousHighlightedRowIndex)
            {
                _previousHighlightedRowIndex = rowIndex;
                _mouseOverFade = 0.0f;
            }

            _highlightedRowIndex = rowIndex;
        }

        internal override void IntMouseClick(Vector2 position, MouseButton button)
        {
            base.IntMouseClick(position, button);
            var rowIndex = GetRowIndex(position);

            if (rowIndex != _selectedRowIndex && rowIndex != -1)
            {
                OnRowSelected(rowIndex);    
            }
            _selectedRowIndex = rowIndex;
        }


        /// <summary>
        /// Get row index using a coordinate within the widgets area. Returns
        /// -1 if no row exists at the specified coordinates.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private int GetRowIndex(Vector2 position)
        {
            var rowIndex = (int)((position.Y - HeaderHeight)/RowHeight);
            return rowIndex > (Data.Count - 1) ? -1 : rowIndex;
        }
    }
}