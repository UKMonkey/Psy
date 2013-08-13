namespace Psy.Graphics.Text
{
    public interface IFontCache
    {
        IFont GetFont(string fontFace="", int fontSize=16, Weight weight=Weight.Normal, bool italic=false);
    }
}