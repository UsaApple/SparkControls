using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;

using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Controls
{
	/// <summary>
	/// ListBox控件
	/// </summary>
	[ToolboxBitmap(typeof(ListBox))]
	public class SparkListBox : ListBox, ISupportInitialize, ISparkTheme<SparkListBoxTheme>
	{
		#region 私有变量
		private bool isIniting = false;
		private int mouseOverIndex = -1;
		private int leftMargin = 0;
		private DrawMode drawMode = DrawMode.OwnerDrawFixed;
		#endregion

		#region 公共属性
		private Font font = Consts.DEFAULT_FONT;
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
		/// 获取或设置控件的绘制模式。
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(DrawMode.OwnerDrawFixed)]
		public override DrawMode DrawMode
		{
			get => this.drawMode;
			set => this.drawMode = value;
		}

		/// <summary>
		/// 获取或设置要显示的图片列表。
		/// </summary>
		[DefaultValue(null)]
		[Category("Spark"), Description("设置要显示的图片集合。")]
		public ImageList ImageList { get; set; }

		/// <summary>
		/// 获取或设置用于显示图标的属性。
		/// </summary>
		[DefaultValue("")]
		[Category("Spark"), Description("设置用于标识图标的属性。")]
		public string ImageMember { get; set; } = string.Empty;

		/// <summary>
		/// 获取或设置控件的左边距。
		/// </summary>
		[DefaultValue(0)]
		[Category("Spark"), Description("控件的左边距。")]
		public int LeftMargin
		{
			get => this.leftMargin;
			set
			{
				if (this.leftMargin != value)
				{
					this.leftMargin = value;
					this.Invalidate();
				}
			}
		}
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkListBox()
		{
			this.BeginInit();
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
						  ControlStyles.DoubleBuffer |                  //双缓冲
						  ControlStyles.OptimizedDoubleBuffer |         //双缓冲
						  ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
						  ControlStyles.SupportsTransparentBackColor |  //透明
						  ControlStyles.UserPaint
						  , true
			);

			base.Font = this.font;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.DrawMode = DrawMode.OwnerDrawFixed;
			this.Theme = new SparkListBoxTheme(this);
			this.EndInit();
		}
		#endregion

		#region 重写方法
		/// <summary>
		/// 绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.isIniting) return;
			if (this.GetStyle(ControlStyles.UserPaint))
			{
				Message m = new Message
				{
					HWnd = this.Handle,
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
			if (e.Index < 0 || this.Items.Count <= 0 || this.Items.Count <= e.Index) return;
			if (this.isIniting) return;
			Graphics g = e.Graphics;
			//组合2：3|图标|3|文字
			//组合2：3|文字
			Rectangle rect = e.Bounds;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			int intLeft = 3 + this.LeftMargin;
			Color backColor;
			Color borderColor;
			Color foreColor;
			if (e.State.HasFlag(DrawItemState.Selected))
			{
				backColor = this.Theme.SelectedBackColor;
				borderColor = this.Theme.SelectedBorderColor;
				foreColor = this.Theme.SelectedForeColor;
			}
			else if (e.Index == this.mouseOverIndex)
			{
				backColor = this.Theme.MouseOverBackColor;
				borderColor = this.Theme.MouseOverBorderColor;
				foreColor = this.Theme.MouseOverForeColor;
			}
			else
			{
				backColor = this.Theme.BackColor;
				borderColor = this.Theme.BorderColor;
				foreColor = this.Theme.ForeColor;
			}

			this.OnDrawItemBack(g, rect, backColor);

			//Draw
			Image image = this.GetDrawImageKey(this.Items[e.Index]);
			if (image != null)
			{
				int num = (e.Bounds.Height - this.ImageList.ImageSize.Height) / 2;
				if (num < 0) num = 0;
				this.OnDrawItemImage(g, image, new Rectangle(new Point(rect.X + intLeft, rect.Y + num), this.ImageList.ImageSize));
				intLeft += this.ImageList.ImageSize.Width + 5;
			}

			//Draw Text
			this.OnDrawItemText(g, e.Index, new RectangleF(rect.X + intLeft, rect.Y, rect.Width - intLeft - this.Padding.Left, rect.Height), foreColor);

			//Draw Line
			//e.Graphics.DrawLine(new Pen(this.Theme.NodeSplitLineColor, 1f), new Point(0, e.Bounds.Y + this._nodeHeight - 1), new Point(base.Width, e.Bounds.Y + this._nodeHeight - 1));
			e.Graphics.DrawLine(new Pen(borderColor, 1f), rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width, rect.Y + rect.Height - 1);
		}

		/// <summary>
		/// 重写鼠标移动事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			int index = this.IndexFromPoint(e.Location);
			int lastIndex = this.mouseOverIndex;
			if (index != this.mouseOverIndex)
			{
				if (index != this.SelectedIndex)
				{
					this.mouseOverIndex = index;
				}
				else
				{
					this.mouseOverIndex = -1;
				}
				if (this.mouseOverIndex != -2)
				{
					if (lastIndex > -1 && this.Items.Count > lastIndex)
					{
						this.Invalidate(this.GetItemRectangle(lastIndex));
					}
					if (this.mouseOverIndex > -1 && this.Items.Count > this.mouseOverIndex)
					{
						this.Invalidate(this.GetItemRectangle(this.mouseOverIndex));
					}
				}
				if (this.mouseOverIndex == -1)
				{
					this.mouseOverIndex = -2;
				}
			}
		}

		/// <summary>
		/// 鼠标离开事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			if (this.mouseOverIndex > -1 && this.Items.Count > this.mouseOverIndex)
			{
				Rectangle rect = this.GetItemRectangle(this.mouseOverIndex);
				this.mouseOverIndex = -1;
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
			if (disposing)
			{
				if (this.ImageList != null)
				{
					this.ImageList.Dispose();
					this.ImageList = null;
				}
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
				item == null) return null;

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
				Type type = item.GetType();
				PropertyInfo pro = type.GetProperty(this.ImageMember, BindingFlags.Public | BindingFlags.Instance);
				if (pro != null)
				{
					key = Convert.ToString(pro.GetValue(item, null));
				}
			}

			key = key.IsNullOrEmpty() ? item.ToString() : key;
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
			{//Draw Image
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
			using (StringFormat sf = new StringFormat())
			{
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags |= StringFormatFlags.NoWrap;
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Near;
				GDIHelper.DrawString(g, rect, this.GetItemText(this.Items[index]), this.Font, foreColor, sf);
			}
		}
		#endregion

		#region ISupportInitialize 接口方法
		public void BeginInit()
		{
			this.isIniting = true;
		}

		public void EndInit()
		{
			this.isIniting = false;
		}
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkListBoxTheme Theme { get; private set; }

		#endregion
	}
}