using Xoru.Controls;

namespace XorLog.WinMain
{
    partial class WinMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinMain));
            this.lstSearchResult = new System.Windows.Forms.ListBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.chkAutoScroll = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.split = new System.Windows.Forms.SplitContainer();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblSizeInBytes = new System.Windows.Forms.Label();
            this.lblSizeReadable = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this._reader = new Xoru.Controls.PageReader();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.linkXoru = new System.Windows.Forms.LinkLabel();
            this.tmrSearchRequest = new System.Windows.Forms.Timer(this.components);
            this.btnDeleteFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstSearchResult
            // 
            this.lstSearchResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSearchResult.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSearchResult.FormattingEnabled = true;
            this.lstSearchResult.HorizontalScrollbar = true;
            this.lstSearchResult.ItemHeight = 14;
            this.lstSearchResult.Location = new System.Drawing.Point(3, 37);
            this.lstSearchResult.Name = "lstSearchResult";
            this.lstSearchResult.Size = new System.Drawing.Size(687, 46);
            this.lstSearchResult.TabIndex = 1;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(292, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 26);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.Location = new System.Drawing.Point(387, 4);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(80, 26);
            this.btnEnd.TabIndex = 3;
            this.btnEnd.Text = "End";
            this.btnEnd.UseVisualStyleBackColor = true;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // chkAutoScroll
            // 
            this.chkAutoScroll.AutoSize = true;
            this.chkAutoScroll.Checked = true;
            this.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoScroll.Location = new System.Drawing.Point(614, 6);
            this.chkAutoScroll.Name = "chkAutoScroll";
            this.chkAutoScroll.Size = new System.Drawing.Size(74, 17);
            this.chkAutoScroll.TabIndex = 4;
            this.chkAutoScroll.Text = "AutoScroll";
            this.chkAutoScroll.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(88, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(602, 20);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Filter: ";
            // 
            // split
            // 
            this.split.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.split.Panel1.Controls.Add(this.btnDeleteFile);
            this.split.Panel1.Controls.Add(this.btnReject);
            this.split.Panel1.Controls.Add(this.btnClose);
            this.split.Panel1.Controls.Add(this.lblSizeInBytes);
            this.split.Panel1.Controls.Add(this.lblSizeReadable);
            this.split.Panel1.Controls.Add(this.label3);
            this.split.Panel1.Controls.Add(this.btnClear);
            this.split.Panel1.Controls.Add(this._reader);
            this.split.Panel1.Controls.Add(this.label2);
            this.split.Panel1.Controls.Add(this.txtFilePath);
            this.split.Panel1.Controls.Add(this.btnEnd);
            this.split.Panel1.Controls.Add(this.btnStart);
            this.split.Panel1.Controls.Add(this.chkAutoScroll);
            // 
            // split.Panel2
            // 
            this.split.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.split.Panel2.Controls.Add(this.linkXoru);
            this.split.Panel2.Controls.Add(this.lstSearchResult);
            this.split.Panel2.Controls.Add(this.label1);
            this.split.Panel2.Controls.Add(this.txtSearch);
            this.split.Size = new System.Drawing.Size(693, 462);
            this.split.SplitterDistance = 269;
            this.split.SplitterWidth = 10;
            this.split.TabIndex = 7;
            this.split.TabStop = false;
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(482, 4);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(80, 26);
            this.btnReject.TabIndex = 14;
            this.btnReject.Text = "Rejected text";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(102, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 26);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Close File";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblSizeInBytes
            // 
            this.lblSizeInBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSizeInBytes.Location = new System.Drawing.Point(589, 42);
            this.lblSizeInBytes.Name = "lblSizeInBytes";
            this.lblSizeInBytes.Size = new System.Drawing.Size(100, 13);
            this.lblSizeInBytes.TabIndex = 12;
            this.lblSizeInBytes.Text = "SizeInBytes";
            // 
            // lblSizeReadable
            // 
            this.lblSizeReadable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSizeReadable.Location = new System.Drawing.Point(589, 26);
            this.lblSizeReadable.Name = "lblSizeReadable";
            this.lblSizeReadable.Size = new System.Drawing.Size(100, 13);
            this.lblSizeReadable.TabIndex = 11;
            this.lblSizeReadable.Text = "SizeReadable";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(555, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Size: ";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(197, 4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 26);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // _reader
            // 
            this._reader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._reader.BackColor = System.Drawing.SystemColors.Control;
            this._reader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._reader.Location = new System.Drawing.Point(16, 59);
            this._reader.Name = "_reader";
            this._reader.Size = new System.Drawing.Size(659, 207);
            this._reader.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "File: ";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePath.Location = new System.Drawing.Point(72, 33);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(442, 20);
            this.txtFilePath.TabIndex = 5;
            // 
            // linkXoru
            // 
            this.linkXoru.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkXoru.AutoSize = true;
            this.linkXoru.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkXoru.Location = new System.Drawing.Point(232, 125);
            this.linkXoru.Name = "linkXoru";
            this.linkXoru.Size = new System.Drawing.Size(65, 20);
            this.linkXoru.TabIndex = 7;
            this.linkXoru.TabStop = true;
            this.linkXoru.Text = "Xoru.eu";
            this.linkXoru.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkXoru_LinkClicked);
            // 
            // tmrSearchRequest
            // 
            this.tmrSearchRequest.Enabled = true;
            this.tmrSearchRequest.Interval = 300;
            this.tmrSearchRequest.Tick += new System.EventHandler(this.tmrSearchRequest_Tick);
            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Location = new System.Drawing.Point(7, 4);
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(80, 26);
            this.btnDeleteFile.TabIndex = 15;
            this.btnDeleteFile.Text = "DeleteFile";
            this.btnDeleteFile.UseVisualStyleBackColor = true;
            this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
            // 
            // WinMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 462);
            this.Controls.Add(this.split);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WinMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XorLog @ThieryBrémard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinMain_FormClosing);
            this.Shown += new System.EventHandler(this.WinMain_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropHandler);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterHandler);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.WinMain_DragOver);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel1.PerformLayout();
            this.split.Panel2.ResumeLayout(false);
            this.split.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstSearchResult;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.CheckBox chkAutoScroll;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilePath;
        private PageReader _reader;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblSizeInBytes;
        private System.Windows.Forms.Label lblSizeReadable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer tmrSearchRequest;
        private System.Windows.Forms.LinkLabel linkXoru;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnDeleteFile;
    }
}

