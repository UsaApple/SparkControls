using System.Windows.Forms;

namespace SparkControls.Win32
{
	/// <summary>
	/// 禁用右键菜单句柄
	/// </summary>
	public class DisableContextMenuHwnd : NativeWindow
	{
		/// <summary>
		/// 处理 Windows 消息。
		/// </summary>
		/// <param name="m">Windows 消息。</param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x7b)
				return;
			base.WndProc(ref m);
		}
	}
}