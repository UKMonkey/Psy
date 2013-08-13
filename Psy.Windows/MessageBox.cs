using System.Windows.Forms;

namespace Psy.Windows
{
// ReSharper disable UnusedMember.Global
    public static class MessageBox
    {
        // ReSharper disable UnusedMember.Local
        public static void Error(string errorMessage, string title = "Error")
        {
            System.Windows.Forms.MessageBox.Show(
                errorMessage,
                title, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
        }

        public static void Warning(string warningMessage)
        {
            System.Windows.Forms.MessageBox.Show(warningMessage, 
                "Warning", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);
        }
    }
}
// ReSharper restore UnusedMember.Local