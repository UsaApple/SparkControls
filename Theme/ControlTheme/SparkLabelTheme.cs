using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 文本标签控件的样式
	/// </summary>
	public class SparkLabelTheme : SparkThemeBase
	{
		private Color backColor = SparkThemeConsts.LabelBackColor;
		/// <summary>
		/// 初始 <see cref="SparkLabelTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkLabelTheme(Control control) : base(control)
		{

		}

		/// <summary>
		/// 背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.LabelBackColorString)]
		[Description("背景色")]
		public override Color BackColor
		{
			get => this.backColor;
			set
			{
				if (this.backColor != value)
				{
					Color oldValue = this.backColor;
					this.backColor = value;
					this.OnPropertyChanged(nameof(this.BackColor), oldValue, this.backColor);
				}
			}
		}
	}
}
