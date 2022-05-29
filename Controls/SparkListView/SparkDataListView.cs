using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 数据列表控件。
	/// </summary>
	[ToolboxBitmap(typeof(ListView))]
	[ToolboxItem(true)]
	public class SparkDataListView : DataListView
	{
		/// <summary>
		/// 初始 <see cref="SparkDataListView"/> 类型的新实例。
		/// </summary>
		public SparkDataListView()
		{
		}
	}
}