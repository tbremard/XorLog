using System.Windows.Forms;
namespace Xoru.Controls
{
    partial class PageReader
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scrollMaster = new System.Windows.Forms.VScrollBar();
            this.lstPageContent = new Xoru.Controls.XoruListBox();
            this.SuspendLayout();
            // 
            // scrollMaster
            // 
            this.scrollMaster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollMaster.Location = new System.Drawing.Point(540, 0);
            this.scrollMaster.Maximum = 109;
            this.scrollMaster.Name = "scrollMaster";
            this.scrollMaster.Size = new System.Drawing.Size(26, 188);
            this.scrollMaster.TabIndex = 9;
            // 
            // lstPageContent
            // 
            this.lstPageContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPageContent.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstPageContent.FormattingEnabled = true;
            this.lstPageContent.HorizontalScrollbar = true;
            this.lstPageContent.ItemHeight = 14;
            this.lstPageContent.Location = new System.Drawing.Point(0, 0);
            this.lstPageContent.Name = "lstPageContent";
            this.lstPageContent.ScrollAlwaysVisible = true;
            this.lstPageContent.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPageContent.Size = new System.Drawing.Size(537, 186);
            this.lstPageContent.TabIndex = 10;
            // 
            // PageReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstPageContent);
            this.Controls.Add(this.scrollMaster);
            this.Name = "PageReader";
            this.Size = new System.Drawing.Size(566, 188);
            this.ResumeLayout(false);

        }

        #endregion

        public VScrollBar scrollMaster;
        public XoruListBox lstPageContent;
    }
}
