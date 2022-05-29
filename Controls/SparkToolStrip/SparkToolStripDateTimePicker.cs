using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
	/// <summary>
	/// 工具栏日期框控件。
	/// </summary>
	[DesignerCategory("code")]
	[ToolStripItemDesignerAvailability(
		ToolStripItemDesignerAvailability.ToolStrip |
		ToolStripItemDesignerAvailability.MenuStrip |
		ToolStripItemDesignerAvailability.ContextMenuStrip)
	]
	public class SparkToolStripDateTimePicker : ToolStripControlHost
	{
		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkToolStripDateTimePicker() : base(CreateCtrlInstance())
		{
			this.Control.CreateControl();
		}

		/// <summary>
		/// 有参构造方法
		/// </summary>
		/// <param name="name"></param>
		public SparkToolStripDateTimePicker(string name) : this()
		{
			this.Name = name;
		}
		#endregion

		#region 属性
		/// <summary>
		/// 承载指定的控件
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("自定义日期框设置")]
		public SparkDateTimePicker DateTimePicker => this.Control as SparkDateTimePicker;

		/// <summary>
		/// 默认大小
		/// </summary>
		protected override Size DefaultSize => new Size(150, 23);

		/// <summary>
		/// 默认边距
		/// </summary>
		protected override Padding DefaultMargin => this.IsOnDropDown ? new Padding(1) : new Padding(1, 0, 1, 0);
		#endregion

		#region 方法
		/// <summary>
		/// 检索适合控件的矩形区域的大小
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			return this.DateTimePicker.Size;
		}

		/// <summary>
		/// 创建 <see cref="SparkDateTimePicker"/>。
		/// </summary>
		/// <returns></returns>
		private static Control CreateCtrlInstance()
		{
			return new SparkDateTimePicker();
		}
		#endregion
	}
}