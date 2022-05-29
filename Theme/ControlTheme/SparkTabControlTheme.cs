using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
    /// <summary>
    /// 选项卡控件的主题属性
    /// </summary>
    public class SparkTabControlTheme : SparkEditTheme
    {
        private Color selectedBottomBorderColor = SparkThemeConsts.TabSelectedBottomBorderColor;

        private Color backColor = SparkThemeConsts.TabBackColor;
        private Color borderColor = SparkThemeConsts.TabBorderColor;
        private Color foreColor = SparkThemeConsts.TabForeColor;

        private Color selectedBackColor = SparkThemeConsts.TabSelectedBackColor;
        private Color selectedBorderColor = SparkThemeConsts.TabSelectedBorderColor;
        private Color selectedForeColor = SparkThemeConsts.TabSelectedForeColor;

        private Color mouseOverBackColor = SparkThemeConsts.TabMouseOverBackColor;
        private Color selectedSplitLineColor = SparkThemeConsts.TabSelectedSplitLineColor;
        private Color splitLineColor = SparkThemeConsts.TabSplitLineColor;

        /// <summary>
        /// 初始 <see cref="SparkTabControlTheme"/> 类型的新实例。
        /// </summary>
        /// <param name="control">应用主题的控件。</param>
        public SparkTabControlTheme(Control control) : base(control)
        {
            this.TitleTheme = new SparkTitleBarTheme(control);
            this.CloseTheme = new SparkTabControlCloseButtonTheme(control);
        }

        /// <summary>
        /// 禁用颜色
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color DisabledBackColor => base.DisabledBackColor;

        /// <summary>
        /// 选中后的底部边框颜色
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Color), SparkThemeConsts.TabSelectedBottomBorderColorString)]
        [Description("选中后的底部边框颜色")]
        public Color SelectedBottomBorderColor
        {
            get => this.selectedBottomBorderColor;
            set
            {
                if (this.selectedBottomBorderColor != value)
                {
                    Color oldValue = this.selectedBottomBorderColor;
                    this.selectedBottomBorderColor = value;
                    this.OnPropertyChanged(nameof(this.SelectedBottomBorderColor), oldValue, this.selectedBottomBorderColor);
                }
            }
        }

        /// <summary>
        /// 关闭按钮的主题样式
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("关闭按钮的主题"), Category("Spark")]
        public SparkTabControlCloseButtonTheme CloseTheme { get; private set; }

        /// <summary>
        /// 标题栏的主题样式
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("标题栏的主题")]
        public SparkTitleBarTheme TitleTheme { get; private set; }

        /// <summary>
        /// 背景色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabBackColorString)]
        [Description("背景色")]
        public override Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                base.BackColor = this.backColor = value;
            }
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabBorderColorString)]
        [Description("边框颜色")]
        public override Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                base.BorderColor = this.borderColor = value;
            }
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabForeColorString)]
        [Description("字体颜色")]
        public override Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
            set
            {
                base.ForeColor = this.foreColor = value;
            }
        }

        /// <summary>
        /// 选中背景色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabSelectedBackColorString)]
        [Description("选中背景色")]
        public override Color SelectedBackColor
        {
            get
            {
                return this.selectedBackColor;
            }
            set
            {
                base.SelectedBackColor = this.selectedBackColor = value;
            }
        }

        /// <summary>
        /// 选中边框色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabSelectedBorderColorString)]
        [Description("选中边框色")]
        public override Color SelectedBorderColor
        {
            get
            {
                return this.selectedBorderColor;
            }
            set
            {
                base.SelectedBorderColor = this.selectedBorderColor = value;
            }
        }

        /// <summary>
        /// 选中的字体颜色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabSelectedForeColorString)]
        [Description("选中的字体颜色")]
        public override Color SelectedForeColor
        {
            get
            {
                return this.selectedForeColor;
            }
            set
            {
                base.SelectedForeColor = this.selectedForeColor = value;
            }
        }

        /// <summary>
        /// 鼠标点击背景色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TabMouseOverBackColorString)]
        [Description("鼠标点击背景色")]
        public override Color MouseOverBackColor
        {
            get => this.mouseOverBackColor; set => base.MouseOverBackColor = this.mouseOverBackColor = value;
        }

        [DefaultValue(typeof(Color), SparkThemeConsts.TabSplitLineColorString)]
        [Description("分割线颜色")]
        public Color SplitLineColor
        {
            get => splitLineColor; set => splitLineColor = value;
        }

        [DefaultValue(typeof(Color), SparkThemeConsts.TabSelectedSplitLineColorString)]
        [Description("选中时的分割线颜色")]
        public Color SelectedSplitLineColor
        {
            get => selectedSplitLineColor; set => selectedSplitLineColor = value;
        }
    }
}