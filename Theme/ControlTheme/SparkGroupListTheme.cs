using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 分组列表控件的样式类。
	/// </summary>
	public class SparkGroupListTheme : SparkEditTheme
	{
		private Color groupBackColor = SparkThemeConsts.GroupBackColor;
		private Color groupBorderColor = SparkThemeConsts.GroupBorderColor;

		/// <summary>
		/// 初始 <see cref="SparkGroupListTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkGroupListTheme(Control control) : base(control)
		{
		}

		/// <summary>
		/// 组的背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.GroupBackColorString)]
		[Description("组的背景色")]
		public virtual Color GroupBackColor
		{
			get => this.groupBackColor;
			set
			{
				if (this.groupBackColor != value)
				{
					Color oldValue = this.groupBackColor;
					this.groupBackColor = value;
					this.OnPropertyChanged(nameof(this.BackColor), oldValue, this.groupBackColor);
				}
			}
		}

		/// <summary>
		/// 组的边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.GroupBorderColorString)]
		[Description("边框颜色")]
		public virtual Color GroupBorderColor
		{
			get => this.groupBorderColor;
			set
			{
				if (this.groupBorderColor != value)
				{
					Color oldValue = this.groupBorderColor;
					this.groupBorderColor = value;
					this.OnPropertyChanged(nameof(this.GroupBorderColor), oldValue, this.groupBorderColor);
				}
			}
		}
	}
}