using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 时间日期单元元素
	/// </summary>
	public class DateTimeCellElement
	{
		#region 属性
		/// <summary>
		/// 元素名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 元素类型
		/// </summary>
		public ElementType EleType { get; private set; }

		/// <summary>
		/// 元素显示文本
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 元素显示文本字体
		/// </summary>
		public Font TextFont { set; get; }

		/// <summary>
		/// 主题
		/// </summary>
		public SparkDateTimePickerTheme Theme { set; get; }

		/// <summary>
		/// 是否绘制边框
		/// </summary>
		public bool DrawBorder { get; set; } = false;

		/// <summary>
		/// 是否选中
		/// </summary>
		public bool Select { get; set; } = false;

		/// <summary>
		/// 元素位置
		/// </summary>
		public Point Location { set; get; }

		/// <summary>
		/// 元素内边界
		/// </summary>
		public Padding Margin { set; get; }

		/// <summary>
		/// 元素大小
		/// </summary>
		public Size Size { get; set; }

		/// <summary>
		/// 元素边界
		/// </summary>
		public Rectangle Bounds => new Rectangle(this.Location, this.Size);

		/// <summary>
		/// 是否需要绘制
		/// </summary>
		public bool Visible { get; set; } = true;
		#endregion

		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="eleType"></param>
		public DateTimeCellElement(ElementType eleType)
		{
			this.EleType = eleType;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 绘制时间日期单元元素
		/// </summary>
		/// <param name="g"></param>
		public void Paint(Graphics g)
		{
			if (!this.Visible) return;

			//填充背景色
			g.FillRectangle(new SolidBrush(this.Select ?
				(this.Theme == null ? SparkThemeConsts.DateTimePickerSelectColor : this.Theme.DateTimeSelectColor) :
				(this.Theme == null ? SparkThemeConsts.BackColor : this.Theme.BackColor)), this.Bounds);
			if (this.DrawBorder)
			{
				Color boderColor = this.Theme == null ? SparkThemeConsts.BorderColor : this.Theme.BorderColor;
				if (boderColor == Color.Transparent) boderColor = Color.FromArgb(122, 122, 122);
				g.DrawRectangle(new Pen(boderColor), this.Bounds);
			}

			//绘制字符串
			if (string.IsNullOrEmpty(this.Text)) return;
			StringFormat strFormat = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			g.DrawString(this.Text, this.TextFont, new SolidBrush(this.Theme == null ? SparkThemeConsts.ForeColor : this.Theme.ForeColor), this.Bounds, strFormat);
		}
		#endregion
	}
}