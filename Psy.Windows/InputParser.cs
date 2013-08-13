using System.Collections.Generic;
using System.Windows.Forms;
using Psy.Core.Input;

namespace Psy.Windows
{
    public static class InputParser
    {
        private static readonly Dictionary<Keys, Key> KeysMappings = 
            new Dictionary<Keys, Key> {
                {Keys.None,Key.None },
                {Keys.LButton, Key.MouseLeft},
                {Keys.RButton, Key.MouseRight},
                {Keys.MButton, Key.MouseMiddle},
                {Keys.XButton1, Key.Mouse4},
                {Keys.XButton2, Key.Mouse5},
                {Keys.Back, Key.Backspace},
                {Keys.Tab, Key.Tab},
                {Keys.LineFeed, Key.Enter},
                {Keys.Return, Key.Enter},
                {Keys.Shift, Key.LeftShift},
                {Keys.ControlKey, Key.LeftCtrl},
                {Keys.Menu, Key.LeftAlt},
                {Keys.Pause, Key.PauseBreak},
                {Keys.Escape, Key.Escape},
                {Keys.Space, Key.Space},
                {Keys.Prior, Key.PageUp},
                {Keys.Next, Key.PageDown},
                {Keys.End, Key.End},
                {Keys.Home, Key.Home},
                {Keys.Left, Key.Left},
                {Keys.Up, Key.Up},
                {Keys.Right, Key.Right},
                {Keys.Down, Key.Down},
                {Keys.Snapshot, Key.PrintScreen},
                {Keys.Insert, Key.Insert},
                {Keys.Delete, Key.Delete},
                {Keys.D0, Key.D0},
                {Keys.D1, Key.D1},
                {Keys.D2, Key.D2},
                {Keys.D3, Key.D3},
                {Keys.D4, Key.D4},
                {Keys.D5, Key.D5},
                {Keys.D6, Key.D6},
                {Keys.D7, Key.D7},
                {Keys.D8, Key.D8},
                {Keys.D9, Key.D9},
                {Keys.A, Key.A},
                {Keys.B, Key.B},
                {Keys.C, Key.C},
                {Keys.D, Key.D},
                {Keys.E, Key.E},
                {Keys.F, Key.F},
                {Keys.G, Key.G},
                {Keys.H, Key.H},
                {Keys.I, Key.I},
                {Keys.J, Key.J},
                {Keys.K, Key.K},
                {Keys.L, Key.L},
                {Keys.M, Key.M},
                {Keys.N, Key.N},
                {Keys.O, Key.O},
                {Keys.P, Key.P},
                {Keys.Q, Key.Q},
                {Keys.R, Key.R},
                {Keys.S, Key.S},
                {Keys.T, Key.T},
                {Keys.U, Key.U},
                {Keys.V, Key.V},
                {Keys.W, Key.W},
                {Keys.X, Key.X},
                {Keys.Y, Key.Y},
                {Keys.Z, Key.Z},
                {Keys.LWin, Key.WindowsLeft},
                {Keys.RWin, Key.WindowsRight},
                {Keys.NumPad0, Key.Num0},
                {Keys.NumPad1, Key.Num1},
                {Keys.NumPad2, Key.Num2},
                {Keys.NumPad3, Key.Num3},
                {Keys.NumPad4, Key.Num4},
                {Keys.NumPad5, Key.Num5},
                {Keys.NumPad6, Key.Num6},
                {Keys.NumPad7, Key.Num7},
                {Keys.NumPad8, Key.Num8},
                {Keys.NumPad9, Key.Num9},
                {Keys.Multiply, Key.NumMultiply},
                {Keys.Add, Key.NumAdd},
                {Keys.Separator, Key.NumEnter},
                {Keys.Subtract, Key.NumMinus},
                {Keys.Decimal, Key.NumPeriod},
                {Keys.Divide, Key.NumDivide},
                {Keys.F1, Key.F1},
                {Keys.F2, Key.F2},
                {Keys.F3, Key.F3},
                {Keys.F4, Key.F4},
                {Keys.F5, Key.F5},
                {Keys.F6, Key.F6},
                {Keys.F7, Key.F7},
                {Keys.F8, Key.F8},
                {Keys.F9, Key.F9},
                {Keys.F10, Key.F10},
                {Keys.F11, Key.F11},
                {Keys.F12, Key.F12},
                {Keys.NumLock, Key.NumLock},
                {Keys.Scroll, Key.ScrollLock},
                {Keys.LShiftKey, Key.LeftShift},
                {Keys.RShiftKey, Key.RightShift},
                {Keys.LControlKey, Key.LeftCtrl},
                {Keys.RControlKey, Key.RightCtrl},
                {Keys.LMenu, Key.LeftAlt},
                {Keys.RMenu, Key.RightAlt}
            };

        private static readonly Dictionary<MouseButtons, Key> MouseButtonsMappings =
            new Dictionary<MouseButtons, Key>
            {
                { MouseButtons.Left, Key.MouseLeft },
                { MouseButtons.None, Key.None },
                { MouseButtons.Middle, Key.MouseMiddle },
                { MouseButtons.Right, Key.MouseRight },
                { MouseButtons.XButton1, Key.Mouse4 },
                { MouseButtons.XButton2, Key.Mouse5 }
            };

        public static Key MouseWheel(float delta)
        {
            return delta > 0 ? Key.MouseWheelUp : Key.MouseWheelDown;
        }

        public static Key MouseButton(MouseButtons mouseButton)
        {
            Key result;
            return !MouseButtonsMappings.TryGetValue(mouseButton, out result) ? Key.None : result;
        }

        public static Key KeyPress(Keys key)
        {
            Key result;
            return !KeysMappings.TryGetValue(key, out result) ? Key.None : result;
        }
    }
}