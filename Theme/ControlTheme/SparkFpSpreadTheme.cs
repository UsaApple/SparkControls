using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	public class SparkFpSpreadTheme : SparkEditTheme
	{
		/// <summary>
		/// 初始 <see cref="SparkFpSpreadTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkFpSpreadTheme(Control control) : base(control)
		{
		}

		private Color selectedBackColor = Color.FromArgb(86, 236, 172);
		/// <summary>
		/// 获取或设置选中状态的背景色。
		/// </summary>
		[DefaultValue(typeof(Color), "86, 236, 172")]
		[Description("选中状态的背景色。")]
		public override Color SelectedBackColor
		{
			get => selectedBackColor;
			set
			{
				if (selectedBackColor != value)
				{
					var oldValue = selectedBackColor;
					selectedBackColor = value;
					OnPropertyChanged(nameof(SelectedBackColor), oldValue, selectedBackColor);
				}
			}
		}

		private Color selectedForeColor = Color.Black;
		/// <summary>
		/// 获取或设置控件选中状态的的前景色。
		/// </summary>
		[DefaultValue(typeof(Color), "Black")]
		[Description("选中状态的的前景色。")]
		public override Color SelectedForeColor
		{
			get => selectedForeColor;
			set
			{
				if (selectedForeColor != value)
				{
					var oldValue = selectedForeColor;
					selectedForeColor = value;
					OnPropertyChanged(nameof(SelectedForeColor), oldValue, selectedForeColor);
				}
			}
		}
	}
}