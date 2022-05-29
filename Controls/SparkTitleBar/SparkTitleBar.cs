using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 标题栏控件
	/// </summary>
	[ToolboxItem(true)]
	public class SparkTitleBar : SparkControl
	{
		#region 私有变量
		private SparkTitleBarDraw titleBarDraw = null;
		#endregion

		#region 属性
		/// <summary>
		/// 标题栏属性
		/// </summary>
		[Category("Spark"), Description("控件的主题。")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public new SparkTitleBarTheme Theme { get; private set; }

		private Font font = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Browsable(false)]
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
		/// 获取自定义按钮的集合。
		/// </summary>
		[Browsable(false)]
		public IList<SparkTitleBarItem> CustomItem => this.titleBarDraw?.CustomItem;

		/// <summary>
		/// 获取或设置一个值，该值指示是窗体否显示标题栏。
		/// </summary>
		[Description("是否显示标题栏。")]
		[DefaultValue(true)]
		public bool IsDrawTitle
		{
			get => this.titleBarDraw.IsDrawTitle;
			set
			{
				if (this.titleBarDraw.IsDrawTitle != value)
				{
					this.titleBarDraw.IsDrawTitle = value;
				}
			}
		}

		/// <summary>
		///  获取或设置一个值，该值指示是否在窗体的标题栏中显示“最小化”按钮。
		/// </summary>
		[DefaultValue(true)]
		[Description("是否显示最小化按钮。")]
		public bool MinimizeBox
		{
			get => this.titleBarDraw.MinimizeBox;
			set => this.titleBarDraw.MinimizeBox = value;
		}

		/// <summary>
		/// 获取或设置一个值，该值指示是否在窗体的标题栏中显示“最大化”按钮。
		/// </summary>
		[DefaultValue(true)]
		[Description("是否显示最大化按钮。")]
		public bool MaximizeBox
		{
			get => this.titleBarDraw.MaximizeBox;
			set => this.titleBarDraw.MaximizeBox = value;
		}

		/// <summary>
		/// 获取或设置一个值，该值指示是否绘制最小化、最大化、关闭等按钮。
		/// </summary>
		[DefaultValue(true)]
		[Description("是否绘制最小化、最大化、关闭等按钮。")]
		public bool ControlBox
		{
			get => this.titleBarDraw.ControlBox;
			set => this.titleBarDraw.ControlBox = value;
		}

		/// <summary>
		/// 获取或设置窗体的图标
		/// </summary>
		[DefaultValue(typeof(Icon), null)]
		[Description("标题图标")]
		public Icon Icon
		{
			get => this.titleBarDraw.Icon;
			set
			{
				if (this.titleBarDraw.Icon != value)
				{
					this.titleBarDraw.Icon = value;
				}
			}
		}

		/// <summary>
		/// 获取或设置标题栏的字体。
		/// </summary>
		[DefaultValue(typeof(Font), "微软雅黑,12pt")]
		[Description("标题栏的字体。")]
		public Font TitleFont
		{
			get => this.titleBarDraw.TitleFont;
			set
			{
				if (this.titleBarDraw.TitleFont != value)
				{
					this.titleBarDraw.TitleFont = value;
				}
			}
		}

		/// <summary>
		/// 获取或设置标题栏的高度。
		/// </summary>
		[DefaultValue(32)]
		[Description("标题栏的高度。")]
		public int TitleHeight
		{
			get => this.titleBarDraw.TitleHeight;
			set
			{
				if (this.titleBarDraw.TitleHeight != value)
				{
					this.titleBarDraw.TitleHeight = value;
				}
			}
		}

		/// <summary>
		/// 获取或设置标题栏的标题文本。
		/// </summary>
		[Browsable(true)]
		[Description("标题栏的标题文本。")]
		public override string Text
		{
			get => this.titleBarDraw.Text;
			set
			{
				if (this.titleBarDraw.Text != value)
				{
					this.titleBarDraw.Text = value;
				}
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示是否注册按钮的单击事件。
		/// </summary>
		[Browsable(true)]
		[Description("是否注册按钮的单击事件。")]
		[DefaultValue(true)]
		public bool IsRegisteredSystemButtonClick
		{
			get => this.titleBarDraw.IsRegisteredSystemButtonClick;
			set
			{
				if (this.titleBarDraw.IsRegisteredSystemButtonClick != value)
				{
					this.titleBarDraw.IsRegisteredSystemButtonClick = value;
				}
			}
		}
		#endregion

		#region 事件定义
		/// <summary>
		/// 自定义标题按钮的单击事件
		/// </summary>
		public event TitleButtonClickEventHandler TitleButtonClick;
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkTitleBar()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
						  ControlStyles.DoubleBuffer |                  //双缓冲
						  ControlStyles.OptimizedDoubleBuffer |         //双缓冲
						  ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
						  ControlStyles.SupportsTransparentBackColor |  //透明
						  ControlStyles.UserPaint, true);
			this.Init();
		}
		#endregion

		#region 事件重写
		/// <summary>
		/// 绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			this.titleBarDraw.Draw(e);
		}
		#endregion

		private void Init()
		{
			this.Theme = new SparkTitleBarTheme(this);
			this.titleBarDraw = new SparkTitleBarDraw(this, this.Theme);
			this.titleBarDraw.TitleButtonClick += this.TitleBarDraw_TitleButtonClick;
		}

		private void TitleBarDraw_TitleButtonClick(object sender, TitleButtonClickEventArgs e)
		{
			TitleButtonClick?.Invoke(sender, e);
		}
	}
}