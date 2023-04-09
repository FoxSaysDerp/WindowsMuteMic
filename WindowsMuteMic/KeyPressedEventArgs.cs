using System;

namespace WindowsMuteMic
{
    public class KeyPressedEventArgs : EventArgs
    {
        public Modifier Modifier { get; set; }
        public ConsoleKey Key { get; set; }
    }

    public enum Modifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}