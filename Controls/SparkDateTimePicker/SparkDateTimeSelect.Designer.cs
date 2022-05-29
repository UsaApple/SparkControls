namespace SparkControls.Controls
{
    partial class SparkDateTimeSelect
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.pnlMain = new System.Windows.Forms.Panel();
			this.pnlMiddle = new System.Windows.Forms.Panel();
			this.dtPnl = new SparkControls.Controls.DateTimePanel();
			this.lblRight = new System.Windows.Forms.Label();
			this.lblLeft = new System.Windows.Forms.Label();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.btnOK = new SparkControls.Controls.SparkButton();
			this.btnCancel = new SparkControls.Controls.SparkButton();
			this.lblToday = new System.Windows.Forms.Label();
			this.dtItemPnl = new SparkControls.Controls.DateTimeItemPanel();
			this.pnlMain.SuspendLayout();
			this.pnlMiddle.SuspendLayout();
			this.dtPnl.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMain
			// 
			this.pnlMain.BackColor = System.Drawing.Color.White;
			this.pnlMain.Controls.Add(this.pnlMiddle);
			this.pnlMain.Controls.Add(this.pnlBottom);
			this.pnlMain.Controls.Add(this.dtItemPnl);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(1, 1);
			this.pnlMain.Margin = new System.Windows.Forms.Padding(2);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
			this.pnlMain.Size = new System.Drawing.Size(342, 306);
			this.pnlMain.TabIndex = 6;
			// 
			// pnlMiddle
			// 
			this.pnlMiddle.Controls.Add(this.dtPnl);
			this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMiddle.Location = new System.Drawing.Point(4, 40);
			this.pnlMiddle.Name = "pnlMiddle";
			this.pnlMiddle.Size = new System.Drawing.Size(334, 230);
			this.pnlMiddle.TabIndex = 4;
			// 
			// dtPnl
			// 
			this.dtPnl.CellEleType = SparkControls.Controls.ElementType.None;
			this.dtPnl.Column = 0;
			this.dtPnl.Controls.Add(this.lblRight);
			this.dtPnl.Controls.Add(this.lblLeft);
			this.dtPnl.DataSource = null;
			this.dtPnl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dtPnl.Location = new System.Drawing.Point(0, 0);
			this.dtPnl.Name = "dtPnl";
			this.dtPnl.Row = 0;
			this.dtPnl.Size = new System.Drawing.Size(334, 230);
			this.dtPnl.TabIndex = 6;
			this.dtPnl.Theme = null;
			// 
			// lblRight
			// 
			this.lblRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblRight.Image = global::SparkControls.Properties.Resources.Right;
			this.lblRight.Location = new System.Drawing.Point(314, 103);
			this.lblRight.Name = "lblRight";
			this.lblRight.Size = new System.Drawing.Size(20, 24);
			this.lblRight.TabIndex = 1;
			// 
			// lblLeft
			// 
			this.lblLeft.Image = global::SparkControls.Properties.Resources.Left;
			this.lblLeft.Location = new System.Drawing.Point(0, 103);
			this.lblLeft.Name = "lblLeft";
			this.lblLeft.Size = new System.Drawing.Size(20, 24);
			this.lblLeft.TabIndex = 0;
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.btnOK);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Controls.Add(this.lblToday);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(4, 270);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(334, 36);
			this.pnlBottom.TabIndex = 2;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(197, 4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 28);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "确  定";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(267, 4);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 28);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "取  消";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// lblToday
			// 
			this.lblToday.AutoSize = true;
			this.lblToday.Font = new System.Drawing.Font("微软雅黑", 10.5F);
			this.lblToday.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblToday.Location = new System.Drawing.Point(1, 8);
			this.lblToday.Margin = new System.Windows.Forms.Padding(0);
			this.lblToday.Name = "lblToday";
			this.lblToday.Size = new System.Drawing.Size(51, 20);
			this.lblToday.TabIndex = 3;
			this.lblToday.Text = "今天：";
			// 
			// dtItemPnl
			// 
			this.dtItemPnl.Dock = System.Windows.Forms.DockStyle.Top;
			this.dtItemPnl.Location = new System.Drawing.Point(4, 4);
			this.dtItemPnl.Name = "dtItemPnl";
			this.dtItemPnl.Padding = new System.Windows.Forms.Padding(8);
			this.dtItemPnl.Size = new System.Drawing.Size(334, 36);
			this.dtItemPnl.TabIndex = 0;
			this.dtItemPnl.Theme = null;
			this.dtItemPnl.Value = new System.DateTime(((long)(0)));
			// 
			// SparkDateTimeSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.pnlMain);
			this.Name = "SparkDateTimeSelect";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.Size = new System.Drawing.Size(344, 308);
			this.Load += new System.EventHandler(this.DateTimeSelect_Load);
			this.pnlMain.ResumeLayout(false);
			this.pnlMiddle.ResumeLayout(false);
			this.dtPnl.ResumeLayout(false);
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblToday;
        private DateTimeItemPanel dtItemPnl;
        private DateTimePanel dtPnl;
        private SparkButton btnCancel;
        private SparkButton btnOK;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.Label lblLeft;
    }
}
