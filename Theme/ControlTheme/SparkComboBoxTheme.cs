using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// ComboBox 控件样式类。
	/// </summary>
	public class SparkComboBoxTheme : SparkTextBoxTheme
	{
		private Color buttonForeColor = SparkThemeConsts.ComboBoxButtonForeColor;
		private Color buttonBackColor = SparkThemeConsts.ComboBoxButtonBackColor;

		/// <summary>
		/// 按钮字体颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ComboBoxButtonForeColorString)]
		[Description("按钮字体颜色")]
		public Color ButtonForeColor
		{
			get => this.buttonForeColor;
			set
			{
				if (this.buttonForeColor != value)
				{
					Color oldValue = this.buttonForeColor;
					this.buttonForeColor = value;
					this.OnPropertyChanged(nameof(this.ButtonForeColor), oldValue, this.buttonForeColor);
				}
			}
		}
		/// <summary>
		/// 按钮背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ComboBoxButtonBackColorString)]
		[Description("按钮背景色")]
		public Color ButtonBackColor
		{
			get => this.buttonBackColor;
			set
			{
				if (this.buttonBackColor != value)
				{
					Color oldValue = this.buttonBackColor;
					this.buttonBackColor = value;
					this.OnPropertyChanged(nameof(this.ButtonBackColor), oldValue, this.buttonBackColor);
				}
			}
		}

		/// <summary>
		/// 初始 <see cref="SparkComboBoxTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkComboBoxTheme(Control control) : base(control)
		{
		}
	}
}