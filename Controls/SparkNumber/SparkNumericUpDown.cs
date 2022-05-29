using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Controls
{
	/// <summary>
	/// 数字输入控件。
	/// </summary>
	[ToolboxBitmap(typeof(NumericUpDown)), Description("数字控件")]
	public class SparkNumericUpDown : NumericUpDown, ISparkTheme<SparkNumberTheme>,IDataBinding
	{
		#region 字段

		/// <summary>
		/// 按钮背景色
		/// </summary>
		private Color _backColor = default;

		/// <summary>
		/// 按钮边框色
		/// </summary>
		private Color _borderColor = default;

		/// <summary>
		/// 按钮前景色
		/// </summary>
		private Color _foreColor = default;

		/// <summary>
		/// 控件当前状态
		/// </summary>
		private ControlState _controlState = ControlState.Default;

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
				this.PerformLayout();
				this.Invalidate();
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
		/// 获取或设置一个值，该值指示控件是否可以对用户交换做出回应。
		/// </summary>
		[Category("Spark"), Description("表示是否可以对用户交换做出回应。")]
		public new bool Enabled
		{
			set
			{
				base.Enabled = value;
				base.BackColor = this.Theme.DisabledBackColor;
			}
			get => base.Enabled;
		}

		/// <summary>
		/// 获取或设置一个值，该值指示控件是否启用数字键盘。
		/// </summary>
		[Category("Spark"), Description("表示是否启用数字键盘。")]
		public bool UseKeyBoard { set; get; }

		#endregion

		public SparkNumericUpDown()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
				ControlStyles.DoubleBuffer |                      // 双缓冲
				ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
				ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
				ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
				ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
			);

			this.Font = this.mFont;
			this.Theme = new SparkNumberTheme(this);
		}

		#region 私有方法

		/// <summary>
		/// 初始化控件样式
		/// </summary>
		private void InitControlStyle()
		{
			switch (this._controlState)
			{
				case ControlState.Focused:
					this._backColor = this.Theme.MouseDownBackColor;
					this._borderColor = this.Theme.MouseDownBorderColor;
					break;
				case ControlState.Highlight:
					this._backColor = this.Theme.MouseOverBackColor;
					this._borderColor = this.Theme.MouseOverBorderColor;
					break;
				default:
					this._backColor = this.Theme.BackColor;
					this._borderColor = this.Theme.BorderColor;
					this._foreColor = this.Theme.ForeColor;
					break;
			}
		}

		/// <summary>
		/// 重绘控件
		/// </summary>
		private void Draw(ref Message m)
		{
			if (this.BorderStyle == BorderStyle.None) return;
			IntPtr hDC = NativeMethods.GetWindowDC(m.HWnd);
			if (hDC.ToInt32() == 0) return;
			this.InitControlStyle();
			Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
			Pen p = new Pen(this._borderColor, 1);
			Graphics g = Graphics.FromHdc(hDC);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.DrawRectangle(p, rect);
			p.Dispose();
			m.Result = IntPtr.Zero;
			NativeMethods.ReleaseDC(m.HWnd, hDC);
		}

		/// <summary>
		/// 绘制颜色
		/// </summary>
		private void DrawColor()
		{
			base.ForeColor = this._foreColor;
			base.BackColor = this._backColor;
		}

		#endregion

		#region 重写基类

		protected override void OnMouseEnter(EventArgs e)
		{
			if (!this.Focused) this._controlState = ControlState.Highlight;
			base.OnMouseEnter(e);
			this.DrawColor();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (!this.Focused) this._controlState = ControlState.Default;
			base.OnMouseLeave(e);
			this.DrawColor();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			this._controlState = ControlState.Focused;
			base.OnGotFocus(e);
			this.DrawColor();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			this._controlState = ControlState.Default;
			base.OnLostFocus(e);
			this.DrawColor();
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			e.Handled = this.DecimalPlaces == 0 && e.KeyChar == '.';
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (!this.UseKeyBoard) return;
			SparkNumberKeyboard con = SparkNumberKeyboard.Show(this);
			if (this.DecimalPlaces == 0) con.CanUseDot = false;
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == 0xF || m.Msg == 0x133) this.Draw(ref m);
		}

		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkNumberTheme Theme { get; private set; }

		#endregion


		#region IDataBinding 接口成员

		/// <summary>
		/// 获取或设置控件绑定的字段名。
		/// </summary>
		[Category("Spark"), Description("控件绑定的字段名。")]
		[DefaultValue(null)]
		public virtual string FieldName { get; set; } = null;

		/// <summary>
		/// 获取或设置控件的值。
		/// </summary>
		[Browsable(false)]
		[DefaultValue("")]
		object IDataBinding.Value
		{
			get => this.Value;
			set => this.Value = decimal.TryParse(value?.ToString(), out decimal d) ? d : default;
		}

		#endregion
	}
}