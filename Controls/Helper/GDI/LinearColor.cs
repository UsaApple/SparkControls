using System.Drawing;

namespace SparkControls.Controls
{
	/// <summary>
	/// 线性色彩
	/// </summary>
	internal class LinearColor
	{
		#region Properties

		/// <summary>
		/// 线性色彩1
		/// </summary>
		public Color First;

		/// <summary>
		/// 线性色彩2
		/// </summary>
		public Color Second;

		#endregion

		#region Initializes

		/// <summary>
		/// 初始化 <see cref="LinearColor"/> 结构的新实例。.
		/// </summary>
		/// <param name="color1">色彩1。</param>
		/// <param name="color2">色彩2。</param>
		public LinearColor(Color color1, Color color2)
		{
			this.First = color1;
			this.Second = color2;
		}

		#endregion
	}
}