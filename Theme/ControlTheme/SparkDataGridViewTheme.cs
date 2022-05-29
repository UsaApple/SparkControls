using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// DataGridView样式类
	/// </summary>
	public sealed class SparkDataGridViewTheme : SparkEditTheme
	{

		private Color headerBackColor = SparkThemeConsts.DataGridViewHeaderBackColor;
		private Color headerForeColor = SparkThemeConsts.DataGridViewHeaderForeColor;
		private Color alternatingBackColor = SparkThemeConsts.DataGridViewAlternationBackColor;

		/// <summary>
		/// 初始 <see cref="SparkDataGridViewTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkDataGridViewTheme(Control control) : base(control)
		{
			this.CheckBoxTheme = new SparkCombineCheckBoxTheme(control);
		}

		/// <summary>
		/// 表头背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.DataGridViewHeaderBackColorString)]
		[Description("表头背景色")]
		public Color HeaderBackColor
		{
			get => this.headerBackColor;
			set
			{
				if (this.headerBackColor != value)
				{
					Color oldValue = this.headerBackColor;
					this.headerBackColor = value;
					this.OnPropertyChanged(nameof(this.HeaderBackColor), oldValue, this.headerBackColor);
				}
			}
		}

		/// <summary>
		/// 表头前景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.DataGridViewHeaderForeColorString)]
		[Description("表头前景色")]
		public Color HeaderForeColor
		{
			get => this.headerForeColor;
			set
			{
				if (this.headerForeColor != value)
				{
					Color oldValue = this.headerForeColor;
					this.headerForeColor = value;
					this.OnPropertyChanged(nameof(this.HeaderForeColor), oldValue, this.headerForeColor);
				}
			}
		}

		/// <summary>
		/// 交替背景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.DataGridViewAlternationBackColorString)]
		[Description("交替背景色")]
		public Color AlternatingBackColor
		{
			get => this.alternatingBackColor;
			set
			{
				if (this.alternatingBackColor != value)
				{
					Color oldValue = this.alternatingBackColor;
					this.alternatingBackColor = value;
					this.OnPropertyChanged(nameof(this.HeaderForeColor), oldValue, this.alternatingBackColor);
				}
			}
		}

		/// <summary>
		/// 组合多选框
		/// </summary>
		[DefaultValue(typeof(SparkCombineCheckBoxTheme))]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("组合多选框")]
		public SparkCombineCheckBoxTheme CheckBoxTheme { private set; get; }
	}
}
