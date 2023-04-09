using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace Program
{
    public class MuteMic
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        static void Main(string[] args)
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using ProcessModule module = Process.GetCurrentProcess().MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                // Check if the hotkey combination is pressed (Control + Alt + Shift + `)
                if (Control.ModifierKeys == (Keys.Control | Keys.Alt | Keys.Shift) && key == Keys.Oemtilde)
                {
                    // The hotkey was pressed
                    MuteMicrophone();

                    // Return a non-zero value to indicate that the hook has handled the message
                    return (IntPtr)1;
                }
            }

            // Call the next hook in the chain
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
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

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetForegroundWindow();

        private const int WM_APPCOMMAND = 0x319;
        private const int APPCOMMAND_MICROPHONE_VOLUME_MUTE = 0x180000;
    }
}
