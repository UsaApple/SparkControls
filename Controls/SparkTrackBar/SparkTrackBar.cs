using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 跟踪条控件。
	/// </summary>
	[ToolboxBitmap(typeof(TrackBar))]
	public class SparkTrackBar : TrackBar, ISparkTheme<SparkTrackBarTheme>
	{
		private Font font = Consts.DEFAULT_FONT;
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

		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkTrackBar()
		{
			base.Font = Consts.DEFAULT_FONT;
			this.Theme = new SparkTrackBarTheme(this);
		}

		/// <summary>
		/// 重写绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			//if (this.BorderStyle == BorderStyle.None) return;
			ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, this.Theme.BorderColor, ButtonBorderStyle.Solid);
		}

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkTrackBarTheme Theme { get; private set; }

		#endregion
	}
}