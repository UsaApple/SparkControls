namespace SparkControls.Controls
{
	partial class SparkDataGridComboBoxListControl
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SparkDataGridComboBoxListControl));
			this.mDgvList = new SparkControls.Controls.SparkDataGridView();
			this.mPnlContainer = new SparkControls.Controls.SparkPanel();
			this.mBtnOk = new SparkControls.Controls.SparkButton();
			this.mBtnCancel = new SparkControls.Controls.SparkButton();
			this.mTxtFilter = new SparkControls.Controls.SparkTextBox();
			((System.ComponentModel.ISupportInitialize)(this.mDgvList)).BeginInit();
			this.mPnlContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// mDgvList
			// 
			this.mDgvList.AllowUserToAddRows = false;
			this.mDgvList.AllowUserToDeleteRows = false;
			this.mDgvList.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
			this.mDgvList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 10.5F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.mDgvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.mDgvList.ColumnHeadersHeight = 28;
			this.mDgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("微软雅黑", 10.5F);
			dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(194)))), ((int)(((byte)(172)))));
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.mDgvList.DefaultCellStyle = dataGridViewCellStyle3;
			this.mDgvList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mDgvList.EnableHeadersVisualStyles = false;
			this.mDgvList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(214)))), ((int)(((byte)(214)))));
			this.mDgvList.Location = new System.Drawing.Point(0, 28);
			this.mDgvList.Name = "mDgvList";
			this.mDgvList.RowHeadersWidth = 36;
			this.mDgvList.RowTemplate.Height = 23;
			this.mDgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.mDgvList.Size = new System.Drawing.Size(540, 332);
			this.mDgvList.TabIndex = 3;
			this.mDgvList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvList_CellClick);
			this.mDgvList.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DgvList_ColumnAdded);
			this.mDgvList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvList_KeyDown);
			// 
			// mPnlContainer
			// 
			this.mPnlContainer.BackColor = System.Drawing.SystemColors.Control;
			this.mPnlContainer.Controls.Add(this.mBtnOk);
			this.mPnlContainer.Controls.Add(this.mBtnCancel);
			this.mPnlContainer.Controls.Add(this.mTxtFilter);
			this.mPnlContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.mPnlContainer.ForeColor = System.Drawing.SystemColors.ControlText;
			this.mPnlContainer.Location = new System.Drawing.Point(0, 0);
			this.mPnlContainer.Name = "mPnlContainer";
			this.mPnlContainer.Size = new System.Drawing.Size(540, 28);
			this.mPnlContainer.TabIndex = 2;
			// 
			// mBtnOk
			// 
			this.mBtnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mBtnOk.BackColor = System.Drawing.SystemColors.Control;
			this.mBtnOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.mBtnOk.ItemSize = new System.Drawing.Size(0, 0);
			this.mBtnOk.Location = new System.Drawing.Point(412, 0);
			this.mBtnOk.Name = "mBtnOk";
			this.mBtnOk.Size = new System.Drawing.Size(64, 28);
			this.mBtnOk.TabIndex = 2;
			this.mBtnOk.Text = "确 定";
			this.mBtnOk.UseVisualStyleBackColor = true;
			this.mBtnOk.Click += new System.EventHandler(this.BtnOk_Click);
			// 
			// mBtnCancel
			// 
			this.mBtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mBtnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.mBtnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.mBtnCancel.ItemSize = new System.Drawing.Size(0, 0);
			this.mBtnCancel.Location = new System.Drawing.Point(476, 0);
			this.mBtnCancel.Name = "mBtnCancel";
			this.mBtnCancel.Size = new System.Drawing.Size(64, 28);
			this.mBtnCancel.TabIndex = 1;
			this.mBtnCancel.Text = "取消";
			this.mBtnCancel.UseVisualStyleBackColor = true;
			this.mBtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			// 
			// mTxtFilter
			// 
			this.mTxtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mTxtFilter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.mTxtFilter.Location = new System.Drawing.Point(0, 1);
			this.mTxtFilter.MatchPattern = null;
			this.mTxtFilter.Name = "mTxtFilter";
			this.mTxtFilter.Size = new System.Drawing.Size(412, 26);
			this.mTxtFilter.TabIndex = 0;
			this.mTxtFilter.WordWrap = false;
			
			// 
			// SparkDataGridComboBoxListControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mDgvList);
			this.Controls.Add(this.mPnlContainer);
			this.Name = "SparkDataGridComboBoxListControl";
			this.Size = new System.Drawing.Size(540, 360);
			((System.ComponentModel.ISupportInitialize)(this.mDgvList)).EndInit();
			this.mPnlContainer.ResumeLayout(false);
			this.mPnlContainer.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		internal SparkPanel mPnlContainer;
		internal SparkButton mBtnOk;
		internal SparkButton mBtnCancel;
		internal SparkTextBox mTxtFilter;
		internal SparkDataGridView mDgvList;
        
    }
}
