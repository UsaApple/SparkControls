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
	/// 列表视图控件。
	/// </summary>
	[ToolboxBitmap(typeof(ListView))]
	public class SparkListView : ListView, ISupportInitialize, ISparkTheme<SparkListViewTheme>
	{
		#region 私有变量
		private bool isIniting = false;
		#endregion

		#region 属性
		private Font font = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文字的字体。
		/// </summary>
		[Category("Spark"), Description("获取或设置控件显示的文字的字体")]
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
		/// 背景色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 字体颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		/// <summary>
		/// 获取或设置要显示的图片列表。
		/// </summary>
		[DefaultValue(null)]
		[Category("Spark"), Description("设置要显示的图片列表。")]
		public ImageList ImageList { get; set; }

		/// <summary>
		/// 获取或设置用于标识图标的属性。
		/// </summary>
		[DefaultValue("")]
		[Category("Spark"), Description("设置用于标识图标的属性。")]
		public string ImageMember { get; set; } = string.Empty;
		#endregion

		#region 构造函数
		/// <summary>
		/// 初始 <see cref="SparkListView"/> 类型的新实例。
		/// </summary>
		public SparkListView()
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
			base.Font = this.font;
			base.BorderStyle = BorderStyle.FixedSingle;
			this.Theme = new SparkListViewTheme(this);
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
				this.ImageMember.IsNullOrEmpty() ||
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

			if (!key.IsNullOrEmpty() &&
				this.ImageList.Images.ContainsKey(key))
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
		#endregion

		#region ISupportInitialize 接口方法。
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
		public SparkListViewTheme Theme { get; private set; }

		#endregion
	}
}