using System.Collections.Generic;

namespace Psy.Gui
{
    public class NinePatchHandle
    {
        public readonly string Top;
        public readonly string TopRight;
        public readonly string TopLeft;
        public readonly string Bottom;
        public readonly string BottomLeft;
        public readonly string BottomRight;
        public readonly string Centre;
        public readonly string Left;
        public readonly string Right;
 
        public IEnumerable<string> Textures
        {
            get{ return new List<string>
                            {
                                TopLeft, Top, TopRight,
                                Left, Centre, Right,
                                BottomLeft, Bottom, BottomRight
                            };}
        }

        private static readonly Dictionary<string, NinePatchHandle> Handles = new Dictionary<string, NinePatchHandle>(10);

        private NinePatchHandle(string top, string topRight, string topLeft, string bottom, 
            string bottomLeft, string bottomRight, string centre, string left, string right)
        {
            Top = top;
            TopRight = topRight;
            TopLeft = topLeft;
            Bottom = bottom;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Centre = centre;
            Left = left;
            Right = right;
        }

        public static NinePatchHandle Create(string name)
        {
            NinePatchHandle result;

            if (Handles.TryGetValue(name, out result))
            {
                return result;
            }

            result = new NinePatchHandle(
                string.Format("{0}_T", name),
                string.Format("{0}_TR", name),
                string.Format("{0}_TL", name),
                string.Format("{0}_B", name),
                string.Format("{0}_BL", name),
                string.Format("{0}_BR", name),
                string.Format("{0}_C", name),
                string.Format("{0}_L", name),
                string.Format("{0}_R", name)
            );

            return result;
        }
    }
}