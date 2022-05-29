using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 导航条控件的主题属性
	/// </summary>
	public class SparkNavigationBarTheme : SparkEditTheme
	{
		private Color titleFontColor = SparkThemeConsts.NavigationBarTitleFontColor;
		private Color backColor = SparkThemeConsts.NavigationBarBackColor;
		private Color toolBackColor = SparkThemeConsts.NavigationBarToolBackColor;
		private Color toolSelectedColor = SparkThemeConsts.NavigationBarToolSelectedColor;

		//private Color selectedBottomBorderColor = DefaultThemeConsts.SelectedBottomBorderColor;
		//private Color borderColor = DefaultThemeConsts.TabBorderColor;
		//private Color foreColor = DefaultThemeConsts.TabForeColor;
		//private Color selectedBackColor = DefaultThemeConsts.TabSelectedBackColor;
		//private Color selectedBorderColor = DefaultThemeConsts.TabSelectedBorderColor;
		//private Color selectedForeColor = DefaultThemeConsts.TabSelectedForeColor;

		/// <summary>
		/// 初始 <see cref="SparkNavigationBarTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkNavigationBarTheme(Control control) : base(control)
		{

		}

		/// <summary>
		/// 面板的标题栏字体颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.NavigationBarTitleFontColorString)]
		[Description("面板的标题栏字体颜色")]
		public virtual Color TitleFontColor
		{
			get
			{
				return this.titleFontColor;
			}
			set
			{
				if (this.titleFontColor != value)
				{
					Color oldValue = this.titleFontColor;
					this.titleFontColor = value;
					this.OnPropertyChanged(nameof(this.TitleFontColor), oldValue, this.titleFontColor);
				}
			}
		}

		/// <summary>
		/// 面板的背景颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.NavigationBarToolBackColorString)]
		[Description("面板的背景颜色")]
		public virtual Color ToolBackColor
		{
			get => this.toolBackColor;
			set
			{
				if (this.toolBackColor != value)
				{
					Color oldValue = this.toolBackColor;
					this.toolBackColor = value;
					this.OnPropertyChanged(nameof(this.ToolBackColor), oldValue, this.toolBackColor);
				}
			}
		}

		/// <summary>
		/// 选项卡选中颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.NavigationBarToolSelectedColorString)]
		[Description("选项卡选中颜色")]
		public virtual Color ToolSelectedColor
		{
			get => this.toolSelectedColor;
			set
			{
				if (this.toolSelectedColor != value)
				{
					Color oldValue = this.toolSelectedColor;
					this.toolSelectedColor = value;
					this.OnPropertyChanged(nameof(this.ToolSelectedColor), oldValue, this.toolSelectedColor);
				}
			}
		}

		/// <summary>
		/// 禁用颜色
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color DisabledBackColor => base.DisabledBackColor;

		/// <summary>
		/// 背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.NavigationBarBackColorString)]
		[Description("背景色")]
		public override Color BackColor
		{
			get => this.backColor;
			set { base.BackColor = this.backColor = value; }
		}
	}
}