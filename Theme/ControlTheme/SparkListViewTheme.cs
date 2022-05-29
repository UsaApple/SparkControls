using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 列表视图控件的样式
	/// </summary>
	public class SparkListViewTheme : SparkThemeBase
	{
		/// <summary>
		/// 初始 <see cref="SparkListViewTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkListViewTheme(Control control) : base(control)
		{
			this.BorderColor = SparkThemeConsts.LineBorderColor;
		}
	}
}