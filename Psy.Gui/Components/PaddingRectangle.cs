namespace Psy.Gui.Components
{
    public struct PaddingRectangle
    {
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;
        public readonly int Left;

        private PaddingRectangle(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public static PaddingRectangle Parse(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return new PaddingRectangle();

            var parts = inputString.Split(',');

            if (parts.Length == 0)
                return new PaddingRectangle();

            var top = int.Parse(parts[0]);

            if (parts.Length == 1)
            {
                return new PaddingRectangle(top, top, top, top);
            }

            var right = int.Parse(parts[1]);

            if (parts.Length == 2)
            {
                return new PaddingRectangle(top, right, top, right);
            }

            var bottom = int.Parse(parts[2]);

            if (parts.Length == 3)
            {
                return new PaddingRectangle(top, right, bottom, right);
            }

            var left = int.Parse(parts[3]);

            return new PaddingRectangle(top, right, bottom, left);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Top, Right, Bottom, Left);
        }
    }
}