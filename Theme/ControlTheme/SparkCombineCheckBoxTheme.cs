using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 组合的CheckBox 控件样式类。
	/// </summary>
	public class SparkCombineCheckBoxTheme : SparkCheckBoxTheme
	{
		private Color combineSelectedColor = SparkThemeConsts.CombinedCheckBoxSelectedColor;
		private Color combineBackColor = SparkThemeConsts.CombinedCheckBoxBackColor;
		/// <summary>
		/// 组合选中的颜色，包含边框和勾勾
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.CombinedCheckBoxSelectedColorString)]
		[Description("组合选中的颜色，包含边框和勾勾")]
		public virtual Color CombinedSelectedColor
		{
			get => this.combineSelectedColor;
			set
			{
				if (this.combineSelectedColor != value)
				{
					Color oldValue = this.combineSelectedColor;
					this.combineSelectedColor = value;
					this.OnPropertyChanged(nameof(this.CombinedSelectedColor), oldValue, this.combineSelectedColor);
				}
			}
		}

		/// <summary>
		/// 组合选中时的背景颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.CombinedCheckBoxBackColorString)]
		[Description("组合选中时的背景的颜色")]
		public virtual Color CombinedBackColor
		{
			get => this.combineBackColor;
			set
			{
				if (this.combineBackColor != value)
				{
					Color oldValue = this.combineBackColor;
					this.combineBackColor = value;
					this.OnPropertyChanged(nameof(this.CombinedBackColor), oldValue, this.combineBackColor);
				}
			}
		}

		/// <summary>
		/// 初始 <see cref="SparkCombineCheckBoxTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkCombineCheckBoxTheme(Control control) : base(control)
		{

		}
	}
}