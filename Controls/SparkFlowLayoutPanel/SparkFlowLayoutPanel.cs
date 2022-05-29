﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 流布局容器控件
	/// </summary>
	[ToolboxBitmap(typeof(FlowLayoutPanel))]
	[ToolboxItem(true)]
	public class SparkFlowLayoutPanel : FlowLayoutPanel, ISparkTheme<SparkFlowLayoutPanelTheme>
	{
		#region 私有变量
		#endregion

		#region 属性

		private Font mFont = Consts.DEFAULT_FONT;
		/// <summary>
		/// 获取或设置控件显示的文字的字体。
		/// </summary>
		[Description("获取或设置控件显示的文字的字体")]
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

		#endregion

		#region 构造函数
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkFlowLayoutPanel()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
						  ControlStyles.DoubleBuffer |                  //双缓冲
						  ControlStyles.OptimizedDoubleBuffer |         //双缓冲
						  ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
						  ControlStyles.SupportsTransparentBackColor |  //透明
						  ControlStyles.UserPaint, true);
			this.Font = this.mFont;
			this.Theme = new SparkFlowLayoutPanelTheme(this);
		}
		#endregion

		#region 重写事件
		/// <summary>
		/// 绘制事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			//e.Graphics.FillRectangle(new SolidBrush(Theme.BackColor), new Rectangle(1, 1, this.Width - 2, this.Height - 2));
			if (this.BorderStyle != BorderStyle.None)
			{
				GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
			}
			//ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Theme.BorderColor, ButtonBorderStyle.Solid);
		}
		#endregion

		#region ISparkTheme 接口成员

		/// <summary>
		/// 获取控件的主题。
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Category("Spark"), Description("控件的主题。")]
		public SparkFlowLayoutPanelTheme Theme { get; private set; }

		#endregion
	}
}