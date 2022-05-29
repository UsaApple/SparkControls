using System.Windows.Forms;

namespace SparkControls.Forms
{
	/// <summary>
	/// 弹出窗口的基类。
	/// </summary>
	public partial class SparkPopupForm : SparkFormBase
	{
		/// <summary>
		/// 初始 <see cref="SparkPopupForm"/> 类型的基类。
		/// </summary>
		public SparkPopupForm()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// 重写 ProcessCmdKey 事件。
		/// </summary>
		/// <param name="msg">Windows 消息。</param>
		/// <param name="keyData">按键枚举。</param>
		/// <returns>是否完成处理。</returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			int WM_KEYDOWN = 256;
			int WM_SYSKEYDOWN = 260;
			
			if (msg.Msg == WM_KEYDOWN | msg.Msg == WM_SYSKEYDOWN)
			{
				switch (keyData)
				{
					case Keys.Escape:
						this.Close(); //Esc 关闭窗体
						break;
				}
			}
			return false;
		}
	}
}