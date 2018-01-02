using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Xoru.Controls
{
    public class XoruListBox : ListBox
    {
        public event EventHandler<ScrollLimitEventArgs> ScrollLimitReached;
        public event EventHandler<ScrollValueEventArgs> ScrollValueChanged;

        [Serializable, StructLayout(LayoutKind.Sequential)]
        struct Scrollinfo
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
        static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref Scrollinfo lpsi, bool fRedraw);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref Scrollinfo lpsi);

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SB_LINELEFT = 0;
        private const int SB_LINERIGHT = 1;
        private const int SB_PAGELEFT = 2;
        private const int SB_PAGERIGHT = 3;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_RIGHT = 7;
        private const int SB_TOP = 6;
        private const int SB_LEFT = 6;
        private const int SB_BOTTOM = 7;
        private const int SB_ENDSCROLL = 8;

        protected virtual void OnScrollLimit(ScrollLimit limit)
        {
            if (ScrollLimitReached!=null)
            {
                var arg = new ScrollLimitEventArgs(limit);
                ScrollLimitReached(this, arg);
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (IsScrolling(m))
            {
                InspectScrollBarAndTriggerEventIfNeeded();
            }
        }

        private bool IsScrolling(Message m)
        {
            const int WM_VSCROLL = 0x0115;
            const int WM_MOUSEWHEEL = 0x020A;
            return m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL;
        }

        private void SetScrollBar()
        {
            int sizeOfFile = 10000000;
            int positionInFile = sizeOfFile / 2;
            Scrollinfo scrollinfo = new Scrollinfo();
            scrollinfo.Size = (uint) Marshal.SizeOf(typeof(Scrollinfo));
            scrollinfo.Mask = (uint) Convert.ToInt32(ScrollInfoMask.SIF_ALL);
            scrollinfo.Min = 0;
            scrollinfo.Max = sizeOfFile; // for example the number of items in the control
            scrollinfo.Position = positionInFile;
            scrollinfo.Page = 1;
            bool redraw = true;
            int scrollBoxPosition = SetScrollInfo(Handle, SB_VERT, ref scrollinfo, redraw);//The return value is the current position of the scroll box. 
        }

        private Scrollinfo? lastScrollinfo;
        private bool InspectScrollBarAndTriggerEventIfNeeded()
        {
            bool ret = false;
            Scrollinfo scrollinfo = new Scrollinfo
            {
                Size = (uint) Marshal.SizeOf(typeof(Scrollinfo)),
                Mask = (uint) Convert.ToInt32(ScrollInfoMask.SIF_ALL)
            };

            bool success = GetScrollInfo(Handle, SB_VERT, ref scrollinfo);
            if (!success)
                return false;

            if (ScrollInfoChanged(scrollinfo))
            {
                OnScrollValueChanged(scrollinfo);
            }

            if (scrollinfo.Position == 0)
            {
                OnScrollLimit(ScrollLimit.FirstLine);
                ret = true;
            }
            if (scrollinfo.Position + scrollinfo.Page - 1 == scrollinfo.Max)
            {
                OnScrollLimit(ScrollLimit.LastLine);
                ret = true;
            }
            lastScrollinfo = scrollinfo;
            return ret;
        }

        private bool ScrollInfoChanged(Scrollinfo scrollinfo)
        {
            if (lastScrollinfo.HasValue == false)
            {
                return true;
            }
            if (scrollinfo.Position != lastScrollinfo.Value.Position)
            {
                return true;
            }
            return false;
        }

        private void OnScrollValueChanged(Scrollinfo scrollinfo)
        {
            if (ScrollValueChanged == null)
                return;
            var value = new ScrollValueEventArgs(scrollinfo.Min, scrollinfo.Max, scrollinfo.Position);
            ScrollValueChanged(this, value);
        }
    }
}