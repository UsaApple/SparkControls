using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Controls.Design;
using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 选项卡控件。
    /// </summary>
    [Designer(typeof(SparkTabControlDesigner))]
    [DefaultProperty(nameof(Items))]
    [DefaultEvent(nameof(SelectedIndexChanged))]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TabControl))]
    public class SparkTabControl : ContainerControl, ISupportInitialize, ISparkTheme<SparkTabControlTheme>
    {
        #region 常量
        private const int DEF_HEADER_HEIGHT = 32;
        private const int DEF_GLYPH_WIDTH = 40;
        /// <summary>
        /// 起始位置
        /// </summary>
        private float DEF_START_POS = 10;
        #endregion

        #region 静态变量
        internal static int PreferredWidth = 500;
        internal static int PreferredHeight = 200;
        #endregion

        #region 私有变量
        private readonly ContextMenuStrip menu = null;
        private readonly SparkTabControlMenuGlyph menuGlyph = null;
        private readonly SparkTabControlCloseButton closeButton = null;
        private readonly StringFormat sf = null;
        private readonly SparkTitleBarDraw titleBarDraw = null;
        private readonly Color mTitleBackColor = Color.FromArgb(244, 244, 244);

        /// <summary>
        /// 绘制按钮的区域大小
        /// </summary>
        private Rectangle stripButtonRect = Rectangle.Empty;
        /// <summary>
        /// 当前选中的选项卡
        /// </summary>
        private SparkTabPage selectedItem = null;
        /// <summary>
        /// 鼠标进入的选项卡
        /// </summary>
        private SparkTabPage mouseOverItem = null;

        private CloseButtonDisplayModes _closeButtonDisplayMode = CloseButtonDisplayModes.None; //默认没有关闭按钮
        protected bool isIniting = false;
        private bool alwaysShowMenuGlyph = true;
        private bool menuOpen = false;
        private TabAlignment alignment = TabAlignment.Top;
        private Font defaultFont = Consts.DEFAULT_FONT;
        private int tabPageSpacing = 5;
        private int tabPageTopMargin = 5;
        private int tabPageBottomMargin = 0;
        private bool autoSize = true;
        private int tabPageWidth = 200;
        private int tabPageMaxWidth = 0;
        private int tabPageMinWidth = 0;
        private int leftMargin = 0;
        #endregion

        #region 事件定义
        /// <summary>
        /// TabPage关闭前事件
        /// </summary>
        public event TabPageClosingEventHandler TabPageClosing;
        /// <summary>
        /// TabPage改变事件(包含增删改)
        /// </summary>
        public event TabPageChangedEventHandler TabPageSelectionChanged;

        /// <summary>
        /// 在选项卡索引SelectedIndex属性更改后发生(比TabPageSelectionChanged晚)
        /// </summary>
        public event TabPageSelectedIndexEventHandler SelectedIndexChanged;

        /// <summary>
        /// 选择框选择前事件，比SelectedIndexChanged先触发
        /// </summary>
        public event TabPageBeforeSelectEventHandler TabPageBeforeSelect;

        /// <summary>
        /// 菜单加载中
        /// </summary>
        public event HandledEventHandler MenuItemsLoading;
        /// <summary>
        /// 菜单加载后
        /// </summary>
        public event EventHandler MenuItemsLoaded;
        /// <summary>
        /// 关闭事件
        /// </summary>
        public event EventHandler TabPageClosed;
        #endregion

        #region 属性
        /// <summary>
        /// 图标的绘制的位置
        /// </summary>
        [Category("Spark"), Description("选择页的横线的位置，")]
        [DefaultValue(TabImageAlignment.Left)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual TabImageAlignment ImageAlignment { get => TabImageAlignment.Left; set => imageAlignment = value; }

        /// <summary>
        /// 选择TabPage时的线条的位置
        /// </summary>
        [Category("Spark"), Description("选择TabPage时的线条的位置,默认LeftTop")]
        [DefaultValue(TabPageSelectedLineAlignment.LeftTop)]
        public virtual TabPageSelectedLineAlignment SelectedLineAlignment
        {
            get => selectedLineAlignment;
            set
            {
                if (selectedLineAlignment != value)
                {
                    selectedLineAlignment = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 标题绘制类
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SparkTitleBarDraw TitleBarDraw => this.titleBarDraw;

        /// <summary>
        /// 左边距，Top,Bottom,左端的边距，Left，Right，上端的边距
        /// </summary>
        [Category("Spark"), Description("左边距,")]
        [DefaultValue(0)]
        public int LeftMargin
        {
            get => this.leftMargin;
            set
            {
                if (this.leftMargin != value)
                {
                    this.leftMargin = value;
                    this.PerformLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get => this.defaultFont;
            set
            {
                base.Font = this.defaultFont = value;
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
        /// 选中的标签页
        /// </summary>
        [DefaultValue(null)]
        //[Browsable(false)]
        [Category("Spark"), Description("当前选中的项")]
        public virtual SparkTabPage SelectedItem
        {
            get => this.selectedItem;
            set
            {
                if (this.selectedItem == value) return;

                if (this.selectedItem != null)
                {
                    var beforeSelect = new TabPageBeforeSelectEventArgs(this.selectedItem);
                    TabPageBeforeSelect?.Invoke(beforeSelect);
                    if (beforeSelect.Cancel)
                    {
                        return;
                    }
                }
                if (value == null && this.Items.Count > 0)
                {
                    this.selectedItem = this.Items.FirstVisible;
                    this.selectedItem.Selected = true;
                }
                else
                {
                    this.selectedItem = value;
                }

                foreach (SparkTabPage itm in this.Items)
                {
                    if (itm == this.selectedItem)
                    {
                        this.SelectItem(itm);
                        itm.Show();
                    }
                    else
                    {
                        this.UnSelectItem(itm);
                        itm.Hide();
                    }
                }
                if (this.selectedItem != null)
                {
                    this.SelectItem(this.selectedItem);

                }
                //要先计算下能不能显示的下，如果不能显示的下
                //根据固定项判断，移动到最后固定项的后面
                if (!this.IsCanDraw(this.selectedItem))
                {
                    if (this.Items.FixedCount > 0)
                    {
                        this.Items.MoveTo(this.Items.FixedCount, this.selectedItem);
                    }
                    else
                    {
                        this.Items.MoveTo(0, this.selectedItem);
                    }
                }
                this.OnTabStripItemChanged(new SparkTabPageChangedEventArgs(this.selectedItem, TabPageChangeAction.SelectionChanged));
                this.Invalidate();

            }
        }

        /// <summary>
        /// 是否一直显示菜单按钮
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("是否一直显示菜单按钮")]
        public virtual bool AlwaysShowMenuGlyph
        {
            get => this.alwaysShowMenuGlyph;
            set
            {
                if (this.alwaysShowMenuGlyph == value) return;
                this.alwaysShowMenuGlyph = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 关闭按钮显示模式
        /// </summary>
        [DefaultValue(CloseButtonDisplayModes.None)]
        [Category("Spark"), Description("关闭按钮显示模式")]
        public CloseButtonDisplayModes CloseButtonDisplayMode
        {
            get => this._closeButtonDisplayMode;
            set
            {
                if (this._closeButtonDisplayMode != value)
                {
                    this._closeButtonDisplayMode = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// <see cref="SparkTabPage"/> 集合对象。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Spark"), Description("TabPage集合")]
        public SparkTabControlCollection Items { get; }

        /// <summary>
        /// 控件大小
        /// </summary>
        [DefaultValue(typeof(Size), "350,200")]
        [Category("Spark"), Description("尺寸")]
        public new Size Size
        {
            get => base.Size;
            set
            {
                if (base.Size == value)
                    return;

                base.Size = value;
                this.UpdateLayout();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 子控件集合
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new ControlCollection Controls => base.Controls;

        /// <summary>
        /// 获取或设置一个值，该值指示选项卡在其中对齐的控件区域。
        /// </summary>
        [Description("选项卡在其中对齐的控件区域。")]
        [DefaultValue(TabAlignment.Top)]
        [Category("Spark")]
        public virtual TabAlignment Alignment
        {
            get => this.alignment;
            set
            {
                if (this.alignment != value)
                {
                    this.alignment = value;
                    _cornerRadius = this.GetCornerRadius();
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 选中页的索引号
        /// </summary>
        [Browsable(false)]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get => this.Items.IndexOf(this.SelectedItem);
            set
            {
                if (this.SelectedIndex != value && this.Items.Count > value && value >= 0)
                {
                    this.SelectedItem = this.Items[value];
                }
            }
        }

        #region 标题栏属性
        /// <summary>
        /// 自定义按钮集合,定义一个集合对象，增加设计器，可前台编辑
        /// </summary>
        [Browsable(false)]
        public IList<SparkTitleBarItem> CustomItem => this.titleBarDraw?.CustomItem;

        /// <summary>
        /// 是否显示标题栏
        /// </summary>
        [Category("Spark"), Description("获取或设置窗体是否标题栏。")]
        [DefaultValue(false)]
        [Browsable(false)]
        public virtual bool IsDrawTitle
        {
            get => this.titleBarDraw.IsDrawTitle;
            set => this.titleBarDraw.IsDrawTitle = value;
        }

        /// <summary>
        ///  获取或设置一个值，该值指示是否在窗体的标题栏中显示“最小化”按钮。
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("是否显示最小化按钮")]
        [Browsable(false)]
        public virtual bool MinimizeBox
        {
            get => this.titleBarDraw.MinimizeBox;
            set => this.titleBarDraw.MinimizeBox = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否在窗体的标题栏中显示“最大化”按钮。
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("是否显示最大化按钮")]
        [Browsable(false)]
        public virtual bool MaximizeBox
        {
            get => this.titleBarDraw.MaximizeBox;
            set => this.titleBarDraw.MaximizeBox = value;
        }

        /// <summary>
        /// 获取或设置窗体的图标
        /// </summary>
        [DefaultValue(null)]
        [Category("Spark"), Description("标题图标")]
        [Browsable(false)]
        public virtual Icon Icon
        {
            get => this.titleBarDraw.Icon;
            set => this.titleBarDraw.Icon = value;
        }

        /// <summary>
        /// 标题字体
        /// </summary>
        [DefaultValue(typeof(Font), "微软雅黑,12pt")]
        [Category("Spark"), Description("标题字体")]
        [Browsable(false)]
        public Font TitleFont
        {
            get => this.titleBarDraw.TitleFont;
            set => this.titleBarDraw.TitleFont = value;
        }

        /// <summary>
        /// 标题高度
        /// </summary>
        [DefaultValue(DEF_HEADER_HEIGHT)]
        [Category("Spark"), Description("标题高度")]
        public int TitleHeight
        {
            get => this.titleBarDraw.TitleHeight;
            set
            {
                this.titleBarDraw.TitleHeight = value;
                this.UpdateLayout();
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        [Browsable(false)]
        [Category("Spark"), Description("标题文本")]
        public override string Text
        {
            get => "";
            set => this.titleBarDraw.Text = "";
        }
        #endregion

        #region 选项卡属性
        /// <summary>
        /// 选项卡的间距
        /// </summary>
        [DefaultValue(5)]
        [Category("Spark"), Description("选项卡的间距")]
        public int TabPageSpacing
        {
            get => this.tabPageSpacing;
            set
            {
                if (this.tabPageSpacing != value)
                {
                    this.tabPageSpacing = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 选项卡的边距
        /// </summary>
        [DefaultValue(5)]
        [Category("Spark"), Description("选项卡的边距(顶部边距)")]
        public int TabPageMargin
        {
            get => this.tabPageTopMargin;
            set
            {
                if (this.tabPageTopMargin != value)
                {
                    if (this.TitleHeight - value <= 10) value = this.TitleHeight - 10;
                    this.tabPageTopMargin = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 选项卡底部的边距
        /// </summary>
        [DefaultValue(0)]
        [Category("Spark"), Description("选项卡的边距(底部边距)")]
        public int TabPageBottomMargin
        {
            get => this.tabPageBottomMargin;
            set
            {
                if (this.tabPageBottomMargin != value)
                {
                    if (this.TitleHeight - value - this.TabPageMargin < 0) value = 0;
                    this.tabPageBottomMargin = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// TabPage自动大小,但受TabPageMaxWidth值的限制
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("TabPage自动大小,但受TabPageMaxWidth值的限制")]
        [Browsable(true)]
        public new bool AutoSize
        {
            get => this.autoSize;
            set
            {
                if (this.autoSize != value)
                {
                    this.autoSize = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// TabPage的固定宽度,AutoSize=false才生效，但受TabPageMaxWidth值的限制
        /// </summary>
        [DefaultValue(200)]
        [Category("Spark"), Description("TabPage的固定宽度,AutoSize=false才生效，但受TabPageMaxWidth值的限制")]
        public virtual int TabPageWidth
        {
            get => this.tabPageWidth;
            set
            {
                if (this.tabPageWidth != value)
                {
                    this.tabPageWidth = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// TabPage的最大宽度，等于0时不受限制
        /// </summary>
        [DefaultValue(0)]
        [Category("Spark"), Description("TabPage的最大宽度，等于0时不受限制")]
        public int TabPageMaxWidth
        {
            get => this.tabPageMaxWidth;
            set
            {
                if (this.tabPageMaxWidth != value)
                {
                    this.tabPageMaxWidth = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// TabPage的最小宽度，当AutoSize=true时生效，等于0时不受限制
        /// </summary>
        [DefaultValue(0)]
        [Category("Spark"), Description("TabPage的最小宽度，当AutoSize=true时生效，等于0时不受限制，")]
        public int TabPageMinWidth
        {
            get => this.tabPageMinWidth;
            set
            {
                if (this.tabPageMinWidth != value)
                {
                    this.tabPageMinWidth = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        ///  该值指示是否将控件的元素对齐以支持使用从右向左的字体的区域设置。
        /// </summary>
        [DefaultValue(RightToLeft.No)]
        [Category("Spark"), Description("该值指示是否将控件的元素对齐以支持使用从右向左的字体的区域设置")]
        public override RightToLeft RightToLeft
        {
            get => base.RightToLeft;
            set
            {
                if (base.RightToLeft != value)
                {
                    base.RightToLeft = value;
                    this.UpdateLayout();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// TabPage的高度
        /// </summary>
        [Category("Spark"), Description("TabPage的高度=TitleHeight - TabPageMargin - TabPageBottomMargin")]
        public int TabPageHeight => this.TitleHeight - this.TabPageMargin - this.TabPageBottomMargin;

        private SparkTabControlStyle mStyle = SparkTabControlStyle.Default;
        /// <summary>
        /// 获取或设置控件的风格。
        /// </summary>
        [Category("Spark"), Description("控件的显示风格。")]
        [TypeConverter(typeof(EnumConverter))]
        [DefaultValue(SparkTabControlStyle.Default)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SparkTabControlStyle Style
        {
            get { return this.mStyle; }
            set
            {
                if (this.mStyle != value)
                {
                    this.mStyle = value;
                    if (this.mStyle == SparkTabControlStyle.Ellipse)
                    {
                        SetEllipseStyle();
                    }
                    else if (this.mStyle == SparkTabControlStyle.SplitBar)
                    {
                        SetSplitBarStyle();
                    }
                    else
                    {
                        SetDefaultStyle();
                    }
                    _cornerRadius = this.GetCornerRadius();
                    this.Invalidate();
                }
            }
        }

        [Category("Spark"), Description("控件的边框样式。")]
        [DefaultValue(BorderStyle.None)]
        public BorderStyle BorderStyle { get; set; }
        #endregion

        /// <summary>
        /// 管理TabPage的图标用，暂未实现
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        internal ImageList ImageList { get; } = new ImageList();

        private CornerRadius _cornerRadius = new CornerRadius(0);
        private TabImageAlignment imageAlignment = TabImageAlignment.Left;
        private TabPageSelectedLineAlignment selectedLineAlignment = TabPageSelectedLineAlignment.LeftTop;

        [Category("Spark"), Description("矩形的圆角角度")]
        [DefaultValue(typeof(CornerRadius), "0")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = value;
                    this.Invalidate();
                }
            }
        }

        [Category("Spark"), Description("分割线的宽度（粗细）")]
        [DefaultValue(0f)]
        public float SplitLineWidth { get; set; } = 0f;

        [Category("Spark"), Description("分割线的长度,默认位0，表示和标签页一样")]
        [DefaultValue(0f)]
        public float SplitLineLength { get; set; } = 0f;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkTabControl()
        {
            this.BeginInit();
            this.SetStyle(ControlStyles.ResizeRedraw |//调整大小时重绘
                          ControlStyles.DoubleBuffer |//双缓冲
                          ControlStyles.OptimizedDoubleBuffer |//双缓冲
                          ControlStyles.AllPaintingInWmPaint |//禁止擦除背景
                          ControlStyles.UserPaint |
                          ControlStyles.ContainerControl |
                          ControlStyles.Selectable, true);
            this.Font = this.defaultFont;
            this.BackColor = SparkThemeConsts.BackColor;
            this.Theme = new SparkTabControlTheme(this);
            this.Items = OnCreateControlCollection();
            this.Items.CollectionChanged += new CollectionChangeEventHandler(this.CollectionChanged);
            this.titleBarDraw = new SparkTitleBarDraw(this, Theme.TitleTheme);
            this.titleBarDraw.TitleHitTest += this.TitleBarDraw_TitleHitTest;
            base.Size = new Size(350, 200);
            this.menu = new ContextMenuStrip();
            this.menu.ItemClicked += new ToolStripItemClickedEventHandler(this.MenuItemClicked);
            this.menu.VisibleChanged += new EventHandler(this.MenuVisibleChanged);
            this.menuGlyph = new SparkTabControlMenuGlyph();
            this.closeButton = new SparkTabControlCloseButton(this);
            this.sf = new StringFormat();
            this.IsDrawTitle = false;
            this._cornerRadius = GetCornerRadius();
            this.EndInit();
        }
        #endregion

        #region 方法
        #region public方法

        /// <summary>
        /// Returns hit test results
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public HitTestResult HitTest(Point pt)
        {
            //if (closeButton.Bounds.Contains(pt))
            //    return HitTestResult.CloseButton;
            if (this.menuGlyph.Bounds.Contains(pt))
                return HitTestResult.MenuGlyph;

            SparkTabPage item = this.GetTabPageByPoint(pt);
            if (item != null)
            {
                if (item.CloseButtonRect.Contains(pt) && item.IsDrawClose && this._closeButtonDisplayMode > 0)
                {
                    return HitTestResult.CloseButton;
                }
                else
                {
                    return HitTestResult.TabPage;
                }
            }
            //No other result is available.
            return HitTestResult.None;
        }

        /// <summary>
        /// Add a <see cref="SparkTabPage"/> to this control without selecting it.
        /// </summary>
        /// <param name="tabItem"></param>
        public virtual void AddTab(SparkTabPage tabItem)
        {
            this.AddTab(tabItem, false);
        }

        /// <summary>
        /// 添加标签页
        /// </summary>
        /// <param name="tabItem"><see cref="SparkTabPage"/>对象</param>
        /// <param name="autoSelect">是否选中</param>
        public virtual void AddTab(SparkTabPage tabItem, bool autoSelect)
        {
            this.Items.Add(tabItem);
            if (this.isIniting) return;
            if ((autoSelect && tabItem.Visible) || (tabItem.Visible && this.Items.DrawnCount < 1))
            {
                this.SelectedItem = tabItem;
                this.SelectItem(tabItem);
            }
        }

        /// <summary>
        /// Remove a <see cref="SparkTabPage"/> from this control.
        /// </summary>
        /// <param name="tabItem"></param>
        public void RemoveTab(SparkTabPage tabItem)
        {
            int tabIndex = this.Items.IndexOf(tabItem);
            bool isCurrentItem = this.SelectedIndex == tabIndex;

            if (tabIndex >= 0)
            {
                this.UnSelectItem(tabItem);
                this.Items.Remove(tabItem);
                this.Controls.Remove(tabItem);
                tabItem.Dispose();  //add
            }
            if (this.Items.Count > 0 && isCurrentItem)
            {
                if (this.Items.Count > tabIndex)
                {
                    this.SelectedItem = this.Items.NextTabPageVisible(tabIndex);
                }
                else
                {
                    this.SelectedItem = this.Items.LastVisible;
                }
            }
        }

        /// <summary>
        /// Get a <see cref="SparkTabPage"/> at provided point.
        /// If no item was found, returns null value.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public SparkTabPage GetTabPageByPoint(Point pt)
        {
            return this.Items.Cast<SparkTabPage>().FirstOrDefault(a => a.StripRect.Contains(pt) && a.Visible && a.IsDrawn);
        }

        /// <summary>
        /// Display items menu
        /// </summary>
        public virtual void ShowMenu()
        {
            if (this.menu.Visible == false && this.menu.Items.Count > 0)
            {
                if (this.RightToLeft == RightToLeft.No)
                {
                    this.menu.Show(this, new Point(this.menuGlyph.Bounds.Left, this.menuGlyph.Bounds.Bottom));
                }
                else
                {
                    this.menu.Show(this, new Point(this.menuGlyph.Bounds.Right, this.menuGlyph.Bounds.Bottom));
                }

                this.menuOpen = true;
            }
        }

        #endregion

        #region Internal方法

        internal void UnDrawAll()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].IsDrawn = false;
            }
        }

        internal void SelectItem(SparkTabPage tabItem)
        {
            tabItem.Visible = true;
            tabItem.Selected = true;
        }

        internal void UnSelectItem(SparkTabPage tabItem)
        {
            //tabItem.Visible = false;
            tabItem.Selected = false;
        }

        #endregion

        #region Protected方法

        /// <summary>
        /// TabPage关闭前方法
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabPageClosing(TabPageClosingEventArgs e)
        {
            TabPageClosing?.Invoke(e);
        }

        /// <summary>
        /// TabPage关闭后事件
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabPageClosed(EventArgs e)
        {
            TabPageClosed?.Invoke(this, e);
        }

        /// <summary>
        /// 加载菜单前方法
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoading(HandledEventArgs e)
        {
            MenuItemsLoading?.Invoke(this, e);
        }

        /// <summary>
        /// 加载菜单后方法
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoaded(EventArgs e)
        {
            MenuItemsLoaded?.Invoke(this, e);
        }

        /// <summary>
        /// TabPage改变事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTabStripItemChanged(SparkTabPageChangedEventArgs e)
        {
            TabPageSelectionChanged?.Invoke(e);
            if (SelectedIndexChanged != null && e.ChangeAction == TabPageChangeAction.SelectionChanged)
            {
                SelectedIndexChanged.Invoke(new TabPageSelectedIndexEventArgs(e.Item, this.SelectedIndex));
            }
        }

        /// <summary>
        ///加载菜单方法
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoad(EventArgs e)
        {
            this.menu.RightToLeft = this.RightToLeft;
            this.menu.Items.Clear();

            for (int i = 0; i < this.Items.Count; i++)
            {
                SparkTabPage item = this.Items[i];
                if (!item.Visible)
                    continue;

                ToolStripMenuItem tItem = new ToolStripMenuItem(item.Text)
                {
                    Tag = item,
                    Image = item.Image
                };
                this.menu.Items.Add(tItem);
            }
            this.OnMenuItemsLoaded(EventArgs.Empty);
        }

        #endregion

        #region 重写事件
        /// <summary>
        /// RightToLeft改变方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.UpdateLayout();
            this.Invalidate();
        }

        /// <summary>
        /// 重绘方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.isIniting) return;
            base.OnPaint(e);
            this.SetDefaultSelected();//默认选中
            this.OnDrawTitleBack(e);
            Rectangle borderRc = this.ClientRectangle;
            borderRc.Width--;
            borderRc.Height--;

            //绘制底线
            //OnDrawLine(e.Graphics, ClientRectangle);
            GDIHelper.InitializeGraphics(e.Graphics);
            //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.CalcPos();
            #region 绘制TabPages
            for (int i = 0; i < this.Items.Count; i++)
            {
                SparkTabPage currentItem = this.Items[i];
                if (!currentItem.Visible && !this.DesignMode)
                {
                    currentItem.IsDrawn = false;
                    continue;
                }

                this.CalcTabPage(e.Graphics, currentItem);
                currentItem.IsDrawn = false;

                if (!this.AllowDraw(currentItem))
                    continue;

                this.OnDrawTabPage(e.Graphics, currentItem);
            }
            #endregion

            #region 绘制Menu
            if (this.AlwaysShowMenuGlyph || this.Items.DrawnCount < this.Items.VisibleCount)
                this.menuGlyph.DrawGlyph(e.Graphics);
            #endregion

            #region 绘制边框
            if (BorderStyle != BorderStyle.None && this.Theme.BorderColor != Color.Transparent) GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
            #endregion
        }

        /// <summary>
        /// 鼠标点击方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button != MouseButtons.Left)
                return;

            HitTestResult result = this.HitTest(e.Location);
            if (result == HitTestResult.MenuGlyph && (this.AlwaysShowMenuGlyph || this.Items.DrawnCount < this.Items.VisibleCount))
            {
                HandledEventArgs args = new HandledEventArgs(false);
                this.OnMenuItemsLoading(args);
                if (!args.Handled)
                    this.OnMenuItemsLoad(EventArgs.Empty);
                this.ShowMenu();
                this.Invalidate();
            }
            else if (result == HitTestResult.CloseButton)
            {
                if (this.SelectedItem != null)
                {
                    SparkTabPage item = this.GetTabPageByPoint(e.Location);
                    TabPageClosingEventArgs args = new TabPageClosingEventArgs(item);
                    this.OnTabPageClosing(args);
                    if (!args.Cancel)
                    {
                        this.RemoveTab(item);
                        this.OnTabPageClosed(EventArgs.Empty);
                    }
                    this.Invalidate();
                }
            }
            else if (result == HitTestResult.TabPage)
            {
                SparkTabPage item = this.GetTabPageByPoint(e.Location);
                if (item != null)
                    this.SelectedItem = item;
            }


        }

        /// <summary>
        /// 鼠标移动方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.mouseOverItem = null;
            if (this.menuGlyph.Bounds.Contains(e.Location))
            {
                this.menuGlyph.IsMouseOver = true;
                this.Invalidate(this.menuGlyph.Bounds);
            }
            else
            {
                if (this.menuGlyph.IsMouseOver && !this.menuOpen)
                {
                    this.menuGlyph.IsMouseOver = false;
                    this.Invalidate(this.menuGlyph.Bounds);
                }
                else
                {
                    SparkTabPage item = this.GetTabPageByPoint(e.Location);
                    if (item != null)
                    {
                        if (item.CloseButtonRect.Contains(e.Location))
                        {
                            this.closeButton.Bounds = item.CloseButtonRect;
                            this.closeButton.IsMouseOver = true;
                            //Invalidate(item.CloseButtonRect);
                        }
                        else
                        {
                            if (this.closeButton.IsMouseOver == true)
                            {
                                this.closeButton.IsMouseOver = false;
                                //Invalidate(closeButton.Bounds);
                            }
                        }
                        this.mouseOverItem = item;
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标离开方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.menuGlyph.IsMouseOver = false;

            this.closeButton.IsMouseOver = false;
            //if (mouseOverItem != null)
            //{
            //    Invalidate(new Rectangle((int)mouseOverItem.StripRect.X,
            //        (int)mouseOverItem.StripRect.Y,
            //        (int)mouseOverItem.StripRect.Width,
            //        (int)mouseOverItem.StripRect.Height));
            //}
            this.Invalidate();
            this.mouseOverItem = null;
        }

        /// <summary>
        /// Size改变方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (this.isIniting)
                return;
            this.UpdateLayout();
        }

        protected virtual SparkTabControlCollection OnCreateControlCollection()
        {
            return new SparkTabControlCollection();
        }
        #endregion;


        #region 设置样式
        private void SetDefaultStyle()
        {
            //改变样式
            var defaultTheme = new SparkTabControlTheme(this);
            this.Theme = defaultTheme;
            this.SplitLineLength = 0;
            this.SplitLineWidth = 0;
            this.TitleHeight = DEF_HEADER_HEIGHT;
            this.TabPageMargin = 5;
            this.TabPageBottomMargin = 0;
            this.TabPageSpacing = 5;
        }

        private void SetSplitBarStyle()
        {
            //改变样式
            var ellipseTheme = new SparkTabControlTheme(this);
            ellipseTheme.ForeColor = Color.Black; //Color.FromArgb(66, 66, 66)
            ellipseTheme.SelectedForeColor = Color.White;
            ellipseTheme.CloseTheme.BackColor = Color.Transparent;
            ellipseTheme.CloseTheme.ForeColor = Color.White;
            ellipseTheme.SelectedBackColor = ellipseTheme.TitleTheme.BackColor;
            ellipseTheme.SelectedBottomBorderColor = Color.Transparent;
            ellipseTheme.SelectedBorderColor = Color.Transparent;
            ellipseTheme.BackColor = ellipseTheme.TitleTheme.BackColor;
            ellipseTheme.BorderColor = Color.Transparent;
            ellipseTheme.SelectedSplitLineColor = Color.White;
            ellipseTheme.SplitLineColor = Color.Black;// Color.FromArgb(66, 66, 66);
            ellipseTheme.MouseDownBackColor = ellipseTheme.MouseOverBackColor = ellipseTheme.TitleTheme.BackColor;
            ellipseTheme.MouseDownBorderColor = ellipseTheme.MouseOverBorderColor = Color.Transparent;
            ellipseTheme.MouseDownForeColor = ellipseTheme.MouseOverForeColor = Color.White;

            this.SplitLineLength = 0;
            this.SplitLineWidth = 1.5f;
            this.TitleHeight = 41;
            this.TabPageMargin = 0;
            this.TabPageBottomMargin = 0;
            this.TabPageSpacing = 0;
            this.Theme = ellipseTheme;
        }

        private void SetEllipseStyle()
        {
            //改变样式
            var ellipseTheme = new SparkTabControlTheme(this);
            ellipseTheme.SelectedForeColor = Color.White;
            ellipseTheme.SelectedBackColor = SparkThemeConsts.SelectedBackColor;
            ellipseTheme.SelectedBottomBorderColor = Color.Transparent;
            ellipseTheme.SelectedBorderColor = Color.Transparent;
            ellipseTheme.BackColor = Color.FromArgb(252, 253, 253);
            ellipseTheme.BorderColor = Color.FromArgb(213, 213, 213);
            this.SplitLineLength = 0;
            this.SplitLineWidth = 0;
            this.TitleHeight = 41;
            this.TabPageMargin = 8;
            this.TabPageBottomMargin = 8;
            this.TabPageSpacing = 5;
            this.Theme = ellipseTheme;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 是否允许绘制当前TabPage页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool AllowDraw(SparkTabPage item)
        {
            bool result = true;
            if (this.Alignment == TabAlignment.Top || this.Alignment == TabAlignment.Bottom)
            {
                if (this.RightToLeft == RightToLeft.No)
                {
                    if (item.StripRect.Right >= this.stripButtonRect.Width)
                        result = false;
                }
                else
                {
                    if (item.StripRect.Left <= this.stripButtonRect.Left)
                        return false;
                }
            }
            else
            {
                if (item.StripRect.Bottom > this.stripButtonRect.Height)
                    result = false;
            }
            return result;
        }

        /// <summary>
        /// 设置默认选中项
        /// </summary>
        private void SetDefaultSelected()
        {
            if (this.selectedItem == null && this.Items.Count > 0)
                this.SelectedItem = this.Items[0];
        }

        private void MenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SparkTabPage clickedItem = (SparkTabPage)e.ClickedItem.Tag;
            this.SelectedItem = clickedItem;
        }

        private void MenuVisibleChanged(object sender, EventArgs e)
        {
            if (this.menu.Visible == false)
            {
                this.menuOpen = false;
            }
        }

        /// <summary>
        /// 计算左端绘制的开始位置
        /// </summary>
        private void CalcPos()
        {
            if (this.Alignment == TabAlignment.Top || this.Alignment == TabAlignment.Bottom)
            {
                this.DEF_START_POS = this.RightToLeft == RightToLeft.No ? this.leftMargin : this.stripButtonRect.Right;
            }
            else
            {
                this.DEF_START_POS = this.leftMargin;
            }
        }

        /// <summary>
        /// 计算选项卡的大小
        /// </summary>
        /// <param name="g"></param>
        /// <param name="currentItem"></param>
        private void CalcTabPage(Graphics g, SparkTabPage currentItem)
        {
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    this.CalcTabPageToTop(g, currentItem);
                    break;
                case TabAlignment.Bottom:
                    this.CalcTabPageToBottom(g, currentItem);
                    break;
                case TabAlignment.Left:
                    this.CalcTabPageToLeft(g, currentItem);
                    break;
                case TabAlignment.Right:
                    this.CalcTabPageToRight(g, currentItem);
                    break;
            }
        }

        /// <summary>
        /// 绘制TabPage
        /// </summary>
        /// <param name="g"></param>
        /// <param name="currentItem"></param>
        private void OnDrawTabPage(Graphics g, SparkTabPage currentItem)
        {
            Font currentFont = this.Font;

            bool isCurrentSelect = currentItem == this.SelectedItem && this.SelectedItem?.Selected == true;
            bool isMouseSelect = this.mouseOverItem != null && this.mouseOverItem == currentItem && this.mouseOverItem != this.SelectedItem;

            //if (isCurrentSelect)
            currentFont = new Font(this.Font, FontStyle.Bold);

            RectangleF buttonRect = currentItem.StripRect;

            Color backColor = isCurrentSelect ?
                              this.Theme.SelectedBackColor :
                              (isMouseSelect ? this.Theme.MouseOverBackColor : this.Theme.BackColor);

            Color borderColor = isCurrentSelect ?
                                this.Theme.SelectedBorderColor :
                                (isMouseSelect ? this.Theme.MouseOverBorderColor : this.Theme.BorderColor);

            Color foreColor = isCurrentSelect ?
                              this.Theme.SelectedForeColor :
                              (isMouseSelect ? this.Theme.MouseOverForeColor : this.Theme.ForeColor);

            #region 绘制背景和边框

            RoundRectangle rr = new RoundRectangle(buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height, CornerRadius);
            GDIHelper.DrawPathBorder(g, rr, borderColor);
            GDIHelper.FillRectangle(g, rr, backColor);
            //using (Pen pen = new Pen(borderColor))
            //{
            //    g.DrawRectangle(pen, buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height);
            //    g.FillRectangle(new SolidBrush(backColor), buttonRect);
            //};
            #endregion

            //绘制选中的选项卡的底线和分割线
            bool isLast = this.Items.LastVisible == currentItem;
            bool isFirst = this.Items.FirstVisible == currentItem;
            if (isCurrentSelect)
            {
                this.OnDrawBottomLine(g, buttonRect);
            }
            int currentIndex = this.Items.IndexOf(currentItem) + 1;
            var isLeftAdjacent = this.Items.NextTabPageVisible(currentIndex) == this.SelectedItem;
            //绘制分割线
            this.OnSplitLine(g, buttonRect, isCurrentSelect, isFirst, isLast, isLeftAdjacent);

            #region 绘制图标
            //选项卡的组成:空白3+16+空白3+Text+空白3+16+5
            if (currentItem.Image != null)
            {
                float x = buttonRect.X + 3;
                float y = buttonRect.Y + (buttonRect.Height - 16 > 0 ? (buttonRect.Height - 16) / 2.0F : 0);
                RectangleF imageRect = new RectangleF(x, y, 16, 16);
                OnDrawTabPageImage(g, currentItem, imageRect);
            }
            #endregion

            #region 绘制 Text
            RectangleF textRect = buttonRect;
            textRect.X += 16 + 3 + 3;
            textRect.Width -= -3 - 16 - 3 - 3 - 16 - 5;

            OnDrawTabPageText(g, currentItem, textRect, currentFont, foreColor, this.sf);
            //GDIHelper.DrawString(g, textRect, currentItem.Text, currentFont, foreColor, this.sf);
            #endregion

            #region 绘制关闭按钮
            this.OnDrawClose(g, currentItem);
            #endregion

            currentItem.IsDrawn = true;
        }

        /// <summary>
        /// 绘制TabPage的图标
        /// </summary>
        /// <param name="g"></param>
        /// <param name="currentItem"></param>
        /// <param name="imageRect"></param>
        protected virtual void OnDrawTabPageImage(Graphics g, SparkTabPage currentItem, RectangleF imageRect)
        {
            bool isSelected = currentItem == this.SelectedItem;
            var image = isSelected ? (currentItem.SelectedImage ?? currentItem.Image) : currentItem.Image;
            if (image != null) g.DrawImage(image, imageRect);
        }

        /// <summary>
        /// 绘制TabPage的Text文本描述
        /// </summary>
        /// <param name="g"></param>
        /// <param name="currentItem"></param>
        /// <param name="textRect"></param>
        /// <param name="font"></param>
        /// <param name="foreColor"></param>
        /// <param name="sf"></param>
        protected virtual void OnDrawTabPageText(Graphics g, SparkTabPage currentItem, RectangleF textRect, Font font, Color foreColor, StringFormat sf)
        {
            GDIHelper.DrawString(g, textRect, currentItem.Text, font, foreColor, sf);
        }

        private void OnDrawClose(Graphics g, SparkTabPage currentItem)
        {
            if (this._closeButtonDisplayMode != 0 && currentItem.IsDrawClose)
            {
                bool selectClose = this.SelectedItem == this.mouseOverItem;
                if (this._closeButtonDisplayMode == CloseButtonDisplayModes.FocusOrMouseEnter)
                {
                    //进去或者当前页才绘制
                    if (currentItem == this.SelectedItem)
                    {
                        this.closeButton.DrawCross(g, currentItem.CloseButtonRect, selectClose);
                    }
                    if (currentItem == this.mouseOverItem)
                    {
                        this.closeButton.DrawCross(g, this.mouseOverItem.CloseButtonRect, true);
                    }
                }
                else
                {//关闭按钮一直绘制
                    this.closeButton.DrawCross(g, currentItem.CloseButtonRect, currentItem == this.mouseOverItem);
                }
            }
        }

        /// <summary>
        /// 绘制选中TabPage的横线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="buttonRect"></param>
        private void OnDrawBottomLine(Graphics g, RectangleF buttonRect)
        {
            LineDrawDirection lineDrawDirection = LineDrawDirection.Bottom;
            if (this.SelectedLineAlignment == TabPageSelectedLineAlignment.LeftTop)
            {
                lineDrawDirection = this.Alignment.ToString().ToEnum<LineDrawDirection>();
                //switch (this.Alignment)
                //{
                //    case TabAlignment.Top:
                //        lineDrawDirection = LineDrawDirection.Top;
                //        break;
                //    case TabAlignment.Bottom:
                //        lineDrawDirection = LineDrawDirection.Bottom;
                //        break;
                //    case TabAlignment.Left:
                //        lineDrawDirection = LineDrawDirection.Left;
                //        break;
                //    case TabAlignment.Right:
                //        lineDrawDirection = LineDrawDirection.Right;
                //        break;
                //}
            }
            else
            {
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        lineDrawDirection = LineDrawDirection.Bottom;
                        break;
                    case TabAlignment.Bottom:
                        lineDrawDirection = LineDrawDirection.Top;
                        break;
                    case TabAlignment.Left:
                        lineDrawDirection = LineDrawDirection.Right;
                        break;
                    case TabAlignment.Right:
                        lineDrawDirection = LineDrawDirection.Left;
                        break;
                }
            }
            if (this.Theme.SelectedBottomBorderColor != Color.Transparent)
            {
                g.DrawLines(new Pen(this.Theme.SelectedBottomBorderColor, 3),
                      GDIHelper.GetLineByRectangle(buttonRect, lineDrawDirection));
            }
        }

        /// <summary>
        /// 绘制分割线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="buttonRect"></param>
        /// <param name="isSelected">是否选中</param>
        /// <param name="isFisrt">是否第一个</param>
        /// <param name="isLast">是否最后一个</param>
        /// <param name="isLeftAdjacent">是否和当前选项卡左边相邻</param>
        private void OnSplitLine(Graphics g, RectangleF buttonRect, bool isSelected, bool isFisrt, bool isLast, bool isLeftAdjacent)
        {
            if (isLast || SplitLineWidth <= 0) return;//最后一个或宽度小于1跳出
            float offValueX = 0;
            float offValueY = 0;
            LineDrawDirection lineDrawDirection = LineDrawDirection.Bottom;
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    lineDrawDirection = LineDrawDirection.Right;
                    offValueX = SplitLineWidth / 2;
                    break;
                case TabAlignment.Left:
                case TabAlignment.Right:
                    lineDrawDirection = LineDrawDirection.Bottom;
                    offValueY = SplitLineWidth / 2;
                    break;
            }

            PointF[] rect = null;
            if (SplitLineLength <= 0 || buttonRect.Height < SplitLineLength)
            {
                rect = GDIHelper.GetLineByRectangle(buttonRect, lineDrawDirection, 0, 0, offValueX, offValueY);
            }
            else
            {
                float value = (buttonRect.Height - SplitLineLength) / 2;
                rect = GDIHelper.GetLineByRectangle(buttonRect, lineDrawDirection, value, value, offValueX, offValueY);
            }

            g.DrawLines(new Pen(isSelected || isLeftAdjacent ? this.Theme.SelectedSplitLineColor : this.Theme.SplitLineColor, SplitLineWidth), rect);

        }

        /// <summary>
        /// 更新布局位置
        /// </summary>
        protected void UpdateLayout()
        {
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    this.UpdateLayoutToTop();
                    break;
                case TabAlignment.Bottom:
                    this.UpdateLayoutToBottom();
                    break;
                case TabAlignment.Left:
                    this.UpdateLayoutToLeft();
                    break;
                case TabAlignment.Right:
                    this.UpdateLayoutToRight();
                    break;
            }
        }

        private void CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            SparkTabPage itm = (SparkTabPage)e.Element;

            if (e.Action == CollectionChangeAction.Add)
            {
                this.Controls.Add(itm);
                itm.BringToFront();
                this.OnTabStripItemChanged(new SparkTabPageChangedEventArgs(itm, TabPageChangeAction.Added));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                this.Controls.Remove(itm);
                this.OnTabStripItemChanged(new SparkTabPageChangedEventArgs(itm, TabPageChangeAction.Removed));
            }
            //else
            //{
            //    this.OnTabStripItemChanged(new SparkTabPageChangedEventArgs(itm, TabPageChangeAction.Changed));
            //}
            this.UpdateLayout();
            this.Invalidate();
        }

        /// <summary>
        /// 绘制标题栏
        /// </summary>
        /// <param name="e"></param>
        private void OnDrawTitleBack(PaintEventArgs e)
        {
            if (this.Alignment == TabAlignment.Top)
            {
                if (this.IsDrawTitle)
                {
                    this.titleBarDraw?.Draw(e);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.BackColor),
                  new RectangleF(0, 0, this.ClientRectangle.Width, this.TitleHeight));
                }
            }
            else if (this.Alignment == TabAlignment.Bottom)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor),
                    new RectangleF(0, this.stripButtonRect.Y, this.ClientRectangle.Width, this.TitleHeight));
            }
            else if (this.Alignment == TabAlignment.Left)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor),
                    new RectangleF(0, 0, this.tabPageWidth, this.ClientRectangle.Height));
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor),
                    new RectangleF(this.ClientRectangle.Width - this.tabPageWidth, 0, this.tabPageWidth, this.ClientRectangle.Height));
            }
        }
        #endregion
        #endregion

        #region 事件
        private bool TitleBarDraw_TitleHitTest(object sender, MouseEventArgs e)
        {
            return this.HitTest(e.Location) != HitTestResult.None;
        }
        #endregion

        #region 计算布局位置
        private void UpdateLayoutToLeft()
        {
            this.sf.Trimming = StringTrimming.EllipsisCharacter;
            this.sf.FormatFlags |= StringFormatFlags.NoWrap;
            this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
            this.sf.LineAlignment = StringAlignment.Center;
            this.sf.Alignment = StringAlignment.Near;
            //左右布局，必须设置TagPageWidth,如果=0，默认200
            if (this.TabPageWidth <= 0) this.TabPageWidth = 200;
            if (this.TabPageMargin >= this.TabPageWidth) this.TabPageMargin = this.TabPageWidth / 2;
            int width = this.TabPageWidth - this.TabPageMargin > 0 ? this.TabPageWidth - this.TabPageMargin : this.TabPageWidth;
            int height = this.ClientSize.Height - 16 - 2 > 0 ? this.ClientSize.Height - 16 - 2 : this.ClientSize.Height;

            this.stripButtonRect = new Rectangle(this.TabPageMargin, 0, width, height);

            int menuY = this.stripButtonRect.Width >= 16 ? (this.stripButtonRect.Width - 16) / 2 : 0;
            this.menuGlyph.Bounds = new Rectangle(menuY, this.stripButtonRect.Height + 2, 16, 16);

            this.DockPadding.Top = 1;
            this.DockPadding.Bottom = 1;
            this.DockPadding.Right = 1;
            this.DockPadding.Left = this.TabPageWidth;
        }

        private void UpdateLayoutToRight()
        {
            this.sf.Trimming = StringTrimming.EllipsisCharacter;
            this.sf.FormatFlags |= StringFormatFlags.NoWrap;
            this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
            this.sf.LineAlignment = StringAlignment.Center;
            this.sf.Alignment = StringAlignment.Near;
            if (this.TabPageWidth <= 0) this.TabPageWidth = 200;
            int width = this.TabPageWidth;
            int x = this.ClientSize.Width - width - this.TabPageMargin > 0 ? this.ClientSize.Width - width - this.TabPageMargin : this.ClientSize.Width;
            int height = this.ClientSize.Height - 16 - 2 > 0 ? this.ClientSize.Height - 16 - 2 : 0;
            this.stripButtonRect = new Rectangle(x, 0, width, height);

            int menuX = width - 16 >= 0 ? (width - 16) / 2 : 0;
            this.menuGlyph.Bounds = new Rectangle(x + menuX, this.stripButtonRect.Height + 2, 16, 16);

            this.DockPadding.Top = 1;
            this.DockPadding.Bottom = 1;
            this.DockPadding.Right = this.TabPageWidth;
            this.DockPadding.Left = 1;
        }

        private void UpdateLayoutToTop()
        {
            if (this.RightToLeft == RightToLeft.No)
            {
                //LeftToRight
                this.sf.Trimming = StringTrimming.EllipsisCharacter;
                this.sf.FormatFlags |= StringFormatFlags.NoWrap;
                this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
                this.sf.LineAlignment = StringAlignment.Center;
                this.sf.Alignment = StringAlignment.Near;
                //区域大小=总体 - TitleButtonSum - 菜单图标 -2
                this.stripButtonRect = new Rectangle(0, 0, this.ClientSize.Width - this.titleBarDraw.ItemWidthSum - 16 - 2, this.TabPageHeight);
                int menuY = this.TabPageHeight >= 16 ? (this.TabPageHeight - 16) / 2 + this.TabPageMargin : 0;
                this.menuGlyph.Bounds = new Rectangle(this.stripButtonRect.Width, menuY, 16, 16);
            }
            else
            {
                //RightToLeft
                this.sf.Trimming = StringTrimming.EllipsisCharacter;
                this.sf.FormatFlags |= StringFormatFlags.NoWrap;
                this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
                this.sf.LineAlignment = StringAlignment.Center;
                this.sf.Alignment = StringAlignment.Near;
                //区域大小=总体 - TitleButtonSum - 菜单图标 -2
                this.stripButtonRect = new Rectangle(16 + 2, 0, this.ClientSize.Width - this.titleBarDraw.ItemWidthSum - 16 - 2, this.TabPageHeight);
                int menuY = this.TabPageHeight >= 16 ? (this.TabPageHeight - 16) / 2 + this.TabPageMargin : 0;
                this.menuGlyph.Bounds = new Rectangle(2, menuY, 16, 16);
            }
            this.DockPadding.Top = this.TitleHeight;
            this.DockPadding.Bottom = 1;
            this.DockPadding.Right = 1;
            this.DockPadding.Left = 1;
        }

        private void UpdateLayoutToBottom()
        {
            if (this.RightToLeft == RightToLeft.No)
            {
                //LeftToRight
                this.sf.Trimming = StringTrimming.EllipsisCharacter;
                this.sf.FormatFlags |= StringFormatFlags.NoWrap;
                this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
                this.sf.LineAlignment = StringAlignment.Center;
                this.sf.Alignment = StringAlignment.Near;
                this.stripButtonRect = new Rectangle(0, this.ClientSize.Height - this.TitleHeight, this.ClientSize.Width - 16 - 2, this.TabPageHeight);
                int menuY = this.TabPageHeight >= 16 ? (this.TabPageHeight - 16) / 2 + this.TabPageMargin : 0;
                this.menuGlyph.Bounds = new Rectangle(this.stripButtonRect.Width, this.stripButtonRect.Y + menuY, 16, 16);
            }
            else
            {
                //RightToLeft
                this.sf.Trimming = StringTrimming.EllipsisCharacter;
                this.sf.FormatFlags |= StringFormatFlags.NoWrap;
                this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
                this.sf.LineAlignment = StringAlignment.Center;
                this.sf.Alignment = StringAlignment.Near;
                this.stripButtonRect = new Rectangle(16 + 2, this.ClientSize.Height - this.TitleHeight, this.ClientSize.Width - 16 - 2 - 10, this.TabPageHeight);
                int menuY = this.TabPageHeight >= 16 ? (this.TabPageHeight - 16) / 2 + this.TabPageMargin : 0;
                this.menuGlyph.Bounds = new Rectangle(2, this.stripButtonRect.Y + menuY, 16, 16);
            }
            this.DockPadding.Top = 1;
            this.DockPadding.Bottom = this.TitleHeight;
            this.DockPadding.Right = 1;
            this.DockPadding.Left = 1;
        }
        #endregion

        #region 计算TabPage大小
        /// <summary>
        /// 判断是否绘制
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsCanDraw(SparkTabPage item)
        {
            int index = this.Items.IndexOf(item);
            if (index > 0)
            {
                float width = 0;
                SparkTabPage last = this.Items.Cast<SparkTabPage>().Take(index).Where(a => a.Visible).LastOrDefault();
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                        using (Graphics g = this.CreateGraphics())
                        {
                            using (Font font = new Font(this.Font, FontStyle.Bold))
                            {
                                width = this.GetMaxTabPageWidth(g, item.Text, font);
                            }
                        }
                        if (last != null && last.StripRect.Right + width <= this.stripButtonRect.Width)
                        {
                            return true;
                        }
                        break;
                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        width = this.TabPageHeight;
                        if (last != null && last.StripRect.Bottom + width <= this.stripButtonRect.Height)
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }
            else if (index == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float GetMaxTabPageWidth(Graphics g, string text, Font font)
        {
            float width;
            if (this.AutoSize)
            {
                SizeF sizeF = GDIHelper.MeasureString(g, text, font, this.sf);
                if (this.TabPageMaxWidth == 0)
                {
                    width = Math.Max(sizeF.Width, this.TabPageMinWidth);
                }
                else
                {
                    width = Math.Min(sizeF.Width, this.TabPageMaxWidth);
                }
            }
            else
            {
                width = Math.Min(this.TabPageWidth, this.TabPageMaxWidth);
                width = Math.Max(width, this.TabPageMinWidth);
            }
            width += 3 + 16 + 3 + 16 + 3 + 5; //选项卡的组成:空白3+16+空白3+Text+空白3+16+5
            return width;
        }

        private void CalcTabPageToTop(Graphics g, SparkTabPage currentItem)
        {
            //要根据TabPageWidth，TabPageMaxWidth，AutoSize三个计算宽度
            //还要根据Aliment布局方式计算宽度
            Font currentFont = this.Font;
            int bottomLineHeight = 0;
            int topLineHeight = 0;
            if (currentItem == this.SelectedItem)
            {
                currentFont = new Font(this.Font, FontStyle.Bold);
                if (this.Theme.SelectedBottomBorderColor != Color.Transparent)
                {
                    if (this.SelectedLineAlignment == TabPageSelectedLineAlignment.LeftTop)
                    {
                        bottomLineHeight = 1;
                        topLineHeight = 1;
                    }
                    else
                    {
                        bottomLineHeight = 1;
                        topLineHeight = 0;
                    }
                }
            }

            float width = this.GetMaxTabPageWidth(g, currentItem.Text, currentFont);



            if (this.RightToLeft == RightToLeft.No)
            {
                RectangleF buttonRect = new RectangleF(this.DEF_START_POS, this.TabPageMargin + topLineHeight, width, this.TabPageHeight - bottomLineHeight);
                currentItem.StripRect = buttonRect;
                this.DEF_START_POS += width + this.TabPageSpacing + SplitLineWidth; //+间距
            }
            else
            {
                RectangleF buttonRect = new RectangleF(this.DEF_START_POS - width, this.TabPageMargin + topLineHeight, width, this.TabPageHeight - bottomLineHeight);
                currentItem.StripRect = buttonRect;
                this.DEF_START_POS = this.DEF_START_POS - width - this.TabPageSpacing - SplitLineWidth;
            }
        }

        private void CalcTabPageToBottom(Graphics g, SparkTabPage currentItem)
        {
            //要根据TabPageWidth，TabPageMaxWidth，AutoSize三个计算宽度
            //还要根据Aliment布局方式计算宽度
            Font currentFont = this.Font;
            if (currentItem == this.SelectedItem)
                currentFont = new Font(this.Font, FontStyle.Bold);

            float width = this.GetMaxTabPageWidth(g, currentItem.Text, currentFont);

            int bottomLineHeight = 0;
            int topLineHeight = 0;
            if (this.Theme.SelectedBottomBorderColor != Color.Transparent)
            {
                if (this.SelectedLineAlignment == TabPageSelectedLineAlignment.LeftTop)
                {
                    bottomLineHeight = 1;
                    topLineHeight = 1;
                }
                else
                {
                    bottomLineHeight = 1;
                    topLineHeight = 0;
                }
            }

            if (this.RightToLeft == RightToLeft.No)
            {
                RectangleF buttonRect = new RectangleF(this.DEF_START_POS, this.ClientRectangle.Height - this.TabPageMargin - this.TabPageHeight + topLineHeight, width, this.TabPageHeight - bottomLineHeight);
                currentItem.StripRect = buttonRect;
                this.DEF_START_POS += (int)width + this.TabPageSpacing; //+间距
            }
            else
            {
                RectangleF buttonRect = new RectangleF(this.DEF_START_POS - width, this.ClientRectangle.Height - this.TabPageMargin - this.TabPageHeight + topLineHeight, width, this.TabPageHeight - bottomLineHeight);
                currentItem.StripRect = buttonRect;
                this.DEF_START_POS -= (int)width - this.TabPageSpacing;
            }
        }

        private void CalcTabPageToLeft(Graphics g, SparkTabPage currentItem)
        {
            //要根据TabPageWidth，TabPageMaxWidth，AutoSize三个计算宽度
            //还要根据Aliment布局方式计算宽度
            //Font currentFont = this.Font;
            //if (currentItem == this.SelectedItem)
            //	currentFont = new Font(this.Font, FontStyle.Bold);
            //var width = GetMaxTabPageWidth(g, currentItem.Text, currentFont);

            int rightLineHeight = 0;
            int leftLineHeight = 0;
            if (this.Theme.SelectedBottomBorderColor != Color.Transparent)
            {
                if (this.SelectedLineAlignment == TabPageSelectedLineAlignment.LeftTop)
                {
                    leftLineHeight = 1;
                    rightLineHeight = 1;
                }
                else
                {
                    leftLineHeight = 0;
                    rightLineHeight = 1;
                }
            }

            RectangleF buttonRect = new RectangleF(this.TabPageMargin, this.DEF_START_POS + leftLineHeight, this.TabPageWidth - this.TabPageMargin - rightLineHeight, this.TabPageHeight);
            currentItem.StripRect = buttonRect;
            this.DEF_START_POS += this.TabPageHeight + this.TabPageSpacing;
        }

        private void CalcTabPageToRight(Graphics g, SparkTabPage currentItem)
        {
            //要根据TabPageWidth，TabPageMaxWidth，AutoSize三个计算宽度
            //还要根据Aliment布局方式计算宽度
            //Font currentFont = this.Font;
            //if (currentItem == this.SelectedItem)
            //	currentFont = new Font(this.Font, FontStyle.Bold);
            //var width = GetMaxTabPageWidth(g, currentItem.Text, currentFont);


            int rightLineHeight = 0;
            int leftLineHeight = 0;
            if (this.Theme.SelectedBottomBorderColor != Color.Transparent)
            {
                if (this.SelectedLineAlignment == TabPageSelectedLineAlignment.LeftTop)
                {
                    leftLineHeight = 1;
                    rightLineHeight = 1;
                }
                else
                {
                    leftLineHeight = 0;
                    rightLineHeight = 1;
                }
            }

            RectangleF buttonRect = new RectangleF(this.ClientRectangle.Width - this.TabPageWidth, this.DEF_START_POS + leftLineHeight
                                    , this.TabPageWidth - this.tabPageTopMargin - rightLineHeight, this.TabPageHeight);
            currentItem.StripRect = buttonRect;
            this.DEF_START_POS += this.TabPageHeight + this.TabPageSpacing;
        }

        protected virtual CornerRadius GetCornerRadius()
        {
            if (this.Style == SparkTabControlStyle.Ellipse)
            {
                return new CornerRadius(13.5f);
            }
            else if (this.Style == SparkTabControlStyle.SplitBar)
            {
                return new CornerRadius(0);
            }
            else
            {
                return new CornerRadius(0);
                //switch (this.Alignment)
                //{
                //    case TabAlignment.Top:
                //        return new CornerRadius(5, 5, 0, 0);
                //    case TabAlignment.Bottom:
                //        return new CornerRadius(0, 0, 5, 5);
                //    case TabAlignment.Left:
                //        return new CornerRadius(5, 0, 5, 0);
                //    case TabAlignment.Right:
                //        return new CornerRadius(0, 5, 0, 5);
                //}
            }
        }
        #endregion

        #region 接口
        #region ISupportInitialize接口
        /// <summary>
        /// 开始初始化
        /// </summary>
        public void BeginInit()
        {
            this.isIniting = true;
        }

        /// <summary>
        /// 结束初始化
        /// </summary>
        public virtual void EndInit()
        {
            this.isIniting = false;
            this.UpdateLayout();
        }
        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkTabControlTheme Theme { get; private set; }

        #endregion

        #region IDisposable接口
        /// <summary>
        /// 释放方法
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Items.CollectionChanged -= new CollectionChangeEventHandler(this.CollectionChanged);
                this.menu.ItemClicked -= new ToolStripItemClickedEventHandler(this.MenuItemClicked);
                this.menu.VisibleChanged -= new EventHandler(this.MenuVisibleChanged);

                foreach (SparkTabPage item in this.Items)
                {
                    if (item != null && !item.IsDisposed)
                        item.Dispose();
                }

                if (this.menu != null && !this.menu.IsDisposed)
                    this.menu.Dispose();

                if (this.sf != null)
                    this.sf.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// TabControl样式枚举
    /// </summary>
    public enum SparkTabControlStyle
    {
        /// <summary>
        /// 默认样式（矩形）
        /// </summary>
        Default,
        /// <summary>
        /// 椭圆风格
        /// </summary>
        Ellipse,
        /// <summary>
        /// 分隔栏风格
        /// </summary>
        SplitBar
    }

    /// <summary>
    /// TabControl图标绘制位置枚举
    /// </summary>
    public enum TabImageAlignment
    {
        /// <summary>
        /// 左端中间
        /// </summary>
        Left = 0,
        /// <summary>
        /// 上端中间
        /// </summary>
        Top = 1,
        /// <summary>
        /// 右端终端
        /// </summary>
        //Right = 2,
        /// <summary>
        /// 下端中间
        /// </summary>
        //Bottom = 3

    }


    public enum TabPageSelectedLineAlignment
    {
        /// <summary>
        /// 上下时，在上方；左右时，在左方
        /// </summary>
        LeftTop = 0,
        /// <summary>
        /// 上下时，在下方；左右时，在右方
        /// </summary>
        RightBottom = 1,
    }
}