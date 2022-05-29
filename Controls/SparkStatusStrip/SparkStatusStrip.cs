using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Controls
{
	/// <summary>
	/// 状态栏
	/// </summary>
	[ToolboxBitmap(typeof(StatusStrip))]
	public class SparkStatusStrip : StatusStrip, ISparkTheme<SparkToolStripTheme>
	{
		#region 变量
		private readonly ToolStripRenderer _customRenderer;
		#endregion

		#region 构造方法
		/// <summary>
		///构造方法
		/// </summary>
		public SparkStatusStrip() : base()
		{
			this.Theme = new SparkToolStripTheme(this);
			Renderer = _customRenderer = new SparkToolStripRenderer(Theme);
			Font = mFont;
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
			get => mFont;
			set
			{
				base.Font = mFont = value;
				PerformLayout();
				Invalidate();
				OnFontChanged(EventArgs.Empty);
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
		/// 获取或设置一个值，该值指示将把哪种视觉样式应用到 <see cref="SparkStatusStrip"/>。
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
					Renderer = _customRenderer;
				}
				Invalidate();
			}
		}
		#endregion

		#region 重写事件
		/// <summary>
		/// 重写WndProc
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (!this.DesignMode && m.Msg == (int)Msgs.WM_NCHITTEST && SizeGripBounds.Contains(this.PointToClient(Cursor.Position)))
			{
				m.Result = (IntPtr)(-1);
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