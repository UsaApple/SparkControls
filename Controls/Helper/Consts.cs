using System.Drawing;

namespace SparkControls.Controls
{
	public static class Consts
	{
		/// <summary>
		/// 勾选框的大小
		/// </summary>
		public static readonly Size CHECK_BOX_SIZE = new Size(14, 14);

		/// <summary>
		/// 系统默认字体
		/// </summary>
		public static readonly Font DEFAULT_FONT = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point);

		/// <summary>
		/// 默认粗体字体
		/// </summary>
		public static readonly Font DEFAULT_BOLD_FONT = new Font("微软雅黑", 10.5F, FontStyle.Bold, GraphicsUnit.Point);

		/// <summary>
		/// 默认字体的字符串表示
		/// </summary>
		public const string DEFAULT_FONT_STRING = "微软雅黑, 10.5pt";

		/// <summary>
		/// 默认字体字的符串表示 - 粗体
		/// </summary>
		public const string DEFAULT_BOLD_FONT_STRING = "微软雅黑, 10.5pt, style=Bold";
	}
}