using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	[ToolboxBitmap(typeof(UserControl)), Description("数字键盘")]
	public partial class SparkNumberKeyboard : UserControl, ISparkTheme<SparkKeyboardTheme>
	{
		#region 字段
		
		/// <summary>
		/// 标签集合
		/// </summary>
		private List<Label> _labels = new List<Label>();

		/// <summary>
		/// 边框集合
		/// </summary>
		private List<Control> _borders = new List<Control>();

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
		/// 获取或设置一个值，该值指示是否使用自定义的事件来接收按键，当为 true 时将不再向系统发送按键请求。
		/// </summary>
		[Category("Spark"), Description("是否使用自定义的事件来接收按键，当为 true 时将不再向系统发送按键请求")]
		public bool UseCustomEvent { get; set; } = false;

		/// <summary>
		/// 获取或设置一个值，是否可以使用小数点。
		/// </summary>
		[Category("Spark"), Description("是否可以使用小数点")]
		public bool CanUseDot
		{
			set
			{
				this.label13.Enabled = value;
				this.label13.BackColor = value ? this.Theme.BackColor : this.Theme.DisabledBackColor;
			}
			get => this.label13.Enabled;
		}

		#endregion

		#region 自定义事件

		/// <summary>
		/// 数字点击事件
		/// </summary>
		[Description("点击数字事件"), Category("Spark")]
		public event EventHandler NumClick = null;

		/// <summary>
		/// 删除点击事件
		/// </summary>
		[Description("点击删除事件"), Category("Spark")]
		public event EventHandler BackspaceClick = null;

		/// <summary>
		/// 回车点击事件
		/// </summary>
		[Description("点击回车事件"), Category("Spark")]
		public event EventHandler EnterClick = null;

		#endregion

		public SparkNumberKeyboard()
		{
			this.InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
				ControlStyles.DoubleBuffer |                      // 双缓冲
				ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
				ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
				ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
				ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
			);

			this.Font = this.mFont;
			this.Theme = new SparkKeyboardTheme(this);
			this.Init();
		}

		#region 方法

		private void Init()
		{
			this._labels = new List<Label>() {
				this.label1, this.label2, this.label3, this.label4, this.label5, this.label6,
				this.label7, this.label8, this.label9, this.label10, this.label11, this.label12, this.label13, this.label14
			};

			this._borders = new List<Control>() {
				this.ucSplitLine_H1, this.ucSplitLine_H2, this.ucSplitLine_H3, this.ucSplitLine_H4,
				this.ucSplitLine_H5,this.ucSplitLine_H6,this.ucSplitLine_H7,this.ucSplitLine_H8,this.ucSplitLine_H9,this.ucSplitLine_H10,
				this.ucSplitLine_H11,this.ucSplitLine_H12,this.ucSplitLine_V1,this.ucSplitLine_V2,this.ucSplitLine_V3,this.ucSplitLine_V4,
				this.ucSplitLine_V5,this.ucSplitLine_V6,this.ucSplitLine_V7,this.ucSplitLine_V8,this.ucSplitLine_V9,this.ucSplitLine_V10,
				this.ucSplitLine_V11,this.ucSplitLine_V12,this.ucSplitLine_V13,this.ucSplitLine_V14
			};
			foreach (Label con in this._labels)
			{
				con.BackColor = this.Theme.BackColor;
				con.Font = this.mFont;
			}
			foreach (Control con in this._borders) con.BackColor = this.Theme.BorderColor;
		}

		public static SparkNumberKeyboard Show(Control parentControl)
		{
			SparkPopup frmAnchor = null;
			SparkNumberKeyboard keyNum = new SparkNumberKeyboard();
			keyNum.EnterClick += (a, b) => frmAnchor.Hide();
			parentControl.LostFocus += (s, e) => frmAnchor.Hide();
			parentControl.KeyDown += (s, e) =>
			{
				if (e.KeyCode != Keys.Escape) return;
				frmAnchor.Hide();
			};
			frmAnchor = new SparkPopup(parentControl, keyNum);
			frmAnchor.Show(parentControl.FindForm());
			return keyNum;
		}

		#endregion

		#region 事件处理程序

		private void Num_MouseDown(object sender, MouseEventArgs e)
		{
			Label label = sender as Label;
			label.BackColor = this.Theme.MouseDownBackColor;
			NumClick?.Invoke(sender, e);
			if (this.UseCustomEvent) return;
			SendKeys.Send(label.Tag.ToString());
		}

		private void Backspace_MouseDown(object sender, MouseEventArgs e)
		{
			Label label = sender as Label;
			label.BackColor = this.Theme.MouseDownBackColor;
			BackspaceClick?.Invoke(sender, e);
			if (this.UseCustomEvent) return;
			SendKeys.Send("{BACKSPACE}");
		}

		private void Enter_MouseDown(object sender, MouseEventArgs e)
		{
			Label label = sender as Label;
			label.BackColor = this.Theme.MouseDownBackColor;
			EnterClick?.Invoke(sender, e);
			if (this.UseCustomEvent) return;
			SendKeys.Send("{ENTER}");
		}

		private void Label_MouseEnter(object sender, EventArgs e)
		{
			Label label = sender as Label;
			label.BackColor = this.Theme.MouseOverBackColor;
		}

		private void Label_MouseLeave(object sender, EventArgs e)
		{
			Label label = sender as Label;
			label.BackColor = this.Theme.BackColor;
		}

		private void Label_MouseUp(object sender, MouseEventArgs e)
		{
			Label label = sender as Label;
			label.BackColor = this.Theme.BackColor;
		}

		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkKeyboardTheme Theme { get; private set; }

		#endregion
	}
}