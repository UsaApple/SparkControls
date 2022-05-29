namespace SparkControls.Forms
{
    partial class SparkFormMemo
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
			this.txtInputBox = new SparkControls.Controls.SparkTextBox();
			this.lblPrompt = new SparkControls.Controls.SparkLabel();
			this.btnOk = new SparkControls.Controls.SparkButton();
			this.btnCancel = new SparkControls.Controls.SparkButton();
			this.SuspendLayout();
			// 
			// txtInputBox
			// 
			this.txtInputBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.txtInputBox.Location = new System.Drawing.Point(13, 63);
			this.txtInputBox.MatchPattern = null;
			this.txtInputBox.Multiline = true;
			this.txtInputBox.Name = "txtInputBox";
			this.txtInputBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtInputBox.Size = new System.Drawing.Size(574, 300);
			this.txtInputBox.TabIndex = 0;
			// 
			// lblPrompt
			// 
			this.lblPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPrompt.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblPrompt.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblPrompt.Location = new System.Drawing.Point(13, 38);
			this.lblPrompt.Name = "lblPrompt";
			this.lblPrompt.Size = new System.Drawing.Size(574, 23);
			this.lblPrompt.TabIndex = 1;
			this.lblPrompt.Text = "请在文本框中输入文本内容：";
			// 
			// btnOk
			// 
			this.btnOk.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.BackColor = System.Drawing.SystemColors.Control;
			this.btnOk.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOk.ItemSize = new System.Drawing.Size(0, 0);
			this.btnOk.Location = new System.Drawing.Point(401, 376);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(90, 30);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "确认(&S)";
			this.btnOk.Theme.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(182)))), ((int)(((byte)(161)))));
			this.btnOk.Theme.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(182)))), ((int)(((byte)(161)))));
			this.btnOk.Theme.ForeColor = System.Drawing.Color.White;
			this.btnOk.Theme.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(136)))), ((int)(((byte)(117)))));
			this.btnOk.Theme.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(136)))), ((int)(((byte)(117)))));
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.ItemSize = new System.Drawing.Size(0, 0);
			this.btnCancel.Location = new System.Drawing.Point(497, 376);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 30);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "取消(&C)";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// SparkFormMemo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 420);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblPrompt);
			this.Controls.Add(this.txtInputBox);
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SparkFormMemo";
			this.ShowInTaskbar = false;
			this.Text = "文本编辑器";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private Controls.SparkTextBox txtInputBox;
        private Controls.SparkLabel lblPrompt;
        private Controls.SparkButton btnOk;
        private Controls.SparkButton btnCancel;
    }
}