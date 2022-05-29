using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// MenuWidget控件样式。
	/// </summary>
	public class SparkMenuWidgetTheme : SparkEditTheme
	{
		#region 变量
		private Color _panoramaBackColor = SparkThemeConsts.PanoramaBackColor;
		//组标题的颜色
		private Color _groupForeColor = SparkThemeConsts.TileGroupForeColor;

		#endregion

		#region 构造方法
		/// <summary>
		/// 初始 <see cref="SparkMenuWidgetTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkMenuWidgetTheme(Control control) : base(control)
		{
			this.BackColor = SparkThemeConsts.TileBackColor;
			this.MouseOverBackColor = SparkThemeConsts.TileMouseOverBackColor;
			this.MouseDownBackColor = SparkThemeConsts.TileMouseDownBackColor;
			this.MouseOverBorderColor = SparkThemeConsts.TileMouseOverBorderColor;
			this.ForeColor = SparkThemeConsts.TileForeColor;
			this.BorderColor = SparkThemeConsts.TileBordeColor;
		}
		#endregion

		#region 属性
		/// <summary>
		/// 全景控件背景色
		/// </summary>
		[Description("全景控件背景色"), DefaultValue(typeof(Color), SparkThemeConsts.PanoramaBackColorString)]
		public Color PanoramaBackColor
		{
			get => this._panoramaBackColor;
			set
			{
				if (this._panoramaBackColor != value)
				{
					Color oldValue = this._panoramaBackColor;
					this._panoramaBackColor = value;
					this.OnPropertyChanged(nameof(this.PanoramaBackColor), oldValue, value);
				}
			}
		}

		/// <summary>
		/// 组标题的颜色
		/// </summary>
		[Description("组标题的颜色"), DefaultValue(typeof(Color), SparkThemeConsts.TileGroupForeColorString)]
		public Color GroupForeColor
		{
			get => this._groupForeColor;
			set
			{
				if (this._groupForeColor != value)
				{
					Color oldValue = this._groupForeColor;
					this._groupForeColor = value;
					this.OnPropertyChanged(nameof(this.GroupForeColor), oldValue, value);
				}
			}
		}

		/// <summary>
		/// 磁贴背景颜色
		/// </summary>
		[Description("磁贴背景颜色"), DefaultValue(typeof(Color), SparkThemeConsts.TileBackColorString)]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		/// <summary>
		/// 磁贴鼠标悬浮背景色
		/// </summary>
		[Description("磁贴鼠标悬浮背景色"), DefaultValue(typeof(Color), SparkThemeConsts.TileMouseOverBackColorString)]
		public override Color MouseOverBackColor
		{
			get => base.MouseOverBackColor;
			set => base.MouseOverBackColor = value;
		}

		/// <summary>
		/// 磁贴鼠标按下背景色
		/// </summary>
		[Description("磁贴鼠标按下背景色"), DefaultValue(typeof(Color), SparkThemeConsts.TileMouseDownBackColorString)]
		public override Color MouseDownBackColor
		{
			get => base.MouseDownBackColor;
			set => base.MouseDownBackColor = value;
		}

		/// <summary>
		/// 磁贴悬浮边框颜色
		/// </summary>
		[Description("磁贴悬浮边框颜色"), DefaultValue(typeof(Color), SparkThemeConsts.TileMouseOverBorderColorString)]
		public override Color MouseOverBorderColor
		{
			get => base.MouseOverBorderColor;
			set => base.MouseOverBorderColor = value;
		}


		/// <summary>
		/// 前景色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TileForeColorString)]
		[Description("字体颜色")]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		/// <summary>
		/// 边框颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TileBorderColorString)]
		[Description("边框颜色")]
		public override Color BorderColor
		{
			get => base.BorderColor;
			set => base.BorderColor = value;
		}
		#endregion
	}
}