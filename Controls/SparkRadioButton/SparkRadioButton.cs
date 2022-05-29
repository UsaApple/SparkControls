using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 单选框控件。
    /// </summary>
    [ToolboxBitmap(typeof(RadioButton))]
    public class SparkRadioButton : RadioButton, ISparkTheme<SparkRadioButtonTheme>, IDataBinding
    {
        #region 字段

        /// <summary>
        /// 内容边距
        /// </summary>
        private readonly int mMargin = 2;
        /// <summary>
        /// 外边框宽度
        /// </summary>
        private readonly int mOuterBorderWidth = 2;
        /// <summary>
        /// 内边框宽度
        /// </summary>
        private readonly int mInnerBorderWidth = 1;

        /// <summary>
        /// 控件状态
        /// </summary>
        private ControlState mControlState = ControlState.Default;

        /// <summary>
        /// 外圆半径
        /// </summary>
        private int mOuterRadius = 7;

        /// <summary>
        /// 内圆半径
        /// </summary>
        private int mInnerRadius = 4;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始 <see cref="SparkRadioButton"/> 类型的新实例。
        /// </summary>
        public SparkRadioButton() : base()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |                  // 调整大小时重绘
                          ControlStyles.DoubleBuffer |                  // 双缓冲
                          ControlStyles.OptimizedDoubleBuffer |         // 双缓冲
                          ControlStyles.AllPaintingInWmPaint |          // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                          ControlStyles.SupportsTransparentBackColor |  // 模拟透明度
                          ControlStyles.UserPaint, true                 // 控件绘制代替系统绘制
            );
            base.MinimumSize = new Size(22, 22);
            this.Theme = new SparkRadioButtonTheme(this);
        }

        #endregion

        #region 属性

        private Font mFont = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get { return this.mFont; }
            set
            {
                base.Font = this.mFont = value;
                this.PerformLayout();
                this.Invalidate();
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

        /// <summary>
        /// 获取或设置控件的外圆半径。
        /// </summary>
        [Category("Spark"), Description("外圆半径。")]
        [DefaultValue(7)]
        public int OuterRadius
        {
            get => this.mOuterRadius;
            set
            {
                this.mOuterRadius = value >= 3 ? value : 3;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置控件的内圆半径。
        /// </summary>
        [Category("Spark"), Description("内圆半径。")]
        [DefaultValue(4)]
        public int InnerRadius
        {
            get => this.mInnerRadius;
            set
            {
                this.mInnerRadius = value >= 1 ? value : 1;
                this.Invalidate();
            }
        }

        private SparkRadioButtonStyle mStyle = SparkRadioButtonStyle.Default;
        /// <summary>
        /// 获取或设置控件的视觉风格。
        /// </summary>
        [Category("Spark"), Description("控件的视觉风格。")]
        [TypeConverter(typeof(EnumConverter))]
        [DefaultValue(typeof(SparkRadioButtonStyle), "Default")]
        public SparkRadioButtonStyle Style
        {
            get => this.mStyle;
            set
            {
                this.mStyle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置选中状态对应的值。
        /// </summary>
        [Category("Spark"), DisplayName("选中时的值")]
        [Description("选中状态对应的值。")]
        [DefaultValue(null)]
        public virtual string CheckedValue { get; set; }

        /// <summary>
        /// 获取或设置未选中状态对应的值。
        /// </summary>
        [Category("Spark"), DisplayName("未选中的值")]
        [Description("未选中状态对应的值。")]
        [DefaultValue(null)]
        public virtual string UncheckedValue { get; set; }

        /// <summary>
        /// 是否可以取消选中
        /// </summary>
        [Category("Spark"), Description("是否可以取消选中")]
        [DefaultValue(false)]
        public bool CanCancelChecked { get; set; } = false;

        /// <summary>
        /// 获取或设置控件的最小尺寸。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size MinimumSize => new Size(22, 22);

        /// <summary>
        /// 获取或设置一个值，该值指示是否将控件的元素对齐以支持使用从右向左的字体的区域设置。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new RightToLeft RightToLeft => RightToLeft.No;

        /// <summary>
        /// 获取或设置控件上的文本对齐方式。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ContentAlignment TextAlign => ContentAlignment.MiddleLeft;

        #endregion

        #region 重写方法

        /// <summary>
        /// 引发控件的 MouseEnter 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            this.mControlState = (this.mControlState & ControlState.Focused) == ControlState.Focused
                ? ControlState.Focused | ControlState.Highlight
                : ControlState.Highlight;
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// 引发控件的 MouseLeave 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            this.mControlState = (this.mControlState & ControlState.Focused) == ControlState.Focused
                ? ControlState.Focused | ControlState.Default
                : ControlState.Default;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 引发控件的 MouseDown 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                this.mControlState = (this.mControlState & ControlState.Focused) == ControlState.Focused
                    ? ControlState.Focused | ControlState.Highlight
                    : ControlState.Highlight;
            }

            if (CanCancelChecked && base.Checked)
            {
                this.Checked = false;
                return;
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// 引发控件的 MouseUp 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                this.mControlState = (this.mControlState & ControlState.Focused) == ControlState.Focused
                    ? ControlState.Focused | ControlState.Default
                    : ControlState.Default;
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// 引发控件的 Paint 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            base.OnPaintBackground(e);
            switch (this.Style)
            {
                case SparkRadioButtonStyle.CheckBox:
                    this.DrawCheckBoxStyle(e.Graphics);
                    break;
                case SparkRadioButtonStyle.Button:
                    this.DrawButtonStyle(e.Graphics);
                    break;
                default:
                    this.DrawDefaultStyle(e.Graphics);
                    break;
            }
        }

        /// <summary>
        /// 引发控件的 GotFocus 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnGotFocus(EventArgs e)
        {
            this.mControlState = ControlState.Focused;
            base.OnGotFocus(e);
        }

        /// <summary>
        /// 引发控件的 LostFocus 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnLostFocus(EventArgs e)
        {
            this.mControlState = ControlState.Default;
            base.OnLostFocus(e);
        }

        #endregion

        #region 私有方法

        // 绘制默认风格的控件
        private void DrawDefaultStyle(Graphics g)
        {
            GDIHelper.InitializeGraphics(g);

            // 绘制圆圈
            Rectangle outerRect = new Rectangle(1, this.Height / 2 - this.mOuterRadius, this.mOuterRadius * 2, this.mOuterRadius * 2);
            Rectangle innerRect = new Rectangle(this.mOuterRadius - this.mInnerRadius + 1, this.Height / 2 - this.mInnerRadius, this.mInnerRadius * 2, this.mInnerRadius * 2);

            Color borderColor = (this.mControlState & ControlState.Focused) == ControlState.Focused ? this.Theme.FocusedBorderColor : this.Theme.BorderColor;

            GDIHelper.DrawEllipse(g, outerRect, borderColor, this.mOuterBorderWidth);
            GDIHelper.DrawEllipse(g, innerRect, borderColor, this.mInnerBorderWidth);
            switch (this.mControlState)
            {
                case ControlState.Highlight:
                case ControlState.Focused:
                    outerRect.Inflate(1, 1);
                    GDIHelper.DrawEllipse(g, outerRect, borderColor, 1);
                    outerRect.Inflate(-1, -1);
                    GDIHelper.DrawEllipse(g, outerRect, borderColor, 1);
                    break;
            }

            // 绘制文本
            Size textSize = GDIHelper.MeasureString(g, this.Text, this.Font).ToSize();
            Rectangle textRect = new Rectangle
            {
                X = outerRect.Right + this.mMargin,
                Y = (this.Height - textSize.Height) / 2,
                Height = textSize.Height
            };
            textRect.Width = this.Width - textRect.Left - 1;

            TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.Enabled ? this.Theme.ForeColor : this.Theme.DisabledBackColor, TextFormatFlags.Default);
            if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
            {
                // 绘制文本边框颜色
                GDIHelper.DrawRectangle(g, textRect, this.Theme.FocusedBorderColor);
            }

            // 绘制状态
            if (this.Checked)
            {
                GDIHelper.FillEllipse(g, innerRect, this.Theme.CentreForeColor);
            }
        }

        // 绘制复选框风格的控件
        private void DrawCheckBoxStyle(Graphics g)
        {
            GDIHelper.InitializeGraphics(g);

            // 绘制复选框
            Rectangle boxRect = new Rectangle(0, this.Height / 2 - Consts.CHECK_BOX_SIZE.Height / 2, Consts.CHECK_BOX_SIZE.Width, Consts.CHECK_BOX_SIZE.Height);
            RoundRectangle roundRect = new RoundRectangle(boxRect, CornerRadius.Empty);
            if ((this.mControlState & ControlState.Highlight) == ControlState.Highlight)
            {
                GDIHelper.FillRectangle(g, roundRect, this.Theme.MouseOverBackColor);
                if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
                {
                    GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
                }
                else
                {
                    GDIHelper.DrawPathBorder(g, roundRect, this.Theme.MouseOverBorderColor);
                }
            }
            else if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
            {
                GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
            }
            else
            {
                GDIHelper.DrawCheckBox(g, roundRect, this.Theme.BackColor, this.Theme.BorderColor);
            }

            // 绘制文本
            Size textSize = GDIHelper.MeasureString(g, this.Text, this.Font).ToSize();
            Rectangle textRect = new Rectangle
            {
                X = boxRect.Right + this.mMargin,
                Y = this.mMargin,
                Height = this.Height - this.mMargin * 2,
                Width = textSize.Width
            };
            GDIHelper.DrawImageAndString(g, textRect, null, Size.Empty, this.Text, this.Font, this.Theme.ForeColor);
            if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
            {
                // 绘制文本边框颜色
                GDIHelper.DrawRectangle(g, textRect, this.Theme.FocusedBorderColor);
            }

            // 绘制状态
            if (this.Checked)
            {
                GDIHelper.DrawCheckTick(g, boxRect, this.Theme.CentreForeColor);
            }
        }

        // 绘制按钮风格的控件
        private void DrawButtonStyle(Graphics g)
        {
            GDIHelper.InitializeGraphics(g);

            // 计算范围
            this.AutoSize = false;
            Size textSize = GDIHelper.MeasureString(g, this.Text, this.Font).ToSize();
            Rectangle textRect = new Rectangle
            {
                X = 0,
                Y = 0,
                Height = this.AutoSize ? textSize.Height : this.Height - 1,
                Width = this.AutoSize ? textSize.Width : this.Width - 1
            };

            // 绘制按钮
            RoundRectangle roundRect = new RoundRectangle(textRect, CornerRadius.Empty);
            if (this.Checked)
            {
                GDIHelper.FillRectangle(g, roundRect, this.Theme.SelectedBackColor);
                if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
                {
                    GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
                }
                else
                {
                    GDIHelper.DrawPathBorder(g, roundRect, this.Theme.SelectedBorderColor);
                }
            }
            else
            {
                GDIHelper.FillRectangle(g, roundRect, this.Theme.BackColor);
                if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
                {
                    GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
                }
                else
                {
                    GDIHelper.DrawPathBorder(g, roundRect, this.Theme.BorderColor);
                }
            }

            // 绘制文本
            GDIHelper.DrawImageAndString(g, textRect, null, Size.Empty, this.Text, this.Font, this.Theme.ForeColor);
        }

        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkRadioButtonTheme Theme { get; private set; }

        #endregion

        #region IDataBinding 接口成员

        /// <summary>
        /// 获取或设置控件绑定的字段名。
        /// </summary>
        [Category("Spark"), Description("控件绑定的字段名。")]
        [DefaultValue(null)]
        public virtual string FieldName { get; set; } = null;

        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        [Browsable(false)]
        [DefaultValue("False")]
        public virtual object Value
        {
            get
            {
                if (this.Checked)
                {
                    return string.IsNullOrEmpty(this.CheckedValue) ? true.ToString() : this.CheckedValue;
                }
                else
                {
                    return string.IsNullOrEmpty(this.UncheckedValue) ? false.ToString() : this.UncheckedValue;
                }
            }
            set
            {
                bool isEnum = false;
                //如果value是枚举值，枚举值的Name和int都可以和CheckedValue判断
                if (value != null && value is Enum)
                {
                    isEnum = true;
                }

                if (!string.IsNullOrEmpty(this.CheckedValue))
                {
                    if (isEnum)
                    {
                        this.Checked = this.CheckedValue == $"{value}" || this.CheckedValue == $"{Convert.ToInt32(value)}";
                    }
                    else
                    {
                        this.Checked = value != null && (value.Equals(this.CheckedValue) || value.ToString() == this.CheckedValue || value.ToString().ToBool());
                    }
                }
                else if (!string.IsNullOrEmpty(this.UncheckedValue))
                {
                    if (isEnum)
                    {
                        this.Checked = this.UncheckedValue != $"{value}" && this.CheckedValue != $"{Convert.ToInt32(value)}";
                    }
                    else
                    {
                        this.Checked = value != null && !value.Equals(this.UncheckedValue) && !value.ToString().Equals(this.UncheckedValue) && value.ToString().ToBool();
                    }
                }
                else
                {
                    this.Checked = value != null && value.ToString().ToBool();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 单选框样式枚举
    /// </summary>
    public enum SparkRadioButtonStyle
    {
        /// <summary>
        /// 默认样式
        /// </summary>
        Default,
        /// <summary>
        /// 复选框风格
        /// </summary>
        CheckBox,
        /// <summary>
        /// 按钮风格
        /// </summary>
        Button
    }
}