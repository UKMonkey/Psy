using System.Windows.Forms;

namespace Psy.Graphics
{
    public delegate void KeyPressEvent(object sender, KeyPressEventArgs args);

    public delegate void KeyEvent(object sender, KeyEventArgs args);

    public delegate void MouseEvent(object sender, MouseEventArgs args);

    public delegate void RenderEvent();
}