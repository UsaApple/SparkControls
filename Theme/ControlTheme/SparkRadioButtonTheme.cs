using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// RadioButton 控件主题类。
	/// </summary>
	public class SparkRadioButtonTheme : SparkEditTheme
	{
		private int outerBorderWidth = 2;
		private int innerBorderWidth = 1;
		private Color centreForeColor = SparkThemeConsts.RadioButtonCentreForeColor;
		private Color focusedBorderColor = SparkThemeConsts.RadioButtonFocusedBorderColor;

		/// <summary>
		/// 外圆边框宽度
		/// </summary>
		[DefaultValue(2)]
		[Description("外圆边框宽度")]
		public int OuterBorderWidth
		{
			get => this.outerBorderWidth;
			set
			{
				if (this.outerBorderWidth != value)
				{
					int oldValue = this.outerBorderWidth;
					this.outerBorderWidth = value;
					this.OnPropertyChanged(nameof(this.OuterBorderWidth), oldValue, this.outerBorderWidth);
				}
			}
		}

		/// <summary>
		/// 内圆边框宽度
		/// </summary>
		[DefaultValue(1)]
		[Description("内圆边框宽度")]
		public int InnerBorderWidth
		{
			get => this.innerBorderWidth;
			set
			{
				if (this.innerBorderWidth != value)
				{
					int oldValue = this.innerBorderWidth;
					this.innerBorderWidth = value;
					this.OnPropertyChanged(nameof(this.InnerBorderWidth), oldValue, this.innerBorderWidth);
				}
			}
		}

		/// <summary>
		/// 选中圆心颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.RadioButtonCentreForeColorString)]
		[Description("选中圆心颜色")]
		public Color CentreForeColor
		{
			get => this.centreForeColor;
			set
			{
				if (this.centreForeColor != value)
				{
					Color oldValue = this.centreForeColor;
					this.centreForeColor = value;
					this.OnPropertyChanged(nameof(this.CentreForeColor), oldValue, this.centreForeColor);
				}
			}
		}

		/// <summary>
		/// 焦点状态的边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.RadioButtonFocusedBorderColorString)]
		[Description("焦点状态的边框颜色")]
		public Color FocusedBorderColor
		{
			get => this.focusedBorderColor;
			set
			{
				if (this.focusedBorderColor != value)
				{
					Color oldValue = this.focusedBorderColor;
					this.focusedBorderColor = value;
					this.OnPropertyChanged(nameof(this.FocusedBorderColor), oldValue, this.focusedBorderColor);
				}
			}
		}

		/// <summary>
		/// 初始 <see cref="SparkRadioButtonTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkRadioButtonTheme(Control control) : base(control)
		{
		}
	}
}