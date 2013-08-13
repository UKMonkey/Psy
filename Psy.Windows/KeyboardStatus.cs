using System.Collections.Generic;
using System.Windows.Forms;

namespace Psy.Windows
{
    public class KeyboardStatus
    {
        private readonly HashSet<Keys> _pressedStatus;

        public bool this[Keys key]
        {
            get { return _pressedStatus.Contains(key); }
        }

        public KeyboardStatus()
        {
            _pressedStatus = new HashSet<Keys>();
        }

        public void SetPressedFlag(Keys key, bool isPressed)
        {
            if (isPressed)
            {
                _pressedStatus.Add(key);
            }
            else
            {
                _pressedStatus.Remove(key);
            }
        }
    }
}
