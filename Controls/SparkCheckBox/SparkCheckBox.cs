using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 复选框控件。
    /// </summary>
    [ToolboxBitmap(typeof(CheckBox))]
    public class SparkCheckBox : CheckBox, ISparkTheme<SparkCheckBoxTheme>, IDataBinding
    {
        #region 字段

        // 内容边距
        private readonly int mMargin = 2;

        // 控件状态
        private ControlState mControlState = ControlState.Default;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始类型 <see cref="SparkCheckBox"/> 的新实例。
        /// </summary>
        public SparkCheckBox() : base()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |                      // 调整大小时重绘
                          ControlStyles.DoubleBuffer |                      // 双缓冲
                          ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                          ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                          ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
                          ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
            );
            this.Theme = new SparkCheckBoxTheme(this);
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
            get => this.mFont;
            set
            {
                base.Font = this.mFont = value;
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

        private Size mBoxSize = Consts.CHECK_BOX_SIZE;
        /// <summary>
        /// 获取或设置勾选框的大小。
        /// </summary>
        [Category("Spark"), Description("勾选框的大小。")]
        [DefaultValue(typeof(Size), "14,14")]
        public Size BoxSize
        {
            get { return this.mBoxSize; }
            set
            {
                this.mBoxSize = value;
                if (this.Size.Width <= this.BoxSize.Width || this.Size.Height <= this.BoxSize.Height)
                {
                    this.Size = new Size(Math.Max(this.Width, this.BoxSize.Width + 1), Math.Max(this.Height, this.BoxSize.Height + 1));
                }
                this.Invalidate();
            }
        }

        private CornerRadius mCornerRadius = new CornerRadius(0);
        /// <summary>
        /// 获取或设置控件的圆角半径。
        /// </summary>
        [Category("Spark"), Description("控件的圆角半径。")]
        [DefaultValue(typeof(CornerRadius), "0")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(CornerRadiusConverter))]
        public CornerRadius CornerRadius
        {
            get { return this.mCornerRadius; }
            set
            {
                this.mCornerRadius = value;
                this.Invalidate();
            }
        }

        private SparkCheckBoxStyle mStyle = SparkCheckBoxStyle.Default;
        /// <summary>
        /// 获取或设置控件的风格。
        /// </summary>
        [Category("Spark"), Description("控件的显示风格。")]
        [TypeConverter(typeof(EnumConverter))]
        [DefaultValue(SparkCheckBoxStyle.Default)]
        public SparkCheckBoxStyle Style
        {
            get { return this.mStyle; }
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
        [DefaultValue("")]
        public virtual string CheckedValue { get; set; } = "";

        /// <summary>
        /// 获取或设置未选中状态对应的值。
        /// </summary>
        [Category("Spark"), DisplayName("未选中的值")]
        [Description("未选中状态对应的值。")]
        [DefaultValue("")]
        public virtual string UncheckedValue { get; set; } = "";

        /// <summary>
        /// 获取控件的最小尺寸。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size MinimumSize => new Size(20, 20);

        /// <summary>
        /// 获取一个值，该值指示是否将控件的元素对齐以支持使用从右向左的字体的区域设置。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new RightToLeft RightToLeft => RightToLeft.No;

        /// <summary>
        /// 获取控件上的文本对齐方式。
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
        /// 引发控件的 SizeChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Size.Width <= this.BoxSize.Width || this.Size.Height <= this.BoxSize.Height)
            {
                this.Size = new Size(Math.Max(this.Width, this.BoxSize.Width + 1), Math.Max(this.Height, this.BoxSize.Height + 1));
                base.OnSizeChanged(e);
            }
            else
            {
                base.OnSizeChanged(e);
            }
        }

        /// <summary>
        /// 引发控件的 Paint 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            base.OnPaintBackground(e);
            if (this.Style == SparkCheckBoxStyle.Default)
            {
                this.DrawDefaultStyle(e.Graphics);
            }
            else if (this.Style == SparkCheckBoxStyle.Button)
            {
                this.DrawButtonStyle(e.Graphics);
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

            // 绘制复选框
            Rectangle boxRect = new Rectangle(0, this.Height / 2 - this.mBoxSize.Height / 2, this.mBoxSize.Width, this.mBoxSize.Height);
            RoundRectangle roundRect = new RoundRectangle(boxRect, this.mCornerRadius);
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
                this.DrawCheckBox(g, roundRect);
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
            switch (this.CheckState)
            {
                case CheckState.Checked:
                    GDIHelper.DrawCheckTick(g, boxRect, this.Theme.TickColor);
                    break;
                case CheckState.Indeterminate:
                    Rectangle innerRect = boxRect;
                    innerRect.Inflate(-3, -3);
                    GDIHelper.FillRectangle(g, new RoundRectangle(innerRect, this.mCornerRadius), this.Theme.IndeterminateBackColor);
                    break;
            }
        }

        // 绘制按钮风格的控件
        private void DrawButtonStyle(Graphics g)
        {
            GDIHelper.InitializeGraphics(g);

            // 计算范围
            Size textSize = GDIHelper.MeasureString(g, this.Text, this.Font).ToSize();
            Rectangle textRect = new Rectangle
            {
                X = 0,
                Y = 0,
                Height = this.AutoSize ? textSize.Height : this.Height - 1,
                Width = this.AutoSize ? textSize.Width : this.Width - 1
            };

            // 绘制状态
            RoundRectangle roundRect = new RoundRectangle(textRect, this.mCornerRadius);
            switch (this.CheckState)
            {
                case CheckState.Unchecked:
                    GDIHelper.FillRectangle(g, roundRect, this.Theme.BackColor);
                    if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
                    {
                        GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
                    }
                    else
                    {
                        GDIHelper.DrawPathBorder(g, roundRect, this.Theme.BackColor);
                    }
                    break;
                case CheckState.Checked:
                    GDIHelper.FillRectangle(g, roundRect, this.Theme.SelectedBackColor);
                    if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
                    {
                        GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
                    }
                    else
                    {
                        GDIHelper.DrawPathBorder(g, roundRect, this.Theme.SelectedBorderColor);
                    }
                    break;
                case CheckState.Indeterminate:
                    Rectangle innerRect = textRect;
                    innerRect.Inflate(-3, -3);
                    GDIHelper.FillRectangle(g, roundRect, this.Theme.BackColor);
                    GDIHelper.FillRectangle(g, new RoundRectangle(innerRect, this.mCornerRadius), this.Theme.IndeterminateBackColor);
                    if ((this.mControlState & ControlState.Focused) == ControlState.Focused)
                    {
                        GDIHelper.DrawPathBorder(g, roundRect, this.Theme.FocusedBorderColor);
                    }
                    else
                    {
                        GDIHelper.DrawPathBorder(g, roundRect, this.Theme.BorderColor);
                    }
                    break;
            }

            // 绘制文本
            GDIHelper.DrawImageAndString(g, textRect, null, Size.Empty, this.Text, this.Font, this.Theme.ForeColor);
        }

        // 绘制 CheckBox 的边框和背景。
        private void DrawCheckBox(Graphics g, RoundRectangle roundRect)
        {
            int borderWidth = (this.mControlState & ControlState.Highlight) == ControlState.Highlight ? 2 : 1;
            GDIHelper.DrawCheckBox(g, roundRect, this.Theme.BackColor, this.Theme.BorderColor, borderWidth);
        }

        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkCheckBoxTheme Theme { get; private set; }

        #endregion

        #region IDataBinding 接口成员

        /// <summary>
        /// 获取或设置控件绑定的字段名。
        /// </summary>
        [Category("Spark"), Description("控件绑定的字段名。")]
        [DefaultValue(null)]
        public virtual string FieldName { get; set; } = null;

        /// <summary>
        /// 当CheckedValue或UncheckedValue为空时,是否不返回bool类型的Value值，false返回bool类型，true不返回bool类型的值。默认false
        /// </summary>
        [Category("Spark"), Description("当CheckedValue或UncheckedValue为空时,是否不返回bool类型的Value值，false返回bool类型，true不返回bool类型的值。默认false")]
        [DefaultValue(false)]
        public virtual bool IsNotReturnBoolValue { get; set; } = false;

        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        [Browsable(false)]
        [DefaultValue("")]
        public virtual object Value
        {
            get
            {
                if (IsNotReturnBoolValue)
                {
                    return this.Checked ? (this.CheckedValue ?? "") : (this.UncheckedValue ?? "");
                }
                else
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
                        this.Checked = this.CheckedValue != $"{value}" && this.CheckedValue != $"{Convert.ToInt32(value)}";
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
    /// 复选框样式枚举
    /// </summary>
    public enum SparkCheckBoxStyle
    {
        /// <summary>
        /// 默认样式
        /// </summary>
        Default,
        /// <summary>
        /// 按钮风格
        /// </summary>
        Button
    }
}