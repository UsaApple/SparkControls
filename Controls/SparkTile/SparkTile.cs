using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SparkControls.Foundation;
using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 磁贴类
	/// </summary>
	public class SparkTile : SparkElement
	{
		#region 常量
		private const int COMMON_TAG_WIDTH = 40;
		private const int COMMON_TAG_HEIGHT = 18;
		private const int DATA_ITEM_GAP = 5;
		private const int DATA_ICON_GAP = 15;
		private const string DATA_ITEM_PREFIX = "● ";
		#endregion

		#region 变量
		private Rectangle _fillRect = Rectangle.Empty;
		private Rectangle _tileTextRect = Rectangle.Empty;
		private Dictionary<string, Rectangle> _notifyIconDict = null;
		private Dictionary<BaseObject, Rectangle> _dataItemDict = null;
		private ControlState _ctrlState = ControlState.Default;
		#endregion

		#region 构造方法
		/// <summary>
		/// 无参构造方法
		/// </summary>
		public SparkTile() : base()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name">组名称</param>
		/// <param name="text">组的文本</param>
		public SparkTile(string name, string text) : base(name, text)
		{
		}
		#endregion

		#region 属性
		/// <summary>
		/// 是否为常用
		/// </summary>
		[Category("Spark"), Description("是否为常用"), DefaultValue(false)]
		public bool IsCommon { get; set; } = false;

		/// <summary>
		/// 是否绘制边框
		/// </summary>
		[Category("Spark"), Description("是否绘制边框"), DefaultValue(false)]
		public bool DrawBorder { get; set; } = false;

		/// <summary>
		/// 磁贴描述文本
		/// </summary>
		[Category("Spark"), Description("磁贴描述文本"), DefaultValue(null)]
		public string DescriptionText { get; set; }

		/// <summary>
		/// 磁贴提示文本
		/// </summary>
		[Category("Spark"), Description("磁贴提示文本"), DefaultValue(null)]
		public string ToolTipText { get; set; }

		private int _rowSpan = 1;
		/// <summary>
		/// 磁贴所跨行数
		/// </summary>
		[Category("Spark"), Description("磁贴所跨行数")]
		[DefaultValue(typeof(int), "1")]
		public int RowSpan
		{
			get => this._rowSpan;
			set
			{
				if (value > 2) this._rowSpan = 2;
				else if (value < 1) this._rowSpan = 1;
				else if (value == 2 && this.ColSpan == 1) this._rowSpan = 1;
				else this._rowSpan = value;
			}
		}

		private int _colSpan = 1;
		/// <summary>
		/// 磁贴所跨行数
		/// </summary>
		[Category("Spark"), Description("磁贴所跨列数")]
		[DefaultValue(typeof(int), "1")]
		public int ColSpan
		{
			get => this._colSpan;
			set
			{
				if (value > 2) this._colSpan = 2;
				else if (value < 1) this._colSpan = 1;
				else this._colSpan = value;

				//重新设置RowSpan
				if (value == 1 && this.RowSpan == 2) this.RowSpan = 1;
			}
		}

		private int _cornerRadius = 4;
		/// <summary>
		/// 磁贴角的半径(0-10)
		/// </summary>
		[Category("Spark"), Description("磁贴角的半径")]
		[DefaultValue(typeof(int), "4")]
		public int CornerRadius
		{
			get => this._cornerRadius;
			set
			{
				if (value < 0 || value > 10)
				{
					this._cornerRadius = 4;
				}
				else
				{
					this._cornerRadius = value;
				}
			}
		}

		/// <summary>
		/// 磁贴图像
		/// </summary>
		[Category("Spark"), Description("磁贴图像")]
		[DefaultValue(typeof(Image), null)]
		public Image Image { get; set; }

		/// <summary>
		/// 磁贴图片和文本的关系
		/// </summary>
		[Category("Spark"), Description("磁贴图片和文本的关系")]
		[DefaultValue(typeof(TextImageRelation), "ImageBeforeText")]
		public TextImageRelation TextImageRelation { get; set; }

		/// <summary>
		/// 磁贴内边距
		/// </summary>
		[Category("Spark"), Description("磁贴文本边距")]
		[DefaultValue(typeof(Padding), "4, 0, 0, 4")]
		public Padding TextPadding { set; get; } = new Padding(4, 0, 0, 4);

		/// <summary>
		/// 磁贴小图标集合
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, Image> IconDict { get; set; }

		/// <summary>
		/// 磁贴附加信息
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Tag { get; set; }

		/// <summary>
		/// 磁贴所在行
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Row { get; internal set; }

		/// <summary>
		/// 磁贴所在列
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Column { get; internal set; }
		#endregion

		#region 事件
		/// <summary>
		/// 鼠标点击事件
		/// </summary>
		public event EventHandler Click;

		/// <summary>
		/// 数据项委托
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void DataEventHandler(object sender, DataItemEventArgs e);
		/// <summary>
		/// 数据项目点击事件
		/// </summary>
		public event DataEventHandler DataItemClick;

		/// <summary>
		/// 通知图标点击事件
		/// </summary>
		public event DataEventHandler NotifyIconClick;

		/// <summary>
		/// 鼠标抬起事件
		/// </summary>
		public event MouseEventHandler MouseUp;

		/// <summary>
		/// 鼠标按下事件
		/// </summary>
		public event MouseEventHandler MouseDown;

		/// <summary>
		/// 鼠标进入事件
		/// </summary>
		public event EventHandler MouseEnter;

		/// <summary>
		/// 鼠标离开事件
		/// </summary>
		public event EventHandler MouseLeave;
		#endregion

		#region 公共方法
		/// <summary>
		/// 磁贴区域命中测试
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool HitTileTest(Point p)
		{
			return this._fillRect.Contains(p);
		}

		/// <summary>
		/// 磁贴文本区域命中测试
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool HitTileTextTest(Point p)
		{
			return this._tileTextRect.Contains(p);
		}

		/// <summary>
		/// 根据坐标查找图标名称
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public string FindIconName(Point p)
		{
			if (this._notifyIconDict == null || this._notifyIconDict.Count == 0)
			{
				return null;
			}

			return this._notifyIconDict.Keys.FirstOrDefault(t => this._notifyIconDict[t].Contains(p));
		}

		/// <summary>
		/// 根据坐标查找数据项
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public BaseObject FindDataItem(Point p)
		{
			if (this._dataItemDict == null || this._dataItemDict.Count == 0)
			{
				return null;
			}

			return this._dataItemDict.Keys.FirstOrDefault(t => this._dataItemDict[t].Contains(p));
		}

		/// <summary>
		/// 重置数据项的状态
		/// </summary>
		public void ResetDataItemState()
		{
			if (this._dataItemDict == null || this._dataItemDict.Count == 0)
			{
				return;
			}

			foreach (BaseObject bo in this._dataItemDict.Keys)
			{
				bo.Extension = null;
			}
		}
		#endregion

		#region 方法
		/// <summary>
		/// 绘制磁贴
		/// </summary>
		/// <param name="g"></param>
		/// <param name="theme"></param>
		internal override void Paint(Graphics g, SparkThemeBase theme)
		{
			if (!(theme is SparkMenuWidgetTheme panoramaTheme))
			{
				return;
			}

			//填充背景色
			this._fillRect = new Rectangle(this.Location.X + this.CellPadding.Left, this.Location.Y + this.CellPadding.Top,
				this.Size.Width - this.CellPadding.Horizontal, this.Size.Height - this.CellPadding.Vertical);
			if (this._ctrlState == ControlState.Highlight)
			{
				Rectangle shadowRect = new Rectangle(this._fillRect.X - 4,
					this._fillRect.Y - 4, this._fillRect.Width + 8, this._fillRect.Height + 8);
				ColorBlend colorBlend = new ColorBlend(3)
				{
					Colors = new Color[] {
						Color.Transparent,
						Color.FromArgb(180, Color.FromArgb(223, 223, 223)),
						Color.FromArgb(180, Color.FromArgb(239, 239, 239)) },
					Positions = new float[] { 0f, .1f, 1f }
				};
				GDIHelper.FillPath(g, new RoundRectangle(shadowRect, new CornerRadius(this.CornerRadius + 2)), colorBlend);
			}
			GDIHelper.FillPath(g, new RoundRectangle(this._fillRect,
				new CornerRadius(this.CornerRadius)), this.GetBackColor(panoramaTheme));

			//绘制边框
			if (this.DrawBorder || this._ctrlState == ControlState.Highlight)
			{
				GDIHelper.DrawPathBorder(g, new RoundRectangle(this._fillRect,
					new CornerRadius(this.CornerRadius)), this.GetBorderColor(panoramaTheme), 1);
			}

			//计算文本，描述，图片对应的范围
			Rectangle[] rects = this.ComputeImageAndTextLayout(g, this._fillRect);
			if (rects == null || rects.Length < 3)
			{
				return;
			}
			this._tileTextRect = rects[0];

			//绘制图像
			if (this.Image != null) g.DrawImage(this.Image, rects[2]);

			//绘制热门图标
			this.DrawCommonTag(g);

			//绘制描述
			this.DrawDescriptionText(g, rects[1]);

			//绘制文本
			StringFormat strFrmt = this.ConvertToStrFrmat(this.TextAlignment);
			strFrmt.Trimming = StringTrimming.EllipsisCharacter;
			GDIHelper.DrawString(g, this._tileTextRect, this.Text, this.TextFont, this.GetTxtForeColor(panoramaTheme), strFrmt);

			this.DrawNotifyIcons(g);

			//绘制数据项
			this.DrawDataItems(g);
		}

		/// <summary>
		/// 选中磁贴
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void Select()
		{
			this._ctrlState = ControlState.Highlight;
		}

		/// <summary>
		/// 鼠标点击
		/// </summary>
		/// <param name="e"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void CallClick(EventArgs e)
		{
			this._ctrlState = ControlState.Default;
			this.DoClick(e);
		}

		/// <summary>
		/// 图标点击
		/// </summary>
		/// <param name="name"></param>
		/// <param name="e"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void CallNotifyIconClick(string name, DataItemEventArgs e)
		{
			this.DoNotifyIconClick(name, e);
		}

		/// <summary>
		/// 数据项鼠标点击
		/// </summary>
		/// <param name="bo"></param>
		/// <param name="e"></param>
		internal void CallDataItemClick(BaseObject bo, DataItemEventArgs e)
		{
			this.DoDataItemClick(bo, e);
		}

		/// <summary>
		/// 调用鼠标抬起
		/// </summary>
		/// <param name="e"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void CallDoMouseUp(MouseEventArgs e)
		{
			this._ctrlState = ControlState.Default;
			this.DoMouseUp(e);
		}

		/// <summary>
		/// 调用鼠标按下
		/// </summary>
		/// <param name="e"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void CallDoMouseDown(MouseEventArgs e)
		{
			this._ctrlState = ControlState.Focused;
			this.DoMouseDown(e);
		}

		/// <summary>
		/// 调用鼠标进入
		/// </summary>
		/// <param name="e"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void CallDoMouseEnter(MouseEventArgs e)
		{
			this._ctrlState = ControlState.Highlight;
			this.DoMouseEnter(e);
		}

		/// <summary>
		/// 调用鼠标离开
		/// </summary>
		/// <param name="e"></param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal void CallDoMouseLeave(MouseEventArgs e)
		{
			this._ctrlState = ControlState.Default;
			this.DoMouseLeave(e);
		}

		/// <summary>
		/// 执行鼠标点击
		/// </summary>
		/// <param name="e"></param>
		protected virtual void DoClick(EventArgs e)
		{
			Click?.Invoke(this, e);
		}

		/// <summary>
		/// 执行通知图标点击
		/// </summary>
		/// <param name="name"></param>
		/// <param name="e"></param>
		protected virtual void DoNotifyIconClick(string name, DataItemEventArgs e)
		{
			NotifyIconClick?.Invoke(name, e);
		}

		/// <summary>
		/// 数据项点击处理事件
		/// </summary>
		/// <param name="bo"></param>
		/// <param name="e"></param>
		protected virtual void DoDataItemClick(BaseObject bo, DataItemEventArgs e)
		{
			DataItemClick?.Invoke(bo, e);
		}

		/// <summary>
		/// 执行鼠标抬起
		/// </summary>
		/// <param name="e"></param>
		protected virtual void DoMouseUp(MouseEventArgs e)
		{
			MouseUp?.Invoke(this, e);
		}

		/// <summary>
		/// 执行鼠标按下
		/// </summary>
		/// <param name="e"></param>
		protected virtual void DoMouseDown(MouseEventArgs e)
		{
			MouseDown?.Invoke(this, e);
		}

		/// <summary>
		/// 执行鼠标进入
		/// </summary>
		/// <param name="e"></param>
		protected virtual void DoMouseEnter(MouseEventArgs e)
		{
			MouseEnter?.Invoke(this, e);
		}

		/// <summary>
		/// 执行鼠标离开
		/// </summary>
		/// <param name="e"></param>
		protected virtual void DoMouseLeave(MouseEventArgs e)
		{
			MouseLeave?.Invoke(this, e);
		}

		/// <summary>
		/// 绘制热门标记
		/// </summary>
		private void DrawCommonTag(Graphics g)
		{
			if (!this.IsCommon)
			{
				return;
			}

			int tagStrOffSet = 5;
			Size tagSize = new Size(COMMON_TAG_WIDTH, COMMON_TAG_HEIGHT);
			Rectangle tagRect = new Rectangle(this._tileTextRect.X + this._tileTextRect.Width,
				this._tileTextRect.Y + (this._tileTextRect.Height - tagSize.Height) / 2, tagSize.Width, tagSize.Height);

			int radius = tagSize.Height / 2;
			GDIHelper.FillPath(g, new RoundRectangle(tagRect, new CornerRadius(radius)), Color.FromArgb(237, 72, 73));
			GDIHelper.DrawString(g, new Rectangle(tagRect.X + tagStrOffSet, tagRect.Y,
				tagSize.Width - tagStrOffSet * 2, tagSize.Height), "热门", new Font("微软雅黑", 9f), Color.White);
		}

		/// <summary>
		/// 绘制描述信息
		/// </summary>
		/// <param name="g"></param>
		/// <param name="despRect"></param>
		private void DrawDescriptionText(Graphics g, Rectangle despRect)
		{
			if (this.DescriptionText.IsNullOrEmpty())
			{
				return;
			}

			GDIHelper.DrawString(g, despRect, this.DescriptionText, new Font("微软雅黑", 9f),
				Color.FromArgb(170, 170, 170),
				new StringFormat()
				{
					Alignment = StringAlignment.Near,
					LineAlignment = StringAlignment.Near,
					Trimming = StringTrimming.EllipsisCharacter
				});
		}

		/// <summary>
		/// 绘制数据项条目
		/// </summary>
		/// <param name="g"></param>
		private void DrawDataItems(Graphics g)
		{
			if (this._dataItemDict == null || this._dataItemDict.Count == 0)
			{
				return;
			}

			//绘制数据项
			Font dataItemFont = new Font("微软雅黑", 10.5f);
			StringFormat format = new StringFormat()
			{
				Alignment = StringAlignment.Near,
				LineAlignment = StringAlignment.Center,
				Trimming = StringTrimming.EllipsisCharacter
			};
			foreach (BaseObject bo in this._dataItemDict.Keys)
			{
				Color txtForeColor = bo.Extension?.ToString() == "Highlight" ? Color.Black : Color.White;
				GDIHelper.DrawString(g, this._dataItemDict[bo], $"{DATA_ITEM_PREFIX}{bo.Name}",
					dataItemFont, txtForeColor, format);
			}
		}

		/// <summary>
		/// 绘制通知小图标
		/// </summary>
		/// <param name="g"></param>
		private void DrawNotifyIcons(Graphics g)
		{
			if (this.IconDict == null || this.IconDict.Count == 0 ||
				this._notifyIconDict == null || this._notifyIconDict.Count == 0)
			{
				return;
			}

			foreach (string imgKey in this.IconDict.Keys)
			{
				if (!this._notifyIconDict.ContainsKey(imgKey))
				{
					continue;
				}

				Rectangle iconRect = this._notifyIconDict[imgKey];
				g.DrawImage(this.IconDict[imgKey], iconRect);
			}
		}

		/// <summary>
		/// 计算文本和图片对应的矩形
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fillRect"></param>
		/// <returns>数组0:文本矩形，1：图像矩形</returns>
		private Rectangle[] ComputeImageAndTextLayout(Graphics g, Rectangle fillRect)
		{
			switch (this.TextImageRelation)
			{
				case TextImageRelation.ImageBeforeText:
					return this.ComputeWhenImageBeforeText(g, fillRect);
				case TextImageRelation.ImageAboveText:
					return this.ComputeWhenImageAboveText(g, fillRect);
				case TextImageRelation.Overlay:
					return this.ComputeWhenOverlay(g, fillRect);
				case TextImageRelation.TextBeforeImage:
				case TextImageRelation.TextAboveImage:
					return null;
				default:
					return null;
			}
		}

		/// <summary>
		/// 图片在文本前面时计算范围
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fillRect"></param>
		/// <returns></returns>
		private Rectangle[] ComputeWhenImageBeforeText(Graphics g, Rectangle fillRect)
		{
			SizeF textSize = g.MeasureString(this.Text, this.TextFont);
			int txtWidth = (int)textSize.Width;
			int txtHeight = (int)textSize.Height;

			//图片与文本的左右间距
			int imgAndDespHGap = 14;
			//文本于描述上下的间距
			int textAndDespVGap = 10;
			int imgDeflWidth = 58;

			int maxTextRectWidth = fillRect.Width - this.TextPadding.Horizontal -
				imgDeflWidth - imgAndDespHGap - COMMON_TAG_WIDTH - 3;
			Rectangle textRect = new Rectangle(fillRect.X + this.TextPadding.Left +
				(this.Image == null ? imgDeflWidth : this.Image.Width) + imgAndDespHGap, fillRect.Y + this.TextPadding.Top,
				Math.Min(txtWidth + 1, maxTextRectWidth), txtHeight);

			//描述文本矩形
			Rectangle despTextRect = new Rectangle(textRect.X, textRect.Y + textRect.Height + textAndDespVGap,
				fillRect.Width - this.TextPadding.Horizontal - imgDeflWidth - imgAndDespHGap,
				fillRect.Height - textRect.Height - textAndDespVGap - this.TextPadding.Vertical);

			//计算图像对象的矩形
			Rectangle imgRect = Rectangle.Empty;
			if (this.Image != null)
			{
				imgRect = new Rectangle(fillRect.X + this.TextPadding.Left, fillRect.Y +
					(fillRect.Height - this.Image.Height) / 2, this.Image.Width, this.Image.Height);
			}

			return new Rectangle[] { textRect, despTextRect, imgRect };
		}

		/// <summary>
		/// 图片在文本上面时计算范围
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fillRect"></param>
		/// <returns></returns>
		private Rectangle[] ComputeWhenImageAboveText(Graphics g, Rectangle fillRect)
		{
			int txtHeight = (int)g.MeasureString(this.Text, this.TextFont).Height;
			int offsetY = fillRect.Height - txtHeight - this.TextPadding.Bottom;

			Rectangle textRect = new Rectangle(fillRect.X + this.TextPadding.Left,
				fillRect.Y + offsetY, fillRect.Width - this.TextPadding.Horizontal, txtHeight);

			//计算图像对象的矩形
			Rectangle imgRect = Rectangle.Empty;
			if (this.Image != null)
			{
				imgRect = new Rectangle(fillRect.X + (fillRect.Width - this.Image.Width) / 2,
					fillRect.Y + this.TextPadding.Top, this.Image.Width, this.Image.Height);
			}

			return new Rectangle[] { textRect, Rectangle.Empty, imgRect };
		}

		/// <summary>
		/// 图片在文本在同一空间时计算范围
		/// </summary>
		/// <param name="g"></param>
		/// <param name="fillRect"></param>
		/// <returns></returns>
		private Rectangle[] ComputeWhenOverlay(Graphics g, Rectangle fillRect)
		{
			int extraBtnWidth = 16 * 2 + DATA_ICON_GAP;
			//标题文本区域
			int txtHeight = (int)g.MeasureString(this.Text, this.TextFont).Height;
			Rectangle textRect = new Rectangle(fillRect.X + this.TextPadding.Left,
				fillRect.Y + this.TextPadding.Top, fillRect.Width - this.TextPadding.Horizontal - extraBtnWidth, txtHeight);

			//刷新和更新按钮区域
			if (this._notifyIconDict == null)
			{
				this._notifyIconDict = new Dictionary<string, Rectangle>();
			}
			this._notifyIconDict.Clear();

			//这里固定只绘制这两个图标
			this._notifyIconDict.Add("notify_refresh", new Rectangle(
				textRect.Right, fillRect.Y + this.TextPadding.Top, 16, 16));
			this._notifyIconDict.Add("notify_more", new Rectangle(textRect.Right + 16 + DATA_ICON_GAP,
				fillRect.Y + this.TextPadding.Top, 16, 16));

			//计算图像对象的矩形
			Rectangle imgRect = fillRect;

			//计算数据项的位置
			if (this.Tag is IEnumerable<BaseObject> boLst)
			{
				if (this._dataItemDict == null)
				{
					this._dataItemDict = new Dictionary<BaseObject, Rectangle>(new BaseObjectEqualityComparer());
				}
				this._dataItemDict.Clear();

				int index = 0;
				Font dataItemFont = new Font("微软雅黑", 10.5f);
				foreach (BaseObject bo in boLst)
				{
					int itemHeight = (int)g.MeasureString(bo.Name ?? this.Text, dataItemFont).Height;
					Rectangle itemRect = new Rectangle(fillRect.X + this.TextPadding.Left,
						textRect.Bottom + 4 + (itemHeight + DATA_ITEM_GAP) * index,
						fillRect.Width - this.TextPadding.Horizontal, itemHeight);
					if (itemRect.Bottom > fillRect.Bottom)
					{
						break;
					}

					if (!this._dataItemDict.ContainsKey(bo))
					{
						this._dataItemDict.Add(bo, itemRect);
					}
					index++;
				}
			}
			return new Rectangle[] { textRect, Rectangle.Empty, imgRect };
		}

		/// <summary>
		/// 根据状态获取背景色
		/// </summary>
		/// <param name="panoramaTheme"></param>
		/// <returns></returns>
		private Color GetBackColor(SparkMenuWidgetTheme panoramaTheme)
		{
			switch (this._ctrlState)
			{
				case ControlState.Default:
					return panoramaTheme.BackColor;
				case ControlState.Highlight:
					return panoramaTheme.MouseOverBackColor;
				case ControlState.Focused:
					return panoramaTheme.MouseDownBackColor;
				case ControlState.Selected:
					return panoramaTheme.SelectedBackColor;
				default:
					return panoramaTheme.BackColor;
			}
		}

		/// <summary>
		/// 根据状态获取文本前景色
		/// </summary>
		/// <param name="panoramaTheme"></param>
		/// <returns></returns>
		private Color GetTxtForeColor(SparkMenuWidgetTheme panoramaTheme)
		{
			if (this.TextColor == Color.Empty)
			{
				return this._ctrlState == ControlState.Default ?
				panoramaTheme.ForeColor : panoramaTheme.MouseOverForeColor;
			}
			return this.TextColor;
		}

		/// <summary>
		/// 根据状态获取边框颜色
		/// </summary>
		/// <param name="panoramaTheme"></param>
		/// <returns></returns>
		private Color GetBorderColor(SparkMenuWidgetTheme panoramaTheme)
		{
			return this._ctrlState == ControlState.Default ?
				panoramaTheme.BorderColor : panoramaTheme.MouseOverBorderColor;
		}

		/// <summary>
		/// 获取截断的字符串(暂未使用)
		/// </summary>
		/// <param name="g"></param>
		/// <param name="originalTxt"></param>
		/// <param name="format"></param>
		/// <param name="maxWidth"></param>
		/// <returns></returns>
		private string GetTruncateTxt(Graphics g, string originalTxt,
			StringFormat format, float maxWidth)
		{
			if (originalTxt.IsNullOrEmpty() || maxWidth <= 0)
			{
				return originalTxt;
			}

			SizeF txtSize = GDIHelper.MeasureString(g, this.Text, this.TextFont, format);
			if (txtSize.Width <= maxWidth) return originalTxt;

			//截取字符串
			StringBuilder sb = new StringBuilder(originalTxt);
			float measureWidth = maxWidth + 1;
			while (measureWidth > maxWidth && sb.Length > 0)
			{
				sb.Remove(sb.Length - 1, 1);
				//预留两个点的位置
				measureWidth = GDIHelper.MeasureString(g, $"{sb.ToString()}.....",
					this.TextFont, format).Width;
			}
			return $"{sb.ToString()}...";
		}
		#endregion
	}
}