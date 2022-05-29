using SparkControls.Controls;
using System;
using System.Windows.Forms;

namespace SparkControls.Forms
{
    /// <summary>
    /// 定义公共文本编辑器
    /// </summary>
    public partial class SparkFormMemo : SparkFormBase
    {
        /// <summary>
        /// 获取或设置窗口标题
        /// </summary>
        public string Title
		{
            get => this.Text;
            set => this.Text = value;
		}

        /// <summary>
        /// 获取或设置提示信息
        /// </summary>
        public string PromptText
        {
            get => this.lblPrompt.Text;
            set => this.lblPrompt.Text = value;
        }

        /// <summary>
        /// 获取或设置文本内容
        /// </summary>
        public string TextContent
		{
			get => this.txtInputBox.Text;
			set => this.txtInputBox.Text = value;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public SparkFormMemo()
        {
            InitializeComponent();
        }

        //“确认”按钮单击事件处理器
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtInputBox.Text.Trim()))
            {
                SparkMessageBox.ShowInfoMessage("请输入文本内容！");
                return;
            }

            this.TextContent = this.txtInputBox.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        //“取消”按钮单击事件处理器
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}