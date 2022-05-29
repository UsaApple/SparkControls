using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 分组框控件。
    /// </summary>
    [ToolboxBitmap(typeof(GroupBox))]
    public class SparkGroupBox : GroupBox, ISparkTheme<SparkGroupBoxTheme>
    {
        #region 构造函数

        /// <summary>
        /// 初始 <see cref="SparkGroupBox"/> 类型的新实例。
        /// </summary>
        public SparkGroupBox() : base()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |                      // 调整大小时重绘
                          ControlStyles.DoubleBuffer |                      // 双缓冲
                          ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                          ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                          ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
                          ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
            );
            this.Theme = new SparkGroupBoxTheme(this);
        }

        #endregion

        #region 属性

        private Font mCaptionFont = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get { return this.mCaptionFont; }
            set
            {
                base.Font = this.mCaptionFont = value;
                this.Invalidate(true);
                this.OnFontChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 获取或设置控件的背景色。
        /// </summary>
        [Category("Spark"), Description("控件的背景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Category("Spark"), Description("控件的前景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        private int mCornerRadius = 0;
        /// <summary>
        /// 获取或设置控件的圆角半径。
        /// </summary>
        [Category("Spark"), Description("控件的圆角半径。")]
        [DefaultValue(0)]
        public int CornerRadius
        {
            get { return this.mCornerRadius; }
            set
            {
                this.mCornerRadius = value > 0 ? value : 0;
                this.Invalidate();
            }
        }

        private int mTextMargin = 8;
        /// <summary>
        /// 获取或设置控件的标题边距。
        /// </summary>
        [Category("Spark"), Description("控件的标题边距。")]
        [DefaultValue(8)]
        public int TextMargin
        {
            get { return this.mTextMargin; }
            set
            {
                this.mTextMargin = value > this.mCornerRadius ? value : this.mCornerRadius;
                this.Invalidate();
            }
        }

        private int mBorderWidth = 1;
        /// <summary>
        /// 获取或设置控件的边框宽度。
        /// </summary>
        [Category("Spark"), Description("控件的边框宽度。")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return this.mBorderWidth; }
            set
            {
                this.mBorderWidth = value > 1 ? value : 1;
                this.Invalidate();
            }
        }

        private SparkGroupBoxBorderStyle mBorderStyle = SparkGroupBoxBorderStyle.Rectangle;
        /// <summary>
        /// 获取或设置控件的边框样式。
        /// </summary>
        [Category("Spark"), Description("控件的边框样式。")]
        [DefaultValue(typeof(SparkGroupBoxBorderStyle), "Rectangle")]
        public SparkGroupBoxBorderStyle BorderStyle
        {
            get { return this.mBorderStyle; }
            set
            {
                this.mBorderStyle = value;
                this.Invalidate();
            }
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 引发 Paint 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            base.OnPaintBackground(e);

            Graphics g = e.Graphics;
            GDIHelper.InitializeGraphics(g);

            RectangleF textRect = new RectangleF();
            SizeF textSize = GDIHelper.MeasureString(g, this.Text, this.Font);
            switch (this.mBorderStyle)
            {
                case SparkGroupBoxBorderStyle.Line:
                    textRect.X = 0;
                    textRect.Y = 0;
                    textRect.Width = Math.Min(textSize.Width + 1, this.Width - 20);
                    textRect.Height = textSize.Height * (int)Math.Ceiling((double)textSize.Width / textRect.Width);
                    break;
                case SparkGroupBoxBorderStyle.Rectangle:
                    textRect.X = this.ClientRectangle.X + this.TextMargin;
                    textRect.Y = 0;
                    textRect.Width = Math.Min(textSize.Width + this.TextMargin, this.Width - this.TextMargin - 20);
                    textRect.Height = textSize.Height * (int)Math.Ceiling((double)textSize.Width / textRect.Width);
                    break;
            }

            switch (this.mBorderStyle)
            {
                case SparkGroupBoxBorderStyle.Line:
                    this.DrawLineBorder(g, textRect, textSize.Height);
                    TextRenderer.DrawText(g, this.Text, this.Font, Rectangle.Round(textRect), this.Theme.ForeColor, TextFormatFlags.NoPadding | TextFormatFlags.WordBreak);
                    break;
                case SparkGroupBoxBorderStyle.Rectangle:
                    this.DrawRectBorder(g, textRect, textSize.Height);
                    TextRenderer.DrawText(g, this.Text, this.Font, Rectangle.Round(textRect), this.Theme.ForeColor, TextFormatFlags.NoPadding | TextFormatFlags.WordBreak);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 绘制矩形边框
        /// </summary>
        private void DrawRectBorder(Graphics g, RectangleF textRect, float textHeight)
        {
            RectangleF rect = new RectangleF
            {
                X = 0,
                Y = textHeight / 2,
                Width = this.Width - 1,
                Height = this.Height - textHeight / 2 - 1
            };
            RoundRectangle roundRect = new RoundRectangle(Rectangle.Round(rect), new CornerRadius(this.mCornerRadius));
            g.SetClip(textRect, CombineMode.Exclude);
            GDIHelper.DrawPathBorder(g, roundRect, this.Theme.BorderColor, this.BorderWidth);
            g.ResetClip();
        }

        /// <summary>
        /// 绘制线段边框
        /// </summary>
        private void DrawLineBorder(Graphics g, RectangleF textRect, float textHeight)
        {
            Color c1 = this.Theme.BorderColor;
            Color c2 = Color.FromArgb(20, c1);
            RectangleF rect = new RectangleF(
                textRect.Right,
                textHeight / 2,
                this.Width - textRect.Right,
                this.BorderWidth
            );
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, c1, c2, 180))
            {
                Blend blend = new Blend
                {
                    Positions = new float[] { 0f, .2f, 1f },
                    Factors = new float[] { 1f, .6f, 0.2f }
                };
                brush.Blend = blend;
                using (Pen pen = new Pen(brush, this.BorderWidth))
                {
                    g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                }
            }
        }

        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkGroupBoxTheme Theme { get; private set; }

        #endregion
    }

    /// <summary>
    /// 分组框控件边框样式枚举。
    /// </summary>
    public enum SparkGroupBoxBorderStyle
    {
        /// <summary>
        /// 紧跟标题的线段
        /// </summary>
        Line,
        /// <summary>
        /// 嵌入标题的矩形
        /// </summary>
        Rectangle
    }
}