using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// DateTimePicker控件主题类。
	/// </summary>
	public class SparkDateTimePickerTheme : SparkEditTheme
	{
		private Color _mouseOverBorderColor = SparkThemeConsts.DateTimePickerMouseOverBorderColor;
		private Color _mouseDownBorderColor = SparkThemeConsts.DateTimePickerMouseDownBorderColor;
		private Color _dateTimeSelectColor = SparkThemeConsts.DateTimePickerSelectColor;

		/// <summary>
		/// 初始 <see cref="SparkDateTimePickerTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkDateTimePickerTheme(Control control) : base(control)
		{
		}

		/// <summary>
		/// 背景色
		/// </summary>
		[Browsable(false)]
		[DefaultValue(typeof(Color), "White")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get => Color.White; }

		/// <summary>
		/// 鼠标进入边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.DateTimePickerMouseOverBorderColorString)]
		[Description("鼠标进入边框色")]
		public override Color MouseOverBorderColor
		{
			get => this._mouseOverBorderColor;
			set
			{
				if (this._mouseOverBorderColor != value)
				{
					Color oldValue = this._mouseOverBorderColor;
					this._mouseOverBorderColor = value;
					this.OnPropertyChanged(nameof(this.MouseOverBorderColor), oldValue, this._mouseOverBorderColor);
				}
			}
		}
		/// <summary>
		/// 鼠标点击边框色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.DateTimePickerMouseDownBorderColorString)]
		[Description("鼠标点击边框色")]
		public override Color MouseDownBorderColor
		{
			get => this._mouseDownBorderColor;
			set
			{
				if (this._mouseDownBorderColor != value)
				{
					Color oldValue = this._mouseDownBorderColor;
					this._mouseDownBorderColor = value;
					this.OnPropertyChanged(nameof(this.MouseDownBorderColor), oldValue, this._mouseDownBorderColor);
				}
			}
		}

		/// <summary>
		/// 时间或日期的选中颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.DateTimePickerSelectColorString)]
		[Description("时间或日期的选中颜色")]
		public Color DateTimeSelectColor
		{
			get => this._dateTimeSelectColor;
			set
			{
				if (this._dateTimeSelectColor != value)
				{
					Color oldValue = this._dateTimeSelectColor;
					this._dateTimeSelectColor = value;
					this.OnPropertyChanged(nameof(this.DateTimeSelectColor), oldValue, value);
				}
			}
		}
	}
}