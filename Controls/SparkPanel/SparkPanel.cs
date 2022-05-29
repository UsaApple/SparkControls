using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// Panel容器控件
    /// </summary>
    [ToolboxBitmap(typeof(Panel))]
    [ToolboxItem(true)]
    public class SparkPanel : Panel, ISparkTheme<SparkPanelTheme>
    {
        #region 属性
        private Font font = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get => this.font;
            set
            {
                if (this.font != value)
                {
                    base.Font = this.font = value;
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
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Category("Spark"), Description("控件的前景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        [DefaultValue(BorderStyle.None)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            set
            {
                base.BorderStyle = value;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkPanel()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |                  //调整大小时重绘
                          ControlStyles.DoubleBuffer |                  //双缓冲
                          ControlStyles.OptimizedDoubleBuffer |         //双缓冲
                          ControlStyles.AllPaintingInWmPaint |          //禁止擦除背景
                          ControlStyles.SupportsTransparentBackColor    //透明
                         | ControlStyles.UserPaint, true
            );
            base.BorderStyle = BorderStyle.None;
            base.BackColor = SparkThemeConsts.BackColor;
            this.Font = this.font;
            this.Theme = new SparkPanelTheme(this);
        }
        #endregion

        #region 重写事件
        /// <summary>
        /// 引发 Paint 事件
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="PaintEventArgs"/>。</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.BorderStyle == BorderStyle.None) return;
            GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
        }
        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkPanelTheme Theme { get; private set; }

        #endregion
    }
}