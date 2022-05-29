
namespace SparkControls.Controls
{
    partial class SparkPager
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
			this.label1 = new System.Windows.Forms.Label();
			this.cmbPageSize = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtNum = new SparkControls.Controls.SparkNumberBox();
			this.btnLast = new SparkControls.Controls.SparkButton();
			this.btnDown = new SparkControls.Controls.SparkButton();
			this.btnUp = new SparkControls.Controls.SparkButton();
			this.btnFirst = new SparkControls.Controls.SparkButton();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(3, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(510, 26);
			this.label1.TabIndex = 5;
			this.label1.Text = "共 M 条记录，每页 N 条，共 K 页";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbPageSize
			// 
			this.cmbPageSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmbPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPageSize.FormattingEnabled = true;
			this.cmbPageSize.Items.AddRange(new object[] {
            "500",
            "1000",
            "2000"});
			this.cmbPageSize.Location = new System.Drawing.Point(863, 4);
			this.cmbPageSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cmbPageSize.Name = "cmbPageSize";
			this.cmbPageSize.Size = new System.Drawing.Size(90, 28);
			this.cmbPageSize.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(823, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 20);
			this.label2.TabIndex = 8;
			this.label2.Text = "每页";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(956, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(23, 20);
			this.label3.TabIndex = 9;
			this.label3.Text = "条";
			// 
			// txtNum
			// 
			this.txtNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtNum.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.txtNum.IsDecimal = true;
			this.txtNum.Location = new System.Drawing.Point(617, 5);
			this.txtNum.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtNum.Name = "txtNum";
			this.txtNum.Size = new System.Drawing.Size(98, 26);
			this.txtNum.TabIndex = 10;
			this.txtNum.Text = "0";
			this.txtNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnLast
			// 
			this.btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLast.BackColor = System.Drawing.SystemColors.Control;
			this.btnLast.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnLast.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnLast.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLast.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnLast.Location = new System.Drawing.Point(771, 5);
			this.btnLast.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnLast.Name = "btnLast";
			this.btnLast.Size = new System.Drawing.Size(42, 26);
			this.btnLast.TabIndex = 3;
			this.btnLast.UseVisualStyleBackColor = false;
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.BackColor = System.Drawing.SystemColors.Control;
			this.btnDown.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnDown.Location = new System.Drawing.Point(722, 5);
			this.btnDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(42, 26);
			this.btnDown.TabIndex = 2;
			this.btnDown.UseVisualStyleBackColor = false;
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.BackColor = System.Drawing.SystemColors.Control;
			this.btnUp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnUp.Location = new System.Drawing.Point(568, 5);
			this.btnUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(42, 26);
			this.btnUp.TabIndex = 1;
			this.btnUp.UseVisualStyleBackColor = false;
			// 
			// btnFirst
			// 
			this.btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFirst.BackColor = System.Drawing.SystemColors.Control;
			this.btnFirst.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnFirst.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnFirst.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
			this.btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnFirst.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnFirst.Location = new System.Drawing.Point(519, 5);
			this.btnFirst.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnFirst.Name = "btnFirst";
			this.btnFirst.Size = new System.Drawing.Size(42, 26);
			this.btnFirst.TabIndex = 0;
			this.btnFirst.UseVisualStyleBackColor = false;
			// 
			// SparkPager
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.txtNum);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cmbPageSize);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnLast);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.btnFirst);
			this.Font = new System.Drawing.Font("微软雅黑", 10.5F);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimumSize = new System.Drawing.Size(0, 36);
			this.Name = "SparkPager";
			this.Size = new System.Drawing.Size(986, 36);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private SparkButton btnFirst;
        private SparkButton btnUp;
        private SparkButton btnDown;
        private SparkButton btnLast;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPageSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private SparkNumberBox txtNum;
    }
}
