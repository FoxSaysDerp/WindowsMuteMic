using System;
using System.Windows.Forms;
using WindowsMuteMic.Hooks;

namespace WindowsMuteMic
{
    public class TrayIconContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly KeyboardHook _keyboardHook;

        public TrayIconContext()
        {
            _trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.AppIcon,
                Text = "MuteMic",
                Visible = true
            };
            _trayIcon.MouseClick += TrayIcon_Click;

            _keyboardHook = new KeyboardHook();
            _keyboardHook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift, Keys.Oemtilde);
            _keyboardHook.HotKeyPressed += KeyboardHook_HotKeyPressed;
        }

        private void TrayIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Create a context menu with the "Exit" option
                var contextMenu = new ContextMenuStrip();
                var exitMenuItem = new ToolStripMenuItem("Exit");
                exitMenuItem.Click += ExitMenuItem_Click;
                contextMenu.Items.Add(exitMenuItem);

                // Show the context menu at the mouse position
                contextMenu.Show(Control.MousePosition);
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _keyboardHook.Dispose();
            Application.Exit();
        }

        private void KeyboardHook_HotKeyPressed(object sender, EventArgs e)
        {
            // Mute the microphone when the hotkey is pressed
            MuteMic.MuteMicrophone();
        }
    }
}