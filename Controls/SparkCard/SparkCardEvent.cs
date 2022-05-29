using System.Drawing;

namespace SparkControls.Controls
{
	/// <summary>
	/// 定义卡片事件参数
	/// </summary>
	public class SparkCardEventArgs
	{
		/// <summary>
		/// 获取当前卡片对象
		/// </summary>
		public SparkCard Card { get; } = null;

		/// <summary>
		/// 初始 <see cref="SparkCardEventArgs"/> 类型的新实例。
		/// </summary>
		/// <param name="card">卡片对象</param>
		public SparkCardEventArgs(SparkCard card)
		{
			this.Card = card;
		}
	}

	/// <summary>
	/// 定义绘制卡片项事件参数
	/// </summary>
	public class SparkCardDrawItemEventArgs
	{
		/// <summary>
		/// 获取当前卡片对象。
		/// </summary>
		public SparkCardTemplate Template { get; set; } = null;

		/// <summary>
		/// 获取要在其上绘制项的图形表面。
		/// </summary>
		public Graphics Graphics { get; } = null;

		/// <summary>
		/// 获取当前卡片索引，从 0 开始计。
		/// </summary>
		public int Index { get; } = -1;

		/// <summary>
		/// 获取当前卡片位置。
		/// </summary>
		public Point Location { get; } = Point.Empty;

		/// <summary>
		/// 获取当前卡片的数据源。
		/// </summary>
		public object DataSource { get; }

		/// <summary>
		/// 初始 <see cref="SparkCardDrawItemEventArgs"/> 类型的新实例。
		/// </summary>
		/// <param name="graphics">绘图图面。</param>
		/// <param name="index">卡片索引。</param>
		/// <param name="location">卡片位置。</param>
		public SparkCardDrawItemEventArgs(Graphics graphics, int index, Point location, object dataSource)
		{
			this.Graphics = graphics;
			this.Index = index;
			this.Location = location;
			this.DataSource = dataSource;
		}
	}

	/// <summary>
	/// 定义卡片事件委托
	/// </summary>
	/// <param name="sender">触发对象</param>
	/// <param name="e">事件参数</param>
	public delegate void SparkCardEventHandler(object sender, SparkCardEventArgs e);

	/// <summary>
	/// 定义绘制卡片项事件委托
	/// </summary>
	/// <param name="sender">触发对象</param>
	/// <param name="e">事件参数</param>
	public delegate void SparkCardDrawItemEventHandler(object sender, SparkCardDrawItemEventArgs e);
}