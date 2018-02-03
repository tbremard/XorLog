using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XorLog.WinMain
{
    public partial class Rejection : Form
    {
        public Rejection()
        {
            InitializeComponent();
        }

        public void SetInitialList(IList<string> rejectList)
        {
            foreach (string word in rejectList)
            {
                lstRejection.Items.Add(word);                
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SaveList();
            Close();
        }

        public IList<string> RejectionList { get; private set; }
        private void SaveList()
        {
            RejectionList = new List<string>();
            foreach (ListViewItem item in lstRejection.Items)
            {
                RejectionList.Add(item.Text);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddWordHandler();
        }

        private void AddWordHandler()
        {
            string word = txtWord.Text;
            if (string.IsNullOrEmpty(word))
                return;
            lstRejection.Items.Add(word);
            txtWord.Clear();
        }

        private void txtWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddWordHandler();                
            }
        }

    }
}
