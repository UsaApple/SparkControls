using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 组合框控件。
    /// </summary>
    [ToolboxBitmap(typeof(ComboBox))]
    [ToolboxItem(false)]
    public class SparkComboBoxBase : ListControl, ISparkTheme<SparkComboBoxTheme>, IDualDataBinding
    {
        #region 字段

        /// <summary>
        /// 控件当前状态
        /// </summary>
        internal ControlState _controlState = ControlState.Default;
        /// <summary>
        /// 下拉列表
        /// </summary>
        protected readonly SparkComboBoxListBox mListBox;
        /// <summary>
        /// 文本框
        /// </summary>
        protected readonly TextBox mTextBox;

        private readonly int mMargin = 4;
        private bool mResize = false;

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backColor = default;
        /// <summary>
        /// 边框色
        /// </summary>
        private Color _borderColor = default;

        // 内容矩形
        private Rectangle rectContent = new Rectangle(0, 0, 1, 1);
        // 按钮矩形
        private Rectangle rectBtn = new Rectangle(0, 0, 1, 1);

        //标记是否是清空选项，是的话，相应的事件不触发
        private readonly bool _isClearItemFlag = false; //暂时不启用此功能，如果启用此功能去掉readonly
        #endregion

        #region 事件

        /// <summary>
        /// 显示下拉部分时发生。
        /// </summary>
        [Category("行为"), Description("显示下拉部分时发生。")]
        public event EventHandler DropDown;

        /// <summary>
        /// 绘制列表项时发生。
        /// </summary>
        [Category("行为"), Description("绘制列表项时发生。")]
        public event DrawItemEventHandler DrawItem;

        /// <summary>
        /// 确定列表项的大小时发生。
        /// </summary>
        [Category("行为"), Description("确定列表项的大小时发生。")]
        public event MeasureItemEventHandler MeasureItem;

        /// <summary>
        /// 当前选择项索引改变时发生。
        /// </summary>
        [Category("行为"), Description("当前选择项索引改变时发生。")]
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// 当前选择项改变时发生。
        /// </summary>
        [Category("行为"), Description("当前选择项改变时发生。")]
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// 当前选择项改变前发生。
        /// </summary>
        [Category("行为"), Description("当前选择项改变前发生。")]
        public event CancelEventHandler SelectingItemChanged;

        /// <summary>
        /// 当Items改变时发生。
        /// </summary>
        [Category("行为"), Description("当Items改变时发生。")]
        internal event ItemsChangedEventHandler ItemsChanged;

        /// <summary>
        /// 在下拉部分不再可见时发生
        /// </summary>
        [Category("行为"), Description("在下拉部分不再可见时发生。")]
        public event EventHandler DropDownClosed;

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
                this.mResize = true;
                this.mListBox.Font = this.mTextBox.Font = this.mFont = base.Font = value;
                this.AdjustControls();
                this.Invalidate(true);
                this.OnFontChanged(EventArgs.Empty);
                this.mListBox.ItemHeight = this.mTextBox.Height + this.mMargin * 2;
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
        /// 获取或设置控件的边框样式。
        /// </summary>
        [Category("Spark"), Description("控件的边框样式。")]
        [DefaultValue(BorderStyle.FixedSingle)]
        public BorderStyle BorderStyle { get; set; } = BorderStyle.FixedSingle;

        private CornerRadius mCornerRadius = new CornerRadius(0);
        /// <summary>
        /// 获取或设置控件的圆角半径。
        /// </summary>
        [Category("Spark"), Description("控件的圆角半径。")]
        [DefaultValue(typeof(CornerRadius), "0")]
        [TypeConverter(typeof(CornerRadiusConverter))]
        [Localizable(true)]
        public CornerRadius CornerRadius
        {
            get => this.mCornerRadius;
            set
            {
                if (mCornerRadius != value)
                {
                    this.mCornerRadius = value;
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="SparkComboBox"/> 的下拉部分的高度(以像素为单位)。
        /// </summary>
        [Category("Spark"), Description("下拉部分的高度(以像素为单位)。")]
        [DefaultValue(200)]
        public int DropDownHeight { get; set; } = 200;

        /// <summary>
        /// 获取或设置 <see cref="SparkComboBox"/> 的下拉部分的宽度(以像素为单位)。
        /// </summary>
        [Category("Spark"), Description("下拉部分的宽度(以像素为单位)。")]
        [DefaultValue(150)]
        public int DropDownWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置要在 <see cref="SparkComboBox"/> 的下拉部分中显示的最大项数。
        /// </summary>
        [Category("Spark"), Description("下拉部分中显示的最大项数。")]
        [DefaultValue(8)]
        public int MaxDropDownItems { get; set; } = 8;

        /// <summary>
        /// 获取或设置 <see cref="SparkComboBox"/> 的数据源。
        /// </summary>
        [Category("Spark"), Description("下拉列表的数据源。")]
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        public new virtual object DataSource
        {
            get => base.DataSource;
            set
            {
                IList objs;
                if (value is IEnumerable enumerable)
                {
                    objs = enumerable.Cast<object>().ToList();
                }
                else if (value is IList list)
                {
                    objs = list;
                }
                else if (value is IListSource listSource)
                {
                    objs = listSource.GetList();
                }
                else
                {
                    objs = null;
                    //throw new Exception("复杂的 DataBinding 接受 IEnumerable、IList 或 IListSource 作为数据源。");
                }

                base.DataSource = objs;
                this.Items.Clear();
                if (objs != null)
                {
                    foreach (object obj in objs)
                    {
                        this.Items.Add(obj);
                    }
                }
                this.SelectedIndex = -1;
                this.OnDataSourceChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 获取一个对象，该对象表示此 <see cref="SparkComboBox"/> 所含项的集合。
        /// </summary>
        [Category("Spark"), Description("列表项的集合。")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ObjectCollection Items => (ObjectCollection)this.mListBox.Items;

        private bool mIntegralHeight = false;
        /// <summary>
        /// 获取或设置一个值，该值指示控件是否应调整大小以避免只显示项的局部。
        /// </summary>
        [Category("Spark"), Description("控件是否应调整大小以避免只显示项的局部。")]
        [DefaultValue(false)]
        public bool IntegralHeight
        {
            get => this.mIntegralHeight;
            set => this.mListBox.IntegralHeight = this.mIntegralHeight = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示列表项是否按字母顺序排序。
        /// </summary>
        [Category("Spark"), Description("列表项是否按字母顺序排序。")]
        [DefaultValue(false)]
        public bool Sorted
        {
            get => this.mListBox.Sorted;
            set => this.mListBox.Sorted = value;
        }

        private ImageList mImageList = null;
        /// <summary>
        /// 获取或设置要显示的图片集合。
        /// </summary>
        [Category("Spark"), Description("要显示的图片集合。")]
        [DefaultValue(null)]
        public ImageList ImageList
        {
            get => this.mImageList;
            set
            {
                this.mImageList = value;
                if (this.mListBox != null) { this.mListBox.ImageList = this.mImageList; }
            }
        }

        private string mImageMember = null;
        /// <summary>
        /// 获取或设置用于标识图标的属性。
        /// </summary>
        [Category("Spark"), Description("设置用于标识图标的属性。")]
        [DefaultValue(null)]
        public string ImageMember
        {
            get => this.mImageMember;
            set
            {
                this.mImageMember = value;
                if (this.mListBox != null) { this.mListBox.ImageMember = this.mImageMember; }
            }
        }

        /// <summary>
        /// 获取或设置属性的路径，它将用作 ComboBox 中的项的实际值。
        /// </summary>
        [Category("Spark"), Description("指示用作控件中项的实际值的属性。")]
        [DefaultValue("Id")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public new string ValueMember
        {
            get => base.ValueMember;
            set => this.mListBox.ValueMember = base.ValueMember = value;
        }

        /// <summary>
        /// 获取或设置属性的路径，它将用作 ComboBox 中的项的显示值。
        /// </summary>
        [Category("Spark"), Description("指示用作控件中项的显示值的属性。")]
        [DefaultValue("Name")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public new string DisplayMember
        {
            get => base.DisplayMember;
            set => this.mListBox.DisplayMember = base.DisplayMember = value;
        }

        /// <summary>
        /// 获取或设置控件的绘制模式。
        /// </summary>
        [Category("Spark"), Description("控件的绘制模式。")]
        [DefaultValue(DrawMode.OwnerDrawFixed)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public DrawMode DrawMode
        {
            get { return this.mListBox.DrawMode; }
            set { this.mListBox.DrawMode = value; }
        }

        private ComboBoxStyle mDropDownStyle = ComboBoxStyle.DropDownList;
        /// <summary>
        /// 获取或设置组合框的样式。
        /// </summary>
        [Category("Spark"), Description("组合框的样式。")]
        [DefaultValue(ComboBoxStyle.DropDownList)]
        public virtual ComboBoxStyle DropDownStyle
        {
            get => this.mDropDownStyle;
            set
            {
                this.mDropDownStyle = value;
                this.mTextBox.Visible = this.mDropDownStyle != ComboBoxStyle.DropDownList;
                this.AdjustControls();
                if (this.mDropDownStyle == ComboBoxStyle.Simple)
                {
                    this.mListBox.Width = this.Width;
                    this.mListBox.Top = this.mTextBox.Height + this.mMargin * 2;
                    this.Controls.Add(this.mListBox);
                }
                else if (this.Controls.Contains(this.mListBox))
                {
                    // 移除列表框
                    this.Controls.Remove(this.mListBox);
                }

                this.Invalidate(true);
            }
        }

        /// <summary>
        /// 获取或设置控件关联的文本。
        /// </summary>
        [Category("Spark"), Description("控件关联的文本。")]
        [DefaultValue("")]
        public override string Text
        {
            get => this.mTextBox.Text;
            set
            {
                this.SelectedText = value;
                if (this.SelectedItem == null && this.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    if (this.mTextBox.Text != value)
                    {
                        this.mTextBox.Text = base.Text = value;
                        this.Invalidate(true);
                        if (this._isClearItemFlag == false) this.OnTextChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    if (this._isClearItemFlag == false) this.OnTextChanged(EventArgs.Empty);
                }
            }
        }

        protected int mSelectedIndex = -1;
        /// <summary>
        /// 获取或设置当前选定项从零开始的索引。
        /// </summary>
        [Category("Spark"), Description("当前选定项从零开始的索引。")]
        [DefaultValue(-1)]
        public override int SelectedIndex
        {
            get => this.mSelectedIndex;
            set
            {
                if (value < 0)
                {
                    this.mTextBox.Text = "";
                    this.mSelectedIndex = -1;
                    this.Invalidate(true);
                }
                else if (this.mListBox.Items.Count > 0 && value >= -1 && value < this.mListBox.Items.Count)
                {
                    CancelEventArgs cancel = new CancelEventArgs(false);
                    this.OnSelectingItemChanged(cancel);
                    if (cancel.Cancel)
                    {
                        return;
                    }
                    this.mListBox.SelectedIndex = this.mSelectedIndex = value;
                    if (this.mListBox.SelectedItem != null)
                    {
                        this.mTextBox.Text = this.mListBox.GetItemText(this.mListBox.SelectedItem);
                        this.mTextBox.SelectAll();
                    }
                    this.Invalidate(true);
                    if (this._isClearItemFlag == false)
                    {
                        this.OnSelectedIndexChanged(EventArgs.Empty);
                        this.OnSelectedItemChanged(EventArgs.Empty);
                        this.OnSelectedValueChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    this.mSelectedIndex = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置由 <see cref="SparkComboBox"/>.ValueMember 属性指定的成员属性的值。
        /// </summary>
        [Category("Spark"), Description("当前选定项的值。")]
        [DefaultValue(null)]
        public new virtual object SelectedValue
        {
            get
            {
                if (this.SelectedItem != null)
                {
                    if (!this.ValueMember.IsNullOrEmpty())
                    {
                        return SparkComboBoxDatasourceResolver.GetMemberValue(this.SelectedItem, this.ValueMember);
                    }
                    else
                    {
                        return this.SelectedItem.ToString();
                    }
                }
                return null;
            }
            set
            {
                object item = null;
                if (!this.ValueMember.IsNullOrEmpty())
                {
                    item = SparkComboBoxDatasourceResolver.GetItemByMemeberValue(this.Items, this.ValueMember, value);
                }
                else
                {
                    item = this.Items.Cast<object>().FirstOrDefault(i => i.Equals(value));
                }
                this.SelectedItem = item;
            }
        }

        /// <summary>
        /// 获取或设置由 <see cref="SparkComboBox"/> DisplayMember 属性指定的成员属性的值。
        /// </summary>
        [Category("Spark"), Description("当前选定项的文本。")]
        [DefaultValue("")]
        public virtual string SelectedText
        {
            get => this.Text;
            set
            {
                object item = SparkComboBoxDatasourceResolver.GetItemByMemeberValue(this.Items, this.DisplayMember, value);
                if (item != null)
                {
                    this.SelectedItem = item;
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="SparkComboBox"/> 的当前选定项。
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public virtual object SelectedItem
        {
            get
            {
                if (this.SelectedIndex >= 0 && this.SelectedIndex < this.Items.Count)
                {
                    return this.Items[this.SelectedIndex];
                }
                return null;
            }
            set
            {
                if (this.SelectedItem != value)
                {
                    CancelEventArgs cancel = new CancelEventArgs(false);
                    this.OnSelectingItemChanged(cancel);
                    if (cancel.Cancel)
                    {
                        return;
                    }
                    this.mListBox.SelectedItem = value;
                    this.SelectedIndex = this.mListBox.SelectedIndex;
                }
            }
        }

        private bool mIsDroppedDown = false;
        /// <summary>
        /// 获取或设置一个值，该值指示下拉部分是否显示。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool IsDroppedDown
        {
            get => this.mIsDroppedDown;
            set
            {
                if (this.Enabled == false || this.Visible == false)
                {
                    return;
                }
                this.mIsDroppedDown = value;
                if (value)
                {
                    this.ShowDropDown();
                }
                else
                {
                    this.HideDropDown();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件的弹出组件。
        /// </summary>
        internal virtual Popup PopupDropDown { get; set; }

        /// <summary>
        /// 获取控件的内容区域。
        /// </summary>
        internal Rectangle ContentRect
        {
            get
            {
                return this.rectContent;
            }
        }

        /// <summary>
        /// 获取控件的按钮区域。
        /// </summary>
        internal Rectangle ButtonRect
        {
            get
            {
                return this.rectBtn;
            }
        }

        /// <summary>
        /// 鼠标滚轮滑动是否改变下拉框的选项值，true改变，false不改变（默认）
        /// </summary>
        [DefaultValue(false)]
        public bool IsChangeValueByMouseWheel { get; set; } = false;
        #endregion

        #region 构造函数

        /// <summary>
        /// 初始 <see cref="SparkComboBox"/> 类型的新实例。
        /// </summary>
        public SparkComboBoxBase()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |      // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                ControlStyles.ContainerControl |                    // 容器控件
                ControlStyles.DoubleBuffer |                        // 双缓冲
                ControlStyles.OptimizedDoubleBuffer |               // 双缓冲
                ControlStyles.ResizeRedraw |                        // 调整大小时重绘
                ControlStyles.Selectable |                          // 可以接收焦点
                ControlStyles.SupportsTransparentBackColor |        // 模拟透明度
                ControlStyles.UserMouse |                           // 控件代替系统处理鼠标事件
                ControlStyles.UserPaint, true                       // 控件绘制代替系统绘制
            );

            this.DropDownWidth = this.Width = 150;

            // 创建文本框
            this.mTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = this.Font,
                Location = new Point(this.mMargin, this.mMargin),
                Margin = new Padding(0),
                Padding = new Padding(0),
                TabIndex = 0,
                TextAlign = HorizontalAlignment.Left,
                WordWrap = false,
                CausesValidation = true,
            };
            this.mTextBox.Validating += this.TextBox_Validating;
            this.Controls.Add(this.mTextBox);
            this.AdjustControls();

            // 创建列表框
            this.mListBox = new SparkComboBoxListBox(this)
            {
                Font = this.Font,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = IntegralHeight,
                SelectionMode = SelectionMode.One,
                TabStop = false,
                ItemHeight = 25,
                ImageList = this.ImageList,
                ImageMember = this.ImageMember
            };
            this.PopupDropDown = new Popup(this.mListBox)
            {
                AutoSize = false,
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoClose = false,
                Resizable = false,
            };

            // 绑定事件 - ListBox
            this.mListBox.DrawItem += (sender, e) => this.OnDrawItem(e);
            this.mListBox.MeasureItem += (sender, e) => this.OnMeasureItem(e);
            this.mListBox.MouseClick += this.ListBox_MouseClick;
            this.mListBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    this.ListBox_MouseClick(null, null);
                }
            };

            // 绑定事件 - Popup
            this.PopupDropDown.Closed += this.PopupControl_Closed;

            // 绑定事件 - TextBox
            this.mTextBox.KeyDown += (sender, e) => this.OnKeyDown(e);
            this.mTextBox.KeyPress += (sender, e) => this.OnKeyPress(e);
            this.mTextBox.KeyUp += (sender, e) => this.OnKeyUp(e);
            this.mTextBox.PreviewKeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter) this.OnKeyDown(new KeyEventArgs(e.KeyCode));
                this.OnPreviewKeyDown(e);
            };
            this.mTextBox.TextChanged += (sender, e) => this.OnTextChanged(e);

            this.Theme = new SparkComboBoxTheme(this);
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 引发 ControlAdded 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            e.Control.GotFocus += (a, b) => this.OnGotFocus(b);
            e.Control.LostFocus += (a, b) => this.OnLostFocus(b);
            e.Control.MouseDown += (a, b) => this.OnMouseDown(b);
            e.Control.MouseEnter += (a, b) => this.OnMouseEnter(b);
            e.Control.MouseLeave += (a, b) => this.OnMouseLeave(b);

            base.OnControlAdded(e);
        }

        /// <summary>
        /// 引发 DisplayMemberChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnDisplayMemberChanged(EventArgs e)
        {
            if (this.IsHandleCreated == false) return;
            this.mListBox.DisplayMember = this.DisplayMember;
            if (this.IsHandleCreated)
            {
                this.SelectedIndex = this.SelectedIndex;
            }
            base.OnDisplayMemberChanged(e);
        }

        /// <summary>
        /// 引发 ValueMemberChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnValueMemberChanged(EventArgs e)
        {
            if (this.IsHandleCreated == false) return;
            this.mListBox.ValueMember = this.ValueMember;
            if (this.IsHandleCreated)
            {
                this.SelectedIndex = this.SelectedIndex;
            }
            base.OnValueMemberChanged(e);
        }

        /// <summary>
        /// 引发 EnabledChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            this.Invalidate(true);
            base.OnEnabledChanged(e);
        }

        /// <summary>
        /// 引发 ForeColorChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            this.mTextBox.ForeColor = this.ForeColor;
            base.OnForeColorChanged(e);
        }

        /// <summary>
        /// 引发 FormatInfoChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnFormatInfoChanged(EventArgs e)
        {
            this.mListBox.FormatInfo = this.FormatInfo;
            base.OnFormatInfoChanged(e);
        }

        /// <summary>
        /// 引发 FormatStringChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnFormatStringChanged(EventArgs e)
        {
            this.mListBox.FormatString = this.FormatString;
            base.OnFormatStringChanged(e);
        }

        /// <summary>
        /// 引发 FormattingEnabledChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnFormattingEnabledChanged(EventArgs e)
        {
            this.mListBox.FormattingEnabled = this.FormattingEnabled;
            base.OnFormattingEnabledChanged(e);
        }

        /// <summary>
        /// 引发 GotFocus 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnGotFocus(EventArgs e)
        {
            this._controlState = ControlState.Focused;
            if (!this.mTextBox.Focused)
            {
                this.mTextBox.Focus();
            }
            this.Invalidate(true);
            base.OnGotFocus(e);
        }

        /// <summary>
        /// 引发 LostFocus 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (this.mTextBox.Focused == false && this.Focused == false && this.PopupDropDown.Visible == false)
            {
                this._controlState = ControlState.Default;
            }
            if (!this.ContainsFocus)
            {
                this.Invalidate(true);
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// 引发 MouseEnter 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.mTextBox.Focused && !this.Focused && this.PopupDropDown.Visible == false)
            {
                this._controlState = ControlState.Highlight;
            }
            this.Invalidate(true);
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// 引发 MouseLeave 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.mTextBox.Focused && !this.Focused && this.PopupDropDown.Visible == false)
            {
                this._controlState = ControlState.Default;
            }
            if (!this.RectangleToScreen(this.ClientRectangle).Contains(MousePosition))
            {
                this.Invalidate(true);
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 引发 MouseDown 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //if (this.DropDownStyle != ComboBoxStyle.DropDownList)
            //{
            //    mTextBox.LostFocus();
            //}

            if (this.DropDownStyle == ComboBoxStyle.DropDown && this.RectangleToScreen(this.rectBtn).Contains(MousePosition) ||
                this.DropDownStyle == ComboBoxStyle.DropDownList && this.RectangleToScreen(this.rectContent).Contains(MousePosition))
            {
                if (this.PopupDropDown.Visible)
                {
                    this.IsDroppedDown = false;
                }
                else
                {
                    this.IsDroppedDown = true;
                }
                this.Invalidate(true);
            }
        }

        /// <summary>
        /// 引发 MouseUp 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Invalidate(true);
        }

        /// <summary>
        /// 引发 MouseWheel 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (IsChangeValueByMouseWheel == false) return;
            if (e.Delta < 0 && this.SelectedIndex < this.Items.Count - 1)
                this.SelectedIndex += 1;
            else if (e.Delta > 0 && this.SelectedIndex > 0)
                this.SelectedIndex -= 1;

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// 引发 Paint 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            GDIHelper.InitializeGraphics(g);
            this.InitControlStyle();
            // 控件外边框
            Rectangle rectOuter = this.rectContent;
            rectOuter.Width -= 1;
            rectOuter.Height -= 1;

            // 控件内边框
            Rectangle rectInner = this.rectContent;
            rectInner.X += 1;
            rectInner.Y += 1;
            rectInner.Width -= 2;
            rectInner.Height -= 2;

            // 填充内容区域
            GDIHelper.FillPath(g, new RoundRectangle(this.rectContent, this.CornerRadius), this._backColor);
            this.mTextBox.BackColor = this._backColor;
            if (this.DropDownStyle == ComboBoxStyle.Simple)
            {
                // 绘制边框
                GDIHelper.DrawPathBorder(g, new RoundRectangle(rectOuter, this.CornerRadius), this.Theme.SelectedBorderColor);

                // 定位列表
                this.mListBox.Top = this.mTextBox.Height + this.mMargin * 2;
            }
            else
            {
                // 绘制按钮
                GDIHelper.FillPath(g, new RoundRectangle(this.rectBtn, this.CornerRadius), this.Theme.ButtonBackColor);
                GDIHelper.DrawArrow(g, ArrowDirection.Down, this.rectBtn, new Size(10, 7), 0f, this.Theme.ButtonForeColor);
                g.DrawLines(new Pen(this._borderColor), GDIHelper.GetLineByRectangle(this.rectBtn, LineDrawDirection.Left));

                if (this.BorderStyle == BorderStyle.FixedSingle)
                {   // 绘制边框
                    GDIHelper.DrawPathBorder(g, new RoundRectangle(rectOuter, this.CornerRadius), this._borderColor);
                }
            }

            // 绘制图片
            if (this.ImageList != null)
            {
                // 显示图片的情况
                this.AdjustControls();

                if (this.SelectedItem != null)
                {
                    string value = SparkComboBoxDatasourceResolver.GetMemberValue(this.SelectedItem, this.ImageMember)?.ToString();
                    if (this.ImageList.Images.ContainsKey(value))
                    {
                        Image image = this.ImageList.Images[value];
                        if (image != null)
                        {
                            GDIHelper.DrawImage(g, new Rectangle(this.mMargin, this.mMargin, this.mTextBox.Height, this.mTextBox.Height), image);
                        }
                    }
                }
            }

            if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                // 隐藏文本框
                this.mTextBox.Visible = false;

                // 绘制文本
                Rectangle rectText = this.mTextBox.Bounds;
                using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap) { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
                {
                    if (this.Enabled)
                    {
                        using (SolidBrush foreBrush = new SolidBrush(this.ForeColor))
                        {
                            g.DrawString(this.mTextBox.Text, this.Font, foreBrush, rectText, sf);
                        }
                    }
                    else
                    {
                        ControlPaint.DrawStringDisabled(g, this.mTextBox.Text, this.Font, this.BackColor, rectText, sf);
                    }
                }
            }
        }

        /// <summary>
        /// 引发 SelectedIndexChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 引发 Resize 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnResize(EventArgs e)
        {
            if (this.DesignMode)
            {
                this.DropDownWidth = this.Width;
            }
            if (this.mListBox != null)
            {
                this.mListBox.Width = this.DropDownWidth;
            }
            if (this.mResize)
            {
                this.mResize = false;
                this.AdjustControls();
            }
            base.OnResize(e);
        }

        /// <summary>
        /// 刷新指定索引的项。
        /// </summary>
        /// <param name="index">项索引。</param>
        protected override void RefreshItem(int index)
        {
            //throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// 刷新项目。
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
        }

        /// <summary>
        /// 设置指定索引项的值。
        /// </summary>
        /// <param name="index">项索引。</param>
        /// <param name="value">项的值。</param>
        protected override void SetItemCore(int index, object value)
        {
            base.SetItemCore(index, value);
        }

        /// <summary>
        /// 设置项的值。
        /// </summary>
        /// <param name="items">项集合。</param>
        protected override void SetItemsCore(IList items)
        {
            //throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// 引发 System.Windows.Forms.Control.KeyPress 事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        /// <summary>
        /// 引发 System.Windows.Forms.Control.KeyDown 事件。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                if (this.PopupDropDown.Visible == false)
                {
                    this.IsDroppedDown = true;
                }
            }
        }

        /// <summary>
        /// 引发 ItemsChanged 事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemsChanged(ItemsChangedEventArgs e)
        {
            ItemsChanged?.Invoke(this, e);
        }

        #endregion

        #region 嵌套控件事件
        private void ListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.mListBox.Items.Count == 0)
                return;

            if (this.mListBox.SelectedItems.Count != 1)
                return;

            this.SelectedIndex = this.mListBox.SelectedIndex;
            if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                this.Invalidate(true);
            }
            if (this.DropDownStyle != ComboBoxStyle.Simple)
            {
                this.IsDroppedDown = false;
            }
        }

        private void PopupControl_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.mIsDroppedDown = false;
            this.Invalidate(true);
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (this.PopupDropDown.Visible)
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region 虚方法

        /// <summary>
        /// 引发 DroppedDown 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected virtual void OnDropDown(EventArgs e)
        {
            DropDown?.Invoke(this, e);
        }

        /// <summary>
        /// 引发 DrawItem 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                DrawItem?.Invoke(this, e);
            }
        }

        /// <summary>
        /// 引发 MeasureItem 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected virtual void OnMeasureItem(MeasureItemEventArgs e)
        {
            MeasureItem?.Invoke(this, e);
        }

        /// <summary>
        /// 引发 SelectedItemChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 引发 SelectedItemChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected virtual void OnSelectingItemChanged(CancelEventArgs e)
        {
            if (SelectingItemChanged != null)
            {
                SelectingItemChanged(this, e);
            }
        }

        /// <summary>
        /// 隐藏下拉框
        /// </summary>
        protected virtual void HideDropDown()
        {
            if (this.PopupDropDown?.Visible == true)
            {
                this.PopupDropDown?.Close();
                this.OnDropDownClosed(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 下拉部分不可见时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            DropDownClosed?.Invoke(this, e);
        }

        /// <summary>
        /// 显示下拉框
        /// </summary>
        protected virtual void ShowDropDown()
        {
            // 刷新下拉框
            this.mListBox.Refresh();
            // 引发事件
            this.OnDropDown(EventArgs.Empty);
            int height;
            // 计算下拉框的高度
            if (this.mListBox.Items.Count > 0)
            {
                int maxItemHeight = 0;
                int highestItemHeight = 0;
                for (int i = 0; i < this.mListBox.Items.Count; i++)
                {
                    int itemHeight = this.mListBox.GetItemRectangle(i).Height;
                    if (highestItemHeight < itemHeight) highestItemHeight = itemHeight;

                    maxItemHeight += itemHeight;
                    if (i >= this.MaxDropDownItems - 1) break;
                }
                height = Math.Min(this.DropDownHeight, Math.Max(maxItemHeight, highestItemHeight)) + 2;
            }
            else
            {
                height = 15;
            }
            //设置下拉框的大小
            this.PopupDropDown.Size = new Size(this.DropDownWidth, height);
            // 显示下拉框
            this.PopupDropDown.Show(this, this.CalculateDropPosition(), ToolStripDropDownDirection.BelowRight);

            this.Invalidate(true);
        }

        /// <summary>
        /// 清空选项时的初始化
        /// </summary>
        protected virtual void ClearItemInit()
        {
            //_isClearItemFlag = true;
            if (this.SelectedIndex != -1)
            {
                this.SelectedIndex = -1;
            }
            if (!this.Text.IsNullOrEmpty())
            {
                this.Text = "";
            }
            //_isClearItemFlag = false;
        }
        #endregion

        #region 公有方法

        /// <summary>
        /// 选定所有文本。
        /// </summary>
        public void SelectAll()
        {
            this.mTextBox.SelectAll();
        }

        #endregion

        #region 私有方法

        // 校正控件尺寸和位置
        private void AdjustControls()
        {
            this.SuspendLayout();

            this.mResize = true;

            this.rectContent = new Rectangle(this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width, this.mTextBox.Height + this.mMargin * 2);
            if (this.DropDownStyle == ComboBoxStyle.Simple)
            {
                this.rectBtn = new Rectangle(this.ClientRectangle.Width, this.ClientRectangle.Top, 0, 0);

                this.mTextBox.AutoSize = false;
                this.mTextBox.Width = this.Width - this.mMargin * 2 - 2;
            }
            else
            {
                this.rectBtn = new Rectangle(this.ClientRectangle.Width - 20, this.ClientRectangle.Top, 20, this.mTextBox.Height + this.mMargin * 2);

                this.mTextBox.AutoSize = true;
                this.mTextBox.Width = this.rectBtn.Left - this.mTextBox.Left - 1;
                this.Height = this.mTextBox.Height + this.mMargin * 2;
            }

            // 显示图片的情况，修装文本框的位置和尺寸
            if (this.ImageList != null)
            {
                this.mTextBox.Location = new Point(this.mMargin + this.mTextBox.Height + this.mMargin, this.mTextBox.Location.Y);
                this.mTextBox.Width -= this.mTextBox.Height;
            }

            this.ResumeLayout();
        }

        // 计算下拉框位置
        private Point CalculateDropPosition()
        {
            Point point = new Point(0, this.Height);
            //考虑多屏的情况
            Screen screen = Screen.FromControl(this);
            if ((this.PointToScreen(Point.Empty).Y + this.Height + this.PopupDropDown.Height - screen.Bounds.Y) > screen.Bounds.Height)
            {
                point.Y += -this.Height - this.PopupDropDown.Height - 3;
            }
            return point;
        }

        /// <summary>
        /// 初始化样式
        /// </summary>
        private void InitControlStyle()
        {
            switch (this._controlState)
            {
                case ControlState.Focused:
                    this._backColor = this.Theme.MouseOverBackColor;
                    this._borderColor = this.Theme.MouseDownBorderColor;
                    break;
                case ControlState.Highlight:
                    this._backColor = this.Theme.MouseOverBackColor;
                    this._borderColor = this.Theme.MouseOverBorderColor;
                    break;
                default:
                    this._backColor = this.Theme.BackColor;
                    this._borderColor = this.Theme.BorderColor;
                    break;
            }

            if (!this.Enabled)
            {
                this._backColor = this.Theme.DisabledBackColor;
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
        public SparkComboBoxTheme Theme { get; private set; }

        #endregion

        #region IDualDataBinding 接口成员

        /// <summary>
        /// 获取或设置控件实际绑定的字段名。
        /// </summary>
        [Category("Spark"), Description("控件实际值绑定的字段名。")]
        [DefaultValue(null)]
        public virtual string FieldName { get; set; } = null;

        /// <summary>
        /// 获取或设置控件显示值绑定的字段名。
        /// </summary>
        [Category("Spark"), Description("控件显示值绑定的字段名。")]
        [DefaultValue(null)]
        public virtual string DisplayFieldName { get; set; } = null;

        /// <summary>
        /// 获取或设置控件的实际值。
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public virtual object Value
        {
            get => this.SelectedValue;
            set => this.SelectedValue = value;
        }

        /// <summary>
        /// 获取或设置控件的显示值。
        /// </summary>
        [Browsable(false)]
        public virtual string DisplayValue => this.SelectedText;

        #endregion

        /// <summary>
        /// 组合框下拉列表。
        /// </summary>
        protected class SparkComboBoxListBox : SparkListBox
        {
            private readonly SparkComboBoxBase mComboBox = null;
            /// <summary>
            /// 初始 <see cref="SparkComboBoxListBox"/> 类型的新实例。
            /// </summary>
            /// <param name="comboBox"></param>
            internal SparkComboBoxListBox(SparkComboBoxBase comboBox)
            {
                this.mComboBox = comboBox ?? throw new ArgumentNullException(nameof(comboBox), "值不能为空。");
            }

            /// <summary>
            /// 创建项集合的新实例。
            /// </summary>
            /// <returns>表示项集合的 <see cref="ObjectCollection"/> 类型的实例。</returns>
            protected override ObjectCollection CreateItemCollection()
            {
                return new SparkComboBoxBase.ObjectCollection(this.mComboBox);
            }
        }

        /// <summary>
        /// 组合框项集合类。
        /// </summary>
        public class ObjectCollection : ListBox.ObjectCollection
        {
            private readonly SparkComboBoxBase mComboBox = null;
            private readonly SparkComboBoxListBox mListBox = null;

            /// <summary>
            /// 初始 <see cref="ObjectCollection"/> 类型的新实例。
            /// </summary>
            /// <param name="comboBox">所属组合框对象。</param>
            public ObjectCollection(SparkComboBoxBase comboBox) : base(comboBox.mListBox)
            {
                this.mComboBox = comboBox ?? throw new ArgumentNullException(nameof(comboBox), "值不能为空。");
                this.mListBox = comboBox.mListBox;
            }

            /// <summary>
            /// 初始 <see cref="ObjectCollection"/> 类型的新实例。
            /// </summary>
            /// <param name="comboBox">所属组合框对象。</param>
            /// <param name="value">现有集合对象。</param>
            public ObjectCollection(SparkComboBoxBase comboBox, ObjectCollection value) : base(comboBox.mListBox, value)
            {
                this.mComboBox = comboBox ?? throw new ArgumentNullException(nameof(comboBox), "值不能为空。");
                this.mListBox = comboBox.mListBox;
            }

            /// <summary>
            /// 初始 <see cref="ObjectCollection"/> 类型的新实例。
            /// </summary>
            /// <param name="comboBox">所属组合框对象。</param>
            /// <param name="value">要添加到集合的对象数组。</param>
            public ObjectCollection(SparkComboBoxBase comboBox, object[] value) : base(comboBox.mListBox, value)
            {
                this.mComboBox = comboBox ?? throw new ArgumentNullException(nameof(comboBox), "值不能为空。");
                this.mListBox = comboBox.mListBox;
            }

            /// <summary>
            /// 向集合中添加项。
            /// </summary>
            /// <param name="item">要添加的项。</param>
            /// <returns>集合中从零开始的索引。</returns>
            public new int Add(object item)
            {
                int result = base.Add(item);
                this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Add, new object[] { item }));
                return result;
            }

            /// <summary>
            /// 将现有集合的项添加到集合中。
            /// </summary>
            /// <param name="items">现有集合。</param>
            public void AddRange(ObjectCollection items)
            {
                base.AddRange(items);
                this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Add, items.Cast<object>()));
            }

            /// <summary>
            /// 向集合中插入项的数组。
            /// </summary>
            /// <param name="items">要插入项的数组。</param>
            public new void AddRange(object[] items)
            {
                base.AddRange(items);
                this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Add, items));
            }

            /// <summary>
            /// 从集合中移除所有项。
            /// </summary>
            public override void Clear()
            {
                if (this.mListBox.Items.Count > 0)
                {
                    ObjectCollection items = this;
                    base.Clear();
                    this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Clear, items.Cast<object>()));
                    this.mComboBox.ClearItemInit();
                }
            }

            /// <summary>
            /// 将一个项插入到集合中指定的索引处。
            /// </summary>
            /// <param name="index">要插入项的索引。</param>
            /// <param name="item">要插入的项。</param>
            public new void Insert(int index, object item)
            {
                base.Insert(index, item);
                this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Add, new object[] { item }));
            }

            /// <summary>
            /// 从集合中删除指定的对象。
            /// </summary>
            /// <param name="item">要删除的对象。</param>
            public new void Remove(object item)
            {
                if (this.Contains(item))
                {
                    base.Remove(item);
                    this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Remove, new object[] { item }));
                }
            }

            /// <summary>
            /// 移除集合中指定索引的项。
            /// </summary>
            /// <param name="index">索引号。</param>
            public new void RemoveAt(int index)
            {
                if (index >= 0 && index < this.Count)
                {
                    object item = this[index];
                    base.RemoveAt(index);
                    this.mComboBox.OnItemsChanged(new ItemsChangedEventArgs(ItemsAction.Remove, new object[] { item }));
                }
            }
        }
    }

    /// <summary>
    /// 组合框项集合改变事件委托。
    /// </summary>
    /// <param name="sender">发送对象</param>
    /// <param name="e">事件数据</param>
    public delegate void ItemsChangedEventHandler(object sender, ItemsChangedEventArgs e);

    /// <summary>
    /// 包含组合框项集合改变事件数据。
    /// </summary>
    public class ItemsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public ItemsAction Action { get; }
        /// <summary>
        /// 发生改变的项
        /// </summary>
        public IEnumerable<object> ChangedItems { get; }

        /// <summary>
        /// 初始 <see cref="ItemsChangedEventArgs"/> 类型的新实例。
        /// </summary>
        /// <param name="action">操作类型</param>
        /// <param name="changedItems">发生改变的项</param>
        public ItemsChangedEventArgs(ItemsAction action, IEnumerable<object> changedItems)
        {
            this.Action = action;
            this.ChangedItems = changedItems;
        }
    }

    /// <summary>
    /// 组合框项集合操作枚举。
    /// </summary>
    public enum ItemsAction
    {
        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 移除
        /// </summary>
        Remove,
        /// <summary>
        /// 清空
        /// </summary>
        Clear
    }
}