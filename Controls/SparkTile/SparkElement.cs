using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 元素基类
	/// </summary>
	public abstract class SparkElement
	{
		#region 构造方法
		/// <summary>
		/// 无参构造方法
		/// </summary>
		protected SparkElement()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name">元素名称</param>
		/// <param name="text">元素文本</param>
		protected SparkElement(string name, string text)
		{
			this.Name = name;
			this.Text = text;
		}
		#endregion

		#region 属性
		/// <summary>
		/// 元素名称
		/// </summary>
		[Category("Spark"), Description("元素名称。"), DefaultValue(null)]
		public string Name { get; set; }

		/// <summary>
		/// 元素关联的文本。
		/// </summary>
		[Category("Spark"), Description("元素关联的文本。"), DefaultValue(null)]
		public string Text { get; set; }

		/// <summary>
		/// 单元格的内边距。
		/// </summary>
		[Category("Spark"), Description("单元格的内边距。")]
		[DefaultValue(typeof(Padding), "2, 2, 2, 2")]
		public Padding CellPadding { set; get; } = new Padding(2);

		/// <summary>
		/// 元素文本的对齐方式。
		/// </summary>
		[Category("Spark"), Description("元素文本的对齐方式。")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		public ContentAlignment TextAlignment { get; set; } = ContentAlignment.MiddleCenter;

		/// <summary>
		/// 元素显示文本字体。
		/// </summary>
		[Category("Spark"), Description("元素文本字体。")]
		[DefaultValue(typeof(Font), "微软雅黑, 9pt")]
		public Font TextFont { set; get; } = new Font("微软雅黑", 9f);

		/// <summary>
		/// 元素显示文本颜色。
		/// </summary>
		[Category("Spark"), Description("元素文本颜色。")]
		[DefaultValue(typeof(Color), "Empty")]
		public Color TextColor { set; get; } = Color.Empty;

		/// <summary>
		/// 元素位置
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Point Location { get; internal set; }

		/// <summary>
		/// 元素大小
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Size Size { get; internal set; }

		/// <summary>
		/// 元素内部边距
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Padding Padding { get; internal set; }

		/// <summary>
		/// 元素外部边距
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Padding Margin { get; internal set; }

		/// <summary>
		/// 元素边界
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Rectangle Bounds => new Rectangle(this.Location, this.Size);
		#endregion

		#region 抽象方法
		/// <summary>
		/// 绘制元素
		/// </summary>
		/// <param name="g"></param>
		/// <param name="theme"></param>
		internal abstract void Paint(Graphics g, SparkThemeBase theme);
		#endregion

		#region 方法
		/// <summary>
		/// 根据ContentAlignment转为StringFormat
		/// </summary>
		/// <param name="textAlignment"></param>
		/// <returns></returns>
		protected StringFormat ConvertToStrFrmat(ContentAlignment textAlignment)
		{
			StringFormat stringFormat = new StringFormat();
			switch (textAlignment)
			{
				case ContentAlignment.TopLeft:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.TopCenter:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.TopRight:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Far;
					break;
				case ContentAlignment.MiddleLeft:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.MiddleCenter:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.MiddleRight:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Far;
					break;
				case ContentAlignment.BottomLeft:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.BottomCenter:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.BottomRight:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Far;
					break;
				default:
					break;
			}
			return stringFormat;
		}
		#endregion
	}
}