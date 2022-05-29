using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// CheckBox 控件样式类。
	/// </summary>
	public class SparkCheckBoxTheme : SparkEditTheme
	{
		private Color mTickColor = SparkThemeConsts.CheckBoxTickColor;
		private Color mIndeterminateBackColor = SparkThemeConsts.CheckBoxIndeterminateBackColor;
		private Color mFocusedBorderColor = SparkThemeConsts.CheckBoxFocusedBorderColor;

		private Color mouseOverBackColor = SparkThemeConsts.CheckBoxMouseOverBackColor;
		private Color mouseDownBackColor = SparkThemeConsts.CheckBoxMouseDownBackColor;
		private Color selectedBackColor = SparkThemeConsts.CheckBoxSelectedBackColor;

		private Color mouseOverBorderColor = SparkThemeConsts.CheckBoxMouseOverBorderColor;
		private Color mouseDownBorderColor = SparkThemeConsts.CheckBoxMouseDownBorderColor;
		private Color selectedBorderColor = SparkThemeConsts.CheckBoxSelectedBorderColor;

		private Color mouseOverForeColor = SparkThemeConsts.CheckBoxMouseOverForeColor;
		private Color mouseDownForeColor = SparkThemeConsts.CheckBoxMouseDownForeColor;
		private Color selectedForeColor = SparkThemeConsts.CheckBoxSelectedForeColor;

		/// <summary>
		/// 勾勾颜色
		/// </summary>
		[Description("勾勾颜色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxTickColorString)]
		public virtual Color TickColor
		{
			get => this.mTickColor;
			set
			{
				if (this.mTickColor != value)
				{
					Color oldValue = this.mTickColor;
					this.mTickColor = value;
					this.OnPropertyChanged(nameof(this.TickColor), oldValue, this.mTickColor);
				}
			}
		}

		/// <summary>
		/// 不确定状态的背景色
		/// </summary>
		[Description("不确定状态的背景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxIndeterminateBackColorString)]
		public virtual Color IndeterminateBackColor
		{
			get => this.mIndeterminateBackColor;
			set
			{
				if (this.mIndeterminateBackColor != value)
				{
					Color oldValue = this.mIndeterminateBackColor;
					this.mIndeterminateBackColor = value;
					this.OnPropertyChanged(nameof(this.IndeterminateBackColor), oldValue, this.mIndeterminateBackColor);
				}
			}
		}

		/// <summary>
		/// 焦点状态的边框颜色
		/// </summary>
		[Description("焦点状态的边框颜色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxFocusedBorderColorString)]
		public virtual Color FocusedBorderColor
		{
			get => this.mFocusedBorderColor;
			set
			{
				if (this.mFocusedBorderColor != value)
				{
					Color oldValue = this.mFocusedBorderColor;
					this.mFocusedBorderColor = value;
					this.OnPropertyChanged(nameof(this.FocusedBorderColor), oldValue, this.mFocusedBorderColor);
				}
			}
		}

		/// <summary>
		/// 鼠标进入背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxMouseOverBackColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxMouseDownBackColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxSelectedBackColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxMouseOverBorderColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxMouseDownBorderColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxSelectedBorderColorString)]
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
		/// 鼠标进入字体颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxMouseOverForeColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxMouseDownForeColorString)]
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
		[DefaultValue(typeof(Color), SparkThemeConsts.CheckBoxSelectedForeColorString)]
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

		/// <summary>
		/// 初始 <see cref="SparkCheckBoxTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkCheckBoxTheme(Control control) : base(control)
		{
		}
	}
}