using System;
using System.Windows.Forms;

namespace Xoru.Controls
{
    public partial class PageReader : UserControl
    {
        public PageReader()
        {
            InitializeComponent();
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
    }
}
