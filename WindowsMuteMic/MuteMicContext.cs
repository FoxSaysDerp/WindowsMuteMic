using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.Control;
using WindowsMuteMic.Hooks;

namespace WindowsMuteMic
{
    public class MutedEventArgs : EventArgs
    {
        public bool IsMuted { get; set; }
    }

    public class MuteMicContext : ApplicationContext
    {
        private readonly KeyboardHook _hook;
        private readonly NotifyIcon _trayIcon;

        private bool _isMuted;

        public event EventHandler<MutedEventArgs> MicMutedStateChanged;

        public MuteMicContext()
        {
            _isMuted = false;

            _hook = new KeyboardHook();
            _hook.KeyPressed += Hook_KeyPressed;

            _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            _trayIcon.ContextMenuStrip.Opening += TrayIcon_Opening;

            UpdateContextMenu();
        }

        private void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Modifier == Modifier.Control && e.Key == Keys.M)
            {
                MuteMicrophone();
            }
        }

        private void TrayIcon_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateContextMenu();
        }

        private void MuteMicrophone()
        {
            _isMuted = !_isMuted;

            MicMutedStateChanged?.Invoke(this, new MutedEventArgs { IsMuted = _isMuted });

            UpdateContextMenu();
        }

        private void UpdateContextMenu()
        {
            _trayIcon.ContextMenuStrip.Items.Clear();

            if (_isMuted)
            {
                _trayIcon.ContextMenuStrip.Items.Add("Unmute Microphone", null, OnContextMenuClick);
            }
            else
            {
                _trayIcon.ContextMenuStrip.Items.Add("Mute Microphone", null, OnContextMenuClick);
            }

            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitClick);
        }

        private void OnContextMenuClick(object sender, EventArgs e)
        {
            MuteMicrophone();
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            _hook.Dispose();
            _trayIcon.Visible = false;

            Application.Exit();
        }
    }
}
