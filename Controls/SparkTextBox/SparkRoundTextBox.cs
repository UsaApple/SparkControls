using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 圆角文本框
    /// </summary>
    [ToolboxBitmap(typeof(TextBox)), Description("圆角文本框")]
    public partial class SparkRoundTextBox : UserControl, IDataBinding
    {
        #region 常量
        private const int IMG_AND_TXT_GAP = 3;
        #endregion

        #region 变量
        /// <summary>
        /// 文本框主体
        /// </summary>
        private SparkTextBox _textBoxMain;

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backColor = default;

        /// <summary>
        /// 边框色
        /// </summary>
        private Color _borderColor = default;

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;
        #endregion

        #region 属性
        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SparkTextBoxTheme Theme => _textBoxMain.Theme;

        private Font _font = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get => this._textBoxMain.Font;
            set
            {
                this._textBoxMain.Font = value;
                base.Font = this._font = value;
                this.AdjustCtrlSize();
                this.PerformLayout();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置控件的背景色。
        /// </summary>
        [Category("Spark"), Description("控件的背景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
        public override Color BackColor
        {
            get => this._textBoxMain.BackColor;
            set => this._textBoxMain.BackColor = value;
        }

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Category("Spark"), Description("控件的前景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
        public override Color ForeColor
        {
            get => this._textBoxMain.ForeColor;
            set => this._textBoxMain.ForeColor = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否将回车键的动作转为 Tab 键。
        /// </summary>
        [Category("Spark"), Description("是否将回车键的动作转为 Tab 键。")]
        [DefaultValue(true)]
        public bool IsEnterToTab
        {
            get => this._textBoxMain.IsEnterToTab;
            set => this._textBoxMain.IsEnterToTab = value;
        }

        /// <summary>
        /// 获取或设置描述输入预期值的提示信息。
        /// </summary>
        [Category("Spark"), Description("描述输入预期值的提示信息。")]
        public string Placeholder
        {
            get => this._textBoxMain.Placeholder;
            set => this._textBoxMain.Placeholder = value;
        }

        /// <summary>
        /// 获取或设置与此控件关联的文本。
        /// </summary>
        [Category("Spark"), Description("与此控件关联的文本。")]
        public override string Text
        {
            get => this._textBoxMain.Text;
            set => this._textBoxMain.Text = value;
        }

        private int _cornerRadius = 10;
        /// <summary>
        /// 获取或设置控件的圆角的半径。
        /// </summary>
        [Category("Spark"), Description("控件的圆角的半径。")]
        [DefaultValue(typeof(int), "10")]
        public int CornerRadius
        {
            get => this._cornerRadius;
            set => this._cornerRadius = value < 0 || value > 50 ? this.Height / 2 : value;
        }

        private Padding _padding = new Padding(6, 4, 6, 4);
        /// <summary>
        /// 获取或设置控件内的空白。
        /// </summary>
        [Category("Spark"), Description("控件内的空白。")]
        [DefaultValue(typeof(Padding), "6, 4, 6, 4")]
        public new Padding Padding
        {
            get => base.Padding;
            set
            {
                base.Padding = this._padding = value;
                if (this._image != null)
                {
                    base.Padding = this.AdjustPadding();
                }
                this.AdjustCtrlSize();
            }
        }

        private Image _image = null;
        /// <summary>
        /// 左侧图片
        /// </summary>
        [Category("Spark"), Description("文本左侧图片")]
        [DefaultValue(null)]
        public Image Image
        {
            get
            {
                return this._image;
            }
            set
            {
                this._image = value;
                if (this._image != null)
                {
                    base.Padding = this.AdjustPadding();
                }
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 文本改变处理事件
        /// </summary>
        public new event EventHandler TextChanged;

        /// <summary>
        /// 键盘按下处理事件
        /// </summary>
        public new event KeyPressEventHandler KeyPress;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkRoundTextBox()
        {
            this.InitTextBox();
            this.InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw |                 // 调整大小时重绘
                ControlStyles.DoubleBuffer |                      // 双缓冲
                ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                ControlStyles.SupportsTransparentBackColor, true  // 模拟透明度
            );

            this.Font = this._font;
            this.Padding = this._padding;
            this.BorderStyle = BorderStyle.None;
            base.BackColor = Color.Transparent;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 重绘处理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            this.InitControlStyle();

            GDIHelper.InitializeGraphics(e.Graphics);
            Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
            GDIHelper.FillRectangle(e.Graphics, rect, Color.Transparent);

            //设置圆角矩形
            RoundRectangle roundRect = new RoundRectangle(rect, new CornerRadius(this.CornerRadius));
            GDIHelper.FillPath(e.Graphics, roundRect, this._backColor);
            GDIHelper.DrawPathBorder(e.Graphics, roundRect, this._borderColor);

            //绘制图标
            if (this.Image != null)
            {
                Image drawImg = this.Image;
                if (this.Image.Width > this._textBoxMain.Height || this.Image.Height > this._textBoxMain.Height)
                {
                    drawImg = this.Image.GetThumbnailImage(this._textBoxMain.Height,
                        this._textBoxMain.Height, null, IntPtr.Zero);
                }
                Rectangle imgRect = new Rectangle(this.CornerRadius / 2 + IMG_AND_TXT_GAP,
                    (this.Height - drawImg.Height) / 2, drawImg.Width, drawImg.Height);
                e.Graphics.DrawImage(drawImg, imgRect);
            }

            base.OnPaint(e);
        }

        /// <summary>
        ///  执行设置该控件的指定边界的工作
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="specified"></param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.Padding.Vertical > 0)
            {
                height = this._textBoxMain.Height + this.Padding.Vertical;
            }
            else
            {
                height = this._textBoxMain.Height;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化TextBox
        /// </summary>
        private void InitTextBox()
        {
            this._textBoxMain = new SparkTextBox
            {
                BorderStyle = BorderStyle.None,
                Placeholder = "",
                Dock = DockStyle.Fill,
                ImeMode = ImeMode.NoControl,
                Location = new Point(0, 0),
                MatchPattern = null,
                Name = "textBoxMain",
                ValidateType = ValidateType.None,
                UseNumberKeyboard = false,
                Size = new Size(201, 23),
                TextType = TextType.String
            };
            this.Controls.Add(this._textBoxMain);

            this._textBoxMain.MouseEnter += this.TextBoxMain_MouseEnter;
            this._textBoxMain.MouseLeave += this.TextBoxMain_MouseLeave;
            this._textBoxMain.GotFocus += this.TextBoxMain_GotFocus;
            this._textBoxMain.LostFocus += this.TextBoxMain_LostFocus;
            this._textBoxMain.TextChanged += this.SparkTextBoxMain_TextChanged;
            this._textBoxMain.KeyPress += this.SparkTextBoxMain_KeyPress;
        }

        /// <summary>
        /// 文本改变处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SparkTextBoxMain_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// 键盘按下处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SparkTextBoxMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(sender, e);
        }

        /// <summary>
        /// 文本框失去焦点处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMain_LostFocus(object sender, EventArgs e)
        {
            this._controlState = ControlState.Default;
            this.Invalidate();
        }

        /// <summary>
        /// 文本框获得焦点处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMain_GotFocus(object sender, EventArgs e)
        {
            this._controlState = ControlState.Focused;
            this.Invalidate();
        }

        /// <summary>
        /// 文本框鼠标离开处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMain_MouseLeave(object sender, EventArgs e)
        {
            if (!this._textBoxMain.Focused)
            {
                this._controlState = ControlState.Default;
            }
            this.Invalidate();
        }

        /// <summary>
        /// 文本框鼠标进入处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMain_MouseEnter(object sender, EventArgs e)
        {
            if (!this._textBoxMain.Focused)
            {
                this._controlState = ControlState.Highlight;
            }
            this.Invalidate();
        }

        /// <summary>
        /// 调整控件大小
        /// </summary>
        private void AdjustCtrlSize()
        {
            if (this.Padding.Horizontal > 0)
            {
                this.Width = this._textBoxMain.Width + this.Padding.Horizontal;
            }
            else
            {
                this.Width = this._textBoxMain.Width;
            }

            if (this.Padding.Vertical > 0)
            {
                this.Height = this._textBoxMain.Height + this.Padding.Vertical;
            }
            else
            {
                this.Height = this._textBoxMain.Height;
            }
        }

        /// <summary>
        /// 调整内部边距
        /// </summary>
        private Padding AdjustPadding()
        {
            int offsetLeft = this.Image.Width;
            if (this.Image.Width > this._textBoxMain.Height)
            {
                offsetLeft = this._textBoxMain.Height;
            }
            return new Padding(this._padding.Left + offsetLeft + IMG_AND_TXT_GAP * 2,
                this._padding.Top, this._padding.Right, this._padding.Bottom);
        }

        /// <summary>
        /// 初始化样式
        /// </summary>
        private void InitControlStyle()
        {
            switch (this._controlState)
            {
                case ControlState.Focused:
                    this._backColor = Theme.MouseDownBackColor;
                    this._borderColor = Theme.MouseDownBorderColor;
                    break;
                case ControlState.Highlight:
                    this._backColor = Theme.MouseOverBackColor;
                    this._borderColor = Theme.MouseOverBorderColor;
                    break;
                default:
                    this._backColor = Theme.BackColor;
                    this._borderColor = Theme.BorderColor;
                    break;
            }

            if (!this.Enabled || this._textBoxMain.ReadOnly == true)
            {
                this._backColor = Theme.DisabledBackColor;
            }
        }
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
        [DefaultValue("")]
        public virtual object Value
        {
            get => this.Text;
            set => this.Text = value?.ToString();
        }

        #endregion
    }
}