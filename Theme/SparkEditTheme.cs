using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 表单控件主题类的基类
	/// </summary>
    [Serializable]
	public class SparkEditTheme : SparkThemeBase
    {
        private Color mouseOverBackColor = SparkThemeConsts.MouseOverBackColor;
        private Color mouseDownBackColor = SparkThemeConsts.MouseDownBackColor;
        private Color selectedBackColor = SparkThemeConsts.SelectedBackColor;

        private Color mouseOverBorderColor = SparkThemeConsts.MouseOverBorderColor;
        private Color mouseDownBorderColor = SparkThemeConsts.MouseDownBorderColor;
        private Color selectedBorderColor = SparkThemeConsts.SelectedBorderColor;

        private Color mouseOverForeColor = SparkThemeConsts.MouseOverForeColor;
        private Color mouseDownForeColor = SparkThemeConsts.MouseDownForeColor;
        private Color selectedForeColor = SparkThemeConsts.SelectedForeColor;

        private Color disabledBackColor = SparkThemeConsts.DisabledBackColor;

        /// <summary>
        /// 初始 <see cref="SparkEditTheme"/> 类型的新实例。
        /// </summary>
        internal SparkEditTheme() { }

        /// <summary>
        /// 初始 <see cref="SparkEditTheme"/> 类型的新实例。
        /// </summary>
        /// <param name="control">应用主题的控件。</param>
        public SparkEditTheme(Control control) : base(control)
        {
        }

        /// <summary>
        /// 获取或设置鼠标进入的背景色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.MouseOverBackColorString)]
        [Description("鼠标进入的背景色。")]
        public virtual Color MouseOverBackColor
        {
            get => mouseOverBackColor;
            set
            {
                if (mouseOverBackColor != value)
                {
                    var oldValue = mouseOverBackColor;
                    mouseOverBackColor = value;
                    OnPropertyChanged(nameof(MouseOverBackColor), oldValue, mouseOverBackColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置鼠标点击的背景色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.MouseDownBackColorString)]
        [Description("鼠标点击的背景色。")]
        public virtual Color MouseDownBackColor
        {
            get => mouseDownBackColor;
            set
            {
                if (mouseDownBackColor != value)
                {
                    var oldValue = mouseDownBackColor;
                    mouseDownBackColor = value;
                    OnPropertyChanged(nameof(MouseDownBackColor), oldValue, mouseDownBackColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置选中状态的背景色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.SelectedBackColorString)]
        [Description("选中状态的背景色。")]
        public virtual Color SelectedBackColor
        {
            get => selectedBackColor;
            set
            {
                if (selectedBackColor != value)
                {
                    var oldValue = selectedBackColor;
                    selectedBackColor = value;
                    OnPropertyChanged(nameof(SelectedBackColor), oldValue, selectedBackColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置鼠标进入的边框色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.MouseOverBorderColorString)]
        [Description("鼠标进入的边框色。")]
        public virtual Color MouseOverBorderColor
        {
            get => mouseOverBorderColor;
            set
            {
                if (mouseOverBorderColor != value)
                {
                    var oldValue = mouseOverBorderColor;
                    mouseOverBorderColor = value;
                    OnPropertyChanged(nameof(MouseOverBorderColor), oldValue, mouseOverBorderColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置鼠标点击的边框色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.MouseDownBorderColorString)]
        [Description("鼠标点击的边框色。")]
        public virtual Color MouseDownBorderColor
        {
            get => mouseDownBorderColor;
            set
            {
                if (mouseDownBorderColor != value)
                {
                    var oldValue = mouseDownBorderColor;
                    mouseDownBorderColor = value;
                    OnPropertyChanged(nameof(MouseDownBorderColor), oldValue, mouseDownBorderColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置选中状态的边框色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.SelectedBorderColorString)]
        [Description("选中状态的边框色。")]
        public virtual Color SelectedBorderColor
        {
            get => selectedBorderColor;
            set
            {
                if (selectedBorderColor != value)
                {
                    var oldValue = selectedBorderColor;
                    selectedBorderColor = value;
                    OnPropertyChanged(nameof(SelectedBorderColor), oldValue, selectedBorderColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置鼠标进入的前景色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.MouseOverForeColorString)]
        [Description("鼠标进入的前景色。")]
        public virtual Color MouseOverForeColor
        {
            get => mouseOverForeColor;
            set
            {
                if (mouseOverForeColor != value)
                {
                    var oldValue = mouseOverForeColor;
                    mouseOverForeColor = value;
                    OnPropertyChanged(nameof(MouseOverForeColor), oldValue, mouseOverForeColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置鼠标点击的前景色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.MouseDownForeColorString)]
        [Description("鼠标点击的前景色。")]
        public virtual Color MouseDownForeColor
        {
            get => mouseDownForeColor;
            set
            {
                if (mouseDownForeColor != value)
                {
                    var oldValue = mouseDownForeColor;
                    mouseDownForeColor = value;
                    OnPropertyChanged(nameof(MouseDownForeColor), oldValue, mouseDownForeColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置控件选中状态的的前景色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.SelectedForeColorString)]
        [Description("选中状态的的前景色。")]
        public virtual Color SelectedForeColor
        {
            get => selectedForeColor;
            set
            {
                if (selectedForeColor != value)
                {
                    var oldValue = selectedForeColor;
                    selectedForeColor = value;
                    OnPropertyChanged(nameof(SelectedForeColor), oldValue, selectedForeColor);
                }
            }
        }

        /// <summary>
        /// 获取或设置控件不可用状态的背颜色。
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.DisabledBackColorString)]
        [Description("不可用状态的背颜色。")]
        public virtual Color DisabledBackColor
        {
            get => disabledBackColor;
            set
            {
                if (disabledBackColor != value)
                {
                    var oldValue = disabledBackColor;
                    disabledBackColor = value;
                    OnPropertyChanged(nameof(DisabledBackColor), oldValue, disabledBackColor);
                }
            }
        }
    }
}