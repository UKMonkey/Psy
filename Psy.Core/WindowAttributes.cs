namespace Psy.Core
{
    public class WindowAttributes
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool AllowResize { get; set; }

        public WindowAttributes()
        {
            AllowResize = false;
        }
    }
}