using System.Drawing;

namespace SparkControls.Controls
{
	/// <summary>
	/// 梯度渐变色彩
	/// </summary>
	public struct GradientColor
	{
		/// <summary>
		/// 初始色彩
		/// </summary>
		public Color First;

		/// <summary>
		/// 结束色彩
		/// </summary>
		public Color Second;

		/// <summary>
		/// 色彩渲染系数（0到1的浮点数值）
		/// </summary>
		public float[] Factors;

		/// <summary>
		/// 色彩渲染位置（0到1的浮点数值）
		/// </summary>
		public float[] Positions;

		/// <summary>
		/// 初始化 <see cref="GradientColor"/> 结构的新实例。
		/// </summary>
		/// <param name="color1">色彩1。</param>
		/// <param name="color2">色彩2。</param>
		/// <param name="factors">渲染系数。</param>
		/// <param name="positions">渲染位置。</param>
		public GradientColor(Color color1, Color color2, float[] factors, float[] positions)
		{
			this.First = color1;
			this.Second = color2;
			this.Factors = factors ?? (new float[] { });
			this.Positions = positions ?? (new float[] { });
		}

		public override bool Equals(object obj)
		{
			if (obj is GradientColor gc)
			{
				return First == gc.First && Second == gc.Second && Factors == gc.Factors && Positions == gc.Positions;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return $"{First}_{Second}_{string.Join("^", Factors)}_{string.Join("^", Positions)}".GetHashCode();
		}

		public static bool operator ==(GradientColor left, GradientColor right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(GradientColor left, GradientColor right)
		{
			return !(left == right);
		}
	}
}