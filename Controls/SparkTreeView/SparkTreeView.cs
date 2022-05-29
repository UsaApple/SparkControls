using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using SparkControls.Foundation;
using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Controls
{
    /// <summary>
    /// 树控件。
    /// </summary>
    [ToolboxBitmap(typeof(TreeView))]
    [ToolboxItem(true)]
    public partial class SparkTreeView : TreeView, ISupportInitialize, ISparkTheme<SparkTreeViewTheme>
    {
        #region 私有变量

        private readonly CornerRadius mCornerRadius = new CornerRadius(0, 0, 0, 0);

        /// <summary>
        /// 是否选择了Check区域
        /// </summary>
        private bool isSelectCheckRang = false;
        private TreeNodeStates _treeNodeStates = TreeNodeStates.Default;
        // 内容边距
        private readonly int mMargin = 4;
        // 初始化标志
        private bool isIniting = false;

        /// <summary>
        /// The is show by custom model
        /// </summary>
        private bool _isShowByCustomModel = true;

        /// <summary>
        /// The node height
        /// </summary>
        private int _nodeHeight = 20;

        /// <summary>
        /// The node is show split line
        /// </summary>
        private readonly bool _nodeIsShowSplitLine = false;

        /// <summary>
        /// The parent node can select
        /// </summary>
        private bool _parentNodeCanSelect = true;

        /// <summary>
        /// The tree font size
        /// </summary>
        private SizeF treeFontSize = SizeF.Empty;

        /// <summary>
        /// The BLN has v bar
        /// </summary>
        private bool blnHasVBar = false;

        private Size mBoxSize = Consts.CHECK_BOX_SIZE;

        private Dictionary<TreeNode, CheckState> _nodeStateDict = new Dictionary<TreeNode, CheckState>();

        /// <summary>
        /// The node down pic
        /// </summary>
        private Image _nodeDownPic = null;

        /// <summary>
        /// The node up pic
        /// </summary>
        private Image _nodeUpPic = null;

        private StringFormat stringFormat = null;

        /// <summary>
        /// 鼠标点击的时间间隔
        /// </summary>
        private int _mouseUp_TickCount = 0;
        #endregion

        #region 属性
        private Font defaultFont = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get { return this.defaultFont; }
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
        /// Gets or sets a value indicating whether this instance is show by custom model.
        /// </summary>
        /// <value><c>true</c> if this instance is show by custom model; otherwise, <c>false</c>.</value>
        [Category("Spark"), Description("使用自定义模式")]
        [Browsable(false)]
        [DefaultValue(true)]
        public bool IsShowByCustomModel
        {
            get => this._isShowByCustomModel;
            set
            {
                if (this._isShowByCustomModel != value)
                {
                    this._isShowByCustomModel = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the node.
        /// </summary>
        /// <value>The height of the node.</value>
        [Category("Spark"), Description("节点高度")]
        [DefaultValue(20)]
        public new int ItemHeight
        {
            get
            {
                return this._nodeHeight;
            }
            set
            {
                if (this._nodeHeight != value)
                {
                    this._nodeHeight = value;
                    base.ItemHeight = value;
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [parent node can select].
        /// </summary>
        /// <value><c>true</c> if [parent node can select]; otherwise, <c>false</c>.</value>
        [Category("Spark"), Description("父节点是否可选中")]
        [Browsable(false)]
        [DefaultValue(true)]
        public bool ParentNodeCanSelect
        {
            get
            {
                return this._parentNodeCanSelect;
            }
            set
            {
                this._parentNodeCanSelect = value;
            }
        }

        /// <summary>
        /// 获取或设置复选框的大小。
        /// </summary>
        [Category("Spark"), Description("复选框的大小。")]
        [DefaultValue(typeof(Size), "14,14")]
        [Browsable(false)]
        public Size BoxSize
        {
            get { return this.mBoxSize; }
            set
            {
                if (this.mBoxSize != value)
                {
                    this.mBoxSize = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示应用于控件的绘制模式。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TreeViewDrawMode DrawMode => TreeViewDrawMode.OwnerDrawAll;

        /// <summary>
        /// 获取或设置一个值，该值指示双击节点时触发的动作。
        /// </summary>
        [DefaultValue(TreeViewDoubleClickAction.Expand)]
        [Category("Spark"), Description("双击节点时触发的动作。")]
        public TreeViewDoubleClickAction DoubleClickAction { get; set; } = TreeViewDoubleClickAction.Expand;

        /// <summary>
        /// 获取或设置一个值，该值指示展开全部节点后滚动条是否定位到头部。
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("展开全部节点后滚动条是否定位到头部。")]
        public bool ExpandAllToHeader { get; set; } = true;
        #endregion

        #region 事件定义
        /// <summary>
        /// 
        /// </summary>
        [
            Browsable(true),
            Category("Property Changed"),
            Description("TreeNode的CheckBox的状态修改事件")
        ]
        public event EventHandler<CheckBoxStateChangedEventArgs> CheckBoxStateChanged;
        #endregion

        #region 构造方法
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkTreeView" /> class.
        /// </summary>
        public SparkTreeView()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw, true
            );
            base.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Font = this.defaultFont;
            this.Theme = new SparkTreeViewTheme(this);
            base.HideSelection = false;
            base.FullRowSelect = true;
            this.HotTracking = true;
            base.ShowLines = false;
            base.ShowPlusMinus = false;
            base.ShowRootLines = false;
            base.HotTracking = true;
            this.stringFormat = new StringFormat()
            {
                Trimming = StringTrimming.None,
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap,
            };

            this.InitArrowImage();
        }

        private IContainer components;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SparkTreeView));
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
        #endregion

        #region 事件重写
        /// <summary>
        /// 绘制事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.isIniting)
            {
                if (this.GetStyle(ControlStyles.UserPaint))
                {
                    Message m = new Message
                    {
                        HWnd = Handle,
                        Msg = (int)Msgs.WM_PRINTCLIENT,
                        WParam = e.Graphics.GetHdc(),
                        LParam = (IntPtr)Win32Consts.PRF_CLIENT
                    };
                    this.DefWndProc(ref m);
                    e.Graphics.ReleaseHdc(m.WParam);
                }
                if (this.BorderStyle != BorderStyle.None)
                {
                    GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
                }
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateExtendedStyles();
            //if (!Comm.IsWinXP)
            NativeMethods.SendMessage(Handle, Win32Consts.TVM_SETBKCOLOR, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(this.BackColor));
        }

        /// <summary>
        /// 重写 <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />。
        /// </summary>
        /// <param name="m">要处理的 Windows<see cref="T:System.Windows.Forms.Message" />。</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Msgs.WM_ERASEBKGND) // 禁掉清除背景消息WM_ERASEBKGND
            {
                base.WndProc(ref m);
            }
            else if (m.Msg == (int)Msgs.WM_LBUTTONDBLCLK) //&& this.CheckBoxes  0x203
            {
                try
                {
                    //WM_LBUTTONDBLCLK  
                    //wParam:
                    //MK_CONTROL  0x0008  键盘CTRL键是按下状态
                    //MK_LBUTTON  0x0001  鼠标左键是按下状态
                    //MK_MBUTTON  0x0010  鼠标中键是按下状态
                    //MK_RBUTTON  0x0002  鼠标右键是按下状态
                    //MK_SHIFT    0x0004  键盘SHIFT键是按下状态
                    //MK_XBUTTON1 0x0020  鼠标第一个X按钮是按下状态
                    //MK_XBUTTON2 0x0040  鼠标第二个X按钮是按下状态
                    //lParam
                    //低16位标识光标的横坐标。这个坐标是相对客户区左上角而言。
                    //高16位标识光标的纵坐标。这个坐标是相对客户区左上角而言。
                    Point point = new Point(m.LParam.ToInt32());
                    TreeViewHitTestInfo item = this.HitTest(point);
                    if (item != null && item.Node != null)
                    {
                        void Check(TreeNode tn)
                        {
                            if (this.CheckBoxes) this.ToggleTreeNodeState(tn, !tn.Checked);
                        };
                        void ExpandOrCollapse(TreeNode tn)
                        {
                            if (tn == null) return;
                            if (tn.Nodes.Count > 0)
                            {
                                if (tn.IsExpanded)
                                {
                                    tn.Collapse();
                                }
                                else
                                {
                                    tn.Expand();
                                }
                            }
                        };
                        switch (this.DoubleClickAction)
                        {
                            case TreeViewDoubleClickAction.Check:
                                Check(this.SelectedNode);
                                break;
                            case TreeViewDoubleClickAction.Expand:
                                ExpandOrCollapse(this.SelectedNode);
                                break;
                            case TreeViewDoubleClickAction.Both:
                                Check(this.SelectedNode);
                                ExpandOrCollapse(this.SelectedNode);
                                break;
                        }
                        this.OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(item.Node, MouseButtons.Left, 2, point.X, point.Y));
                    }
                    this.OnDoubleClick(EventArgs.Empty);
                    //m.Result = IntPtr.Zero;
                }
                catch (Exception e)
                {
                    Comm.Logger.WriteErr(e.ToString());
                    SparkMessageBox.ShowErrorMessage(e.Message);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Handles the AfterSelect event of the TreeViewEx control.
        /// </summary>
        /// <param name="e">The <see cref="TreeViewEventArgs" /> instance containing the event data.</param>
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    if (!this._parentNodeCanSelect)
                    {
                        if (e.Node.Nodes.Count > 0)
                        {
                            e.Node.Expand();
                            base.SelectedNode = e.Node.Nodes[0];
                        }
                    }
                }
                base.OnAfterSelect(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the TreeViewEx control.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Refresh();
        }

        /// <summary>
        /// Handles the NodeMouseClick event of the TreeViewEx control.
        /// </summary>
        /// <param name="e">The <see cref="TreeNodeMouseClickEventArgs" /> instance containing the event data.</param>
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            try
            {

                if (e.Node != null)
                {
                    if (e.Node.Nodes.Count > 0)
                    {
                        Rectangle rect = e.Node.Bounds;
                        rect.X = base.Width - (this.blnHasVBar ? 50 : 30) - 5;
                        rect.Width = Math.Max(rect.Height, 30);
                        //图片的按钮位置
                        //new Rectangle(base.Width - (this.blnHasVBar ? 50 : 30), e.Bounds.Y + (e.Bounds.Height - 20) / 2, 20, 20)

                        if (rect.Contains(e.Location))
                        {
                            if (e.Node.IsExpanded)
                            {
                                e.Node.Collapse();
                            }
                            else
                            {
                                e.Node.Expand();
                            }
                        }
                    }
                    if (base.SelectedNode != null)
                    {
                        if (base.SelectedNode == e.Node && e.Node.IsExpanded)
                        {
                            if (!this._parentNodeCanSelect)
                            {
                                if (e.Node.Nodes.Count > 0)
                                {
                                    base.SelectedNode = e.Node.Nodes[0];
                                }
                            }
                        }
                    }
                    base.OnNodeMouseClick(e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Handles the DrawNode event of the treeview control.
        /// </summary>
        /// <param name="e">The <see cref="DrawTreeNodeEventArgs" /> instance containing the event data.</param>
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (this.isIniting) return;
            try
            {
                if (e.Node == null || !this._isShowByCustomModel || (e.Node.Bounds.Width <= 0 && e.Node.Bounds.Height <= 0 && e.Node.Bounds.X <= 0 && e.Node.Bounds.Y <= 0))
                {
                    e.DrawDefault = true;
                    return;
                }
                if (this.Disposing || this.IsDisposed) return;
                //Console.WriteLine($"Draw:{e.Node.Text} Index:{e.Node.Index}");
                //if (!e.Node.IsVisible) { return; }
                if (e.Node.Bounds.IsEmpty) { return; }
                Graphics g = e.Graphics;
                GDIHelper.InitializeGraphics(g);
                this.blnHasVBar = this.IsVerticalScrollBarVisible();
                //Console.WriteLine(this.blnHasVBar);
                Font font = e.Node.NodeFont;
                if (font == null)
                {
                    font = this.Font;
                }
                //if (this.treeFontSize == SizeF.Empty)
                //{
                //    this.treeFontSize = GDIHelper.MeasureString(g, "A", font);
                //}
                int intLeft = 0;
                if (this.CheckBoxes)
                {
                    intLeft = 20;
                }
                int num = 0;
                Image treeNodeImage = this.GetTreeNodeImage(e.Node);
                if (treeNodeImage != null)
                {
                    num = (e.Bounds.Height - base.ImageList.ImageSize.Height) / 2;
                    intLeft += base.ImageList.ImageSize.Width;
                }
                intLeft += e.Node.Level * this.Indent;

                Color backColor;
                Color foreColor;
                if (e.Node.IsSelected)
                {
                    this._treeNodeStates = TreeNodeStates.Selected;
                    //选中，或者获取焦点
                    backColor = this.Theme.SelectedBackColor;
                    foreColor = this.Theme.SelectedForeColor;
                }
                else if (e.State.HasFlag(TreeNodeStates.Hot))
                {
                    this._treeNodeStates = TreeNodeStates.Hot;
                    //鼠标在节点上
                    backColor = this.Theme.MouseOverBackColor;
                    foreColor = this.Theme.MouseOverForeColor;
                }
                else
                {
                    this._treeNodeStates = TreeNodeStates.Default;

                    backColor = e.Node.BackColor.IsEmpty ? this.Theme.BackColor : e.Node.BackColor;
                    foreColor = e.Node.ForeColor.IsEmpty ? this.Theme.ForeColor : e.Node.ForeColor;
                }
                e.Graphics.FillRectangle(new SolidBrush(backColor), new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(base.Width, e.Node.Bounds.Height)));

                //g.DrawString(e.Node.Text, font, new SolidBrush(this.Theme.ForeColor), (float)e.Bounds.X + intLeft + mMargin, (float)e.Bounds.Y + ((float)this._nodeHeight - this.treeFontSize.Height) / 2f);
                RectangleF stringRect = new RectangleF
                {
                    X = e.Bounds.X + intLeft + this.mMargin,
                    Y = e.Bounds.Y,
                    Width = e.Bounds.Width - intLeft - this.mMargin - 30,// (this.blnHasVBar ? 50 : 30);
                    Height = e.Bounds.Height
                };
                GDIHelper.DrawString(g, stringRect, e.Node.Text, font, foreColor, this.stringFormat);

                //Image,CheckBox=20,Indent,mMargin,Text,Arrow=20

                //有复选框的情况
                if (this.CheckBoxes)
                {
                    //绘制复选框
                    Rectangle rectCheck = new Rectangle(e.Bounds.X + 3 + e.Node.Level * this.Indent, e.Bounds.Y + (e.Bounds.Height - 16) / 2, 16, 16);
                    this.DrawCheck(g, rectCheck, e.Node);
                }
                if (treeNodeImage != null)
                {//Draw Image
                    int num2 = e.Bounds.X + intLeft - base.ImageList.ImageSize.Width + 3;
                    //if (num2 < 0)
                    //{
                    //    num2 = 3;
                    //}
                    Rectangle imageRect = new Rectangle(new Point(num2, e.Bounds.Y + num),
                                                  base.ImageList.ImageSize);
                    e.Graphics.DrawImage(treeNodeImage, imageRect);
                }
                if (this._nodeIsShowSplitLine)
                {//Draw Line
                    e.Graphics.DrawLine(new Pen(this.Theme.NodeSplitLineColor, 1f), new Point(0, e.Bounds.Y + e.Bounds.Height - 1), new Point(base.Width, e.Bounds.Y + e.Bounds.Height - 1));
                }
                if (e.Node.Nodes.Count > 0)
                {//Draw Arrow
                    if (e.Node.IsExpanded && this._nodeUpPic != null)
                    {
                        Rectangle rect = new Rectangle(base.Width - (this.blnHasVBar ? 50 : 30), e.Bounds.Y + (e.Bounds.Height - 20) / 2, 20, 20);
                        g.DrawImage(this._nodeUpPic, rect);
                        //GDIHelper.DrawArrow(g, ArrowDirection.Up, rect, new Size(16, 16), 8, this.Theme.BackColor);
                    }
                    else if (this._nodeDownPic != null)
                    {
                        Rectangle rect = new Rectangle(base.Width - (this.blnHasVBar ? 50 : 30), e.Bounds.Y + (e.Bounds.Height - 20) / 2, 20, 20);
                        Console.WriteLine($"Draw Arrow:{e.Node.Text} Rect:{rect}");
                        g.DrawImage(this._nodeDownPic, rect);
                        //GDIHelper.DrawArrow(g, ArrowDirection.Down, rect, new Size(16, 16), 8, this.Theme.BackColor);
                    }
                }



                //if (this._isShowTip && this._lstTips.ContainsKey(e.Node.Name) && !string.IsNullOrWhiteSpace(this._lstTips[e.Node.Name]))
                //{
                //    int num3 = base.Width - (this.blnHasVBar ? 50 : 30) - (flagArrow ? 50 : 0);
                //    int num4 = e.Bounds.Y + (this._nodeHeight - 20) / 2;
                //    e.Graphics.DrawImage(this._tipImage, new Rectangle(num3, num4, 20, 20));
                //    SizeF sizeF = GDIHelper.MeasureString(g, this._lstTips[e.Node.Name], this._tipFont, StringFormat.GenericTypographic);
                //    e.Graphics.DrawString(this._lstTips[e.Node.Name], this._tipFont, new SolidBrush(Color.White), (float)(num3 + 10) - sizeF.Width / 2f - 3f, (float)(num4 + 10) - sizeF.Height / 2f);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (this.blnHasVBar == false && e.Button == MouseButtons.None)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// 鼠标释放事件重写
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.CheckBoxes)
            {
                TreeViewHitTestInfo test = this.HitTest(e.X, e.Y);
                if (test != null && test.Node != null && test.Location == TreeViewHitTestLocations.StateImage)
                {
                    //if (Environment.TickCount - _mouseUp_TickCount < 300) return;
                    this._mouseUp_TickCount = Environment.TickCount;
                    this.isSelectCheckRang = true;
                    this.ToggleTreeNodeState(test.Node);
                    this.isSelectCheckRang = false;
                }
            }
        }

        /// <summary>
        /// 单选框选择后
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            if (this.isSelectCheckRang || e.Action == TreeViewAction.Unknown || e.Action == TreeViewAction.ByKeyboard)
            {
                base.OnAfterCheck(e);
            }
        }

        /// <summary>
        /// Node鼠标双击事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseDoubleClick(e);
        }

        /// <summary>
        /// 键盘释放事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Space && this.CheckBoxes)
            {
                if ((this.SelectedNode != null))
                {
                    this.ToggleTreeNodeState(this.SelectedNode);
                }
            }
        }

        ///// <summary>
        ///// 展开前事件
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        //{
        //	base.OnBeforeExpand(e);
        //	//this.BeginUpdate();
        //}

        ///// <summary>
        ///// 展开后事件
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnAfterExpand(TreeViewEventArgs e)
        //{
        //	base.OnAfterExpand(e);
        //	//this.EndUpdate();
        //}

        /// <summary>
        /// 展开所有树节点
        /// </summary>
        public new void ExpandAll()
        {
            base.ExpandAll();
            if (this.ExpandAllToHeader)
            {
                //this.BeginUpdate();
                if (this.Nodes.Count > 0)
                {
                    this.Nodes[0].EnsureVisible();
                }
                //this.EndUpdate();
            }
        }

        ///// <summary>
        ///// 收缩前事件
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        //{
        //	base.OnBeforeCollapse(e);
        //	//this.BeginUpdate();
        //}

        ///// <summary>
        ///// 收缩后事件
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnAfterCollapse(TreeViewEventArgs e)
        //{
        //	base.OnAfterCollapse(e);
        //	//this.EndUpdate();
        //}

        /// <summary>
        /// 释放方法
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._nodeDownPic != null)
                {
                    this._nodeDownPic.Dispose();
                    this._nodeDownPic = null;
                }
                if (this._nodeUpPic != null)
                {
                    this._nodeUpPic.Dispose();
                    this._nodeUpPic = null;
                }
                this._nodeStateDict.Clear();
                this._nodeStateDict = null;
                if (this.stringFormat != null)
                {
                    this.stringFormat.Dispose();
                    this.stringFormat = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 私有方法
        private Image GetTreeNodeImage(TreeNode node)
        {
            if (this.ImageList == null || this.ImageList.Images.Count == 0) return null;
            if (node.IsSelected)
            {
                return GetSelectedImage(node) ?? GetImage(node);
            }
            else
            {
                return GetImage(node);
            }
        }

        private Image GetSelectedImage(TreeNode node)
        {
            if (node.SelectedImageIndex >= 0 && node.SelectedImageIndex < base.ImageList.Images.Count)
            {
                return this.ImageList.Images[node.SelectedImageIndex];
            }
            else if (!node.SelectedImageKey.IsNullOrEmpty() && this.ImageList.Images.ContainsKey(node.SelectedImageKey))
            {
                return this.ImageList.Images[node.SelectedImageKey];
            }
            else if (this.SelectedImageIndex >= 0 && this.SelectedImageIndex < this.ImageList.Images.Count)
            {
                return this.ImageList.Images[this.SelectedImageIndex];
            }
            else if (!this.SelectedImageKey.IsNullOrEmpty() && this.ImageList.Images.ContainsKey(this.SelectedImageKey))
            {
                return this.ImageList.Images[this.SelectedImageKey];
            }
            return null;
        }

        private Image GetImage(TreeNode node)
        {
            if (node.ImageIndex >= 0 && node.ImageIndex < base.ImageList.Images.Count)
            {
                return this.ImageList.Images[node.ImageIndex];
            }
            else if (!node.ImageKey.IsNullOrEmpty() && this.ImageList.Images.ContainsKey(node.ImageKey))
            {
                return this.ImageList.Images[node.ImageKey];
            }
            else if (this.ImageIndex >= 0 && this.ImageIndex < this.ImageList.Images.Count)
            {
                return this.ImageList.Images[this.ImageIndex];
            }
            else if (!this.ImageKey.IsNullOrEmpty() && this.ImageList.Images.ContainsKey(this.ImageKey))
            {
                return this.ImageList.Images[this.ImageKey];
            }
            return null;
        }

        private void DrawCheck(Graphics g, Rectangle boxRect, TreeNode node)
        {
            RoundRectangle roundRect = new RoundRectangle(boxRect, this.mCornerRadius);
            Color backColor;
            Color borderColor;
            Color tickColor;
            // 绘制状态
            CheckState checkState = this.GetTreeNodeCheckBoxState(node);
            if (this._treeNodeStates == TreeNodeStates.Selected && checkState != CheckState.Unchecked)
            {
                //组合选中
                backColor = this.Theme.CheckBoxTheme.CombinedBackColor;
                borderColor = this.Theme.CheckBoxTheme.CombinedSelectedColor;
                tickColor = this.Theme.CheckBoxTheme.CombinedSelectedColor;
            }
            else
            {
                switch (checkState)
                {
                    case CheckState.Checked:
                        backColor = this.Theme.CheckBoxTheme.SelectedBackColor;
                        borderColor = this.Theme.CheckBoxTheme.SelectedBorderColor;
                        tickColor = this.Theme.CheckBoxTheme.TickColor;
                        break;
                    default:
                        backColor = this.Theme.BackColor;
                        borderColor = this.Theme.BorderColor;
                        tickColor = this.Theme.CheckBoxTheme.TickColor;
                        break;
                }
            }
            switch (checkState)
            {
                case CheckState.Indeterminate:
                    GDIHelper.DrawCheckBox(g, roundRect, backColor, borderColor, 1);
                    Rectangle innerRect = boxRect;
                    innerRect.Inflate(-3, -3);
                    GDIHelper.FillRectangle(g, new RoundRectangle(innerRect, this.mCornerRadius), tickColor);
                    break;
                case CheckState.Checked:
                    GDIHelper.DrawCheckBox(g, roundRect, backColor, borderColor, 1);
                    GDIHelper.DrawCheckTick(g, boxRect, tickColor);
                    break;
                default:
                    GDIHelper.DrawCheckBox(g, roundRect, backColor, borderColor, 1);
                    break;
            }
        }

        /// <summary>
        /// Determines whether [is vertical scroll bar visible].
        /// </summary>
        /// <returns><c>true</c> if [is vertical scroll bar visible]; otherwise, <c>false</c>.</returns>
        private bool IsVerticalScrollBarVisible()
        {
            return base.IsHandleCreated && (NativeMethods.GetWindowLong(base.Handle, -16) & 2097152) != 0;
        }

        private void InitArrowImage()
        {
            Rectangle rect = new Rectangle(0, 0, 20, 20);
            this._nodeDownPic = new Bitmap(rect.Width, rect.Height);
            Size size = new Size(10, 6);
            Graphics g = Graphics.FromImage(this._nodeDownPic);
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.Clear(Color.Transparent);
            GDIHelper.DrawArrow(g, ArrowDirection.Down, rect, size, 0, Color.Black);

            this._nodeUpPic = new Bitmap(rect.Width, rect.Height);
            g = Graphics.FromImage(this._nodeUpPic);
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.Clear(Color.Transparent);
            GDIHelper.DrawArrow(g, ArrowDirection.Up, rect, size, 0, Color.Black);
        }

        /// <summary>
        /// Set treenode checkbox state 
        /// </summary>
        /// <param name="treeNode">treenode</param>
        /// <param name="state">checkbox state</param>
        private void SetNodeState(TreeNode treeNode, CheckState state)
        {
            if (!this._nodeStateDict.ContainsKey(treeNode))
            {
                this._nodeStateDict.Add(treeNode, state);
            }
            else
            {
                this._nodeStateDict[treeNode] = state;
            }
            CheckBoxStateChanged?.Invoke(this, new CheckBoxStateChangedEventArgs(state, treeNode));
        }

        /// <summary>
        /// Get treenode checkbox state 
        /// </summary>
        /// <param name="treeNode">treenode</param>
        /// <returns>checkbox state</returns>
        private CheckState GetNodeState(TreeNode treeNode)
        {
            if (this._nodeStateDict.ContainsKey(treeNode))
            {
                return this._nodeStateDict[treeNode];
            }
            else
            {
                CheckState state = treeNode.Checked ? CheckState.Checked : CheckState.Unchecked;
                this._nodeStateDict.Add(treeNode, state);
                return state;
            }
        }

        /// <summary>
        /// Toggle treenode state
        /// </summary>
        /// <param name="treeNode">tree node</param>
        private void ToggleTreeNodeState(TreeNode treeNode)
        {
            CheckState checkboxState = treeNode.Checked ? CheckState.Checked : CheckState.Unchecked;
            Console.WriteLine($"ToggleTreeNodeState: treeNode:{treeNode.Checked}");
            this.BeginUpdate();
            switch (checkboxState)
            {
                case CheckState.Unchecked:
                    this.SetTreeNodeAndChildrenStateRecursively(treeNode, CheckState.Unchecked);
                    this.SetParentTreeNodeStateRecursively(treeNode.Parent);
                    break;
                case CheckState.Checked:
                case CheckState.Indeterminate:
                    this.SetTreeNodeAndChildrenStateRecursively(treeNode, CheckState.Checked);
                    this.SetParentTreeNodeStateRecursively(treeNode.Parent);
                    break;
            }
            this.EndUpdate();
        }

        private void ToggleTreeNodeState(TreeNode treeNode, bool isCheck)
        {
            CheckState checkboxState = isCheck ? CheckState.Checked : CheckState.Unchecked;
            Console.WriteLine($"ToggleTreeNodeState: treeNode:{treeNode.Checked}");
            this.BeginUpdate();
            switch (checkboxState)
            {
                case CheckState.Unchecked:
                    this.SetTreeNodeAndChildrenStateRecursively(treeNode, CheckState.Unchecked);
                    this.SetParentTreeNodeStateRecursively(treeNode.Parent);
                    break;
                case CheckState.Checked:
                case CheckState.Indeterminate:
                    this.SetTreeNodeAndChildrenStateRecursively(treeNode, CheckState.Checked);
                    this.SetParentTreeNodeStateRecursively(treeNode.Parent);
                    break;
            }
            this.EndUpdate();
        }

        /// <summary>
        /// Set tree node and his children checkbox state recursively
        /// </summary>
        /// <param name="treeNode">tree node</param>
        /// <param name="checkboxState">checkbox state</param>
        private void SetTreeNodeAndChildrenStateRecursively(TreeNode treeNode, CheckState checkboxState)
        {
            if (treeNode != null)
            {
                this.SetTreeNodeState(treeNode, checkboxState);
                foreach (TreeNode objChildTreeNode in treeNode.Nodes)
                {
                    this.SetTreeNodeAndChildrenStateRecursively(objChildTreeNode, checkboxState);
                }
            }
        }

        /// <summary>
        /// Set parent treenode checkbox state recursively
        /// </summary>
        /// <param name="parentTreeNode"></param>
        private void SetParentTreeNodeStateRecursively(TreeNode parentTreeNode)
        {
            CheckState checkboxState;
            bool bAllChildrenChecked = true;
            bool bAllChildrenUnchecked = true;

            if (parentTreeNode != null)
            {
                foreach (TreeNode treeNode in parentTreeNode.Nodes)
                {
                    checkboxState = this.GetTreeNodeCheckBoxState(treeNode);
                    switch (checkboxState)
                    {
                        case CheckState.Checked:
                            bAllChildrenUnchecked = false;
                            break;
                        case CheckState.Indeterminate:
                            bAllChildrenUnchecked = false;
                            bAllChildrenChecked = false;
                            break;
                        case CheckState.Unchecked:
                            bAllChildrenChecked = false;
                            break;
                    }

                    if (bAllChildrenChecked == false & bAllChildrenUnchecked == false)
                    {
                        // This is an optimization
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                if (bAllChildrenChecked)
                {
                    this.SetTreeNodeState(parentTreeNode, CheckState.Checked);
                }
                else if (bAllChildrenUnchecked)
                {
                    this.SetTreeNodeState(parentTreeNode, CheckState.Unchecked);
                }
                else
                {
                    this.SetTreeNodeState(parentTreeNode, CheckState.Indeterminate);
                }

                // Enter in recursion
                if (parentTreeNode.Parent != null)
                {
                    this.SetParentTreeNodeStateRecursively(parentTreeNode.Parent);
                }
            }
        }

        /// <summary>
        /// Set tree node checkbox state
        /// </summary>
        /// <param name="treeNode">tree node</param>
        /// <param name="checkboxState">checkbox state</param>
        private void SetTreeNodeState(TreeNode treeNode, CheckState checkboxState)
        {
            switch (checkboxState)
            {
                case CheckState.Unchecked:
                case CheckState.Indeterminate:
                    treeNode.Checked = false;
                    if (treeNode.Checked == true)
                    {
                        checkboxState = CheckState.Checked;
                    }
                    break;
                case CheckState.Checked:
                    treeNode.Checked = true;
                    //BeforeCheck事件中,e.Cancel=true
                    if (treeNode.Checked == false)
                    {
                        checkboxState = CheckState.Unchecked;
                    }
                    break;
            }
            this.SetNodeState(treeNode, checkboxState);
        }

        private void UpdateExtendedStyles()
        {
            int Style = 0;

            if (DoubleBuffered)
                Style |= Win32Consts.TVS_EX_DOUBLEBUFFER;

            if (Style != 0)
                SparkControls.Win32.NativeMethods.SendMessage(Handle, Win32Consts.TVM_SETEXTENDEDSTYLE, (IntPtr)Win32Consts.TVS_EX_DOUBLEBUFFER, (IntPtr)Style);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 获取TreeNode的CheckBox的状态
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public CheckState GetTreeNodeCheckBoxState(TreeNode treeNode)
        {
            return this.GetNodeState(treeNode);
        }

        /// <summary>
        /// Set treenode checkbox state 
        /// </summary>
        /// <param name="treeNode">treenode</param>
        /// <param name="isCheck">checkbox state</param>
        public void SetCheckState(TreeNode treeNode, bool isCheck)
        {
            CheckState checkState = isCheck ? CheckState.Checked : CheckState.Unchecked;
            if (!this._nodeStateDict.ContainsKey(treeNode))
            {
                this._nodeStateDict.Add(treeNode, checkState);
            }
            else
            {
                this._nodeStateDict[treeNode] = checkState;
            }
            treeNode.Checked = isCheck;
            CheckBoxStateChanged?.Invoke(this, new CheckBoxStateChangedEventArgs(checkState, treeNode));
        }
        #endregion

        #region ISupportInitialize接口
        /// <summary>
        /// 初始化开始
        /// </summary>
        public void BeginInit()
        {
            this.isIniting = true;
        }

        /// <summary>
        /// 初始化结束
        /// </summary>
        public void EndInit()
        {
            this.isIniting = false;
        }
        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkTreeViewTheme Theme { get; private set; }

        #endregion
    }
}