using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Win32;

using SparkControls.Controls;
using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Forms
{
    /// <summary>
    /// 基类窗口
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    [DefaultEvent("Load")]
    [Designer("System.Windows.Forms.Design.FormDocumentDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner))]
    [DesignerCategory("Form")]
    [DesignTimeVisible(false)]
    [InitializationEvent("Load")]
    [ToolboxItem(false)]
    [ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
    public class SparkFormBase : SparkFormShadow, IProcessCmdKey
    {
        #region 私有变量
        private bool isDrawTitle = true;
        private Size maximumSize = Size.Empty;
        private Font font = Consts.DEFAULT_FONT;
        private readonly SparkTitleBarDraw titleBarDraw = null;
        private readonly object objectLock = new object();
        #endregion

        #region 事件定义
        /// <summary>
        /// 自定义标题按钮单击事件
        /// </summary>
        public event TitleButtonClickEventHandler TitleButtonClick;

        /// <summary>
        /// 窗口状态改变事件
        /// </summary>
        public event FormWindowStateChangedEventHandler FormWindowStateChanged;

        /// <summary>
        /// ProcessCmdKey事件
        /// </summary>
        public event ProcessCmdKeyEventHandler ProcessCmdKeyEvent;
        #endregion

        #region 属性
        /// <summary>
        /// 窗体是否能移动
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        public bool IsMove { get; set; } = true;

        /// <summary>
        /// 获取或设置控件的设计尺寸
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new SizeF AutoScaleDimensions { get { return SizeF.Empty; } set { base.AutoScaleDimensions = SizeF.Empty; } }

        /// <summary>
        /// 获取或设置控件显示的文字的字体。
        /// </summary>
        [Description("获取或设置控件显示的文字的字体")]
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
        /// 获取或设置运行时窗体的起始位置
        /// </summary>
        [DefaultValue(FormStartPosition.WindowsDefaultLocation)]
        public new FormStartPosition StartPosition
        {
            get { return base.StartPosition; }
            set
            {
                if (base.StartPosition != value)
                {
                    base.StartPosition = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置窗体的窗口状态
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FormWindowState.Normal)]
        public new FormWindowState WindowState
        {
            get
            {
                return base.WindowState;
            }
            set
            {
                if (base.WindowState != value)
                {
                    if (value == FormWindowState.Maximized)
                    {
                        this.SetMaximumSize();
                    }
                    base.WindowState = value;
                    this.OnFormWindowStateChanged(new FormWindowStateChangedEventArgs(value));
                }
            }
        }
        private FormBorderStyle _formBorderStyle = FormBorderStyle.Sizable;
        /// <summary>
        /// 窗体边框样式
        /// </summary>
        [Description("获取或设置窗体的边框样式。")]
        [Browsable(false)]
        [DefaultValue(FormBorderStyle.Sizable)]
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return this._formBorderStyle;
            }
            set
            {

                this._formBorderStyle = value;
                switch (this._formBorderStyle)
                {
                    case FormBorderStyle.None:
                        this.IsDrawTitle = false;
                        break;
                    case FormBorderStyle.FixedSingle:
                    case FormBorderStyle.Fixed3D:
                    case FormBorderStyle.FixedDialog:
                        this.IsDrawTitle = true;
                        this.Sizable = false;
                        break;
                    case FormBorderStyle.Sizable:
                        this.IsDrawTitle = true;
                        this.Sizable = true;
                        break;
                    case FormBorderStyle.FixedToolWindow:
                        this.IsDrawTitle = true;
                        this.MaximizeBox = false;
                        this.MinimizeBox = false;
                        this.Sizable = false;
                        break;
                    case FormBorderStyle.SizableToolWindow:
                        this.IsDrawTitle = true;
                        this.MaximizeBox = false;
                        this.MinimizeBox = false;
                        this.Sizable = true;
                        break;
                }
                base.FormBorderStyle = FormBorderStyle.None;
            }
        }

        /// <summary>
        /// 是否能拖动改变窗体大小
        /// </summary>
        [Description("是否能拖动改变窗体大小")]
        [DefaultValue(true)]
        public bool Sizable { get; set; } = true;

        /// <summary>
        /// 是否显示标题栏
        /// </summary>
        [Description("获取或设置窗体是否标题栏。")]
        [DefaultValue(true)]
        public bool IsDrawTitle
        {
            get => this.isDrawTitle;
            set
            {
                if (this.isDrawTitle != value)
                {
                    this.isDrawTitle = value;
                    this.titleBarDraw.IsDrawTitle = this.IsDrawTitle;
                    if (this.isDrawTitle)
                    {
                        this.Padding = new Padding(
                                   Math.Max(1, this.Padding.Left),
                                   Math.Max(this.TitleHeight, this.Padding.Top),
                                   Math.Max(1, this.Padding.Right),
                                   Math.Max(1, this.Padding.Bottom)
                                   );
                    }
                    else
                    {
                        this.Padding = new Padding(
                                   Math.Max(1, this.Padding.Left),
                                   Math.Max(1, this.TitleHeight == this.Padding.Top ? 1 : this.TitleHeight),
                                   Math.Max(1, this.Padding.Right),
                                   Math.Max(1, this.Padding.Bottom)
                                   );
                    }
                }
            }
        }

        /// <summary>
        ///  获取或设置一个值，该值指示是否在窗体的标题栏中显示“最小化”按钮。
        /// </summary>
        [DefaultValue(true)]
        public new bool MinimizeBox
        {
            get
            {
                return this.titleBarDraw.MinimizeBox;
            }
            set
            {
                this.titleBarDraw.MinimizeBox = value;
                base.MinimizeBox = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否在窗体的标题栏中显示“最大化”按钮。
        /// </summary>
        [DefaultValue(true)]
        public new bool MaximizeBox
        {
            get
            {
                return this.titleBarDraw.MaximizeBox;
            }
            set
            {
                this.titleBarDraw.MaximizeBox = value;
                base.MaximizeBox = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示在该窗体的标题栏中是否显示控件框。
        /// </summary>
        [DefaultValue(true)]
        public new bool ControlBox
        {
            get
            {
                return this.titleBarDraw.ControlBox;
            }
            set
            {
                this.titleBarDraw.ControlBox = value;
                base.ControlBox = value;
            }
        }

        /// <summary>
        /// 获取或设置窗体的图标
        /// </summary>
        [DefaultValue(typeof(Icon), null)]
        public new Icon Icon
        {
            get { return base.Icon; }
            set
            {
                if (base.Icon != value)
                {
                    this.titleBarDraw.Icon = value;
                    base.Icon = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    this.titleBarDraw.Text = value;
                }
            }
        }

        /// <summary>
        /// 是否显示图标
        /// </summary>
        [DefaultValue(true)]
        [Browsable(false)]
        public new bool ShowIcon
        {
            get { return true; }
            set { base.ShowIcon = true; }
        }

        /// <summary>
        /// 获取窗体可调整到的最大大小
        /// </summary>
        [DefaultValue(typeof(Size), "0, 0")]
        public new Size MaximumSize
        {
            get => this.maximumSize;
            set
            {
                if (this.maximumSize != value)
                {
                    this.maximumSize = value;
                    if (this.maximumSize != Size.Empty)
                    {
                        base.MaximumSize = value;
                    }
                    else
                    {
                        this.SetMaximumSize();
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置控件内的空白
        /// </summary>
        [DefaultValue(typeof(Padding), "1,32,1,1")]
        public new Padding Padding
        {
            get { return base.Padding; }
            set
            {
                if (base.Padding != value)
                {
                    base.Padding = new Padding(
                                    Math.Max(1, value.Left),
                                    Math.Max(1, value.Top),
                                    Math.Max(1, value.Right),
                                    Math.Max(1, value.Bottom)
                                    );
                }
            }
        }

        /// <summary>
        /// 标题栏样式
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SparkFormBaseTheme Theme { get; private set; }

        /// <summary>
        /// 标题字体
        /// </summary>
        [DefaultValue(typeof(Font), "微软雅黑,12pt")]
        public Font TitleFont
        {
            get => this.titleBarDraw.TitleFont;
            set
            {
                if (this.titleBarDraw.TitleFont != value)
                {
                    this.titleBarDraw.TitleFont = value;
                }
            }
        }

        /// <summary>
        /// 标题高度
        /// </summary>
        [DefaultValue(32)]
        public int TitleHeight
        {
            get => this.titleBarDraw.TitleHeight;
            set
            {
                if (this.titleBarDraw.TitleHeight != value)
                {
                    this.titleBarDraw.TitleHeight = value;
                }
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        /// <summary>
        /// 自定义按钮集合
        /// </summary>
        [Browsable(false)]
        public IList<SparkTitleBarItem> CustomItem
        {
            get => this.titleBarDraw?.CustomItem;
        }

        /// <summary>
        /// 标题栏全部按钮
        /// </summary>
        internal IList<SparkTitleBarItem> TitleButtonList => this.titleBarDraw?.Items;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkFormBase()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw |          //调整大小时重绘
                          ControlStyles.DoubleBuffer |          //双缓冲
                          ControlStyles.OptimizedDoubleBuffer | //双缓冲
                          ControlStyles.AllPaintingInWmPaint |  //禁止擦除背景
                          ControlStyles.Selectable |
                          ControlStyles.ContainerControl, true
            );
            this.Theme = new SparkFormBaseTheme(this);

            this.StartPosition = FormStartPosition.CenterScreen;

            this.titleBarDraw = new SparkTitleBarDraw(this, this.Theme.TitleTheme);
            this.titleBarDraw.TitleButtonClick += this.TitleBarDraw_TitleButtonClick;
            this.titleBarDraw.FormWindowStateChanged += this.TitleBarDraw_FormWindowStateChanged;

            if (!this.DesignMode)
            {
                this.SetMaximumSize();
            }

            //DockPadding.All = 10;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            //订阅显示设置改变事件
            SystemEvents.DisplaySettingsChanged += this.SystemEvents_DisplaySettingsChanged;
            this.Padding = new Padding(1, this.titleBarDraw.TitleHeight, 1, 1);
            this.Font = this.font;

            //this.Deactivate += (sender, e) =>
            //{
            //    PopupCache.ClosePopup();
            //};
        }

        #endregion

        #region 事件绑定
        private void TitleBarDraw_TitleButtonClick(object sender, TitleButtonClickEventArgs e)
        {
            this.OnTitleButtonClick(e);
        }

        private void TitleBarDraw_FormWindowStateChanged(object sender, FormWindowStateChangedEventArgs e)
        {
            this.OnFormWindowStateChanged(e);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            this.SetMaximumSize();
        }
        #endregion

        #region IProcessCmdKey接口
        event ProcessCmdKeyEventHandler IProcessCmdKey.ProcessCmdKey
        {
            add
            {
                lock (this.objectLock)
                {
                    if (ProcessCmdKeyEvent == null)
                    {
                        ProcessCmdKeyEvent += value;
                        this.IsRegisteredProcessCmdKey = true;
                    }
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    ProcessCmdKeyEvent -= value;
                    this.IsRegisteredProcessCmdKey = false;
                }
            }
        }
        private Hashtable shortcuts = null;
        /// <summary>
        /// 保存快捷键的集合
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hashtable Shortcuts
        {
            get
            {
                lock (this.objectLock)
                {
                    if (this.shortcuts == null)
                    {
                        this.shortcuts = new Hashtable(1);
                    }
                }
                return this.shortcuts;
            }
        }
        /// <summary>
        /// 判断接口事件是否注册
        /// </summary>
        public bool IsRegisteredProcessCmdKey { get; private set; } = false;
        #endregion

        #region 事件重写
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            try
            {
                SystemEvents.DisplaySettingsChanged -= this.SystemEvents_DisplaySettingsChanged;
                if (this.titleBarDraw != null)
                {
                    this.titleBarDraw.TitleButtonClick -= this.TitleBarDraw_TitleButtonClick;
                    this.titleBarDraw.FormWindowStateChanged -= this.TitleBarDraw_FormWindowStateChanged;
                }
                if (ProcessCmdKeyEvent != null)
                {
                    this.ProcessCmdKeyEvent -= this.ProcessCmdKeyEvent;
                    this.ProcessCmdKeyEvent = null;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 重绘
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            this.titleBarDraw?.Draw(e);
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, this.Theme.BorderColor, ButtonBorderStyle.Solid);
            //e.Graphics.DrawRectangle(new Pen(this.Theme.BorderColor), this.ClientRectangle);
        }

        /// <summary>
        /// 重写消息监听事件
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)Msgs.WM_SYSCOMMAND:
                    // Console.WriteLine($"1. {m.Msg} {m.WParam} {m.LParam} {m.Result}");//1. 274 61728 0 0
                    switch (m.WParam.ToInt32())
                    {
                        case Win32Consts.SC_MINIMIZE:
                            //捕获最小化消息
                            if (this.IsDrawTitle && this.MinimizeBox)
                            {
                                this.WindowState = FormWindowState.Minimized;
                            }
                            break;
                        //case Win32Consts.SC_RESTORE:
                        //    //捕获还原消息
                        //    this.WindowState = FormWindowState.Normal;
                        //    break;
                        case Win32Consts.SC_MAXIMIZE:
                        case Win32Consts.SC_MAXIMIZE2: //这里是一个巨坑，窗口最大化有两种消息值
                                                       //捕获最大化消息
                            if (this.IsDrawTitle && this.MinimizeBox && this.MaximizeBox)
                            {
                                this.WindowState = FormWindowState.Maximized;
                            }
                            break;
                        default:
                            //Console.WriteLine($"1. {m.Msg} {m.WParam} {m.LParam} {m.Result}");
                            base.WndProc(ref m);
                            break;
                    }
                    break;
                case (int)Msgs.WM_NCHITTEST:
                    base.WndProc(ref m);
                    bool isMove = false;

                    if (this.isDrawTitle)
                    {
                        if (new Rectangle(5, 5, this.Width - this.titleBarDraw.ItemWidthSum - 5, this.TitleHeight - 5).Contains(this.PointToClient(Cursor.Position)))
                        {
                            isMove = true;
                        }
                    }
                    else
                    {
                        //无标题窗口是否能移动，默认可以，后面可以开放参数进行设置
                        if (new Rectangle(5, 5, this.Width - 10, this.Height - 10).Contains(this.PointToClient(Cursor.Position)))
                        {
                            isMove = true;
                        }
                    }

                    if (IsMove && isMove && m.Result == (IntPtr)HitTest.HTCLIENT)//如果返回的是HTCLIENT 
                    {
                        m.Result = (IntPtr)HitTest.HTCAPTION;//把它改为HTCAPTION 
                        return;//直接返回退出方法 
                    }

                    if (this.Sizable && this.titleBarDraw.Maximized == false)
                    {
                        Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                        vPoint = this.PointToClient(vPoint);
                        if (vPoint.X <= 5)
                        {
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)HitTest.HTTOPLEFT;
                            else if (vPoint.Y >= this.ClientSize.Height - 5)
                                m.Result = (IntPtr)HitTest.HTBOTTOMLEFT;
                            else m.Result = (IntPtr)HitTest.HTLEFT;
                        }
                        else if (vPoint.X >= this.ClientSize.Width - 5)
                        {
                            if (vPoint.Y <= 5)
                                m.Result = (IntPtr)HitTest.HTTOPRIGHT;
                            else if (vPoint.Y >= this.ClientSize.Height - 5)
                                m.Result = (IntPtr)HitTest.HTBOTTOMRIGHT;
                            else m.Result = (IntPtr)HitTest.HTRIGHT;
                        }
                        else if (vPoint.Y <= 5)
                        {
                            m.Result = (IntPtr)HitTest.HTTOP;
                        }
                        else if (vPoint.Y >= this.ClientSize.Height - 5)
                        {
                            m.Result = (IntPtr)HitTest.HTBOTTOM;
                        }
                        break;
                    }
                    break;
                //case (int)Msgs.WM_LBUTTONDOWN://鼠标左键按下的消息 用于实现拖动窗口功能
                //    //m.Msg = Msgs.WM_NCLBUTTONDOWN ;//更改消息为非客户区按下鼠标
                //    //m.LParam = IntPtr.Zero;//默认值
                //    //m.WParam = new IntPtr(2);//鼠标放在标题栏内
                //    base.WndProc(ref m);
                //    break;
                case (int)Msgs.WM_ACTIVATE:
                    // Console.WriteLine($"WM_ACTIVATE :{m.Msg} {m.LParam} {m.WParam} {m.Result}");
                    if (m.LParam == IntPtr.Zero && m.WParam == IntPtr.Zero)
                    {
                        PopupCache.ClosePopup();
                    }
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }



        }

        /// <summary>
        /// 重写ProcessCmdKey事件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ProcessCmdKeyEvent != null)
            {
                if (ProcessCmdKeyEvent(ref msg, keyData) == false)
                {
                    return false;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 窗口状态改变事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFormWindowStateChanged(FormWindowStateChangedEventArgs e)
        {
            FormWindowStateChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 标题栏按钮单击事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTitleButtonClick(TitleButtonClickEventArgs e)
        {
            TitleButtonClick?.Invoke(this, e);
        }

        /// <summary>
        /// 重写CreateParams
        /// </summary>
        protected override CreateParams CreateParams
        {
            //实现无标题栏窗体点击任务栏图标正常最小化或还原的解决方法
            get
            {
                const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义  
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作  
                return cp;
            }
        }
        #endregion

        #region 私有方法
        private void SetMaximumSize()
        {
            if (this.maximumSize == Size.Empty && !this.IsDisposed && !this.Disposing)
            {
                //如果窗口被释放，会导致异常
                Screen screen = Screen.FromControl(this);
                base.MaximumSize = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
            }
        }
        #endregion

        #region public
        /// <summary>
        /// 设置标题栏按钮的状态(只能设置,最大化,最小化和关闭按钮)
        /// </summary>
        public void SetTitleButtonState(TitleItemAction itemAction, bool isEnabled)
        {
            var value = Convert.ToInt32(itemAction);
            if (value < 1 || value > 3) return;
            var button = TitleButtonList?.FirstOrDefault(a => a.TitleAction == itemAction);
            if (button != null)
            {
                button.Enabled = isEnabled;
            }
        }
        #endregion
    }
}