using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SparkControls.Win32;

namespace SparkControls.Forms
{
	/// <summary>
	/// 有阴影的窗口
	/// </summary>
	public partial class SparkFormShadow : Form
	{
		#region API
		#region 常量
		private const int CS_DROPSHADOW = 0x00020000;
		#endregion

		private struct MARGINS                           // struct for box shadow
		{
			public int leftWidth;
			public int rightWidth;
			public int topHeight;
			public int bottomHeight;
		}

		[DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
		private static extern IntPtr CreateRoundRectRgn
		(
			int nLeftRect, // x-coordinate of upper-left corner
			int nTopRect, // y-coordinate of upper-left corner
			int nRightRect, // x-coordinate of lower-right corner
			int nBottomRect, // y-coordinate of lower-right corner
			int nWidthEllipse, // height of ellipse
			int nHeightEllipse // width of ellipse
		 );

		[DllImport("dwmapi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(ref int pfEnabled);
		#endregion

		#region 字段
		private bool m_aeroEnabled = false;// variables for box shadow
		#endregion

		#region 属性
		/// <summary>
		/// 是否开启阴影
		/// </summary>
		[Description("是否开启阴影")]
		public bool ShadowEnabled { get; set; } = true;

		#endregion
		public SparkFormShadow()
		{
			this.InitializeComponent();
			this.m_aeroEnabled = false;
			this.FormBorderStyle = FormBorderStyle.None;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				if (this.ShadowEnabled)
				{
					this.m_aeroEnabled = this.CheckAeroEnabled();
					CreateParams cp = base.CreateParams;
					if (!this.m_aeroEnabled)
						cp.ClassStyle |= CS_DROPSHADOW;
					return cp;
				}
				return base.CreateParams;
			}
		}

		private bool CheckAeroEnabled()
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				int enabled = 0;
				DwmIsCompositionEnabled(ref enabled);
				return (enabled == 1) ? true : false;
			}
			return false;
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case (int)Msgs.WM_NCPAINT:                        // box shadow
					if (this.m_aeroEnabled)
					{
						int v = 2;
						DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
						MARGINS margins = new MARGINS()
						{
							bottomHeight = 1,
							leftWidth = 1,
							rightWidth = 1,
							topHeight = 1
						};
						DwmExtendFrameIntoClientArea(this.Handle, ref margins);
					}
					break;
				default:
					break;
			}
			base.WndProc(ref m);

			//if (m.Msg == (int)Msgs.WM_NCHITTEST && (int)m.Result == (int)HitTest.HTCLIENT)     // drag the form
			//{
			//    m.Result = (IntPtr)HitTest.HTCAPTION;
			//}

		}
	}
}