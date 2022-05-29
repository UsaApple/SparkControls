using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 分组列表控件。
	/// </summary>
	public class SparkGroupList : ScrollableControl, ISupportInitialize, ISparkTheme<SparkGroupListTheme>
	{
		#region 私有字段
		private Font font = Consts.DEFAULT_FONT;
		private Font groupFont = Consts.DEFAULT_BOLD_FONT;
		private EnumDataType DataType = EnumDataType.None;
		private readonly List<SparkGroupListItem> items = new List<SparkGroupListItem>();
		private List<SparkGroupListItem> dataSouceItems = new List<SparkGroupListItem>();
		private readonly object dataSouce = null;
		private SparkGroupListItem _hoverItem = null;
		private int groupSpacing = 3;
		private int groupHeight = 30;
		private int itemHeight = 20;
		private bool isInit = false;
		#endregion

		#region 事件定义
		/// <summary>
		/// 点击事件
		/// </summary>
		public event SparkGroupListItemClickEventHandler GroupListItemClick;
		#endregion

		#region 属性

		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文本的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get => this.font;
			set
			{
				if (this.font != value)
				{
					base.Font = this.font = value;
					this.OnFontChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 获取或设置控件组标题的字体。
		/// </summary>
		[Category("Spark"), Description("控件组标题的字体")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_BOLD_FONT_STRING)]
		public Font GroupFont
		{
			get => this.groupFont;
			set
			{
				if (this.font != value)
				{
					this.groupFont = value;
					this.OnFontChanged(EventArgs.Empty);
				}
			}
		}

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

		/// <summary>
		/// 获取或设置一个值，该值指示将哪种边框样式应用于控件。
		/// </summary>
		[DefaultValue(BorderStyle.None)]
		[Description("控件的边框样式。")]
		public BorderStyle BorderStyle { get; set; } = BorderStyle.None;

		/// <summary>
		/// 获取或设置 DataSouce 数据源中，用于归并的字段或者属性或XPath。
		/// </summary>
		[Category("Spark"), Description("DataSouce 数据源中，用于归并的字段或者属性或XPath。")]
		[DefaultValue("")]
		public string GroupName { get; set; }

		/// <summary>
		/// 获取或设置 DataSouce 数据源中需要显示的文本字段或属性或XPath。
		/// </summary>
		[Category("Spark"), Description("DataSouce 数据源中需要显示的文本字段或属性或XPath。")]
		[DefaultValue("")]
		public string DisplayMember { get; set; }

		/// <summary>
		/// 获取或设置项的高度。
		/// </summary>
		[Category("Spark"), Description("项的高度。")]
		[DefaultValue(20)]
		public int ItemHeight
		{
			get => this.itemHeight;
			set
			{
				if (this.itemHeight != value)
				{
					this.itemHeight = value;
					this.Invalidate();
				}
			}
		}
		/// <summary>
		/// 获取或设置组的高度。
		/// </summary>
		[Category("Spark"), Description("组的高度。")]
		[DefaultValue(30)]
		public int GroupHeight
		{
			get => this.groupHeight;
			set
			{
				if (this.groupHeight != value)
				{
					this.groupHeight = value;
					this.Invalidate();
				}
			}
		}
		/// <summary>
		/// 获取或设置组之间的距离。
		/// </summary>
		[Category("Spark"), Description("组之间的距离。")]
		[DefaultValue(3)]
		public int GroupSpacing
		{
			get => this.groupSpacing;
			set
			{
				if (this.groupSpacing != value)
				{
					this.groupSpacing = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 获取或设置子项的右击菜单。
		/// </summary>
		[Category("Spark"), Description("子项的右击菜单。")]
		[DefaultValue(null)]
		public SparkContextMenuStrip ChildContextMenu { get; set; }

		/// <summary>
		/// 获取或设置控件的数据源，优先级高于Items,XElement,DataTable,List的实体集合,DataView，需要设置GroupName和DisplayMember，否则将引起异常
		/// </summary>
		[Browsable(false)]
		public object DataSouce
		{
			get => this.dataSouce;
			set
			{
				if (this.dataSouce != value)
				{
					this.GetData();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 获取控件包含的项的集合。
		/// </summary>
		[Browsable(false)]
		public List<SparkGroupListItem> Items => this.items;
		#endregion

		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkGroupList()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |          //调整大小时重绘
						 ControlStyles.DoubleBuffer |           //双缓冲
						 ControlStyles.OptimizedDoubleBuffer |  //双缓冲
						 ControlStyles.AllPaintingInWmPaint |   //禁止擦除背景
						 ControlStyles.UserPaint, true
			);

			base.Font = Consts.DEFAULT_FONT;
			this.Padding = new Padding(1);
			this.Theme = new SparkGroupListTheme(this);
		}
		#endregion

		#region 重写事件
		/// <summary>
		/// 重写绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.isInit) return;
			Graphics g = e.Graphics;
			g.Clear(this.Theme.BackColor);
			if (this.dataSouceItems.Any() || this.Items.Any())
			{
				GDIHelper.InitializeGraphics(g);
				g.TranslateTransform(0, -this.VerticalScroll.Value);
				this.CalcItem();
				this.DrawItem(g);
				g.ResetTransform();
			}
			if (this.BorderStyle != BorderStyle.None)
			{
				GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
			}
			//ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, this.Theme.BorderColor, ButtonBorderStyle.Solid);
		}

		/// <summary>
		/// 重写鼠标Down
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			if (this._hoverItem == null) { return; }

			if (e.Button == MouseButtons.Right && this.ChildContextMenu != null && !this.dataSouceItems.Contains(this._hoverItem))
			{
				this.ChildContextMenu.Tag = this._hoverItem;
				this.ChildContextMenu.Show(this, e.Location);
			}
			else if (e.Button == MouseButtons.Left)
			{
				if (GroupListItemClick != null)
				{
					SparkGroupListImageButton imageItem = this._hoverItem.RightImages?.FirstOrDefault(a => a.Rectangle.Contains(e.Location));
					GroupListItemClick(this, new SparkGroupListItemClickEventArgs(this._hoverItem, imageItem));
				}
			}
		}

		/// <summary>
		/// 重写鼠标离开
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			if (this._hoverItem != null)
			{
				this.Invalidate();
				this._hoverItem = null;
			}
		}

		/// <summary>
		/// 重写鼠标移动
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.dataSouceItems != null)
			{
				SparkGroupListItem item = this.dataSouceItems.FirstOrDefault(a => a.Rectangle.Contains(e.Location));
				if (item != null)
				{
					if (this._hoverItem == item) return;
					this._hoverItem = item;
					this.Invalidate();
					return;
				}
				else
				{
					item = this.dataSouceItems.FirstOrDefault(a => a.GroupRectangle.Contains(e.Location));
					if (item != null)
					{
						item = item.Items.FirstOrDefault(a => a.Rectangle.Contains(e.Location));
						if (item != null)
						{
							if (this._hoverItem == item) return;
							this._hoverItem = item;
							this.Invalidate();
							return;
						}
					}
				}
				if (item == null && this._hoverItem != null)
				{
					this._hoverItem = null;
					this.Invalidate();
				}
			}
		}

		public override void Refresh()
		{
			base.Refresh();
			this.CalcItem();
			this.Invalidate();
		}
		#endregion

		#region 私有方法
		private void GetData()
		{
			if (this.DataSouce != null)
			{
				if (this.DataSouce is DataTable dt)
				{
					this.DataType = EnumDataType.DataTable;
					EnumerableRowCollection<SparkGroupListItem> list = dt.AsEnumerable().Select(a => new SparkGroupListItem(this.Theme)
					{
						Key = Convert.ToString(a[this.DisplayMember]),
						Text = Convert.ToString(a["DisplayMember"]),
						Tag = a,
					});
					this.dataSouceItems = new List<SparkGroupListItem>();
					foreach (IGrouping<string, SparkGroupListItem> item in list.GroupBy(a => a.Key))
					{
						SparkGroupListItem temp = new SparkGroupListItem(item.Key, item.Key, this.Theme)
						{
							Items = item.ToList()
						};
						this.dataSouceItems.Add(temp);
					}
				}
				else if (this.DataSouce is XElement xel)
				{
					this.DataType = EnumDataType.XElement;

				}
				else if (this.DataSouce is DataView)
				{
					this.DataType = EnumDataType.DataView;
				}
				else if (this.DataSouce is IList list && list.Count > 0 && list[0].GetType().IsClass)
				{
					this.DataType = EnumDataType.ListClass;
				}
			}
			else if (this.Items.Any())
			{
				this.DataType = EnumDataType.Items;
				this.dataSouceItems = this.items;
			}
		}

		private void CalcItem()
		{
			if (this.DataType == EnumDataType.Items || this.DataType == EnumDataType.None)
			{
				this.dataSouceItems = this.items;
			}
			Rectangle rect = new Rectangle(this.Padding.Left, this.Padding.Top, this.Width, this.GroupHeight);
			foreach (SparkGroupListItem group in this.dataSouceItems)
			{
				rect.Width = this.Width - this.Margin.Left - this.Margin.Right - this.Padding.Left - this.Padding.Right;
				rect.Height = this.GroupHeight;
				rect.X = this.Padding.Left + this.Margin.Left;
				rect.Y += this.Margin.Top;
				group.Rectangle = rect;
				group.CalcImageRectangle();
				rect.Y += this.GroupHeight;
				if (group.Items.Any())
				{
					foreach (SparkGroupListItem child in group.Items)
					{
						rect.Height = this.ItemHeight;
						child.Rectangle = rect;
						child.CalcImageRectangle();
						rect.Y += this.ItemHeight;
					}
				}
				rect.Y += this.GroupSpacing;
			}
		}

		private void DrawItem(Graphics g)
		{
			int maxHeight = 0;
			foreach (SparkGroupListItem group in this.dataSouceItems)
			{
				group.DrawGroup(g, this.font, this.groupFont, this._hoverItem);
				maxHeight = group.Rectangle.Bottom;
				if (group.Items.Any())
				{
					maxHeight = group.Items.LastOrDefault().Rectangle.Bottom;
				}
			}

			if (maxHeight > this.Height)
			{
				// 这里 20 为滚动条的宽度
				this.AutoScrollMinSize = new Size(this.Width - 20, maxHeight);
			}
			else
			{
				this.AutoScrollMinSize = new Size(this.Width, this.Height);
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 根据子项Key查询归属的组
		/// </summary>
		/// <param name="itemKey"></param>
		/// <returns></returns>
		public SparkGroupListItem FindGroupByItemKey(string itemKey)
		{
			return this.dataSouceItems.FirstOrDefault(a => a.Items.Any(b => b.Key == itemKey));
		}

		/// <summary>
		/// 查询组
		/// </summary>
		/// <param name="groupKey"></param>
		/// <returns></returns>
		public SparkGroupListItem FindGroup(string groupKey)
		{
			return this.dataSouceItems.FirstOrDefault(a => a.Key == groupKey);
		}

		/// <summary>
		/// 查询子项
		/// </summary>
		/// <param name="groupKey"></param>
		/// <param name="itemKey"></param>
		/// <returns></returns>
		public SparkGroupListItem FindChild(string groupKey, string itemKey)
		{
			SparkGroupListItem item = this.FindGroup(groupKey);
			return item?.Items?.FirstOrDefault(a => a.Key == itemKey);
		}

		/// <summary>
		/// 添加组
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool AddGroup(params SparkGroupListItem[] item)
		{
			this.Items.AddRange(item);
			this.Invalidate();
			return true;
		}

		/// <summary>
		/// 添加子项
		/// </summary>
		/// <param name="groupKey"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		public bool AddChild(string groupKey, params SparkGroupListItem[] items)
		{
			if (this.Items.Any(a => a.Key == groupKey))
			{
				SparkGroupListItem group = this.Items.FirstOrDefault(a => a.Key == groupKey);

				group.Items.AddRange(items);
				this.Invalidate();
				return true;
			}
			return false;
		}

		/// <summary>
		/// 移除组
		/// </summary>
		/// <param name="groupKey"></param>
		public void RemoveGroup(string groupKey)
		{
			if (this.Items.Any(a => a.Key == groupKey))
			{
				SparkGroupListItem group = this.Items.FirstOrDefault(a => a.Key == groupKey);
				this.Items.Remove(group);
				this.Invalidate();
			}
		}

		/// <summary>
		/// 清空组
		/// </summary>
		/// <param name="groupKey"></param>
		public void ClearGroup(string groupKey)
		{
			if (this.Items.Any(a => a.Key == groupKey))
			{
				SparkGroupListItem group = this.Items.FirstOrDefault(a => a.Key == groupKey);
				group.Items.Clear();
				this.Invalidate();
			}
		}

		/// <summary>
		/// 移除子项
		/// </summary>
		/// <param name="groupKey"></param>
		/// <param name="childKey"></param>
		public void RemoveChild(string groupKey, string childKey)
		{
			if (this.Items.Any(a => a.Key == groupKey))
			{
				SparkGroupListItem group = this.Items.FirstOrDefault(a => a.Key == groupKey);
				if (group.Items.Any(a => a.Key == childKey))
				{
					SparkGroupListItem child = group.Items.FirstOrDefault(a => a.Key == childKey);
					group.Items.Remove(child);
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 清空
		/// </summary>
		public void Clear()
		{
			this.Items.Clear();
			this.Invalidate();
		}
		#endregion

		#region ISupportInitialize 成员
		public void BeginInit()
		{
			this.isInit = true;
		}

		public void EndInit()
		{
			this.isInit = false;
		}
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkGroupListTheme Theme { get; private set; }

		#endregion

		private enum EnumDataType
		{
			None = 0,
			Items,
			DataTable,
			DataView,
			ListClass,
			XElement,
		}
	}

	/// <summary>
	/// 表示 <see cref="SparkGroupList"/> 控件的项。
	/// </summary>
	public class SparkGroupListItem : IDisposable
	{
		private StringFormat _stringFormat = null;

		/// <summary>
		/// 右边绘制的图标集合
		/// </summary>
		public List<SparkGroupListImageButton> RightImages { get; set; }

		/// <summary>
		/// 子项集合对象
		/// </summary>
		public List<SparkGroupListItem> Items { get; internal set; } = new List<SparkGroupListItem>();

		/// <summary>
		/// 键值
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// 文本
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 备注信息
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// 主题
		/// </summary>
		public SparkGroupListTheme Theme { get; private set; }

		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkGroupListItem(SparkGroupListTheme theme) : this("", "", "", theme)
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="text"></param>
		public SparkGroupListItem(string text, SparkGroupListTheme theme) : this(text, text, null, theme)
		{

		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="key"></param>
		/// <param name="text"></param>
		public SparkGroupListItem(string key, string text, SparkGroupListTheme theme) : this(key, text, null, theme)
		{

		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="key"></param>
		/// <param name="text"></param>
		/// <param name="tag"></param>
		public SparkGroupListItem(string key, string text, object tag, SparkGroupListTheme theme)
		{
			this.Key = key;
			this.Text = text;
			this.Tag = tag;
			this.Theme = theme;
			this._stringFormat = new StringFormat()
			{
				Alignment = StringAlignment.Near,
				LineAlignment = StringAlignment.Center,
				Trimming = StringTrimming.EllipsisCharacter,
				FormatFlags = StringFormatFlags.NoWrap
			};
		}

		/// <summary>
		/// 重写ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Text ?? "";
		}

		/// <summary>
		/// 项目的位置大小
		/// </summary>
		public Rectangle Rectangle { get; internal set; }

		internal Rectangle GroupRectangle
		{
			get
			{
				if (this.Items.Count > 0)
				{
					SparkGroupListItem last = this.Items.LastOrDefault();
					return new Rectangle(this.Rectangle.X, this.Rectangle.Y, this.Rectangle.Width, last.Rectangle.Bottom);
				}
				else
				{
					return this.Rectangle;
				}
			}
		}

		/// <summary>
		/// 计算图片所在位置大小
		/// </summary>
		internal void CalcImageRectangle()
		{
			if (this.RightImages != null && this.RightImages.Any())
			{
				int y = this.Rectangle.Y + (this.Rectangle.Height - 16) / 2;
				int x = this.Rectangle.Right;
				Rectangle rect = new Rectangle(x, y, 16, 16);
				foreach (SparkGroupListImageButton item in this.RightImages)
				{
					rect.X -= 16 + 3;
					item.Rectangle = rect;
				}
			}
		}

		internal int MaxImageWidth()
		{
			if (this.RightImages != null && this.RightImages.Any())
			{
				return this.RightImages.Count * (16 + 3) + 3;
			}
			return 0;
		}

		/// <summary>
		/// 绘制组
		/// </summary>
		/// <param name="g"></param>
		/// <param name="theme"></param>
		/// <param name="font"></param>
		/// <param name="groupFont"></param>
		/// <param name="hoverItem"></param>
		internal void DrawGroup(Graphics g, Font font, Font groupFont, SparkGroupListItem hoverItem)
		{
			//绘制整个组的背景和边框
			GDIHelper.FillRectangle(g, this.GroupRectangle, this.Theme.GroupBackColor);
			GDIHelper.DrawRectangle(g, this.GroupRectangle, this.Theme.GroupBorderColor);

			if (this == hoverItem)
			{
				GDIHelper.FillRectangle(g, this.Rectangle, this.Theme.MouseOverBackColor);

				GDIHelper.DrawString(g, this.Rectangle, this.Text, groupFont, this.Theme.MouseOverForeColor, this._stringFormat);
			}
			else
			{
				GDIHelper.FillRectangle(g, this.Rectangle, this.Theme.GroupBackColor);
				GDIHelper.DrawString(g, this.Rectangle, this.Text, groupFont, this.Theme.ForeColor, this._stringFormat);
			}

			if (this.RightImages != null && this.RightImages.Any())
			{
				foreach (SparkGroupListImageButton item in this.RightImages)
				{
					item.DrawImage(g);
				}
			}

			if (this.Items.Any())
			{
				foreach (SparkGroupListItem child in this.Items)
				{
					child.DrawItem(g, font, hoverItem);
				}
			}
		}

		/// <summary>
		/// 绘制项目
		/// </summary>
		/// <param name="g"></param>
		/// <param name="theme"></param>
		/// <param name="font"></param>
		/// <param name="hoverItem"></param>
		internal void DrawItem(Graphics g, Font font, SparkGroupListItem hoverItem)
		{
			if (this == hoverItem)
			{
				GDIHelper.FillRectangle(g, this.Rectangle, this.Theme.MouseOverBackColor);
				GDIHelper.DrawString(g, this.Rectangle, this.Text, font, this.Theme.MouseOverForeColor, this._stringFormat);
			}
			else
			{
				GDIHelper.FillRectangle(g, this.Rectangle, this.Theme.GroupBackColor);
				GDIHelper.DrawString(g, this.Rectangle, this.Text, font, this.Theme.ForeColor, this._stringFormat);
			}

			if (this.RightImages != null && this.RightImages.Any())
			{
				foreach (SparkGroupListImageButton item in this.RightImages)
				{
					item.DrawImage(g);

				}
			}
		}

		public void Dispose()
		{
			this._stringFormat = null;
			this.RightImages?.Clear();
			this.Items?.Clear();
			this.Tag = null;
		}
	}

	/// <summary>
	/// 表示 <see cref="SparkGroupList"/> 控件的图标按钮。
	/// </summary>
	public class SparkGroupListImageButton : IDisposable
	{
		/// <summary>
		/// 图片Key
		/// </summary>
		public string Key { get; set; }
		/// <summary>
		/// 图片对象
		/// </summary>
		public Image Image { get; set; }

		/// <summary>
		/// 备注信息
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkGroupListImageButton()
		{

		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="key"></param>
		/// <param name="image"></param>
		public SparkGroupListImageButton(string key, Image image)
		{
			this.Key = key;
			this.Image = image;
		}

		/// <summary>
		/// 图片的所在的位置
		/// </summary>
		internal Rectangle Rectangle { get; set; }

		/// <summary>
		/// 绘制图片
		/// </summary>
		/// <param name="g"></param>
		internal void DrawImage(Graphics g)
		{
			if (this.Image != null)
			{
				GDIHelper.DrawImage(g, this.Rectangle, this.Image);
			}
		}

		public void Dispose()
		{
			this.Image = null;
			this.Tag = null;
		}
	}
}