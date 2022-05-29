using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Foundation;
using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 菜单全景类
	/// </summary>
	[ToolboxItem(true)]
	public partial class SparkPanorama : ScrollableControl, ISupportInitialize, ISparkTheme<SparkMenuWidgetTheme>
	{
		#region 常量
		private const int SCROLL_WIDTH = 20;
		private const int DEFAULT_GRP_HEIGHT = 100;
		#endregion

		#region 变量
		private int _offsetX = 0;
		private bool _isInit = false;
		private bool _setAutoArrange = false;
		private SparkTile _hoverTile = null;
		private BaseObject _hoverBo = null;
		private string _hoverIconName = null;
		private List<int[]> _tileMatrix = null;
		private readonly ToolTip _toolTip = new ToolTip();
		//private Dictionary<string, int> _initGroupMaxColsDict = new Dictionary<string, int>();
		private int _initHorizontalMaxGroup = -1;
		//private Size _initGrpCellSize = Size.Empty;
		#endregion

		#region 属性
		/// <summary>
		/// 获取或设置控件的背景色。
		/// </summary>
		[Category("Spark"), Description("控件的背景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 获取或设置控件的前景色。
		/// </summary>
		[Category("Spark"), Description("控件的前景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		private int _horizontalMaxGroup = 3;
		/// <summary>
		/// 水平方向最大组数
		/// </summary>
		[Category("Spark"), Description("水平方向最大组数"), DefaultValue(3)]
		public int HorizontalMaxGroup
		{
			get { return this._horizontalMaxGroup; }
			set
			{
				this._horizontalMaxGroup = value < 1 ? 1 : value;
			}
		}

		/// <summary>
		/// 组集合
		/// </summary>
		[Category("Spark"), Description("组集合")]
		public List<SparkTileGroup> Groups { get; } = new List<SparkTileGroup>();

		/// <summary>
		/// 是否显示组
		/// </summary>
		[Category("Spark"), Description("是否显示组"), DefaultValue(true)]
		public bool ShowGroups { get; set; } = true;

		//private int _groupMaxColumns = 4;
		///// <summary>
		///// 每组内最大列数
		///// </summary>
		//[Category("Spark"), Description("每组内最大列数"), DefaultValue(4)]
		//public int GroupMaxColumns
		//{
		//    get { return _groupMaxColumns; }
		//    set
		//    {
		//        _groupMaxColumns = value < 1 ? 1 : value;
		//    }
		//}

		/// <summary>
		/// 组的头部(文本)高度
		/// </summary>
		[Category("Spark"), Description("组的头部高度"), DefaultValue(22)]
		public int GroupHeaderHeight { set; get; } = 22;

		//private Size _groupCellSize = new Size(104, 104);
		///// <summary>
		///// 组内单元格的大小
		///// </summary>
		//[Category("Spark"), Description("组内单元格的大小")]
		//[DefaultValue(typeof(Size), "104, 104")]
		//public Size GroupCellSize
		//{
		//    get { return _groupCellSize; }
		//    set
		//    {
		//        _groupCellSize = value;
		//        //_groupCellSize = new Size(value.Width < 74 ? 74 : (value.Width > 204 ? 204 : value.Width),
		//        //    value.Height < 74 ? 74 : (value.Height > 204 ? 204 : value.Height));
		//    }
		//}

		/// <summary>
		/// 组标题的字体
		/// </summary>
		[Category("Spark"), Description("组标题字体")]
		[DefaultValue(typeof(Font), "微软雅黑, 10.5pt")]
		public Font GroupTextFont { set; get; } = new Font("微软雅黑", 10.5f);

		/// <summary>
		/// 组与组之间的间距
		/// </summary>
		[Category("Spark"), Description("组与组之间的间距")]
		[DefaultValue(typeof(Padding), "20, 20, 0, 0")]
		public Padding GroupMargin { set; get; } = new Padding(20, 20, 0, 0);

		///// <summary>
		///// 组内单元格的内边距
		///// </summary>
		//[Category("Spark"), Description("组内单元格的内边距")]
		//[DefaultValue(typeof(Padding), "2, 2, 2, 2")]
		//public Padding GroupCellPadding { set; get; } = new Padding(2);

		///// <summary>
		///// 磁贴内边距
		///// </summary>
		//[Category("Spark"), Description("磁贴内边距")]
		//[DefaultValue(typeof(Padding), "4, 0, 0, 4")]
		//public Padding TilePadding { set; get; } = new Padding(4, 0, 0, 4);

		/// <summary>
		/// 磁贴图片大小
		/// </summary>
		[Category("Spark"), Description("磁贴图片大小")]
		[DefaultValue(typeof(Size), "40, 40")]
		public Size TileImageSize { set; get; } = new Size(40, 40);

		/// <summary>
		/// 当前选中的磁贴
		/// </summary>
		[Browsable(false)]
		public SparkTile SelectedTile => this._hoverTile;
		#endregion

		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkPanorama()
		{
			this._toolTip.AutoPopDelay = 2000;
			this.SetStyle(ControlStyles.ResizeRedraw |                 // 调整大小时重绘
				ControlStyles.Selectable |                        // 可接收焦点
				ControlStyles.DoubleBuffer |                      // 双缓冲
				ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
				ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
				ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
				ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
			);

			this.Theme = new SparkMenuWidgetTheme(this);
			base.BackColor = this.Theme.PanoramaBackColor;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 自动排列磁贴
		/// </summary>
		public void AutoArrangeTiles()
		{
			this.AutoScrollPosition = Point.Empty;
			if (this.Groups == null || this.Groups.Count == 0)
			{
				return;
			}

			foreach (SparkTileGroup grp in this.Groups)
			{
				this.SetTileGrpDependProp(grp);
				this.AutoArrangeGrpTiles(grp);
			}
			this._setAutoArrange = true;
		}

		/// <summary>
		/// 重新排列磁贴
		/// </summary>
		public void ReArrangeTiles()
		{
			if (this._tileMatrix != null)
			{
				this._tileMatrix.ForEach(p => p = null);
				this._tileMatrix.Clear();
				this._tileMatrix = null;
			}

			this.ResetPanorama();
			this.AutoArrangeTiles();
			this.Refresh();
		}

		/// <summary>
		/// 选中磁贴
		/// </summary>
		/// <param name="tile"></param>
		public void SelectTile(SparkTile tile)
		{
			if (tile == null)
			{
				return;
			}
			if (this._hoverTile != null)
			{
				this._hoverTile.CallDoMouseLeave(null);
				this.InvalidDrawBounds(this._hoverTile.Bounds);
			}
			this._hoverTile = tile;
			tile.Select();
			this.InvalidDrawBounds(tile.Bounds);

			int offSetY = tile.Bounds.Y - this.AutoScrollPosition.Y;
			this.AutoScrollPosition = new Point(
				this.AutoScrollPosition.X,
				this.AutoScrollPosition.Y + offSetY);
		}

		/// <summary>
		/// 将组设置到顶部
		/// </summary>
		/// <param name="grp"></param>
		public void SetGroupToTop(SparkTileGroup grp)
		{
			if (grp == null)
			{
				return;
			}

			int offSetY = grp.Bounds.Y - this.AutoScrollPosition.Y;
			this.AutoScrollPosition = new Point(
				this.AutoScrollPosition.X,
				this.AutoScrollPosition.Y + offSetY);
		}

		/// <summary>
		/// 刷新磁贴
		/// </summary>
		/// <param name="tile"></param>
		public void RefreshTile(SparkTile tile)
		{
			if (tile == null)
			{
				return;
			}
			this.InvalidDrawBounds(tile.Bounds);
		}
		#endregion

		#region 重写方法
		/// <summary>
		/// 重绘事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//绘制组集合
			if (this.Groups == null || this.Groups.Count == 0 || !this._setAutoArrange) return;

			//使绘图质量最高，即消除锯齿
			GDIHelper.InitializeGraphics(e.Graphics);
			e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

			this._offsetX = 0;
			for (int i = 0; i < this.HorizontalMaxGroup; i++)
			{
				IEnumerable<SparkTileGroup> tileGrps = this.Groups.Where((p, n) => n % this.HorizontalMaxGroup == i);
				if (!tileGrps.Any()) continue;

				this.DrawGroupsOnVertical(e.Graphics, tileGrps);
				SparkTileGroup tileGrp = tileGrps.First();
				this._offsetX += tileGrp.CellSize.Width * tileGrp.ColumnsCount + tileGrp.Margin.Horizontal;
			}

			this.AutoScrollMinSize = new Size(this.CalcPanoramaWidth(), this.CalcPanoramaHeight());
			e.Graphics.ResetTransform();
		}

		/// <summary>
		/// 鼠标点击处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if ((e is MouseEventArgs me && me.Button ==
				MouseButtons.Right) || this.SelectedTile == null)
			{
				return;
			}

			if (this._hoverBo != null)
			{
				//数据项目点击事件
				this.SelectedTile.CallDataItemClick(this._hoverBo, new DataItemEventArgs(this.SelectedTile.Text));
			}
			else
			{
				//图标点击事件
				if (!string.IsNullOrEmpty(this._hoverIconName))
				{
					this.SelectedTile.CallNotifyIconClick(this._hoverIconName, new DataItemEventArgs(this.SelectedTile.Text));
				}
			}
			this.SelectedTile?.CallClick(e);
		}

		/// <summary>
		/// 鼠标按下处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left) this.Focus();

			IEnumerable<SparkTile> tiles = this.Groups.SelectMany(p => p.Items);
			SparkTile tile = tiles.FirstOrDefault(p => p.HitTileTest(this.GetTilePt(e.Location)));
			if (tile != null)
			{
				tile.CallDoMouseDown(e);
				if (e.Button != MouseButtons.Left) return;
				this.InvalidDrawBounds(tile.Bounds);
			}
		}

		/// <summary>
		/// 鼠标抬起处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			IEnumerable<SparkTile> tiles = this.Groups.SelectMany(p => p.Items);
			SparkTile tile = tiles.FirstOrDefault(p => p.HitTileTest(this.GetTilePt(e.Location)));
			if (tile != null)
			{
				tile.CallDoMouseUp(e);
				if (e.Button != MouseButtons.Left) return;
				this.InvalidDrawBounds(tile.Bounds);
			}
		}

		/// <summary>
		/// 鼠标移动处理
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (this._hoverTile != null)
			{
				if (this._hoverTile.TextImageRelation == TextImageRelation.Overlay)
				{
					this._hoverTile.ResetDataItemState();
				}
				else
				{
					this._hoverBo = null;
					this._hoverIconName = null;
					if (this.Cursor == Cursors.Hand)
					{
						this.Cursor = Cursors.Default;
					}
					this._hoverTile.CallDoMouseLeave(e);
				}
				this.InvalidDrawBounds(this._hoverTile.Bounds);
			}

			Point tilePt = this.GetTilePt(e.Location);
			IEnumerable<SparkTile> tiles = this.Groups.SelectMany(p => p.Items);
			SparkTile findTile = tiles.FirstOrDefault(p => p.HitTileTest(tilePt));
			if (e.Button != MouseButtons.Right)
			{
				this.SetToolTip(findTile, tilePt);
			}

			this._hoverTile = findTile;
			if (this._hoverTile != null)
			{
				if (this._hoverTile.TextImageRelation == TextImageRelation.Overlay)
				{
					this.HandleNotifyTile(this._hoverTile, tilePt);
				}
				else
				{
					this._hoverBo = null;
					this._hoverIconName = null;
					this._hoverTile.CallDoMouseEnter(e);
				}
				this.InvalidDrawBounds(this._hoverTile.Bounds);
			}
		}

		/// <summary>
		/// 释放方法
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this._toolTip?.Dispose();
			}
		}

		/// <summary>
		/// 引发System.Windows.Forms.Control.SizeChanged 事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			if (this._isInit) return;
			this.ReArrangeTiles();

			base.OnSizeChanged(e);
		}
		#endregion

		#region 私有方法
		/// <summary>
		/// 自动排列指定组的磁贴
		/// </summary>
		/// <param name="tileGrpEle"></param>
		private void AutoArrangeGrpTiles(SparkTileGroup tileGrpEle)
		{
			if (tileGrpEle == null) return;

			//尽管组内可能没有磁贴，但这里需要先计算好矩阵(默认矩阵只有1行)
			this._tileMatrix = this.GetTileMatrix(tileGrpEle.ColumnsCount, 1);
			if (this._tileMatrix == null || tileGrpEle.Items == null || tileGrpEle.Items.Count == 0)
			{
				return;
			}

			//安排每个磁贴
			foreach (SparkTile tile in tileGrpEle.Items)
			{
				//设置相关属性
				this.SetTileDependProp(tile);

				this.SetTilePosition(tile);
				this.FillTileMatrix(tile);
			}
			tileGrpEle.RowsCount = this._tileMatrix.Count;
		}

		/// <summary>
		/// 获取磁贴矩阵
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="rows"></param>
		/// <returns></returns>
		private List<int[]> GetTileMatrix(int columns, int rows)
		{
			if (columns <= 0 || rows <= 0)
			{
				return null;
			}

			List<int[]> tileMatrix = new List<int[]>();
			for (int i = 0; i < rows; i++)
			{
				tileMatrix.Add(new int[columns]);
			}
			return tileMatrix;
		}

		/// <summary>
		/// 设置磁贴坐标
		/// </summary>
		/// <param name="tile"></param>
		private void SetTilePosition(SparkTile tile)
		{
			if (this._tileMatrix == null) return;

			int rowIndex = -1; int colIndex = -1;
			for (int i = 0; i < this._tileMatrix.Count; i++)
			{
				colIndex = Array.IndexOf(this._tileMatrix[i], 0);
				if (colIndex == -1) continue;

				if (colIndex + tile.ColSpan <= this._tileMatrix[i].Length &&
					this._tileMatrix[i][colIndex + tile.ColSpan - 1] == 0 &&
					(tile.RowSpan == 1 || i + 1 >= this._tileMatrix.Count ||
					(this._tileMatrix[i + 1][colIndex] == 0 && this._tileMatrix[i + 1][colIndex + 1] == 0)))
				{
					rowIndex = i;
					break;
				}
			}

			tile.Column = rowIndex == -1 ? 0 : colIndex;
			tile.Row = rowIndex == -1 ? this._tileMatrix.Count : rowIndex;
		}

		/// <summary>
		/// 填充磁贴矩阵
		/// </summary>
		/// <param name="tile"></param>
		private void FillTileMatrix(SparkTile tile)
		{
			if (this._tileMatrix == null || this._tileMatrix.Count == 0) return;

			//先扩充矩阵的行
			int columnCount = this._tileMatrix[0].Length;
			int upperLimit = tile.Row + tile.RowSpan - this._tileMatrix.Count;
			for (int i = 0; i < upperLimit; i++)
			{
				this._tileMatrix.Add(new int[columnCount]);
			}

			//填充已占用的位置
			for (int j = 0; j < tile.RowSpan; j++)
			{
				for (int k = 0; k < tile.ColSpan; k++)
				{
					this._tileMatrix[tile.Row + j][tile.Column + k] = 1;
				}
			}
		}

		/// <summary>
		/// 从垂直方向绘制组集合
		/// </summary>
		/// <param name="g"></param>
		/// <param name="tileGrps"></param>
		private void DrawGroupsOnVertical(Graphics g, IEnumerable<SparkTileGroup> tileGrps)
		{
			int startY = 0;
			foreach (SparkTileGroup tileGroup in tileGrps)
			{
				int startX = tileGroup.Margin.Left + this._offsetX;
				startY += tileGroup.Margin.Top;

				//绘制组标题
				tileGroup.Location = new Point(startX, startY);
				if (this.ShowGroups) tileGroup.Paint(g, this.Theme);

				//绘制磁贴
				this.DrawTiles(g, tileGroup);
				startY += tileGroup.RowsCount * tileGroup.CellSize.Height +
					(this.ShowGroups ? tileGroup.HeaderHeight : 0) + tileGroup.Margin.Bottom;
			}
		}

		/// <summary>
		/// 绘制组内磁贴集合
		/// </summary>
		/// <param name="g"></param>
		/// <param name="tileGrp"></param>
		private void DrawTiles(Graphics g, SparkTileGroup tileGrp)
		{
			if (tileGrp.Items == null || tileGrp.Items.Count == 0) return;

			int grpHeight = this.ShowGroups ? tileGrp.HeaderHeight : 0;
			foreach (SparkTile tile in tileGrp.Items)
			{
				tile.Location = new Point(tileGrp.Location.X + tile.Column * tileGrp.CellSize.Width,
						tileGrp.Location.Y + grpHeight + tile.Row * tileGrp.CellSize.Height);
				tile.Size = new Size(tileGrp.CellSize.Width * tile.ColSpan,
					tileGrp.CellSize.Height * tile.RowSpan);
				tile.Paint(g, this.Theme);
			}
		}

		/// <summary>
		/// 获取磁贴计算坐标
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		private Point GetTilePt(Point pt)
		{
			pt.Offset(this.HorizontalScroll.Value, this.VerticalScroll.Value);
			return pt;
		}

		/// <summary>
		/// 处理通知磁贴
		/// </summary>
		/// <param name="hoverTile"></param>
		/// <param name="pt"></param>
		private void HandleNotifyTile(SparkTile hoverTile, Point pt)
		{
			if (hoverTile == null || pt == Point.Empty)
			{
				return;
			}

			string iconName = hoverTile.FindIconName(pt);
			if (string.IsNullOrEmpty(iconName))
			{
				BaseObject bo = hoverTile.FindDataItem(pt);
				this.SetToolTip(bo);

				this._hoverBo = bo;
				if (this._hoverBo != null)
				{
					this._hoverBo.Extension = "Highlight";
				}
			}
			else
			{
				this.SetToolTip(iconName);
			}

			this._hoverIconName = iconName;
		}

		/// <summary>
		/// 重新刷新绘制的区域
		/// </summary>
		/// <param name="bounds"></param>
		private void InvalidDrawBounds(Rectangle bounds)
		{
			bounds.Offset(this.AutoScrollPosition);
			this.Invalidate(bounds);
		}

		/// <summary>
		/// 设置磁贴组依赖属性
		/// </summary>
		/// <param name="grp"></param>
		private void SetTileGrpDependProp(SparkTileGroup grp)
		{
			if (grp == null) return;

			grp.ColumnsCount = grp.GroupMaxColumns;
			//grp.Padding = GroupCellPadding;
			//grp.CellSize = GroupCellSize;
			grp.HeaderHeight = this.GroupHeaderHeight;
			grp.TextFont = this.GroupTextFont;
			grp.Margin = this.GroupMargin;
			grp.TextAlignment = ContentAlignment.TopLeft;
		}

		/// <summary>
		/// 设置磁贴依赖属性
		/// </summary>
		/// <param name="tile"></param>
		private void SetTileDependProp(SparkTile tile)
		{
			if (tile == null) return;

			//tile.TextFont = TileTxtFont;
			//tile.TextPadding = TilePadding;
			//tile.CellPadding = GroupCellPadding;
			//tile.TextImageRelation = TileTextImageRelation;
			//tile.TextAlignment = ContentAlignment.BottomLeft;

			//if (tile.Image == null || tile.Image.Size == TileImageSize) return;
			//tile.Image = tile.Image.GetThumbnailImage(TileImageSize.Width,
			//    TileImageSize.Height, null, IntPtr.Zero);
		}

		/// <summary>
		/// 计算全景控件的高度
		/// </summary>
		/// <returns></returns>
		private int CalcPanoramaHeight()
		{
			if (this.Groups == null || this.Groups.Count == 0) return DEFAULT_GRP_HEIGHT;

			List<int> heightLst = new List<int>();
			for (int i = 0; i < this.HorizontalMaxGroup; i++)
			{
				int height = this.Groups.Where((p, n) => n % this.HorizontalMaxGroup == i).Sum(t =>
				t.RowsCount * t.CellSize.Height + (this.ShowGroups ? t.HeaderHeight : 0) + t.Margin.Vertical);
				heightLst.Add(height);
			}
			return heightLst.Max();
		}

		/// <summary>
		/// 获取组件计算的宽度
		/// </summary>
		/// <returns></returns>
		private int CalcPanoramaWidth()
		{
			return this.Groups.Select(p => (p.CellSize.Width * p.GroupMaxColumns +
			this.GroupMargin.Horizontal) * Math.Min(this.Groups.Count, this.HorizontalMaxGroup)).Max();
		}

		/// <summary>
		/// 设置ToolTip
		/// </summary>
		/// <param name="findTile"></param>
		/// <param name="movePt"></param>
		private void SetToolTip(SparkTile findTile, Point movePt)
		{
			if (findTile == null)
			{
				this._toolTip.Active = false;
				return;
			}

			if (findTile == this._hoverTile || string.IsNullOrEmpty(findTile.ToolTipText)) return;
			this._toolTip.Active = true;
			this._toolTip.SetToolTip(this, findTile.ToolTipText);
		}

		/// <summary>
		/// 设置ToolTip
		/// </summary>
		/// <param name="bo"></param>
		private void SetToolTip(BaseObject bo)
		{
			if (bo == null)
			{
				this._toolTip.Active = false;
				this.Cursor = Cursors.Default;
				return;
			}

			if (bo == this._hoverBo || string.IsNullOrEmpty(bo.Name))
			{
				return;
			}
			this._toolTip.Active = true;
			this.Cursor = Cursors.Hand;
			this._toolTip.SetToolTip(this, bo.Name);
		}

		/// <summary>
		/// 设置ToolTip
		/// </summary>
		/// <param name="iconName"></param>
		private void SetToolTip(string iconName)
		{
			if (string.IsNullOrEmpty(iconName))
			{
				this._toolTip.Active = false;
				this.Cursor = Cursors.Default;
				return;
			}

			if (iconName == this._hoverIconName) return;
			this._toolTip.Active = true;
			this.Cursor = Cursors.Hand;
			this._toolTip.SetToolTip(this, iconName == "notify_more" ? "更多" : "刷新");
		}

		/// <summary>
		/// 重新设置 Panorama 相关信息
		/// </summary>
		private void ResetPanorama()
		{
			this.ResetRelatedParam();

			//减去滚动条的宽度
			int rPanWidth = this.Width - SCROLL_WIDTH;
			//int calcWidth = CalcPanoramaWidth();
			//while (HorizontalMaxGroup > 1 && calcWidth > rPanWidth)

			foreach (SparkTileGroup grp in this.Groups)
			{
				int grpCols = this.GetPerGrpTileCnt(rPanWidth, grp);
				grp.GroupMaxColumns = grpCols;
			}

			//while (calcWidth > rPanWidth)
			//{
			//    if (HorizontalMaxGroup > 1) HorizontalMaxGroup--;
			//    foreach (var grp in Groups)
			//    {
			//        int grpCols = GetPerGrpTileCnt(rPanWidth, grp);
			//        if (grpCols < _initGroupMaxColumns)
			//        {
			//            grp.GroupMaxColumns = grpCols;
			//        }
			//        else
			//        {
			//            grp.GroupMaxColumns = _initGroupMaxColumns;
			//        }
			//    }
			//    //GroupMaxColumns = grpCols < _initGroupMaxColumns ?
			//    //    _initGroupMaxColumns : grpCols;
			//    //if (grpCols < _initGroupMaxColumns)
			//    //{
			//    //    grp.GroupMaxColumns = grpCols;
			//    //}
			//    //else
			//    //{
			//    //    grp.GroupMaxColumns = _initGroupMaxColumns;
			//    //}

			//    calcWidth = CalcPanoramaWidth();
			//}

			//if (HorizontalMaxGroup == 1 && calcWidth > rPanWidth)
			//{
			//    int dp = (rPanWidth / HorizontalMaxGroup -
			//        GroupMargin.Horizontal) / GroupMaxColumns;
			//    GroupCellSize = new Size(dp, dp);
			//}
		}

		/// <summary>
		/// 重置相关参数
		/// </summary>
		private void ResetRelatedParam()
		{
			if (this._initHorizontalMaxGroup == -1)
			{
				//_initGrpCellSize = GroupCellSize;
				//_initGroupMaxColumns = GroupMaxColumns;
				this._initHorizontalMaxGroup = this.HorizontalMaxGroup;
			}
			else
			{
				//GroupCellSize = _initGrpCellSize;
				//GroupMaxColumns = _initGroupMaxColumns;
				this.HorizontalMaxGroup = this._initHorizontalMaxGroup;
			}
		}

		/// <summary>
		/// 获取每组最大磁贴的个数
		/// </summary>
		/// <param name="rowWidth"></param>
		/// <param name="grp"></param>
		/// <returns></returns>
		private int GetPerGrpTileCnt(int rowWidth, SparkTileGroup grp)
		{
			double doubleCnt = (rowWidth * 1.0 / Math.Min(this.Groups.Count, this.HorizontalMaxGroup) -
				this.GroupMargin.Horizontal) / grp.CellSize.Width;
			return (int)doubleCnt;
		}
		#endregion

		#region ISupportInitialize接口
		/// <summary>
		/// 开始初始化
		/// </summary>
		public void BeginInit()
		{
			this._isInit = true;
		}

		/// <summary>
		/// 结束初始化
		/// </summary>
		public void EndInit()
		{
			this._isInit = false;
		}
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkMenuWidgetTheme Theme { get; private set; }

		#endregion
	}
}