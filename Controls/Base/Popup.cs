using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms.VisualStyles;

using SparkControls.Controls;
using SparkControls.Theme;
using SparkControls.Win32;

using VS = System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
	/// <summary>
	/// 弹出框。
	/// </summary>
	[ToolboxItem(false)]
	public partial class Popup : ToolStripDropDown
	{
		#region 字段

		private readonly ToolStripControlHost host;

		private const int frames = 1;
		private const int totalduration = 10000;
		private const int frameduration = totalduration / frames;

		private VisualStyleRenderer sizeGripRenderer;
		private Popup ownerPopup;
		private Popup childPopup;
		private bool resizableTop;
		private bool resizableRight;

		#endregion

		#region 构造函数

		/// <summary>
		/// 初始 <see cref="Popup" /> 类型的新实例。
		/// </summary>
		/// <param name="content">弹出框内容。</param>
		public Popup(Control content)
		{
			this.Content = content ?? throw new ArgumentNullException(nameof(content), "值不能为空。");
			this.host = new ToolStripControlHost(content);

			this.UseFadeEffect = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;
			this.AutoSize = false;
			this.DoubleBuffered = true;
			this.Resizable = true;
			this.ResizeRedraw = true;
			this.Padding = this.Margin = host.Padding = host.Margin = Padding.Empty;
			this.MaximumSize = this.Content.MaximumSize;
			this.MinimumSize = this.Content.MinimumSize;
			this.Size = this.Content.Size;
			this.Items.Add(host);
			this.BackColor = content.BackColor;
			content.Disposed += delegate (object sender, EventArgs e)
			{
				content = null;
				this.host.Dispose();
				this.Dispose(true);
			};
			content.RegionChanged += delegate (object sender, EventArgs e)
			{
				this.UpdateRegion();
			};
			content.Paint += delegate (object sender, PaintEventArgs e)
			{
				this.PaintSizeGrip(e);
			};

			this.UpdateRegion();

			this.InitializeComponent();
		}

		#endregion

		#region 属性

		/// <summary>
		/// 获取弹出框内容控件。
		/// </summary>
		public Control Content { get; private set; }

		/// <summary>
		/// 获取或设置一个值，该值指示是否启用淡入淡出效果。
		/// </summary>
		public bool UseFadeEffect { get; set; } = true;

		/// <summary>
		/// 获取或设置一个值，该值指示打开时是否获取焦点。
		/// </summary>
		public bool FocusOnOpen { get; set; } = true;

		/// <summary>
		/// 获取或设置一个值，该值指示是否接收 “Alt”键。
		/// </summary>
		public bool AcceptAlt { get; set; } = true;

		/// <summary>
		/// 获取或设置一个值，该值指示控件是否可以调整大小。
		/// </summary>
		public bool Resizable { get; set; } = true;

		/// <summary>
		/// 获取或设置控件最后关闭时间戳。
		/// </summary>
		public DateTime LastClosedTimeStamp { get; set; } = DateTime.Now;

		#endregion

		#region 重写方法

		/// <summary>
		/// 获取创建控件句柄时需要的创建参数。
		/// </summary>
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
				return cp;
			}
		}

		/// <summary>
		/// 处理 Windows 消息。
		/// </summary>
		/// <param name="m"> Windows 消息。</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			if (InternalProcessResizing(ref m, false)) { return; }
			base.WndProc(ref m);
		}

		/// <summary>
		/// 处理对话框键。
		/// </summary>
		/// <param name="keyData">要处理的键。</param>
		/// <returns>处理结果，true：处理成功，false：处理失败。</returns>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (AcceptAlt && ((keyData & Keys.Alt) == Keys.Alt)) return false;
			return base.ProcessDialogKey(keyData);
		}

		/// <summary>
		/// 设置控件显示。
		/// </summary>
		/// <param name="visible">表示是否可见。</param>
		protected override void SetVisibleCore(bool visible)
		{
			double opacity = Opacity;

			if (visible && UseFadeEffect && FocusOnOpen) Opacity = 0;
			base.SetVisibleCore(visible);

			if (!visible || !UseFadeEffect || !FocusOnOpen) return;
			for (int i = 1; i <= frames; i++)
			{
				if (i > 1)
				{
					Threading.Thread.Sleep(frameduration);
				}
				Opacity = opacity * i / frames;
			}
			Opacity = opacity;
		}

		/// <summary>
		/// 引发 SizeChanged 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			if (Content == null) return;
			Content.MaximumSize = Size;
			Content.MinimumSize = Size;
			Content.Size = Size;
			Content.Location = Point.Empty;
			base.OnSizeChanged(e);
		}

		/// <summary>
		/// 引发 Opening 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnOpening(CancelEventArgs e)
		{
			if (Content.IsDisposed || Content.Disposing)
			{
				e.Cancel = true;
				return;
			}
			UpdateRegion();
			base.OnOpening(e);
		}

		/// <summary>
		/// 引发 Opened 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnOpened(EventArgs e)
		{
			if (ownerPopup != null)
			{
				ownerPopup.Resizable = false;
			}
			if (FocusOnOpen)
			{
				Content.Focus();
			}
			PopupCache.Add(this);
			base.OnOpened(e);
		}

		/// <summary>
		/// 引发 Closed 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
		{
			if (ownerPopup != null)
			{
				ownerPopup.Resizable = true;
			}
			base.OnClosed(e);
		}

		/// <summary>
		/// 引发 VisibleChanged 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			if (Visible == false) { LastClosedTimeStamp = DateTime.Now; }
			base.OnVisibleChanged(e);
		}

		/// <summary>
		/// 处理命令键。
		/// </summary>
		/// <param name="m">通过引用传递的 Message，表示要处理的窗口消息。</param>
		/// <param name="keyData"><see cref="Keys"/> 值之一，表示要处理的键。</param>
		/// <returns>如果字符已由控件处理，则为 <see cref="true"/>，否则为 <see cref="false"/>。</returns>
		protected override bool ProcessCmdKey(ref Message m, Keys keyData)
		{
			if (this.AutoClose == false && keyData == Keys.Escape)
			{
				this.Close();
			}
			return base.ProcessCmdKey(ref m, keyData);
		}
		#endregion

		#region 共有方法

		/// <summary>
		/// 向用户显示控件。
		/// </summary>
		/// <param name="control">弹出框内容。</param>
		public void Show(Control control)
		{
			Show(control, control.ClientRectangle);
		}

		/// <summary>
		/// 向用户显示控件。
		/// </summary>
		/// <param name="control">弹出框内容。</param>
		/// <param name="area">显示位置。</param>
		public void Show(Control control, Rectangle area)
		{
			if (control == null)
			{
				throw new ArgumentNullException(nameof(control), "值不能为空。");
			}
			SetOwnerItem(control);
			resizableTop = resizableRight = false;
			Point location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
			Rectangle screen = Screen.FromControl(control).WorkingArea;
			if (location.X + Size.Width > (screen.Left + screen.Width))
			{
				resizableRight = true;
				location.X = (screen.Left + screen.Width) - Size.Width;
			}
			if (location.Y + Size.Height > (screen.Top + screen.Height))
			{
				resizableTop = true;
				location.Y -= Size.Height + area.Height;
			}
			location = control.PointToClient(location);
			Show(control, location, ToolStripDropDownDirection.BelowRight);
			control.Focus();
		}

		/// <summary>
		/// 响应尺寸调整。
		/// </summary>
		/// <param name="m">Window 消息。</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public bool ProcessResizing(ref Message m)
		{
			return InternalProcessResizing(ref m, true);
		}

		/// <summary>
		/// 绘制边界手柄。
		/// </summary>
		/// <param name="e">绘制事件参数。</param>
		public void PaintSizeGrip(PaintEventArgs e)
		{
			if (e == null || e.Graphics == null || !Resizable)
			{
				return;
			}
			Size clientSize = Content.ClientSize;
			if (Application.RenderWithVisualStyles)
			{
				if (this.sizeGripRenderer == null)
				{
					this.sizeGripRenderer = new VS.VisualStyleRenderer(VS.VisualStyleElement.Status.Gripper.Normal);
				}
				this.sizeGripRenderer.DrawBackground(e.Graphics, new Rectangle(clientSize.Width - 0x10, clientSize.Height - 0x10, 0x10, 0x10));
			}
			else
			{
				ControlPaint.DrawSizeGrip(e.Graphics, Content.BackColor, clientSize.Width - 0x10, clientSize.Height - 0x10, 0x10, 0x10);
			}
		}

		#endregion

		// 响应尺寸调整
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private bool InternalProcessResizing(ref Message m, bool contentControl)
		{
			if (m.Msg == NativeMethods.WM_NCACTIVATE && m.WParam != IntPtr.Zero && childPopup != null && childPopup.Visible)
			{
				childPopup.Hide();
			}
			else if (m.Msg == (int)Msgs.WM_ACTIVATE && m.LParam == IntPtr.Zero && m.WParam == IntPtr.Zero && this.Visible)
			{
				this.Close();
			}
			if (!Resizable)
			{
				return false;
			}
			if (m.Msg == NativeMethods.WM_NCHITTEST)
			{
				return OnNcHitTest(ref m, contentControl);
			}
			else if (m.Msg == NativeMethods.WM_GETMINMAXINFO)
			{
				return OnGetMinMaxInfo(ref m);
			}
			return false;
		}

		// 获取最大最小信息
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private bool OnGetMinMaxInfo(ref Message m)
		{
			NativeMethods.MINMAXINFO minmax = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
			minmax.maxTrackSize = this.MaximumSize;
			minmax.minTrackSize = this.MinimumSize;
			Marshal.StructureToPtr(minmax, m.LParam, false);
			return true;
		}

		// 命中测试
		private bool OnNcHitTest(ref Message m, bool contentControl)
		{
			int x = NativeMethods.LOWORD(m.LParam);
			int y = NativeMethods.HIWORD(m.LParam);
			Point clientLocation = PointToClient(new Point(x, y));

			GripBounds gripBouns = new GripBounds(contentControl ? Content.ClientRectangle : ClientRectangle);
			IntPtr transparent = new IntPtr(NativeMethods.HTTRANSPARENT);

			if (resizableTop)
			{
				if (resizableRight && gripBouns.TopLeft.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPLEFT;
					return true;
				}
				if (!resizableRight && gripBouns.TopRight.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPRIGHT;
					return true;
				}
				if (gripBouns.Top.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOP;
					return true;
				}
			}
			else
			{
				if (resizableRight && gripBouns.BottomLeft.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMLEFT;
					return true;
				}
				if (!resizableRight && gripBouns.BottomRight.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
					return true;
				}
				if (gripBouns.Bottom.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOM;
					return true;
				}
			}
			if (resizableRight && gripBouns.Left.Contains(clientLocation))
			{
				m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTLEFT;
				return true;
			}
			if (!resizableRight && gripBouns.Right.Contains(clientLocation))
			{
				m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTRIGHT;
				return true;
			}
			return false;
		}

		// 设置归属控件
		private void SetOwnerItem(Control control)
		{
			if (control == null)
			{
				return;
			}
			if (control is Popup popupControl)
			{
				ownerPopup = popupControl;
				ownerPopup.childPopup = this;
				OwnerItem = popupControl.Items[0];
				return;
			}
			if (control.Parent != null)
			{
				SetOwnerItem(control.Parent);
			}
		}

		// 更新区域
		private void UpdateRegion()
		{
			if (this.Region != null)
			{
				this.Region.Dispose();
				this.Region = null;
			}
			if (Content.Region != null)
			{
				this.Region = Content.Region.Clone();
			}
		}

		// Win32
		internal static class NativeMethods
		{
			internal const int WM_NCHITTEST = 0x0084;
			internal const int WM_NCACTIVATE = 0x0086;
			internal const int WS_EX_NOACTIVATE = 0x08000000;
			internal const int HTTRANSPARENT = -1;
			internal const int HTLEFT = 10;
			internal const int HTRIGHT = 11;
			internal const int HTTOP = 12;
			internal const int HTTOPLEFT = 13;
			internal const int HTTOPRIGHT = 14;
			internal const int HTBOTTOM = 15;
			internal const int HTBOTTOMLEFT = 16;
			internal const int HTBOTTOMRIGHT = 17;
			internal const int WM_USER = 0x0400;
			internal const int WM_REFLECT = WM_USER + 0x1C00;
			internal const int WM_COMMAND = 0x0111;
			internal const int CBN_DROPDOWN = 7;
			internal const int WM_GETMINMAXINFO = 0x0024;

			internal static int HIWORD(int n)
			{
				return (n >> 16) & 0xffff;
			}

			internal static int HIWORD(IntPtr n)
			{
				return HIWORD(unchecked((int)(long)n));
			}

			internal static int LOWORD(int n)
			{
				return n & 0xffff;
			}

			internal static int LOWORD(IntPtr n)
			{
				return LOWORD(unchecked((int)(long)n));
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct MINMAXINFO
			{
				public Point reserved;
				public Size maxSize;
				public Point maxPosition;
				public Size minTrackSize;
				public Size maxTrackSize;
			}
		}
	}
}