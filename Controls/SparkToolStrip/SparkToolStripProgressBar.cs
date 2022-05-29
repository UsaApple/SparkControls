using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
	/// <summary>
	/// 工具栏进度条控件。
	/// </summary>
	[DesignerCategory("code")]
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip |
		ToolStripItemDesignerAvailability.StatusStrip)]
	public class SparkToolStripProgressBar : ToolStripControlHost
	{
		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkToolStripProgressBar() : base(CreateCtrlInstance())
		{
		}
		#endregion

		#region 属性
		/// <summary>
		/// 承载指定的控件
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("自定义文本框设置")]
		public SparkProgress Progress => this.Control as SparkProgress;

		/// <summary>
		/// 默认大小
		/// </summary>
		protected override Size DefaultSize => new Size(100, 15);

		/// <summary>
		/// 默认边距
		/// </summary>
		protected override Padding DefaultMargin
		{
			get
			{
				if (this.Owner != null && this.Owner is StatusStrip)
				{
					return new Padding(1, 3, 1, 3);
				}
				else
				{
					return new Padding(1, 2, 1, 1);
				}
			}
		}
		#endregion

		#region 方法
		/// <summary>
		/// 检索适合控件的矩形区域的大小
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			return this.Progress.Size;
		}

		/// <summary>
		/// 创建 <see cref="SparkProgress"/>。
		/// </summary>
		/// <returns></returns>
		private static Control CreateCtrlInstance()
		{
			SparkProgress progress = new SparkProgress
			{
				Size = new Size(100, 15)
			};
			return progress;
		}
		#endregion
	}
}