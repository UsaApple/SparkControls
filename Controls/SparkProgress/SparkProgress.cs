using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	[ToolboxBitmap(typeof(ProgressBar)), Description("进度条")]
	public class SparkProgress : ProgressBar, ISparkTheme<SparkProgressTheme>
	{
		#region 字段

		/// <summary>
		/// 是否显示百分比
		/// </summary>
		private bool _showPercentage = false;

		/// <summary>
		/// 按钮背景色
		/// </summary>
		private Color _backColor = default(Color);

		/// <summary>
		/// 按钮边框色
		/// </summary>
		private Color _borderColor = default(Color);

		/// <summary>
		/// 按钮前景色
		/// </summary>
		private Color _foreColor = default(Color);

		/// <summary>
		/// 控件当前状态
		/// </summary>
		private ControlState _controlState = ControlState.Default;

		#endregion

		public SparkProgress()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
				ControlStyles.DoubleBuffer |                      // 双缓冲
				ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
				ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
				ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
				ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
			);
			this.Font = this.mFont;
			this.Theme = new SparkProgressTheme(this);
		}

		#region 属性

		private Font mFont = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文本的字体。
		/// </summary>
		[Category("Spark"), Description("控件显示的文本的字体。")]
		[DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
		public override Font Font
		{
			get { return this.mFont; }
			set
			{
				base.Font = this.mFont = value;
				this.PerformLayout();
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
		/// 获取或设置一个值，该值指示是否显示百分比。
		/// </summary>
		[Category("Spark"), Description("是否显示百分比。")]
		[DefaultValue(false)]
		public bool ShowPercentage
		{
			get { return this._showPercentage; }
			set { this._showPercentage = value; }
		}

		#endregion

		#region 重写

		protected override void OnMouseEnter(EventArgs e)
		{
			this._controlState = ControlState.Highlight;
			base.OnMouseEnter(e);
			if (this.Value == 0) this.Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this._controlState = ControlState.Default;
			base.OnMouseLeave(e);
			if (this.Value == 0) this.Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			this._controlState = ControlState.Focused;
			base.OnMouseDown(mevent);
			if (this.Value == 0) this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			this._controlState = ControlState.Default;
			base.OnMouseUp(mevent);
			if (this.Value == 0) this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			GDIHelper.InitializeGraphics(pe.Graphics);
			this.Draw(pe.Graphics, pe.ClipRectangle);
		}

		#endregion

		#region 内部方法

		private void InitControlStyle()
		{
			switch (this._controlState)
			{
				case ControlState.Focused:
					this._backColor = this.Theme.SelectedBackColor;
					this._borderColor = this.Theme.BorderColor;
					break;
				case ControlState.Highlight:
					this._backColor = this.Theme.SelectedBackColor;
					this._borderColor = this.Theme.BorderColor;
					break;
				default:
					this._backColor = this.Theme.SelectedBackColor;
					this._borderColor = this.Theme.BorderColor;
					this._foreColor = this.Theme.ForeColor;
					break;
			}
			if (!this.Enabled) this._backColor = this.Theme.DisabledBackColor;
		}

		/// <summary>
		/// 重绘按钮
		/// </summary>
		/// <param name="g">上下文</param>
		/// <param name="rect">按钮区域大小</param>
		private void Draw(Graphics g, Rectangle rect)
		{
			this.InitControlStyle();

			ControlPaint.DrawBorder(g, this.ClientRectangle, this._borderColor, ButtonBorderStyle.Solid);
			int total = this.Maximum - this.Minimum;
			int current = this.Value - this.Minimum;
			int percentage = this.GetPercentage();
			current = Math.Min(total, Math.Max(current, 0));
			Rectangle currentRect = new Rectangle()
			{
				X = 1,
				Y = 1,
				Width = (int)(percentage / (double)100 * (this.Width - 2)),
				Height = this.Height - 2
			};
			using (SolidBrush brush = new SolidBrush(this._backColor))
			{
				g.FillRectangle(brush, currentRect);
			}
			if (!this.ShowPercentage) return;
			GDIHelper.DrawImageAndString(g, rect, null, default(Size), $"{percentage}%", this.Font, this._foreColor);
		}

		/// <summary>
		/// 获取百分比
		/// </summary>
		private int GetPercentage()
		{
			int total = this.Maximum - this.Minimum;
			int current = this.Value - this.Minimum;
			current = Math.Min(total, Math.Max(0, current));
			return (int)Math.Ceiling((double)current / total * 100);
		}

		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkProgressTheme Theme { get; private set; }

		#endregion
	}
}