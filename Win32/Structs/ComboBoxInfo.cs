using System;
using System.Runtime.InteropServices;
using SparkControls.Win32;

namespace SparkControls.Win32
{
	/// <summary>
	/// ComboBox的Windows信息结构
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ComboBoxInfo
	{
		public int cbSize;
		public RECT rcItem;
		public RECT rcButton;
		public ComboBoxButtonState stateButton;
		public IntPtr hwndCombo;
		public IntPtr hwndEdit;
		public IntPtr hwndList;
	}
}