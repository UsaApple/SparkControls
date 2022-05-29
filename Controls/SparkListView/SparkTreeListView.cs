using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 树表格控件。
	/// </summary>
	[ToolboxBitmap(typeof(TreeView))]
	[ToolboxItem(true)]
	public class SparkTreeListView : TreeListView, ISparkTheme<SparkTreeListViewTheme>
	{
		#region 字段
		#endregion

		#region 构造函数

		/// <summary>
		/// 初始 <see cref="SparkTreeListView"/> 类型的新实例。
		/// </summary>
		public SparkTreeListView() : base()
		{
			this.DoubleBuffered = true;
			//this.SetStyle(ControlStyles.ResizeRedraw |                      // 调整大小时重绘
			//              ControlStyles.DoubleBuffer |                      // 双缓冲
			//              ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
			//              ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
			//              ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
			//              ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
			//);
			this.Theme = new SparkTreeListViewTheme(this);
			base.BorderStyle = BorderStyle.FixedSingle;
			this.UseTranslucentHotItem = false;
			this.UseExplorerTheme = false;
			this.UseHotItem = true;
			HotItemStyle hotItemStyle = new HotItemStyle
			{
				ForeColor = this.Theme.MouseOverForeColor,
				BackColor = this.Theme.MouseOverBackColor
			};
			this.HotItemStyle = hotItemStyle;

			//线的颜色
			if (this.TreeColumnRenderer != null)
			{
				this.TreeColumnRenderer.LinePen = new Pen(this.Theme.BorderColor, 1.0f);
			}
			base.Font = this.mFont;
		}

		#endregion

		#region 属性

		private Font mFont = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文本的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get { return this.mFont; }
			set
			{
				base.Font = this.mFont = value;
				this.PerformLayout();
				this.Invalidate();
				this.OnFontChanged(EventArgs.Empty);
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

		/// <summary>
		///  获取或设置控件的边框样式。
		/// </summary>
		[DefaultValue(BorderStyle.FixedSingle)]
		public new BorderStyle BorderStyle
		{
			get => base.BorderStyle;
			set
			{
				if (base.BorderStyle != value)
				{
					base.BorderStyle = value;
				}
			}
		}
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public override SparkTreeListViewTheme Theme { get; }

		#endregion
	}
}