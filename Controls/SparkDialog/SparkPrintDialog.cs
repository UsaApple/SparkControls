using System;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public sealed class SparkPrintDialog
	{
		#region 字段

		private readonly PrintDialog _dialog = null;

		#endregion

		#region 属性

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否显示“当前页”选项按钮。
		//
		// 返回结果:
		//     如果显示“当前页”选项按钮，为 true；否则为 false。默认为 false。
		public bool AllowCurrentPage
		{
			set { this._dialog.AllowCurrentPage = value; }
			get { return this._dialog.AllowCurrentPage; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否启用“页”选项按钮。
		//
		// 返回结果:
		//     如果启用“页”选项按钮，为 true；否则为 false。默认为 false。
		public bool AllowSomePages
		{
			set { this._dialog.AllowSomePages = value; }
			get { return this._dialog.AllowSomePages; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否启用“打印到文件”复选框。
		//
		// 返回结果:
		//     如果启用“打印到文件”复选框，为 true；否则为 false。默认为 true。
		public bool AllowPrintToFile
		{
			set { this._dialog.AllowPrintToFile = value; }
			get { return this._dialog.AllowPrintToFile; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否启用“选择”选项按钮。
		//
		// 返回结果:
		//     如果启用“选择”选项按钮，为 true；否则为 false。默认为 false。
		public bool AllowSelection
		{
			set { this._dialog.AllowSelection = value; }
			get { return this._dialog.AllowSelection; }
		}

		//
		// 摘要:
		//     获取或设置一个值，指示用于获取 System.Drawing.Printing.PrinterSettings 的 System.Drawing.Printing.PrintDocument。
		//
		// 返回结果:
		//     用于获取 System.Drawing.Printing.PrinterSettings 的 System.Drawing.Printing.PrintDocument。默认为
		//     null。
		public PrintDocument Document
		{
			set { this._dialog.Document = value; }
			get { return this._dialog.Document; }
		}

		//
		// 摘要:
		//     获取或设置对话框修改的打印机设置。
		//
		// 返回结果:
		//     对话框修改的 System.Drawing.Printing.PrinterSettings。
		public PrinterSettings PrinterSettings
		{
			set { this._dialog.PrinterSettings = value; }
			get { return this._dialog.PrinterSettings; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否选中“打印到文件”复选框。
		//
		// 返回结果:
		//     如果选中“打印到文件”复选框，为 true；否则为 false。默认为 false。
		public bool PrintToFile
		{
			set { this._dialog.PrintToFile = value; }
			get { return this._dialog.PrintToFile; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否显示“帮助”按钮。
		//
		// 返回结果:
		//     如果“帮助”按钮显示，则为 true；否则为 false。默认为 false。
		public bool ShowHelp
		{
			set { this._dialog.ShowHelp = value; }
			get { return this._dialog.ShowHelp; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示是否显示“网络”按钮。
		//
		// 返回结果:
		//     如果显示“网络”按钮，为 true；否则为 false。默认为 true。
		public bool ShowNetwork
		{
			set { this._dialog.ShowNetwork = value; }
			get { return this._dialog.ShowNetwork; }
		}

		//
		// 摘要:
		//     获取或设置一个值，该值指示在运行 Windows XP Home Edition, Windows XP Professional, Windows Server
		//     2003 或更高版本的系统上，此对话框是否应当以 Windows XP 样式显示。
		//
		// 返回结果:
		//     要指示该对话框应当以 Windows XP 样式显示，则为 true；否则为 false。默认为 true。
		public bool UseEXDialog
		{
			set { this._dialog.UseEXDialog = value; }
			get { return this._dialog.UseEXDialog; }
		}

		//
		// 摘要:
		//     获取或设置一个对象，该对象包含控件的数据。
		//
		// 返回结果:
		//     包含有关 System.Windows.Forms.CommonDialog 的数据的对象。
		public object Tag
		{
			set { this._dialog.Tag = value; }
			get { return this._dialog.Tag; }
		}

		#endregion

		#region 自定义事件

		//
		// 摘要:
		//     当用户单击通用对话框中的“帮助”按钮时发生。
		public event EventHandler HelpRequest = null;

		#endregion

		public SparkPrintDialog()
		{
			this._dialog = new PrintDialog();
			this._dialog.HelpRequest += (s, e) => { HelpRequest?.Invoke(s, e); };
		}

		#region 对外方法

		//
		// 摘要:
		//     将所有选项、最后选定的打印机和页面设置重新设置为其默认值。
		public void Reset()
		{
			this._dialog.Reset();
		}

		//
		// 摘要:
		//     用默认的所有者运行通用对话框。
		//
		// 返回结果:
		//     如果用户在对话框中单击“确定”，则为 System.Windows.Forms.DialogResult.OK；否则为 System.Windows.Forms.DialogResult.Cancel。
		public DialogResult ShowDialog()
		{
			return this._dialog.ShowDialog();
		}

		//
		// 摘要:
		//     运行具有指定所有者的通用对话框。
		//
		// 参数:
		//   owner:
		//     任何实现 System.Windows.Forms.IWin32Window（表示将拥有模式对话框的顶级窗口）的对象。
		//
		// 返回结果:
		//     如果用户在对话框中单击“确定”，则为 System.Windows.Forms.DialogResult.OK；否则为 System.Windows.Forms.DialogResult.Cancel。
		public DialogResult ShowDialog(IWin32Window owner)
		{
			return this._dialog.ShowDialog(owner);
		}

		#endregion
	}
}