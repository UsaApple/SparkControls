using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 自定义日期时间面板
	/// </summary>
	[ToolboxItem(false)]
	public class DateTimePanel : Panel
	{
		#region 常量
		/// <summary>
		/// 无效的键对应的值
		/// </summary>
		public const int INVALID_KEY = -1;
		private const int YEAR_ARROW_WIDTH = 20;
		#endregion

		#region 变量
		private readonly DateTimeElement _dtElement = new DateTimeElement();
		private DateTimeCellElement _selectCell = null;
		#endregion

		#region 事件
		/// <summary>
		/// 日期时间单元格元素单击事件
		/// </summary>
		public event EventHandler DtCellEleClick;

		/// <summary>
		/// 日期时间单元格元素双击事件
		/// </summary>
		public event EventHandler DtCellEleDoubleClick;
		#endregion

		#region 属性
		/// <summary>
		/// 单元格元素行数
		/// </summary>
		public int Row { get; set; } = 0;

		/// <summary>
		/// 单元格元素列数
		/// </summary>
		public int Column { get; set; } = 0;

		/// <summary>
		/// 单元格元素类型
		/// </summary>
		public ElementType CellEleType { get; set; } = ElementType.None;

		private List<KeyValuePair<string, string>> _dataSource = null;
		/// <summary>
		/// 元素数据源
		/// </summary>
		public List<KeyValuePair<string, string>> DataSource
		{
			get => this._dataSource;
			set
			{
				this._dataSource?.Clear();
				this._dataSource = null;
				this._dataSource = value;
				this.SetDateTimeElement(value);
			}
		}

		/// <summary>
		/// 主题
		/// </summary>
		[Browsable(false)]
		public SparkDateTimePickerTheme Theme { get; set; }
		#endregion

		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public DateTimePanel() : base()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |           //调整大小时重绘
				ControlStyles.DoubleBuffer |                //双缓冲
				ControlStyles.OptimizedDoubleBuffer |       //双缓冲
				ControlStyles.AllPaintingInWmPaint |        //禁止擦除背景
				ControlStyles.SupportsTransparentBackColor |//透明
				ControlStyles.Selectable |
				ControlStyles.UserPaint, true);
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 设置选中的单元格元素
		/// </summary>
		/// <param name="key"></param>
		public void SetSelectCell(string key)
		{
			if (key.IsNullOrEmpty()) return;

			this._selectCell = this._dtElement.DtCellEles.FirstOrDefault(p => p.Name == key);
			if (this._selectCell == null) return;
			this._selectCell.Select = true;
		}
		#endregion

		#region 重写方法
		/// <summary>
		/// 绘制方法
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//绘制面板上下边界
			ControlPaint.DrawBorder(e.Graphics, new Rectangle(Point.Empty, this.Size),
				Color.Empty, 0, ButtonBorderStyle.None,
				Color.LightGray, 1, ButtonBorderStyle.Dashed,
				Color.Empty, 0, ButtonBorderStyle.None,
				Color.LightGray, 1, ButtonBorderStyle.Dashed);

			//绘制单元格元素
			if (this._dtElement != null && this._dtElement.DtCellEles?.Count > 0)
			{
				this._dtElement.Paint(e.Graphics);
			}
		}

		/// <summary>
		/// 鼠标单击处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (this._selectCell != null)
			{
				this._selectCell.Select = false;
				this.Invalidate(this._selectCell.Bounds);
			}

			this._selectCell = this._dtElement.DtCellEles.FirstOrDefault(p => p.Bounds.Contains(e.Location));
			if (this._selectCell == null || this._selectCell.Name == INVALID_KEY.ToString()) return;

			this._selectCell.Select = true;
			this.Invalidate(this._selectCell.Bounds);

			//触发点击事件并刷新
			DtCellEleClick?.Invoke(this._selectCell, e);
		}

		/// <summary>
		/// 鼠标双击处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			DtCellEleDoubleClick?.Invoke(this._selectCell, e);
		}

		/// <summary>
		/// 大小改变处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			if (this._dtElement != null && this._dtElement.DtCellEles != null &&
				this._dtElement.DtCellEles.Count > 0)
			{
				Padding margin = this.GetMargin();
				this._dtElement.CellSize = new Size((this.Size.Width - margin.Horizontal) / this.Column,
					(this.Size.Height - margin.Vertical) / this.Row);
				for (int i = 0; i < this._dtElement.DtCellEles.Count; i++)
				{
					this._dtElement.DtCellEles[i].Size = this._dtElement.CellSize;
					this._dtElement.DtCellEles[i].Location = new Point(i % this.Column * this._dtElement.CellSize.Width +
						margin.Left, i / this.Column * this._dtElement.CellSize.Height + margin.Top);
				}
			}

			base.OnSizeChanged(e);
		}
		#endregion

		#region 私有方法
		/// <summary>
		/// 设置日期或时间元素
		/// </summary>
		/// <param name="dataSource"></param>
		private void SetDateTimeElement(List<KeyValuePair<string, string>> dataSource)
		{
			if (dataSource == null || dataSource.Count == 0) return;
			this._dtElement.DtCellEles?.Clear();

			//设置边距
			Padding margin = this.GetMargin();

			this._dtElement.RowsCount = this.Row;
			this._dtElement.ColumnsCount = this.Column;
			this._dtElement.CellSize = new Size((this.Size.Width - margin.Horizontal) / this.Column,
				(this.Size.Height - margin.Vertical) / this.Row);

			Font cellTxtFont = new Font("微软雅黑", 10.5f);
			for (int i = 0; i < dataSource.Count; i++)
			{
				DateTimeCellElement cellEle = new DateTimeCellElement(this.CellEleType)
				{
					Name = dataSource[i].Key,
					Text = dataSource[i].Value,
					TextFont = cellTxtFont,
					Theme = this.Theme,
					Margin = margin,
					Size = this._dtElement.CellSize,
					Location = new Point(i % this.Column * this._dtElement.CellSize.Width + margin.Left,
					i / this.Column * this._dtElement.CellSize.Height + margin.Top)
				};
				this._dtElement.DtCellEles.Add(cellEle);
			}
		}

		/// <summary>
		/// 获取边距
		/// </summary>
		/// <returns></returns>
		private Padding GetMargin()
		{
			//获取边距
			return this.CellEleType == ElementType.Year ? new Padding(
				YEAR_ARROW_WIDTH, 1, YEAR_ARROW_WIDTH, 1) : new Padding(0, 1, 0, 1);
		}
		#endregion
	}
}