using System.Drawing;

namespace SparkControls.Controls
{
	public static class Extensions
	{
		/// <summary>
		/// 转换为文本字符串的对齐方式。
		/// </summary>
		/// <param name="ca">绘图图面上的内容对齐方式。</param>
		/// <param name="hsa">水平对齐方式。</param>
		/// <param name="vsa">垂直对齐方式。</param>
		public static void ToStringAlignment(this ContentAlignment ca, out StringAlignment hsa, out StringAlignment vsa)
		{
			switch (ca)
			{
				case ContentAlignment.BottomLeft:
					hsa = StringAlignment.Near;
					vsa = StringAlignment.Far;
					break;
				case ContentAlignment.MiddleLeft:
					hsa = StringAlignment.Near;
					vsa = StringAlignment.Center;
					break;
				case ContentAlignment.TopLeft:
					hsa = StringAlignment.Near;
					vsa = StringAlignment.Near;
					break;
				case ContentAlignment.BottomCenter:
					hsa = StringAlignment.Center;
					vsa = StringAlignment.Far;
					break;
				case ContentAlignment.MiddleCenter:
					hsa = StringAlignment.Center;
					vsa = StringAlignment.Center;
					break;
				case ContentAlignment.TopCenter:
					hsa = StringAlignment.Center;
					vsa = StringAlignment.Near;
					break;
				case ContentAlignment.BottomRight:
					hsa = StringAlignment.Far;
					vsa = StringAlignment.Far;
					break;
				case ContentAlignment.MiddleRight:
					hsa = StringAlignment.Far;
					vsa = StringAlignment.Center;
					break;
				case ContentAlignment.TopRight:
					hsa = StringAlignment.Far;
					vsa = StringAlignment.Near;
					break;
				default:
					hsa = StringAlignment.Near;
					vsa = StringAlignment.Far;
					break;
			}
		}
	}
}