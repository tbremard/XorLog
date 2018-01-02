namespace XoruListBox.Demo
{
    partial class WinformDemo
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblLimit = new System.Windows.Forms.Label();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.lblScrollPosition = new System.Windows.Forms.Label();
            this.listBox = new Xoru.Controls.XoruListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "Demo Of Xoru ListBox. Events are sent when top and end are reached in vertical sc" +
    "roll. There is infinite scrolling";
            // 
            // lblLimit
            // 
            this.lblLimit.AutoSize = true;
            this.lblLimit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblLimit.Location = new System.Drawing.Point(258, 68);
            this.lblLimit.Name = "lblLimit";
            this.lblLimit.Size = new System.Drawing.Size(35, 13);
            this.lblLimit.TabIndex = 2;
            this.lblLimit.Text = "label2";
            this.lblLimit.Visible = false;
            // 
            // tmr
            // 
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // lblScrollPosition
            // 
            this.lblScrollPosition.AutoSize = true;
            this.lblScrollPosition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblScrollPosition.Location = new System.Drawing.Point(258, 118);
            this.lblScrollPosition.Name = "lblScrollPosition";
            this.lblScrollPosition.Size = new System.Drawing.Size(35, 13);
            this.lblScrollPosition.TabIndex = 3;
            this.lblScrollPosition.Text = "label2";
            this.lblScrollPosition.Visible = false;
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(15, 54);
            this.listBox.Name = "listBox";
            this.listBox.ScrollAlwaysVisible = true;
            this.listBox.Size = new System.Drawing.Size(220, 121);
            this.listBox.TabIndex = 0;
            // 
            // WinformDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 193);
            this.Controls.Add(this.lblScrollPosition);
            this.Controls.Add(this.lblLimit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WinformDemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XoruListBox demo";
            this.Load += new System.EventHandler(this.WinformDemo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Xoru.Controls.XoruListBox listBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLimit;
        private System.Windows.Forms.Timer tmr;
        private System.Windows.Forms.Label lblScrollPosition;
    }
}

