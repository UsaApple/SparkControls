using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 控件类的基类
	/// </summary>
	[ToolboxItem(false)]
	public class SparkControl : Control
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkControl()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
						  ControlStyles.DoubleBuffer |                  //双缓冲
						  ControlStyles.OptimizedDoubleBuffer |         //双缓冲
						  ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
						  ControlStyles.SupportsTransparentBackColor |  //透明
						  ControlStyles.UserPaint, true);
			this.Theme = new SparkThemeBase(this);
			base.Font = this.font;
		}

		#region 属性
		private Font font = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文本的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get => font;
			set
			{
				if (font != value)
				{
					base.Font = font = value;
					OnFontChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 获取或设置控件的背景色。
		/// </summary>
		[Category("Spark"), Description("控件的背景色。")]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 获取或设置控件的前景色。
		/// </summary>
		[Category("Spark"), Description("控件的前景色。")]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}
		#endregion

		#region ISparkTheme接口
		/// <summary>
		/// 主题
		/// </summary>
		[Category("Spark"), Description("控件的主题。")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public SparkThemeBase Theme { get; private set; }
		#endregion
	}
}