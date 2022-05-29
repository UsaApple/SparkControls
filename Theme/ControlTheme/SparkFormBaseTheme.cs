using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 窗口控件的样式
	/// </summary>
	public class SparkFormBaseTheme : SparkThemeBase
	{
		private Color borderColor = SparkThemeConsts.FormBorderColor;

		/// <summary>
		/// 标题栏的主题样式
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("标题栏的主题")]
		public SparkTitleBarTheme TitleTheme { get; private set; }

		/// <summary>
		/// 初始 <see cref="SparkFormBaseTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkFormBaseTheme(Control control) : base(control)
		{
			this.TitleTheme = new SparkTitleBarTheme(control);
		}

		/// <summary>
		/// 边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.FormBorderColorString)]
		[Description("边框颜色")]
		public override Color BorderColor
		{
			get => this.borderColor;
			set
			{
				if (this.borderColor != value)
				{
					Color oldValue = this.borderColor;
					this.borderColor = value;
					this.OnPropertyChanged(nameof(this.BorderColor), oldValue, this.borderColor);
				}
			}
		}
	}
}
