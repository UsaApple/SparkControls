using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SparkControls.Controls.Design;
using SparkControls.Theme;

namespace SparkControls.Controls
{
	[Designer(typeof(SparkNavigationBarDesigner))]
	[DefaultProperty(nameof(TabPages))]
	[DefaultEvent(nameof(SelectedTabChanged))]
	[ToolboxItem(false)]
	[Description("NavigationBar控件")]
	[ToolboxBitmap(typeof(TabControl))]
	public partial class SparkNavigationBar : ContainerControl, ISparkTheme<SparkNavigationBarTheme>
	{
		#region 字段
		private Font font = Consts.DEFAULT_FONT;
		private readonly List<SparkNavigateBarPanel> _pages = new List<SparkNavigateBarPanel>();
		private SparkNavigateBarPanel _selectedPage;
		private static readonly object EventSelectedTabChanged = new object();
		private static readonly object EventExpandedChanged = new object();
		private readonly SparkNavigateToolStrip _tabStrip = new SparkNavigateToolStrip() { Dock = DockStyle.Right };
		private NavigateBarPanelCollection _pageCollection;

		private TabAlignment _alignment = TabAlignment.Right;
		private NavigationBarDisplayStyle _displayStyle = NavigationBarDisplayStyle.ImageAndText;
		private bool _expanded = true;
		private readonly bool _hideSingleTab;
		#endregion

		#region 事件
		public event EventHandler SelectedTabChanged
		{
			add { this.Events.AddHandler(EventSelectedTabChanged, value); }
			remove { this.Events.RemoveHandler(EventSelectedTabChanged, value); }
		}

		public event EventHandler ExpandedChanged
		{
			add { this.Events.AddHandler(EventExpandedChanged, value); }
			remove { this.Events.RemoveHandler(EventExpandedChanged, value); }
		}
		#endregion

		#region 属性
		[Browsable(true)]
		[MergableProperty(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[TypeConverter(typeof(SparkNavigationBarSelectedPageConverter))]
		[Editor(typeof(SparkNavigationBarSelectedPageEditor), typeof(UITypeEditor))]
		public SparkNavigateBarPanel SelectedTab
		{
			get => this._selectedPage;
			set
			{
				if (this._selectedPage != value)
				{
					this._tabStrip.SelectedTab = value?.TabStripButton;
				}
			}
		}

		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavigateBarPanelCollection TabPages
		{
			get
			{
				if (this._pageCollection == null)
				{
					this._pageCollection = new NavigateBarPanelCollection(this);
				}
				return this._pageCollection;
			}
		}


		[Browsable(false)]
		[MergableProperty(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SparkNavigateToolStrip TabStrip => this._tabStrip;

		[Browsable(false)]
		[MergableProperty(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedTabIndex
		{
			get => this._tabStrip.SelectedTabIndex;
			set => this._tabStrip.SelectedTabIndex = value;
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get => base.Text;
			set => base.Text = value;
		}

		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文本的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get => this.font;
			set
			{
				if (this.font != value)
				{
					base.Font = this.font = value;
					this.OnFontChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 获取或设置控件的背景色。
		/// </summary>
		[Category("Spark"), Description("控件的背景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 获取或设置控件的前景色。
		/// </summary>
		[Category("Spark"), Description("控件的前景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		[Description("获取或设置选项卡的位置")]
		[DefaultValue(TabAlignment.Right)]
		[Category("Spark")]
		public virtual TabAlignment Alignment
		{
			get => this._alignment;
			set
			{
				this._alignment = value;
				this.InitAlignment(this._alignment);
			}
		}

		[Description("文本和图片显示样式")]
		[DefaultValue(NavigationBarDisplayStyle.ImageAndText)]
		[Category("Spark")]
		public NavigationBarDisplayStyle DisplayStyle
		{
			get => this._displayStyle;
			set
			{
				if (this._displayStyle != value)
				{
					this._displayStyle = value;
					this.InitDisplay(this._displayStyle);
				}
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示控件是否展开。
		/// </summary>
		[Description("表示是否展开状态。")]
		[DefaultValue(true)]
		[Category("Spark")]
		public bool Expanded
		{
			get => this._expanded;
			set
			{
				this._expanded = value;
				if (value)
				{
					this.ExpandPanel();
				}
				else
				{
					this.CollapsePanel();
					this.SelectedTab = null;
				}
				(this.Events[EventExpandedChanged] as EventHandler)?.Invoke(this, new EventArgs());
			}
		}

		/// <summary>
		/// 获取或设置控件的高度和宽度。
		/// </summary>
		[Localizable(true)]
		[Description("控件的高度和宽度。")]
		public new Size Size
		{
			get => base.Size;
			set => base.Size = value; 
		}

		/// <summary>
		/// 获取或设置控件展开时的尺寸。
		/// </summary>
		[Browsable(true)]
		public Size ExpandedSize { get; set; } 

		/// <summary>
		/// 引发 SizeChanged 事件。
		/// </summary>
		/// <param name="e">包含事件数据的 <see cref="EventArgs"/>。</param>
		protected override void OnSizeChanged(EventArgs e) 
		{
			// 记录控件的展开尺寸
			if (Expanded) ExpandedSize = this.Size;
			base.OnSizeChanged(e);
		}
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkNavigationBar()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |//调整大小时重绘
						ControlStyles.DoubleBuffer |//双缓冲
						ControlStyles.OptimizedDoubleBuffer |//双缓冲
						ControlStyles.AllPaintingInWmPaint |//禁止擦除背景
						ControlStyles.UserPaint |
						ControlStyles.ContainerControl |
						ControlStyles.Selectable, true);
			this.UpdateStyles();
			base.Font = this.font;
			this.Theme = new SparkNavigationBarTheme(this);

			this._pageCollection = new NavigateBarPanelCollection(this);
			this._tabStrip.SelectedTabChanged += this.InvokeSelectedTabChanged;
			this._tabStrip.ItemClicked += this._tabStrip_ItemClicked;
			this.BackColor = SparkThemeConsts.NavigationBarBackColor;
			this.ForeColor = Color.White;
			this._tabStrip.Theme.ForeColor = Color.White;

			this._tabStrip.Theme.BackColor1 = this.Theme.ToolBackColor;
            this._tabStrip.Theme.BackColor2 = this.Theme.ToolBackColor;
            this._tabStrip.Theme.SelectedBackColor = this.Theme.ToolSelectedColor;
            this._tabStrip.Theme.MouseOverBackColor = this.Theme.ToolSelectedColor;
            this._tabStrip.Theme.MouseDownBackColor = this.Theme.ToolSelectedColor;
            this._tabStrip.Theme.SelectedForeColor = Color.Black;
            this._tabStrip.Theme.MouseOverForeColor = Color.Black;
            this._tabStrip.Theme.MouseDownForeColor = Color.Black;
           
        }
        #endregion

		#region 重写

		protected override Size DefaultSize
		{
			get { return new Size(500, 400); }
		}

		protected override Padding DefaultPadding
		{
			get { return new Padding(0); }
		}

		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new ControlCollection(this);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			if (!this.Controls.Contains(this._tabStrip))
			{
				this.Controls.Add(this._tabStrip);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			#region 绘制边框
			GDIHelper.DrawNonWorkAreaBorder(this, SparkThemeConsts.BorderColor);
			#endregion
		}
		#endregion

		#region 事件绑定
		private void _tabStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (this.Expanded == false && e.ClickedItem is SparkNavigationBarStripButton)
			{
				this.Expanded = true;
				if (this._selectedPage != null && this._selectedPage != this.SelectedTab)
				{
					this._selectedPage.TabStripButton.Checked = false;
				}
			}
		}

		private void InvokeSelectedTabChanged(object sender, EventArgs e)
		{
			this.SetRedraw(this, false);
			try
			{
				if (this._selectedPage != null)
				{
					this._selectedPage.Visible = false;
				}

				this._selectedPage = SparkNavigateBarPanel.GetButtonPage(this._tabStrip.SelectedTab);

				if (this._selectedPage != null)
				{
					this._selectedPage.Visible = true;

					this._tabStrip.SendToBack();
					this._tabStrip.SelectedTab = this._selectedPage.TabStripButton;

					this._selectedPage.Focus();
					this._selectedPage.SelectNextControl(this._selectedPage, true, true, true, false);
				}
			}
			finally
			{
				this.SetRedraw(this, true);
				this.Invalidate(true);
			}

			this.OnSelectedTabChanged(e);
		}
		#endregion

		#region protected virtual方法
		protected virtual void OnSelectedTabChanged(EventArgs e)
		{
			if (this.Expanded == false && this.SelectedTab != null)
			{
				this.Expanded = true;
			}
			(this.Events[EventSelectedTabChanged] as EventHandler)?.Invoke(this, e);
		}

		protected virtual void ExpandPanel()
		{
			this.TabStrip.CheckItem(true);
			switch (this.Alignment)
			{
				case TabAlignment.Top:
				case TabAlignment.Bottom:
					this.Height = this.ExpandedSize.Height;
					break;
				case TabAlignment.Left:
				case TabAlignment.Right:
					this.Width = this.ExpandedSize.Width;
					break;
			}
		}

		protected virtual void CollapsePanel()
		{
			this.TabStrip.CheckItem(false);
			switch (this.Alignment)
			{
				case TabAlignment.Top:
				case TabAlignment.Bottom:
					this.Height = this.TabStrip.Height;
					break;
				case TabAlignment.Left:
				case TabAlignment.Right:
					this.Width = this.TabStrip.Width;
					break;
			}
		}
		#endregion

		#region 私有方法
		private void InitAlignment(TabAlignment tabAlignment)
		{
			this._tabStrip.Dock = tabAlignment.ToString().ToEnum<DockStyle>();
			//switch (tabAlignment)
			//{
			//    case TabAlignment.Top:
			//    case TabAlignment.Bottom:
			//        _tabStrip.TextDirection = ToolStripTextDirection.Horizontal;
			//        break;
			//    case TabAlignment.Left:
			//    case TabAlignment.Right:
			//        _tabStrip.TextDirection = ToolStripTextDirection.Vertical90;
			//        break;
			//}
			this._tabStrip.OnRefreshItemsByAlignment();
		}

		private void InitDisplay(NavigationBarDisplayStyle displayStyle)
		{
			ToolStripItemDisplayStyle display = displayStyle.ToString().ToEnum<ToolStripItemDisplayStyle>();
			this._tabStrip.OnRefreshItemsByDisplay(display);
		}

		private void Add(SparkNavigateBarPanel page)
		{
			bool suspendPaint = (this._hideSingleTab && (this._pages.Count == 1));

			if (suspendPaint)
			{
				this.SetRedraw(this, false);
				this.SetRedraw(this._tabStrip, false);
			}

			try
			{
				page.Visible = false;
				page.Dock = DockStyle.Fill;

				this._pages.Add(page);

				this._tabStrip.SuspendLayout();
				page.ButtonInit(this);
				this._tabStrip.Items.Add(page.TabStripButton);

				this._tabStrip.ResumeLayout(false);

				this.UpdateTabStripVisibility();
			}
			finally
			{
				if (suspendPaint)
				{
					this.SetRedraw(this._tabStrip, true);
					this.SetRedraw(this, true);
					this.Invalidate(true);
				}
			}
		}

		private void AddRange(IList<SparkNavigateBarPanel> pages)
		{
			SparkNavigationBarStripButton[] buttons = new SparkNavigationBarStripButton[pages.Count];

			for (int i = 0; i < pages.Count; ++i)
			{
				pages[i].Visible = false;
				pages[i].Dock = DockStyle.Fill;
				buttons[i] = pages[i].TabStripButton;
			}

			this._pages.AddRange(pages);

			this._tabStrip.SuspendLayout();
			this._tabStrip.Items.AddRange(buttons);
			this._tabStrip.ResumeLayout(false);

			this.UpdateTabStripVisibility();

		}

		private void UpdateTabStripVisibility()
		{
			if (this._hideSingleTab)
			{
				this._tabStrip.Visible = this._pages.Count > 1;
			}
			else
			{
				this._tabStrip.Visible = true;
			}
		}

		private void Remove(SparkNavigateBarPanel page)
		{

			if (this._tabStrip.Items.Contains(page.TabStripButton))
			{
				this._tabStrip.SuspendLayout();
				this._tabStrip.Items.Remove(page.TabStripButton);
				this._tabStrip.ResumeLayout(false);
			}

			if (this._pages.Contains(page))
			{
				this._pages.Remove(page);
			}

			this.UpdateTabStripVisibility();

		}

		private void RemoveAllTabs()
		{
			this.SuspendLayout();
			this._tabStrip.SuspendLayout();

			this.SelectedTab = null;

			this._tabStrip.RemoveAllTabs();
			this._pages.Clear();

			this._tabStrip.ResumeLayout(false);
			this.UpdateTabStripVisibility();
			this.ResumeLayout();

		}

		private void SetRedraw(Control control, bool redraw)
		{
            //SendMessage(
            //    new HandleRef(control, control.Handle),
            //    WM_SETREDRAW,
            //    redraw ? new IntPtr(1) : IntPtr.Zero,
            //    IntPtr.Zero);
        }


		#endregion

		#region api
		private const int WM_SETREDRAW = 0x000B;
		[DllImport(
			"user32",
			BestFitMapping = false,
			SetLastError = true
		)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		private static extern IntPtr SendMessage(
			[In] HandleRef hWnd,
			[In] uint Msg,
			[In] IntPtr wParam,
			[In] IntPtr lParam);
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkNavigationBarTheme Theme { get; private set; }

		#endregion
	}
}