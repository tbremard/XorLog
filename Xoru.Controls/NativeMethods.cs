using System;
using System.Runtime.InteropServices;

namespace Xoru.Controls
{
    static class NativeMethods
    {
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Scrollinfo
        {
            public uint Size;
            public uint Mask;
            public int Min;
            public int Max;
            public uint Page;
            public int Position;
            public int TrackPosition;
        }

        [DllImport("user32.dll")]
        public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref Scrollinfo lpsi, bool fRedraw);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref Scrollinfo lpsi);
    }
}