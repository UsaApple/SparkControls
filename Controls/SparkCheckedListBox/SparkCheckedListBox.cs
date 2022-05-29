using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Controls
{
	/// <summary>
	/// 带复选框的列表框控件
	/// </summary>
	[ToolboxBitmap(typeof(CheckedListBox))]
	public class SparkCheckedListBox : CheckedListBox, ISupportInitialize, ISparkTheme<SparkCheckedListBoxTheme>
	{
		#region 私有变量
		private readonly CornerRadius mCornerRadius = new CornerRadius(0, 0, 0, 0);

		private bool mIsIniting = false;
		private int mLeftMargin = 0;
		private int mItemHeight = 30;
		private int mMouseOverIndex = -1;
		private DrawItemState mDrawItemState = DrawItemState.Default;
		#endregion

		#region 属性
		private Font mFont = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文字的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文字的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get => this.mFont;
			set
			{
				if (this.mFont != value)
				{
					base.Font = this.mFont = value;
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
		/// 获取或设置用于标识图标的属性。
		/// </summary>
		[DefaultValue("")]
		[Category("Spark"), Description("设置用于标识图标的属性。")]
		public string ImageMember { get; set; } = "";

		/// <summary>
		/// 获取或设置要显示的图片列表。
		/// </summary>
		[DefaultValue(null)]
		[Category("Spark"), Description("要显示的图片列表。")]
		public ImageList ImageList { get; set; }

		/// <summary>
		/// 获取或设置项的高度。
		/// </summary>
		[DefaultValue(30)]
		[Category("Spark"), Description("项的高度。")]
		public override int ItemHeight
		{
			get => this.mItemHeight;
			set => this.mItemHeight = value;
		}

		/// <summary>
		/// 获取或设置项的左边距。
		/// </summary>
		[DefaultValue(0)]
		[Category("Spark"), Description("项的左边距。")]
		public int LeftMargin
		{
			get => this.mLeftMargin;
			set
			{
				if (this.mLeftMargin != value)
				{
					this.mLeftMargin = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 获取控件的绘制模式。
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DrawMode DrawMode => DrawMode.OwnerDrawFixed;
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkCheckedListBox()
		{
			this.BeginInit();

			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
						  ControlStyles.DoubleBuffer |                  //双缓冲
						  ControlStyles.OptimizedDoubleBuffer |         //双缓冲
						  ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
						  ControlStyles.SupportsTransparentBackColor |  //透明
						  ControlStyles.UserPaint, true
			);

			base.ItemHeight = this.mItemHeight;
			base.Font = this.mFont;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.DrawMode = DrawMode.OwnerDrawFixed;
			this.Theme = new SparkCheckedListBoxTheme(this);

			this.EndInit();
		}
		#endregion

		#region 重写事件
		/// <summary>
		/// 绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.mIsIniting) return;
			if (this.GetStyle(ControlStyles.UserPaint))
			{
				Message m = new Message
				{
					HWnd = Handle,
					Msg = (int)Msgs.WM_PRINTCLIENT,
					WParam = e.Graphics.GetHdc(),
					LParam = (IntPtr)0x00000004
				};
				this.DefWndProc(ref m);
				e.Graphics.ReleaseHdc(m.WParam);
			}

			if (this.BorderStyle != BorderStyle.None)
			{
				GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
			}
			//ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), this.Theme.BorderColor, ButtonBorderStyle.Solid);
			base.OnPaint(e);
		}

		/// <summary>
		/// 重写绘制项目事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index < 0 || this.Items.Count <= 0) return;
			if (this.mIsIniting) return;
			Graphics g = e.Graphics;
			//组合2：3|14|3|图标|3|文字
			//组合2：3|14|3|文字
			Rectangle rect = e.Bounds;
			GDIHelper.InitializeGraphics(g);
			int intLeft = 3 + this.LeftMargin;

			Color backColor;
			Color borderColor;
			Color foreColor;
			if (e.State.HasFlag(DrawItemState.Selected))
			{
				this.mDrawItemState = DrawItemState.Selected;
				backColor = this.Theme.SelectedBackColor;
				borderColor = this.Theme.SelectedBorderColor;
				foreColor = this.Theme.SelectedForeColor;
			}
			else if (e.Index == this.mMouseOverIndex)
			{
				this.mDrawItemState = DrawItemState.HotLight;
				backColor = this.Theme.MouseOverBackColor;
				borderColor = this.Theme.MouseOverBorderColor;
				foreColor = this.Theme.MouseOverForeColor;
			}
			else
			{
				this.mDrawItemState = DrawItemState.Default;
				backColor = this.Theme.BackColor;
				borderColor = this.Theme.BorderColor;
				foreColor = this.Theme.ForeColor;
			}
			//DrawBack
			this.OnDrawItemBack(g, rect, backColor);

			//Draw Check
			int num = (e.Bounds.Height - 14) / 2;
			if (num < 0) num = 0;
			this.OnDrawCheck(g, new Rectangle(new Point(rect.X + intLeft, rect.Y + num), new Size(14, 14)), e.Index);
			intLeft += 14 + 3;

			Image image = this.GetDrawImageKey(this.Items[e.Index]);

			//Draw Image
			if (image != null)
			{
				num = (e.Bounds.Height - this.ImageList.ImageSize.Height) / 2;
				if (num < 0) num = 0;
				this.OnDrawItemImage(g, image, new Rectangle(new Point(rect.X + intLeft, rect.Y + num), this.ImageList.ImageSize));
				intLeft += this.ImageList.ImageSize.Width + 3;
			}

			//DrawText
			this.OnDrawItemText(g, e.Index, new RectangleF(rect.X + intLeft, rect.Y, rect.Width - intLeft, rect.Height), foreColor);

			//Draw Line
			g.DrawLine(new Pen(borderColor, 1f), rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width, rect.Y + rect.Height - 1);

		}

		/// <summary>
		/// 重写鼠标移动事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			int index = this.IndexFromPoint(e.Location);
			int lastIndex = this.mMouseOverIndex;
			if (index != this.mMouseOverIndex)
			{
				this.mMouseOverIndex = index != this.SelectedIndex ? index : -1;
				if (this.mMouseOverIndex != -2)
				{
					if (lastIndex > -1)
					{
						this.Invalidate(this.GetItemRectangle(lastIndex));
					}
					if (this.mMouseOverIndex > -1)
					{
						this.Invalidate(this.GetItemRectangle(this.mMouseOverIndex));
					}
				}
				if (this.mMouseOverIndex == -1)
				{
					this.mMouseOverIndex = -2;
				}
			}
		}

		/// <summary>
		/// 鼠标离开事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			if (this.mMouseOverIndex > -1)
			{
				Rectangle rect = this.GetItemRectangle(this.mMouseOverIndex);
				this.mMouseOverIndex = -1;
				this.Invalidate(rect);
			}
		}

		/// <summary>
		/// 重写 <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />。
		/// </summary>
		/// <param name="m">要处理的 Windows<see cref="T:System.Windows.Forms.Message" />。</param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0014) // 禁掉清除背景消息WM_ERASEBKGND
				return;
			base.WndProc(ref m);
		}

		/// <summary>
		/// 释放方法
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.ImageList != null)
			{
				this.ImageList.Dispose();
				this.ImageList = null;
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// 获取项目的图标
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual Image GetDrawImageKey(object item)
		{
			if (this.ImageList == null ||
				this.ImageList.Images.Count == 0 ||
				this.ImageMember.IsNullOrEmpty() ||
				item == null
			) { return null; }

			string key = "";
			if (item is DataRow dr)
			{
				if (dr.Table.HasColumn(this.ImageMember))
				{
					key = Convert.ToString(dr[this.ImageMember]);
				}
			}
			else if (item is DataRowView drv)
			{
				if (drv.Row.Table.HasColumn(this.ImageMember))
				{
					key = Convert.ToString(drv[this.ImageMember]);
				}
			}
			else if (item.GetType().IsClass)
			{
				PropertyInfo pro = item.GetType().GetProperty(this.ImageMember, BindingFlags.Public | BindingFlags.Instance);
				if (pro != null)
				{
					key = Convert.ToString(pro.GetValue(item, null));
				}
			}

			if (!key.IsNullOrEmpty() && this.ImageList.Images.ContainsKey(key))
			{
				return this.ImageList.Images[key];
			}
			return null;
		}

		/// <summary>
		/// 绘制Image
		/// </summary>
		/// <param name="g"></param>
		/// <param name="image"></param>
		/// <param name="rect"></param>
		protected virtual void OnDrawItemImage(Graphics g, Image image, RectangleF rect)
		{
			if (image != null)
			{
				//Draw Image
				g.DrawImage(image, rect);
			}
		}

		/// <summary>
		/// 绘制背景
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="backColor"></param>
		protected virtual void OnDrawItemBack(Graphics g, RectangleF rect, Color backColor)
		{
			using (SolidBrush sb = new SolidBrush(backColor))
			{
				g.FillRectangle(sb, rect);
			}
		}

		/// <summary>
		/// 绘制文本
		/// </summary>
		/// <param name="g"></param>
		/// <param name="index"></param>
		/// <param name="rect"></param>
		/// <param name="foreColor"></param>
		protected virtual void OnDrawItemText(Graphics g, int index, RectangleF rect, Color foreColor)
		{
			string text = this.GetItemText(this.Items[index]);
			using (StringFormat sf = new StringFormat())
			{
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Near;
				GDIHelper.DrawString(g, rect, text, this.Font, foreColor, sf);
			}
		}

		/// <summary>
		/// 绘制复选框
		/// </summary>
		/// <param name="g"></param>
		/// <param name="boxRect"></param>
		/// <param name="index"></param>
		protected virtual void OnDrawCheck(Graphics g, Rectangle boxRect, int index)
		{
			// 绘制状态
			bool checkState = this.GetItemChecked(index);
			RoundRectangle roundRect = new RoundRectangle(boxRect, this.mCornerRadius);
			Color backColor;
			Color borderColor;
			Color tickColor;
			if (this.mDrawItemState == DrawItemState.Selected && checkState)
			{
				//组合选中
				backColor = this.Theme.CheckBoxTheme.CombinedBackColor;
				borderColor = this.Theme.CheckBoxTheme.CombinedSelectedColor;
				tickColor = this.Theme.CheckBoxTheme.CombinedSelectedColor;
			}
			else if (checkState)
			{
				backColor = this.Theme.CheckBoxTheme.SelectedBackColor;
				borderColor = this.Theme.CheckBoxTheme.SelectedBorderColor;
				tickColor = this.Theme.CheckBoxTheme.TickColor;
			}
			else
			{
				backColor = this.Theme.CheckBoxTheme.BackColor;
				borderColor = this.Theme.CheckBoxTheme.BorderColor;
				tickColor = this.Theme.CheckBoxTheme.TickColor;
			}
			GDIHelper.DrawCheckBox(g, roundRect, backColor, borderColor, 1);
			if (checkState)
			{
				GDIHelper.DrawCheckTick(g, boxRect, tickColor);
			}
		}
		#endregion

		#region ISupportInitialize 接口方法
		public void BeginInit()
		{
			this.mIsIniting = true;
		}

		public void EndInit()
		{
			this.mIsIniting = false;
		}
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkCheckedListBoxTheme Theme { get; private set; }

		#endregion
	}
}