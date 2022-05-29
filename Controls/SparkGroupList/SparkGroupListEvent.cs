using System;

namespace SparkControls.Controls
{
	/// <summary>
	/// <see cref="SparkGroupList"/> 控件单击事件委托。
	/// </summary>
	/// <param name="sender">事件发送对象。</param>
	/// <param name="e">包含事件数据的 <see cref="SparkGroupListItemClickEventArgs"/>。</param>
	public delegate void SparkGroupListItemClickEventHandler(object sender, SparkGroupListItemClickEventArgs e);

	/// <summary>
	/// <see cref="SparkGroupListItemClickEventHandler"/> 事件参数类。
	/// </summary>
	public class SparkGroupListItemClickEventArgs : EventArgs
	{
		/// <summary>
		/// 单击的图标的项
		/// </summary>
		public SparkGroupListImageButton ImageItem { get; }

		/// <summary>
		/// 单击的项
		/// </summary>
		public SparkGroupListItem Item { get; }

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="item"></param>
		/// <param name="imageItem"></param>
		public SparkGroupListItemClickEventArgs(SparkGroupListItem item, SparkGroupListImageButton imageItem)
		{
			Item = item;
			ImageItem = imageItem;
		}
	}
}