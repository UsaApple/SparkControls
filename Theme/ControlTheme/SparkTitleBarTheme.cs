using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 标题栏控件的主题属性
	/// </summary>
	public class SparkTitleBarTheme : SparkThemeBase
	{
		private Color backColor = SparkThemeConsts.TitleBackColor;
		private Color borderColor = SparkThemeConsts.TitleBorderColor;
		private Color foreColor = SparkThemeConsts.TitleForeColor;
		private Color titleMouseOverBackColor = SparkThemeConsts.TitleMouseOverBackColor;
		private Color titleMouseDownBackColor = SparkThemeConsts.TitleMouseDownBackColor;

		/// <summary>
		/// 初始 <see cref="SparkTitleBarTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkTitleBarTheme(Control control) : base(control)
		{
		}

		/// <summary>
		/// 背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TitleBackColorString)]
		[Description("标题栏背景色")]
		public override Color BackColor
		{
			get => this.backColor;
			set
			{
				if (this.backColor != value)
				{
					Color oldValue = this.backColor;
					base.BackColor = this.backColor = value;
					this.OnPropertyChanged(nameof(this.BackColor), oldValue, this.backColor);
				}
			}
		}

		/// <summary>
		/// 边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TitleBorderColorString)]
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

		/// <summary>
		/// 前景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TitleForeColorString)]
		[Description("前景色")]
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
		/// 鼠标进入时颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TitleMouseOverBackColorString)]
		[Description("鼠标进入时颜色")]
		public Color TitleMouseOverBackColor
		{
			get => this.titleMouseOverBackColor;
			set
			{
				if (this.titleMouseOverBackColor != value)
				{
					Color oldValue = this.titleMouseOverBackColor;
					this.titleMouseOverBackColor = value;
					this.OnPropertyChanged(nameof(this.TitleMouseOverBackColor), oldValue, this.titleMouseOverBackColor);
				}
			}
		}

		/// <summary>
		/// 鼠标点击后颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TitleMouseDownBackColorString)]
		[Description("鼠标点击后颜色")]
		public Color TitleMouseDownBackColor
		{
			get => this.titleMouseDownBackColor;
			set
			{
				if (this.titleMouseDownBackColor != value)
				{
					Color oldValue = this.titleMouseDownBackColor;
					this.titleMouseDownBackColor = value;
					this.OnPropertyChanged(nameof(this.TitleMouseDownBackColor), oldValue, this.titleMouseDownBackColor);
				}
			}
		}
	}
}