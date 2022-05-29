using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// SplitContainer容器控件
	/// </summary>
	[ToolboxBitmap(typeof(SplitContainer))]
	[ToolboxItem(true)]
	public class SparkSplitContainer : SplitContainer, ISparkTheme<SparkSplitContainerTheme>
	{
		#region 私有变量
		private Font font = Consts.DEFAULT_FONT;
		#endregion

		#region 属性
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
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkSplitContainer()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
						  ControlStyles.DoubleBuffer |                  //双缓冲
						  ControlStyles.OptimizedDoubleBuffer |         //双缓冲
						  ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
						  ControlStyles.SupportsTransparentBackColor |  //透明
						  ControlStyles.UserPaint, true
			);
			base.BorderStyle = BorderStyle.None;
			this.Font = this.font;
			this.Theme = new SparkSplitContainerTheme(this);
		}
		#endregion

		#region 重写事件
		/// <summary>
		/// 绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.BorderStyle != BorderStyle.None)
			{
				GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
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
		public SparkSplitContainerTheme Theme { get; private set; }

		#endregion
	}
}