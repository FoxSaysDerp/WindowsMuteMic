using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Program
{
    class Program
    {
        private const int WM_APPCOMMAND = 0x319;
        private const int APPCOMMAND_MICROPHONE_VOLUME_MUTE = 0x180000;

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        private static int hotkeyId = 0;

        static void Main(string[] args)
        {
            // Register the hotkey with the Control, Alt, and Shift modifiers and the F12 key
            bool result = RegisterHotKey(IntPtr.Zero, hotkeyId, (uint)(Keys.Control | Keys.Alt | Keys.Shift), (uint)Keys.F12);

            if (!result)
            {
                uint error = GetLastError();
                MessageBox.Show("Failed to register hotkey. Error code: " + error.ToString());
                return;
            }

            // Listen for hotkey events
            Application.AddMessageFilter(new HotkeyMessageFilter());

            Application.Run(new ApplicationContext());
        }

        private static void MuteMicrophone()
        {
            try
            {
                // Get the handle of the active window and send the mute command
                IntPtr hWnd = GetForegroundWindow();
                SendMessageW(hWnd, WM_APPCOMMAND, IntPtr.Zero, (IntPtr)APPCOMMAND_MICROPHONE_VOLUME_MUTE);
            }
            catch (Exception)
            {
            }
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private class HotkeyMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != 0x0312 || m.WParam.ToInt32() != hotkeyId) return false;
                // The hotkey was pressed
                MuteMicrophone();

                // Return true to indicate that the message has been handled
                return true;

                // Return false to indicate that the message has not been handled
            }
        }
    }
}
