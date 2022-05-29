using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 按钮主题类
	/// </summary>
	public sealed class SparkButtonTheme : SparkEditTheme
	{
		private Color mouseOverBackColor = SparkThemeConsts.ButtonMouseOverBackColor;
		private Color mouseDownBackColor = SparkThemeConsts.ButtonMouseDownBackColor;
		private Color selectedBackColor = SparkThemeConsts.ButtonSelectedBackColor;

		private Color mouseOverBorderColor = SparkThemeConsts.ButtonMouseOverBorderColor;
		private Color mouseDownBorderColor = SparkThemeConsts.ButtonMouseDownBorderColor;
		private Color selectedBorderColor = SparkThemeConsts.ButtonSelectedBorderColor;

		private Color mouseOverForeColor = SparkThemeConsts.ButtonMouseOverForeColor;
		private Color mouseDownForeColor = SparkThemeConsts.ButtonMouseDownForeColor;
		private Color selectedForeColor = SparkThemeConsts.ButtonSelectedForeColor;

		private Color disabledBackColor = SparkThemeConsts.ButtonDisabledBackColor;

		/// <summary>
		/// 初始 <see cref="SparkButtonTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkButtonTheme(Control control) : base(control)
		{
		}

		/// <summary>
		/// 鼠标进入背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonMouseOverBackColorString)]
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
		/// 鼠标点击背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonMouseDownBackColorString)]
		[Description("鼠标点击背景色")]
		public override Color MouseDownBackColor
		{
			get => this.mouseDownBackColor;
			set
			{
				if (this.mouseDownBackColor != value)
				{
					Color oldValue = this.mouseDownBackColor;
					this.mouseDownBackColor = value;
					this.OnPropertyChanged(nameof(this.MouseDownBackColor), oldValue, this.mouseDownBackColor);
				}
			}
		}

		/// <summary>
		/// 选中背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonSelectedBackColorString)]
		[Description("选中背景色")]
		public override Color SelectedBackColor
		{
			get => this.selectedBackColor;
			set
			{
				if (this.selectedBackColor != value)
				{
					Color oldValue = this.selectedBackColor;
					this.selectedBackColor = value;
					this.OnPropertyChanged(nameof(this.SelectedBackColor), oldValue, this.selectedBackColor);
				}
			}
		}

		/// <summary>
		/// 鼠标进入边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonMouseOverBorderColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonMouseDownBorderColorString)]
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
		/// 选中边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonSelectedBorderColorString)]
		[Description("选中边框色")]
		public override Color SelectedBorderColor
		{
			get => this.selectedBorderColor;
			set
			{
				if (this.selectedBorderColor != value)
				{
					Color oldValue = this.selectedBorderColor;
					this.selectedBorderColor = value;
					this.OnPropertyChanged(nameof(this.SelectedBorderColor), oldValue, this.selectedBorderColor);
				}
			}
		}

		/// <summary>
		/// 禁用颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonDisabledBackColorString)]
		[Description("控件不可用状态的背景色。")]
		public override Color DisabledBackColor
		{
			get => this.disabledBackColor;
			set
			{
				if (this.disabledBackColor != value)
				{
					Color oldValue = this.disabledBackColor;
					this.disabledBackColor = value;
					this.OnPropertyChanged(nameof(this.DisabledBackColor), oldValue, this.disabledBackColor);
				}
			}
		}

		/// <summary>
		/// 鼠标进入字体颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonMouseOverForeColorString)]
		[Description("鼠标进入字体颜色")]
		public override Color MouseOverForeColor
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
		/// 鼠标点击的字体颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonMouseDownForeColorString)]
		[Description("鼠标点击的字体颜色")]
		public override Color MouseDownForeColor
		{
			get => this.mouseDownForeColor;
			set
			{
				if (this.mouseDownForeColor != value)
				{
					Color oldValue = this.mouseDownForeColor;
					this.mouseDownForeColor = value;
					this.OnPropertyChanged(nameof(this.MouseDownForeColor), oldValue, this.mouseDownForeColor);
				}
			}
		}

		/// <summary>
		/// 选中的字体颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ButtonSelectedForeColorString)]
		[Description("选中的字体颜色")]
		public override Color SelectedForeColor
		{
			get => this.selectedForeColor;
			set
			{
				if (this.selectedForeColor != value)
				{
					Color oldValue = this.selectedForeColor;
					this.selectedForeColor = value;
					this.OnPropertyChanged(nameof(this.SelectedForeColor), oldValue, this.selectedForeColor);
				}
			}
		}
	}
}