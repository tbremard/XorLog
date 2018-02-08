using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace Xoru.Controls
{
    public partial class PageReader : UserControl
    {
        public PageReader()
        {
            InitializeComponent();
//            lstPageContent.DrawMode = DrawMode.OwnerDrawFixed;
//            lstPageContent.DrawItem += listBox_DrawItem;
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            ListBox lb = (ListBox)sender;
            g.DrawString(lb.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Red), new PointF(e.Bounds.X, e.Bounds.Y));
            e.DrawFocusRectangle();
        }

        public void SetScrollMaster(long sizeOfFileInBytes)
        {
            int largeChange = (int)sizeOfFileInBytes / 10;
            scrollMaster.Minimum = 0;
            scrollMaster.Maximum = (int)sizeOfFileInBytes + largeChange - 1; // must be set before large change, because if large change is greater than max, then large change is not updated
            scrollMaster.LargeChange = largeChange;
            scrollMaster.SmallChange = Math.Max(1, (int)sizeOfFileInBytes / 50);
        }

        public bool IsMasterScrollAtEnd(long scrollPosition)
        {
            int tempVal = (int) scrollPosition+ scrollMaster.LargeChange - 1;
            bool ret = scrollMaster.Maximum == tempVal;//65 110
            return ret;
        }

        public void ShowLastLine()
        {
            lstPageContent.TopIndex = lstPageContent.Items.Count-1;
            scrollMaster.Value = scrollMaster.Maximum;
        }

        public void ShowFirstLine()
        {
            lstPageContent.TopIndex = 0;
            scrollMaster.Value = scrollMaster.Minimum;
        }

        public void Clear()
        {
            lstPageContent.Items.Clear();
        }

        private void lstPageContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var x in lstPageContent.SelectedItems)
            {
                sb.AppendLine(x.ToString());
            }
            string buffer = sb.ToString();
            if(!string.IsNullOrEmpty(buffer))
                Clipboard.SetText(buffer);
        }
    }
}
