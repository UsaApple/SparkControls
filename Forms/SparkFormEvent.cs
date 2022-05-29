using System;
using System.Windows.Forms;

namespace SparkControls.Forms
{
	/// <summary>
	/// 窗口状态改变事件
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void FormWindowStateChangedEventHandler(object sender, FormWindowStateChangedEventArgs e);

	/// <summary>
	/// <see cref="FormWindowStateChangedEventHandler"/>事件的参数。
	/// </summary>
	public class FormWindowStateChangedEventArgs : EventArgs
	{
		/// <summary>
		/// 窗口改变后的状态
		/// </summary>
		public FormWindowState FormWindowState { get; }

		/// <summary>
		/// 初始 <see cref="FormWindowStateChangedEventArgs"/> 类型的新实例。
		/// </summary>
		/// <param name="formWindowState">改变后的状态</param>
		public FormWindowStateChangedEventArgs(FormWindowState formWindowState)
		{
			FormWindowState = formWindowState;
		}
	}

	/// <summary>
	/// 系统的 ProcessCmdKey 事件
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="keyData"></param>
	/// <returns></returns>
	public delegate bool ProcessCmdKeyEventHandler(ref Message msg, Keys keyData);
}