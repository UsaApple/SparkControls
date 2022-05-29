using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 文本框样式类
	/// </summary>
	public class SparkTextBoxTheme : SparkEditTheme
	{
		private Color mouseOverBorderColor = SparkThemeConsts.TextBoxMouseOverBorderColor;
		private Color mouseDownBorderColor = SparkThemeConsts.TextBoxMouseDownBorderColor;
		private Color mouseOverBackColor = SparkThemeConsts.TextBoxMouseOverBackColor;

		private Color validatedFailBorderColor = SparkThemeConsts.TextBoxValidateFailedBorderColor;

		/// <summary>
		/// 初始 <see cref="SparkTextBoxTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkTextBoxTheme(Control control) : base(control) { }

		/// <summary>
		/// 鼠标进入边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TextBoxMouseOverBorderColorString)]
		[Description("鼠标进入边框色")]
		public override Color MouseOverBorderColor
		{
			get => this.mouseOverBorderColor;
			set
			{
				if (this.mouseOverBorderColor != value)
				{
					Color oldValue = this.mouseOverBorderColor;
					this.mouseOverBorderColor = value;
					this.OnPropertyChanged(nameof(this.MouseOverBorderColor), oldValue, this.mouseOverBorderColor);
				}
			}
		}
		/// <summary>
		/// 鼠标点击边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TextBoxMouseDownBorderColorString)]
		[Description("鼠标点击边框色")]
		public override Color MouseDownBorderColor
		{
			get => this.mouseDownBorderColor;
			set
			{
				if (this.mouseDownBorderColor != value)
				{
					Color oldValue = this.mouseDownBorderColor;
					this.mouseDownBorderColor = value;
					this.OnPropertyChanged(nameof(this.MouseDownBorderColor), oldValue, this.mouseDownBorderColor);
				}
			}
		}

		/// <summary>
		/// 鼠标进入背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TextBoxMouseOverBackColorString)]
		[Description("鼠标进入背景色")]
		public override Color MouseOverBackColor
		{
			get => this.mouseOverBackColor;
			set
			{
				if (this.mouseOverBackColor != value)
				{
					Color oldValue = this.mouseOverBackColor;
					this.mouseOverBackColor = value;
					this.OnPropertyChanged(nameof(this.MouseOverBackColor), oldValue, this.mouseOverBackColor);
				}
			}
		}

		/// <summary>
		/// 验证失败的边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TextBoxValidateFailedBorderColorString)]
		[Description("验证失败的边框颜色")]
		public virtual Color ValidatedFailBorderColor
		{
			get => this.validatedFailBorderColor;
			set
			{
				if (this.validatedFailBorderColor != value)
				{
					Color oldValue = this.validatedFailBorderColor;
					this.validatedFailBorderColor = value;
					this.OnPropertyChanged(nameof(this.ValidatedFailBorderColor), oldValue, this.validatedFailBorderColor);
				}
			}
		}
	}
}