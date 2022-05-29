using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 选项卡控件中关闭按钮的样式
	/// </summary>
	public class SparkTabControlCloseButtonTheme : SparkThemeBase
	{
		private Color backColor = SparkThemeConsts.TabCloseButtonBackColor;
		private Color borderColor = SparkThemeConsts.TabCloseButtonBorderColor;
		private Color foreColor = SparkThemeConsts.TabCloseButtonForeColor;
		private Color mouseOverBackColor = SparkThemeConsts.TabCloseButtonMouseOverBackColor;
		private Color mouseOverBorderColor = SparkThemeConsts.TabCloseButtonMouseOverBorderColor;
		private Color mouseOverForeColor = SparkThemeConsts.TabCloseButtonMouseOverForeColor;

		/// <summary>
		/// 初始 <see cref="SparkTabControlCloseButtonTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkTabControlCloseButtonTheme(Control control) : base(control)
		{

		}

		/// <summary>
		/// 关闭按钮默认背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TabCloseButtonBackColorString)]
		[Description("关闭按钮默认背景色")]
		public override Color BackColor
		{
			get => this.backColor;
			set => base.BackColor = this.backColor = value;
		}

		/// <summary>
		/// 关闭按钮边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TabCloseButtonBorderColorString)]
		[Description("关闭按钮边框颜色")]
		public override Color BorderColor
		{
			get => this.borderColor;
			set => base.BorderColor = this.borderColor = value;
		}

		/// <summary>
		/// X的颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TabCloseButtonForeColorString)]
		[Description("X的颜色")]
		public override Color ForeColor
		{
			get => this.foreColor;
			set
			{
				if (this.foreColor != value)
				{
					Color oldValue = this.foreColor;
					this.foreColor = value;
					this.OnPropertyChanged(nameof(this.ForeColor), oldValue, this.foreColor);
				}
			}
		}

		/// <summary>
		/// 鼠标进入背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TabCloseButtonMouseOverBackColorString)]
		[Description("鼠标进入背景色")]
		public virtual Color MouseOverBackColor
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
		/// 鼠标进入X的颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TabCloseButtonMouseOverForeColorString)]
		[Description("鼠标进入X的颜色")]
		public virtual Color MouseOverForeColor
		{
			get => this.mouseOverForeColor;
			set
			{
				if (this.mouseOverForeColor != value)
				{
					Color oldValue = this.mouseOverForeColor;
					this.mouseOverForeColor = value;
					this.OnPropertyChanged(nameof(this.MouseOverForeColor), oldValue, this.mouseOverForeColor);
				}
			}
		}

		/// <summary>
		/// 鼠标进入边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TabCloseButtonMouseOverBorderColorString)]
		[Description("鼠标进入边框色")]
		public virtual Color MouseOverBorderColor
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

	}
}
