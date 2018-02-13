using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Xoru.Controls
{
    public partial class ViewListEditor : Form
    {
        public IList<string> ListOfWords { get; private set; }

        public ViewListEditor()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            Text = title;
        }

        public void SetInitialList(IList<string> initialList)
        {
            lstDisplayed.Items.Clear();
            foreach (string word in initialList)
            {
                lstDisplayed.Items.Add(word);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SaveList();
            Close();
        }

        private void SaveList()
        {
            ListOfWords = new List<string>();
            foreach (ListViewItem item in lstDisplayed.Items)
            {
                ListOfWords.Add(item.Text);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddWordToList();
        }

        private void AddWordToList()
        {
            string word = txtWord.Text;
            if (string.IsNullOrEmpty(word))
                return;
            lstDisplayed.Items.Add(word);
            txtWord.Clear();
        }

        private void txtWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddWordToList();                
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = lstDisplayed.SelectedItems;
            foreach (ListViewItem item in selected)
            {
                lstDisplayed.Items.Remove(item);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstDisplayed.Items.Clear();
        }

    }
}
