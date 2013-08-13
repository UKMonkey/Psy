namespace Psy.Gui.Components
{
    public class ListboxColumn
    {
        public string Header { get; private set; }
        public int Width { get; internal set; }
        internal bool AutoSize { get; private set; }

        public ListboxColumn(string header, int width)
        {
            Header = header;
            Width = width;
            AutoSize = width == 0;
        }
    }
}