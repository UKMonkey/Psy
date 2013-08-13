using System.Windows.Forms;

namespace Psy.Core.Input
{
    public class KeyPressEventArguments
    {
        public KeyPressEventArguments(KeyPressEventArgs keyPressEventArgs)
        {
            Handled = keyPressEventArgs.Handled;
            KeyChar = keyPressEventArgs.KeyChar;
        }

        public bool Handled { get; set; }
        public char KeyChar { get; set; }
    }
}