using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	///分割条控件的样式
	/// </summary>
	public class SparkSplitterTheme : SparkEditTheme
	{
		//暂时只需要鼠标进入的颜色，其他的都不要
		private Color mouseOverBackColor = SparkThemeConsts.SplitterMouseOverBackColor;
		private Color handlerBackColor = SparkThemeConsts.SplitterHandlerBackColor;

		/// <summary>
		/// 初始 <see cref="SparkSplitterTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkSplitterTheme(Control control) : base(control)
		{
		}

		/// <summary>
		/// 鼠标进入背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.SplitterMouseOverBackColorString)]
		[Description("鼠标进入背景色")]
		public override Color MouseOverBackColor
		{
			get => this.mouseOverBackColor;
			set => base.MouseOverBackColor = this.mouseOverBackColor = value;
		}

		/// <summary>
		/// 控制手柄背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.SplitterHandlerBackColorString)]
		[Description("控制手柄背景色")]
		public Color HandlerBackColor
		{
			get => this.handlerBackColor;
			set => this.handlerBackColor = value;
		}

		/// <summary>
		/// 鼠标点击背景色 禁用颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color MouseDownBackColor => base.MouseDownBackColor;

		/// <summary>
		/// 选中背景色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color SelectedBackColor => base.SelectedBackColor;

		/// <summary>
		/// 鼠标进入边框色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color MouseOverBorderColor => base.MouseOverBorderColor;

		/// <summary>
		/// 鼠标点击边框色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color MouseDownBorderColor => base.MouseDownBorderColor;

		/// <summary>
		/// 选中边框色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color SelectedBorderColor => base.SelectedBorderColor;

		/// <summary>
		/// 禁用颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color DisabledBackColor => base.DisabledBackColor;

		/// <summary>
		/// 鼠标进入字体颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color MouseOverForeColor => base.MouseOverForeColor;

		/// <summary>
		/// 鼠标点击的字体颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color MouseDownForeColor => base.MouseDownForeColor;

		/// <summary>
		/// 选中的字体颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color SelectedForeColor => base.SelectedForeColor;
	}
}