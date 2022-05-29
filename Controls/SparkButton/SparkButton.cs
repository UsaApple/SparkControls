using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 按钮控件
	/// </summary>
	[ToolboxBitmap(typeof(Button)), Description("按钮")]
	public class SparkButton : Button, ICloneable, ISparkTheme<SparkButtonTheme>
	{
		#region 字段

		/// <summary>
		/// 按钮背景色
		/// </summary>
		private Color mBackColor = default;

		/// <summary>
		/// 按钮边框色
		/// </summary>
		private Color mBorderColor = default;

		/// <summary>
		/// 按钮前景色
		/// </summary>
		private Color mForeColor = default;

		/// <summary>
		/// 控件当前状态
		/// </summary>
		private ControlState mControlState = ControlState.Default;

		/// <summary>
		/// 鼠标是否在控件上
		/// </summary>
		private bool mIsMouseEnter = false;

		#endregion

		#region 属性

		private Font mFont = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文本的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get => this.mFont;
			set
			{
				base.Font = this.mFont = value;
				this.Invalidate();
				this.OnFontChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// 获取或设置控件的背景色。
		/// </summary>
		[Category("Spark"), Description("控件的背景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		/// <summary>
		/// 获取或设置控件的前景色。
		/// </summary>
		[Category("Spark"), Description("控件的前景色。")]
		[DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}

		/// <summary>
		/// 获取或设置下拉项目的集合。
		/// </summary>
		[Category("Spark"), Description("下拉项目的集合。")]
		[DefaultValue(null)]
		//[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string[] Items { set; get; } = null;

		private Size mSize = new Size(90, 30);
		/// <summary>
		/// 获取或设置控件的大小。
		/// </summary>
		[Category("Spark"), Description("控件的大小。")]
		[DefaultValue(typeof(Size), "90,30")]
		public new Size Size
		{
			get => this.mSize;
			set
			{
				base.Size = this.mSize = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// 获取或设置下拉按钮的大小。
		/// </summary>
		[Category("Spark"), Description("下拉按钮的大小。")]
		[DefaultValue(typeof(Size), "90,30")]
		public Size ItemSize { set; get; }

		/// <summary>
		/// 获取或设置下拉项目的集合当前点击的按钮对象。
		/// </summary>
		[Browsable(false)]
		[DefaultValue(null)]
		public SparkButton ClickedButton { set; get; }

		#endregion

		public SparkButton()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |          // 调整大小时重绘
				ControlStyles.DoubleBuffer |                    // 双缓冲
				ControlStyles.OptimizedDoubleBuffer |           // 双缓冲
				ControlStyles.AllPaintingInWmPaint |            // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
				ControlStyles.SupportsTransparentBackColor |    // 模拟透明度
				ControlStyles.UserPaint, true                   // 控件绘制代替系统绘制
			);
			this.Font = this.mFont;
			this.Theme = new SparkButtonTheme(this);
		}

		#region 私有方法

		private void InitControlStyle()
		{
			switch (this.mControlState)
			{
				case ControlState.Focused:
					this.mBackColor = this.Theme.MouseDownBackColor;
					this.mBorderColor = this.Theme.MouseDownBorderColor;
					this.mForeColor = this.Theme.MouseDownForeColor;
					break;
				case ControlState.Highlight:
					this.mBackColor = this.Theme.MouseOverBackColor;
					this.mBorderColor = this.Theme.MouseOverBorderColor;
					this.mForeColor = this.Theme.MouseOverForeColor;
					break;
				default:
					this.mBackColor = this.Theme.BackColor;
					this.mBorderColor = this.Theme.BorderColor;
					this.mForeColor = this.Theme.ForeColor;
					break;
			}
			if (!this.Enabled) this.mBackColor = this.Theme.DisabledBackColor;
		}

		/// <summary>
		/// 重绘按钮
		/// </summary>
		/// <param name="g">上下文</param>
		/// <param name="rect">按钮区域大小</param>
		private void Draw(Graphics g, Rectangle rect)
		{
			this.InitControlStyle();
			GDIHelper.FillRectangle(g, rect, this.mBackColor);
			rect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
			GDIHelper.DrawRectangle(g, rect, this.mBorderColor);
			this.DrawImage(g, rect);
			this.DrawString(g, rect);
			bool hasChildren = this.Items != null && this.Items.Count() != 0;
			if (!hasChildren) return;
			g.SmoothingMode = SmoothingMode.HighQuality;
			int width = 14, height = 10;
			PointF point1 = new PointF(rect.Width - width - 10, (rect.Height - height) / 2 + 1);
			PointF point2 = new PointF(rect.Width - 10, (rect.Height - height) / 2 + 1);
			PointF point3 = new PointF(rect.Width - width / 2 - 10, rect.Height / 2 + height / 2 + 1);
			GraphicsPath gp = new GraphicsPath();
			gp.AddLine(point1, point2);
			gp.AddLine(point2, point3);
			gp.AddLine(point3, point1);
			GDIHelper.FillPath(g, gp, Color.White);
			GDIHelper.DrawPathBorder(g, gp, this.mBorderColor);
		}

		/// <summary>
		/// 绘制图片
		/// </summary>
		private void DrawImage(Graphics g, Rectangle rect)
		{
			Image image = this.GetImage();
			if (image == null) return;
			Point p = this.CalcPointByAlign(image.Size, rect, this.ImageAlign);
			g.DrawImage(image, new Rectangle(p, image.Size));
		}

		/// <summary>
		/// 绘制字符串
		/// </summary>
		private void DrawString(Graphics g, Rectangle rect)
		{
			string[] array = this.ParseShortCut();
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			Size size = GDIHelper.MeasureString(g, string.Join("", array), this.Font).ToSize();
			Point p = this.CalcPointByAlign(size, rect, this.TextAlign);
			using (SolidBrush brush = new SolidBrush(this.mForeColor))
			{
				g.DrawString(array[0], this.Font, brush, p.X, p.Y);
				size = GDIHelper.MeasureString(g, array[0], this.Font).ToSize();
				Font font = new Font(this.Font.FontFamily, this.Font.Size, this.Font.Style | FontStyle.Underline);
				g.DrawString(array[1], font, brush, p.X + size.Width, p.Y);
				font.Dispose();
				size = GDIHelper.MeasureString(g, array[0] + array[1], this.Font).ToSize();
				g.DrawString(array[2], this.Font, brush, p.X + size.Width - 1, p.Y);
			}
		}

		/// <summary>
		/// 转义快捷键符号。
		/// </summary>
		/// <returns>转义后的文本数组。</returns>
		private string[] ParseShortCut()
		{
			if (!this.Text.Contains("&")) return new string[] { this.Text, "", "" };
			int index = this.Text.IndexOf("&");
			string a1 = this.Text.Substring(0, index);
			string a2 = this.Text.Substring(index + 1, 1);
			string a3 = this.Text.Substring(index + 2);
			return new string[] { a1, a2, a3 };
		}

		/// <summary>
		/// 获取绘制图像
		/// </summary>
		private Image GetImage()
		{
			if (this.Image != null) return this.Image;
			ImageList.ImageCollection images = this.ImageList?.Images;
			if (images == null) return null;
			if (!this.ImageKey.IsNullOrEmpty() && images.ContainsKey(this.ImageKey)) return images[this.ImageKey];
			if (this.ImageIndex >= 0 && this.ImageIndex < images.Count) return images[this.ImageKey];
			return null;
		}

		/// <summary>
		/// 根据对齐方式获取坐标点
		/// </summary>
		/// <param name="size">内容大小</param>
		/// <param name="rect">容器大小</param>
		/// <param name="align">对齐方式</param>
		/// <returns></returns>
		private Point CalcPointByAlign(Size size, Rectangle rect, ContentAlignment align)
		{
			int x = 0, y = 0;
			string s = align.ToString().ToUpper();
			if (s.EndsWith("CENTER")) x = (rect.Width - size.Width) / 2;
			else if (s.EndsWith("RIGHT")) x = rect.Width - size.Width;
			else x = 2;

			if (s.StartsWith("MIDDLE")) y = (rect.Height - size.Height) / 2;
			else if (s.StartsWith("BOTTOM")) y = rect.Height - size.Height;
			return new Point(x, y);
		}

		/// <summary>
		/// 显示按钮组内容
		/// </summary>
		private void ShowButtonChildren()
		{
			if (this.Items == null || this.Items.Count() == 0) return;
			FlowLayoutPanel flowPanel = new FlowLayoutPanel()
			{
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				FlowDirection = FlowDirection.TopDown
			};
			foreach (string con in this.Items)
			{
				SparkButton button = this.Clone() as SparkButton;
				button.Margin = new Padding();
				button.Text = con;
				if (this.ItemSize != default) button.Size = this.ItemSize;
				button.Click += (s, e) =>
				{
					flowPanel.FindForm().Close();
					this.ClickedButton = s as SparkButton;
					base.OnClick(e);
					this.Focus();
				};
				flowPanel.Controls.Add(button);
			}
			new SparkPopup(this, flowPanel) { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink }.Show();
		}

		#endregion

		#region 重写方法

		protected override void OnMouseEnter(EventArgs e)
		{
			this.mIsMouseEnter = true;
			this.mControlState = ControlState.Highlight;
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.mIsMouseEnter = false;
			this.mControlState = ControlState.Default;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			this.mIsMouseEnter = true;
			this.mControlState = ControlState.Focused;
			base.OnMouseDown(mevent);
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			this.mControlState = ControlState.Default;
			this.mIsMouseEnter = true;
			base.OnMouseUp(mevent);
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);
			this.Draw(pevent.Graphics, this.ClientRectangle);
		}

		protected override void OnClick(EventArgs e)
		{
			if (this.Items == null || this.Items.Count() == 0)
			{
				this.ClickedButton = this;
				base.OnClick(e);
			}
			else this.ShowButtonChildren();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			if (this.mControlState == ControlState.Default)
			{
				this.mControlState = ControlState.Highlight;
			}
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (this.mIsMouseEnter == false)
			{
				this.mControlState = ControlState.Default;
			}
			base.OnLostFocus(e);
		}

		#endregion

		#region IClonable 接口成员

		public object Clone()
		{
			SparkButton button = new SparkButton()
			{
				Text = this.Text,
				Font = this.Font,
				Width = this.Width,
				Height = this.Height,
				Margin = this.Margin
			};
			return button;
		}

		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkButtonTheme Theme { get; private set; }

		#endregion
	}
}