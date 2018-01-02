using System;
using System.Windows.Forms;
using Xoru.Controls;

namespace XoruListBox.Demo
{
    public partial class WinformDemo : Form
    {
        public WinformDemo()
        {
            InitializeComponent();
            tmr.Start();
        }

        private int top = -1;
        private int last = 0;
        private void WinformDemo_Load(object sender, EventArgs e)
        {
            GoAfter();

            listBox.ScrollLimitReached += ListBoxScrollLimitReached;
            listBox.ScrollValueChanged += listBox_ScrollValueChanged;
        }

        void listBox_ScrollValueChanged(object sender, ScrollValueEventArgs e)
        {
            ShowScrollPosition(e.Position);
        }

        private void ShowScrollPosition(int ePosition)
        {
            lblScrollPosition.Text = ePosition.ToString();
            lblScrollPosition.Show();
            lblScrollPosition.Tag = DateTime.Now;
            
        }

        void ListBoxScrollLimitReached(object sender, ScrollLimitEventArgs e)
        {
            ShowLimit(e.Limit);
            switch (e.Limit)
            {
                case ScrollLimit.FirstLine:
                    GoBefore();
                    break;
                case ScrollLimit.LastLine:
                    GoAfter();
                    break;
            }
        }

        private void ShowLimit(ScrollLimit eLimit)
        {
            lblLimit.Text = eLimit.ToString();
            lblLimit.Show();
            lblLimit.Tag = DateTime.Now;
        }

        private void GoBefore()
        {
            for (int i = 0; i < 10; i++)
            {
                listBox.Items.Insert(0, top--);
            }
        }

        private void GoAfter()
        {
            for (int i = 0; i < 10; i++)
            {
                listBox.Items.Add(last++);
            }
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            object o = lblLimit.Tag;
            if (o == null)
                return;
            DateTime time = (DateTime)o;
            TimeSpan elapsed = DateTime.Now - time;
            if(elapsed.TotalSeconds>1)
                lblLimit.Hide();



            o = lblScrollPosition.Tag;
            if (o == null)
                return;
             time = (DateTime)o;
             elapsed = DateTime.Now - time;
            if (elapsed.TotalSeconds > 1)
                lblScrollPosition.Hide();


        }

    }
}
