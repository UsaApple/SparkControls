using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public sealed class SparkFolderBrowserDialog
	{
		#region 字段

		private readonly FolderBrowserDialog _dialog = null;

		#endregion

		#region 属性

		//
		// 摘要:
		//     获取或设置一个值，该值指示“新建文件夹”按钮是否显示在文件夹浏览对话框中。
		//
		// 返回结果:
		//     如果“新建文件夹”按钮显示在对话框中，则为 true；否则为 false。默认为 true。
		public bool ShowNewFolderButton
		{
			set { this._dialog.ShowNewFolderButton = value; }
			get { return this._dialog.ShowNewFolderButton; }
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

		//
		// 摘要:
		//     获取或设置用户选定的路径。
		//
		// 返回结果:
		//     对话框中选定的第一个文件夹或用户选定的最后一个文件夹的路径。默认值为空字符串 ("")。
		public string SelectedPath
		{
			set { this._dialog.SelectedPath = value; }
			get { return this._dialog.SelectedPath; }
		}

		//
		// 摘要:
		//     获取或设置从其开始浏览的根文件夹。
		//
		// 返回结果:
		//     System.Environment.SpecialFolder 值之一。默认为 Desktop。
		//
		// 异常:
		//   T:System.ComponentModel.InvalidEnumArgumentException:
		//     分配的值不是 System.Environment.SpecialFolder 值之一。
		public Environment.SpecialFolder RootFolder
		{
			set { this._dialog.RootFolder = value; }
			get { return this._dialog.RootFolder; }
		}

		//
		// 摘要:
		//     获取或设置对话框中在树视图控件上显示的说明文本。
		//
		// 返回结果:
		//     要显示的说明。默认值为空字符串 ("")。
		public string Description
		{
			set { this._dialog.Description = value; }
			get { return this._dialog.Description; }
		}

		#endregion

		#region 自定义事件

		public event EventHandler HelpRequest = null;

		#endregion

		public SparkFolderBrowserDialog()
		{
			this._dialog = new FolderBrowserDialog();
			this._dialog.HelpRequest += (s, e) => { HelpRequest?.Invoke(s, e); };
		}

		#region 对外方法

		public void Reset()
		{
			this._dialog.Reset();
		}

		public DialogResult ShowDialog()
		{
			return this._dialog.ShowDialog();
		}

		public DialogResult ShowDialog(IWin32Window owner)
		{
			return this._dialog.ShowDialog(owner);
		}

		#endregion
	}
}