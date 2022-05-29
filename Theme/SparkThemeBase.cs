using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Theme
{
	/// <summary>
	/// 主题类的基类
	/// </summary>
	[Serializable]
	public class SparkThemeBase
	{
		[NonSerialized]
		private Control parent = null;

		private Color backColor = SparkThemeConsts.BackColor;
		private Color borderColor = SparkThemeConsts.BorderColor;
		private Color foreColor = SparkThemeConsts.ForeColor;

		#region 事件
		/// <summary>
		/// 属性修改通知事件
		/// </summary>
		public event SparkPropertyChangedEventHandler PropertyChanged;
		#endregion

		/// <summary>
		/// 初始 <see cref="SparkThemeBase"/> 类型的新实例。
		/// </summary>
		internal SparkThemeBase() { }

		/// <summary>
		/// 初始 <see cref="SparkThemeBase"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkThemeBase(Control control)
		{
			this.Parent = control;
		}

		/// <summary>
		/// 父对象
		/// </summary>
		[Browsable(false)]
		protected internal Control Parent
		{
			get => parent;
			private set => parent = value;
		}

		/// <summary>
		/// 获取或设置控件的背景色。
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
		[Description("控件的背景色。")]
		public virtual Color BackColor
		{
			get => backColor;
			set
			{
				if (backColor != value)
				{
					var oldValue = backColor;
					backColor = value;
					OnPropertyChanged(nameof(BackColor), oldValue, backColor);
				}
			}
		}

		/// <summary>
		/// 获取或设置控件的边框色。
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.BorderColorString)]
		[Description("控件的边框色。")]
		public virtual Color BorderColor
		{
			get => borderColor;
			set
			{
				if (borderColor != value)
				{
					var oldValue = borderColor;
					borderColor = value;
					OnPropertyChanged(nameof(BorderColor), oldValue, borderColor);
				}
			}
		}

		/// <summary>
		/// 获取或设置控件的前景色。
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
		[Description("控件的前景色。")]
		public virtual Color ForeColor
		{
			get => foreColor;
			set
			{
				if (foreColor != value)
				{
					var oldValue = foreColor;
					foreColor = value;
					OnPropertyChanged(nameof(ForeColor), oldValue, foreColor);
				}
			}
		}

		#region 方法
		/// <summary>
		/// 引发 SparkPropertyChanged 事件。
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="oldValue">原属性值</param>
		/// <param name="newValue">新属性值</param>
		protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
		{
			if (oldValue != newValue) return;
			if (this.Parent != null) this.Parent.Invalidate(true);
			PropertyChanged?.Invoke(this, new SparkPropertyChangedEventArgs(propertyName, oldValue, newValue));
		}
		#endregion
	}
}