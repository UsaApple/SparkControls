using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public sealed class SparkOpenFileDialog
	{
		#region 字段

		private readonly OpenFileDialog _dialog = null;

		#endregion

		#region 属性

		public bool CheckFileExists
		{
			set { this._dialog.CheckFileExists = value; }
			get { return this._dialog.CheckFileExists; }
		}

		public bool Multiselect
		{
			set { this._dialog.Multiselect = value; }
			get { return this._dialog.Multiselect; }
		}

		public bool ReadOnlyChecked
		{
			set { this._dialog.ReadOnlyChecked = value; }
			get { return this._dialog.ReadOnlyChecked; }
		}

		public bool ShowReadOnly
		{
			set { this._dialog.ShowReadOnly = value; }
			get { return this._dialog.ShowReadOnly; }
		}

		public string SafeFileName
		{
			get { return this._dialog.SafeFileName; }
		}

		public string[] SafeFileNames
		{
			get { return this._dialog.SafeFileNames; }
		}

		public FileDialogCustomPlacesCollection CustomPlaces
		{
			get { return this._dialog.CustomPlaces; }
		}

		public bool ValidateNames
		{
			set { this._dialog.ValidateNames = value; }
			get { return this._dialog.ValidateNames; }
		}

		public string Title
		{
			set { this._dialog.Title = value; }
			get { return this._dialog.Title; }
		}

		public bool SupportMultiDottedExtensions
		{
			set { this._dialog.SupportMultiDottedExtensions = value; }
			get { return this._dialog.SupportMultiDottedExtensions; }
		}

		public bool ShowHelp
		{
			set { this._dialog.ShowHelp = value; }
			get { return this._dialog.ShowHelp; }
		}

		public bool RestoreDirectory
		{
			set { this._dialog.RestoreDirectory = value; }
			get { return this._dialog.RestoreDirectory; }
		}

		public string InitialDirectory
		{
			set { this._dialog.InitialDirectory = value; }
			get { return this._dialog.InitialDirectory; }
		}

		public int FilterIndex
		{
			set { this._dialog.FilterIndex = value; }
			get { return this._dialog.FilterIndex; }
		}

		public string Filter
		{
			set { this._dialog.Filter = value; }
			get { return this._dialog.Filter; }
		}

		public bool AutoUpgradeEnabled
		{
			set { this._dialog.AutoUpgradeEnabled = value; }
			get { return this._dialog.AutoUpgradeEnabled; }
		}

		public string[] FileNames { get { return this._dialog.FileNames; } }

		public bool DereferenceLinks
		{
			set { this._dialog.DereferenceLinks = value; }
			get { return this._dialog.DereferenceLinks; }
		}

		public string DefaultExt
		{
			set { this._dialog.DefaultExt = value; }
			get { return this._dialog.DefaultExt; }
		}

		public bool CheckPathExists
		{
			set { this._dialog.CheckPathExists = value; }
			get { return this._dialog.CheckPathExists; }
		}

		public bool AddExtension
		{
			set { this._dialog.AddExtension = value; }
			get { return this._dialog.AddExtension; }
		}

		public string FileName
		{
			set { this._dialog.FileName = value; }
			get { return this._dialog.FileName; }
		}

		public object Tag
		{
			set { this._dialog.Tag = value; }
			get { return this._dialog.Tag; }
		}

		#endregion

		#region 自定义事件

		public event CancelEventHandler FileOk = null;

		public event EventHandler HelpRequest = null;

		#endregion

		public SparkOpenFileDialog()
		{
			this._dialog = new OpenFileDialog();
			this._dialog.FileOk += (s, e) => { FileOk?.Invoke(s, e); };
			this._dialog.HelpRequest += (s, e) => { HelpRequest?.Invoke(s, e); };
		}

		#region 对外方法

		public Stream OpenFile()
		{
			return this._dialog.OpenFile();
		}

		public void Reset()
		{
			this._dialog.Reset();
		}

		public override string ToString()
		{
			return this._dialog.ToString();
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