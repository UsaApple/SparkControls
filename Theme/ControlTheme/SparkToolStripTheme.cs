using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Controls;

namespace SparkControls.Theme
{
	/// <summary>
	/// MenuStrip控件主题类。
	/// </summary>
	public class SparkToolStripTheme : SparkEditTheme
	{
		#region 变量
		private Color _backColor1 = ColorTranslator.FromHtml("224,224,224");
		private Color _backColor2 = ColorTranslator.FromHtml("224,224,224");
		private Color _imageMarginBackColor = SparkThemeConsts.ToolStripImageMarginBackColor;
		#endregion

		#region 构造方法
		/// <summary>
		/// 初始 <see cref="SparkToolStripTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkToolStripTheme(Control control) : base(control)
		{
			this.CheckBoxTheme = new SparkCombineCheckBoxTheme(control);
			this.BackColor = SparkThemeConsts.ToolStripBackColor;
			this.MouseDownBackColor = SparkThemeConsts.ToolStripMouseDownBackColor;
			this.MouseOverBackColor = SparkThemeConsts.ToolStripMouseOverBackColor;
			this.SelectedBackColor = SparkThemeConsts.ToolStripSelectedBackColor;
		}
		#endregion

		#region 属性
		/// <summary>
		/// CheckBox的主题
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("CheckBox的主题"), Category("Spark")]
		public SparkCombineCheckBoxTheme CheckBoxTheme { get; private set; }

		/// <summary>
		/// 线性渐变背景色1
		/// </summary>
		[DefaultValue(typeof(Color), "224,224,224")]
		[Description("线性渐变背景色1,对应主项目的背景")]
		public Color BackColor1
		{
			get => this._backColor1;
			set
			{
				if (this._backColor1 != value)
				{
					Color oldValue = this._backColor1;
					this._backColor1 = value;
					this.OnPropertyChanged(nameof(this.BackColor1), oldValue, value);
				}
			}
		}

		/// <summary>
		/// 线性渐变背景色2
		/// </summary>
		[DefaultValue(typeof(Color), "224,224,224")]
		[Description("线性渐变背景色2,对应主项目的背景")]
		public Color BackColor2
		{
			get => this._backColor2;
			set
			{
				if (this._backColor2 != value)
				{
					Color oldValue = this._backColor2;
					this._backColor2 = value;
					this.OnPropertyChanged(nameof(this.BackColor2), oldValue, value);
				}
			}
		}

		/// <summary>
		/// 项目图标栏的背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ToolStripImageMarginBackColorString)]
		[Description("项目图标栏的背景色")]
		public Color ImageMarginBackColor
		{
			get => this._imageMarginBackColor;
			set
			{
				if (this._imageMarginBackColor != value)
				{
					Color oldValue = this._imageMarginBackColor;
					this._imageMarginBackColor = value;
					this.OnPropertyChanged(nameof(this.ImageMarginBackColor), oldValue, value);
				}
			}
		}

		/// <summary>
		/// 弹出菜单的背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ToolStripBackColorString)]
		[Description("弹出菜单的背景色")]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 鼠标点击的背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ToolStripMouseDownBackColorString)]
		[Description("鼠标点击的背景色")]
		public override Color MouseDownBackColor
		{
			get => base.MouseDownBackColor;
			set => base.MouseDownBackColor = value;
		}

		/// <summary>
		/// 鼠标进入背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ToolStripMouseOverBackColorString)]
		[Description("鼠标进入背景色")]
		public override Color MouseOverBackColor
		{
			get => base.MouseOverBackColor;
			set => base.MouseOverBackColor = value;
		}

		/// <summary>
		/// 选中背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ToolStripSelectedBackColorString)]
		[Description("选中背景色")]
		public override Color SelectedBackColor
		{
			get => base.SelectedBackColor;
			set => base.SelectedBackColor = value;
		}

		/// <summary>
		/// 菜单高亮色(鼠标over和选择色的渐变色)
		/// </summary>
		internal GradientColor HighlightColor
		{
			get => new GradientColor(this.MouseOverBackColor, this.SelectedBackColor,
				new float[] { 0.0F, 0.7F, 1.5F }, new float[] { 0.0F, 0.6F, 1F });
		}
		#endregion
	}
}