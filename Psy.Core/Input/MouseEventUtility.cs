using System;
using System.Windows.Forms;

namespace Psy.Core.Input
{
    public static class MouseEventUtility
    {
        public static MouseButton TranslateMouseButton(MouseEventArgs mouseEventArgs)
        {
            MouseButton button;
            switch (mouseEventArgs.Button)
            {
                case MouseButtons.Left:
                    button = MouseButton.Left;
                    break;
                case MouseButtons.Right:
                    button = MouseButton.Right;
                    break;
                case MouseButtons.Middle:
                    button = MouseButton.Middle;
                    break;
                case MouseButtons.XButton1:
                    button = MouseButton.XButton1;
                    break;
                case MouseButtons.XButton2:
                    button = MouseButton.XButton2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return button;
        }
    }
}