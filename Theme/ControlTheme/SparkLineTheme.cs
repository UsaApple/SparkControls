using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 线条的样式
	/// </summary>
	public class SparkLineTheme : SparkThemeBase
	{
		/// <summary>
		/// 初始 <see cref="SparkLineTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkLineTheme(Control control) : base(control)
		{
			this.BorderColor = SparkThemeConsts.LineBorderColor;
		}

		/// <summary>
		/// 边框颜色
		/// </summary>

		[DefaultValue(typeof(Color), SparkThemeConsts.LineBorderColorString), Description("线的颜色")]
		public override Color BorderColor { get => base.BorderColor; set => base.BorderColor = value; }

		/// <summary>
		/// 背景色
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get => base.BackColor; }

		/// <summary>
		/// 线的颜色
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get => base.ForeColor; }
	}
}