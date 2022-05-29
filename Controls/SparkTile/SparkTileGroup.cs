using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 磁贴组类
	/// </summary>
	public class SparkTileGroup : SparkElement
	{
		#region 构造方法
		/// <summary>
		/// 无参构造方法
		/// </summary>
		public SparkTileGroup() : base()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name">组名称</param>
		/// <param name="text">组的文本</param>
		public SparkTileGroup(string name, string text) : base(name, text)
		{
		}
		#endregion

		#region 属性
		private int _groupMaxColumns = 4;
		/// <summary>
		/// 每组内最大列数
		/// </summary>
		[Category("Spark"), Description("每组内最大列数"), DefaultValue(4)]
		public int GroupMaxColumns
		{
			get { return this._groupMaxColumns; }
			set
			{
				this._groupMaxColumns = value < 1 ? 1 : value;
			}
		}

		private Size _cellSize = new Size(104, 104);
		/// <summary>
		/// 组内单元格的大小
		/// </summary>
		[Category("Spark"), Description("组内单元格的大小")]
		[DefaultValue(typeof(Size), "104, 104")]
		public Size CellSize
		{
			get { return this._cellSize; }
			set { this._cellSize = value; }
		}

		/// <summary>
		/// 磁贴组大小
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size Size => new Size(this.CellSize.Width * this.ColumnsCount, this.HeaderHeight);

		/// <summary>
		/// 元素边界
		/// </summary>
		public override Rectangle Bounds => new Rectangle(this.Location.X + this.CellPadding.Left, this.Location.Y, this.Size.Width, this.Size.Height);

		/// <summary>
		/// 组内列数
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ColumnsCount { get; internal set; }

		/// <summary>
		/// 组内行数(初始为1)
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RowsCount { get; internal set; } = 1;

		/// <summary>
		/// 组头高度
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HeaderHeight { get; internal set; }

		/// <summary>
		/// 磁贴项目集合
		/// </summary>
		[Category("Spark"), Description("磁贴集合")]
		public List<SparkTile> Items { get; } = new List<SparkTile>();
		#endregion

		#region 方法
		/// <summary>
		/// 绘制磁贴组方法
		/// </summary>
		/// <param name="g"></param>
		/// <param name="theme"></param>
		internal override void Paint(Graphics g, SparkThemeBase theme)
		{
			//绘制组标题
			if (!(theme is SparkMenuWidgetTheme panoramaTheme))
			{
				return;
			}
			g.DrawString(Text, TextFont, new SolidBrush(panoramaTheme.GroupForeColor), Bounds);
		}
		#endregion
	}
}