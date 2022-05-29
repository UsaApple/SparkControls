using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 工具栏控件。
	/// </summary>
	[ToolboxBitmap(typeof(ToolStrip))]
	public class SparkToolStrip : ToolStrip, ISparkTheme<SparkToolStripTheme>
	{
		#region 变量
		private readonly ToolStripRenderer _customRenderer;
		#endregion

		#region 构造方法
		/// <summary>
		///构造方法
		/// </summary>
		public SparkToolStrip() : base()
		{
			this.Theme = new SparkToolStripTheme(this);
			this.Renderer = this._customRenderer = new SparkToolStripRenderer(this.Theme);
			this.Font = this.mFont;
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
		public new Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 获取或设置控件的前景色。
		/// </summary>
		[Category("Spark"), Description("控件的前景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
		public new Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		/// <summary>
		/// 获取或设置一个值，该值指示将把哪种视觉样式应用到 <see cref="SparkToolStrip"/>。
		/// </summary>
		[Category("Spark"), Description("应用于控件的绘制样式。")]
		[DefaultValue(typeof(ToolStripRenderMode), "Custom")]
		public new ToolStripRenderMode RenderMode
		{
			get => base.RenderMode;
			set
			{
				base.RenderMode = value;
				if (value == ToolStripRenderMode.ManagerRenderMode)
				{
					this.Renderer = this._customRenderer;
				}
				this.Invalidate();
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
		public SparkToolStripTheme Theme { get; private set; }

		#endregion
	}
}