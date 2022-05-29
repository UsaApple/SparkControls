using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Forms;
using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 标题栏绘制类
    /// </summary>
    [ToolboxItem(false)]
    public class SparkTitleBarDraw : IDisposable
    {
        #region 私有变量

        private ButtonState _buttonState = ButtonState.None;
        private int titleHeight = 32;
        private readonly Color foreColor = Color.White;
        private Font titleFont = new Font("微软雅黑", 12f);
        private bool minimizeBox = true;
        private bool maximizeBox = true;
        private bool controlBox = true;
        private readonly Color borderColor = Color.Transparent;
        private readonly Color backColor = SystemColors.Control;
        private string text = string.Empty;
        private readonly BindingList<SparkTitleBarItem> customItem = new BindingList<SparkTitleBarItem>();
        private readonly List<SparkTitleBarItem> items = new List<SparkTitleBarItem>();
        private bool maximized = false;
        private Form _parentForm = null;
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;
        private bool isDrawTitle = true;
        private Icon icon = null;
        private SparkTitleBarItem _customItem = null;

        internal enum ButtonState
        {
            None = 0,

            MaxDown = 1,
            MinDown = 2,
            XDown = 3,

            MaxOver = 101,
            MinOver = 102,
            XOver = 103,

            Custom = 200,
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        internal SparkTitleBarTheme Theme { get; private set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否可以通过鼠标移动窗体位置。
        /// </summary>
        internal bool IsMouseMove { get; set; } = false;

        /// <summary>
        /// 获取控件所在的父窗口。
        /// </summary>
        [Browsable(false)]
        internal Form ParentForm
        {
            get
            {
                if (this._parentForm == null && this.Parent != null)
                {
                    this._parentForm = this.FindParentForm(this.Parent);
                }
                return this._parentForm;
            }
        }

        /// <summary>
        /// 获取控件所在的父控件。
        /// </summary>
        [Browsable(false)]
        public Control Parent { get; }

        /// <summary>
        /// 获取一个值，该值指示父窗口是否最大化。
        /// </summary>
        [Browsable(false)]
        internal bool Maximized => this.ParentForm != null && this.ParentForm.WindowState == FormWindowState.Maximized;

        /// <summary>
        /// 获取最大化按钮
        /// </summary>
        [Browsable(false)]
        public SparkTitleBarItem MaxButton { get; private set; } = new SparkTitleBarItem(TitleItemAction.Max);

        /// <summary>
        /// 获取最小化按钮
        /// </summary>
        [Browsable(false)]
        public SparkTitleBarItem MinButton { get; private set; } = new SparkTitleBarItem(TitleItemAction.Min);

        /// <summary>
        /// 获取关闭按钮
        /// </summary>
        [Browsable(false)]
        public SparkTitleBarItem CloseButton { get; private set; } = new SparkTitleBarItem(TitleItemAction.Close);

        /// <summary>
        /// 获取自定义按钮集合
        /// </summary>
        [Browsable(false)]
        public IList<SparkTitleBarItem> CustomItem => this.customItem;

        /// <summary>
        /// 获取全部按钮集合
        /// </summary>
        internal List<SparkTitleBarItem> Items => this.items;

        /// <summary>
        /// 摘要:
        ///     获取或设置一个值，该值指示是否在窗体的标题栏中显示“最大化”按钮。
        /// 返回结果:
        ///     如果为 true，则显示窗体的“最大化”按钮；否则为 false。默认值为 true。
        /// </summary>
        [DefaultValue(true)]
        public bool MaximizeBox
        {
            get => this.maximizeBox;
            set
            {
                if (this.maximizeBox != value)
                {
                    this.maximizeBox = value;
                    this.MaxButton.Visible = this.ControlBox && value;
                    this.CaclButtonBounds();
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 摘要:
        ///     获取或设置一个值，该值指示是否在窗体的标题栏中显示“最小化”按钮。
        /// 返回结果:
        ///     如果为 true，则显示窗体的“最小化”按钮；否则为 false。默认值为 true。
        /// </summary>
        [DefaultValue(true)]
        public bool MinimizeBox
        {
            get => this.minimizeBox;
            set
            {
                if (this.minimizeBox != value)
                {
                    this.minimizeBox = value;
                    this.MinButton.Visible = this.ControlBox && value;
                    this.CaclButtonBounds();
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示在该窗体的标题栏中是否显示控件框。
        /// </summary>
        [DefaultValue(true)]
        public bool ControlBox
        {
            get => this.controlBox;
            set
            {
                if (this.controlBox != value)
                {
                    this.controlBox = value;
                    if (value)
                    {
                        this.MinButton.Visible = this.MinimizeBox;
                        this.MaxButton.Visible = this.MaximizeBox;
                    }
                    else
                    {
                        this.MinButton.Visible = value;
                        this.MaxButton.Visible = value;
                    }
                    this.CloseButton.Visible = value;
                    this.CaclButtonBounds();
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件的标题字体。
        /// </summary>
        [DefaultValue(typeof(Font), "微软雅黑,12pt")]
        public Font TitleFont
        {
            get => this.titleFont;
            set
            {
                if (this.titleFont != value)
                {
                    this.titleFont = value;
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件的标题高度。
        /// </summary>
        [DefaultValue(32)]
        public int TitleHeight
        {
            get => this.titleHeight;
            set
            {
                //if (this.IsDrawTitle == false) return;
                if (this.titleHeight != value)
                {
                    if (value <= 20) value = 20;
                    int oldValue = this.titleHeight;
                    this.titleHeight = value;
                    this.Parent.Padding = new Padding(0, value, 0, 0);
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件的标题文本。
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否绘制标题。
        /// </summary>
        [Browsable(false)]
        public bool IsDrawTitle
        {
            get => this.isDrawTitle;
            set
            {
                if (this.isDrawTitle = value)
                {
                    this.isDrawTitle = value;
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置窗体的图标
        /// </summary>
        [DefaultValue(typeof(Icon), null)]
        public Icon Icon
        {
            get => this.icon;
            set
            {
                if (this.icon != value)
                {
                    this.icon = value;
                    if (this.ParentForm != null)
                    {
                        this.ParentForm.Icon = this.icon;
                    }
                    this.Parent.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取按钮的宽度。
        /// </summary>
        internal int ItemWidthSum => this.IsDrawTitle ? this.Items.Count(a => a.Visible) * this.TitleHeight : 0;

        /// <summary>
        /// 获取或设置一个值，该值指示是否注册按钮的单击事件。
        /// </summary>
        [Browsable(true)]
        internal bool IsRegisteredSystemButtonClick { get; set; } = true;
        #endregion

        #region 事件定义
        /// <summary>
        /// 自定义按钮单机事件
        /// </summary>
        public event TitleButtonClickEventHandler TitleButtonClick;

        /// <summary>
        /// 窗体状态改变事件
        /// </summary>
        public event FormWindowStateChangedEventHandler FormWindowStateChanged;

        /// <summary>
        /// 命中测试事件
        /// </summary>
        internal event TitleHitTestEventHandler TitleHitTest;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="control"></param>
        /// <param name="theme"></param>
        public SparkTitleBarDraw(Control control, SparkTitleBarTheme theme)
        {
            this.Theme = theme;
            this.Parent = control;
            this.Parent.MouseDown += this.MouseDown;
            this.Parent.MouseUp += this.MouseUp;
            this.Parent.MouseMove += this.MouseMove;
            this.Parent.MouseLeave += this.MouseLeave;
            this.Parent.Resize += this.Resize;
            this.customItem.ListChanged += this.CustomItem_ListChanged;
            this.items.Add(this.CloseButton);
            this.items.Add(this.MaxButton);
            this.items.Add(this.MinButton);
        }
        #endregion

        #region 事件绑定
        private void CustomItem_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    if (this.items.Count - 3 > 0)
                    {
                        this.items.RemoveRange(3, this.items.Count - 3);
                        this.CaclButtonBounds();
                    }
                    break;
                case ListChangedType.ItemAdded:
                    SparkTitleBarItem itemAdd = this.customItem[e.NewIndex];
                    this.items.Add(itemAdd);
                    this.CaclButtonBounds();
                    break;
                case ListChangedType.ItemDeleted:
                    this.items.RemoveAt(e.NewIndex + 3);
                    this.CaclButtonBounds();
                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.ItemChanged:
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    break;
            }
            this.Parent?.Invalidate();
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            this.UpdateButtons(e, true);
            if (this._buttonState == ButtonState.None)
            {
                this.FormMouseUp();
            }
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.isDrawTitle) return;
            this.UpdateButtons(e);
            if (this._buttonState == ButtonState.None)
            {
                if (this.HitTest(e) == false)
                {
                    if (e.Clicks == 2 && e.Button == MouseButtons.Left)
                    {
                        //判断是否在标题栏上
                        Point screenPoint = this.Parent.PointToScreen(new Point(0, 0));
                        RectangleF rect = new RectangleF(screenPoint.X, screenPoint.Y, this.Parent.ClientRectangle.Width, this.titleHeight);
                        if (rect.Contains(Cursor.Position))
                        {
                            this.OnMaximizeRestore(null, null);
                        }
                    }
                    else
                    {
                        this.FormMouseDown(e.Location);
                    }
                }
            }
        }

        private void MouseLeave(object sender, EventArgs e)
        {
            if (this._buttonState != ButtonState.None)
            {
                this._buttonState = ButtonState.None;
                this._customItem = null;
                if (this.ParentForm != null)
                {
                    this.ParentForm.Invalidate();
                }
            }

            if (this.IsMouseMove && IsDrag)
            {
                this.FormMouseUp();
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isDrawTitle) return;
            this.UpdateButtons(e);
            if (this._buttonState == ButtonState.None)
            {
                this.FormMouseMove(e.Location);
            }
        }

        private void Resize(object sender, EventArgs e)
        {
            if (!this.isDrawTitle) return;
            this.CaclButtonBounds();
        }

        #endregion

        #region 私有方法

        private void OnMinimize(object sender, EventArgs args)
        {
            Form form = this.ParentForm;
            if (form != null && this.MinimizeBox)
            {
                form.WindowState = FormWindowState.Minimized;
                this.OnFormWindowStateChanged(new FormWindowStateChangedEventArgs(FormWindowState.Minimized));
            }
        }

        private void OnMaximizeRestore(object sender, EventArgs args)
        {
            Form form = this.ParentForm;
            if ((form != null) && this.MaximizeBox)
            {
                if (form.WindowState == FormWindowState.Maximized)
                {
                    form.WindowState = FormWindowState.Normal;
                    this.OnFormWindowStateChanged(new FormWindowStateChangedEventArgs(FormWindowState.Normal));
                    this.maximized = false;
                }
                else
                {
                    form.WindowState = FormWindowState.Maximized;
                    this.OnFormWindowStateChanged(new FormWindowStateChangedEventArgs(FormWindowState.Maximized));
                    this.maximized = true;
                }
            }
        }

        private void OnFormWindowStateChanged(FormWindowStateChangedEventArgs e)
        {
            FormWindowStateChanged?.Invoke(this, e);
        }

        private void OnClose(object sender, EventArgs args)
        {
            Form form = this.ParentForm;
            if (form != null)
            {
                form.Close();
            }
        }

        private void FormMouseDown(Point p)
        {
            if (this.IsMouseMove && this.Maximized == false)
            {
                IsDrag = true;
                this.enterX = p.X;
                this.enterY = p.Y;
            }
        }

        private void FormMouseUp()
        {
            if (this.IsMouseMove)
            {
                IsDrag = false;
                this.enterX = 0;
                this.enterY = 0;
            }
        }

        private void FormMouseMove(Point p)
        {
            if (this.IsMouseMove && IsDrag && this.ParentForm != null)
            {
                this.ParentForm.Left += p.X - this.enterX;
                this.ParentForm.Top += p.Y - this.enterY;
            }
        }

        private Form FindParentForm(Control ctl)
        {
            if (ctl != null)
            {
                if (((ctl.Parent != null) && (ctl.Parent is Form)) && !((Form)ctl.Parent).TopLevel)
                {
                    return (Form)ctl.Parent;
                }
                Form form = ctl.FindForm();
                if ((form != null))
                {
                    return form;
                }
            }
            return null;
        }

        private void CaclButtonBounds()
        {
            int index = 0;
            foreach (SparkTitleBarItem item in this.items.Where(a => a.Visible))
            {
                index++;
                item.Bounds = new Rectangle(this.Parent.Width - index * this.TitleHeight, 0, this.TitleHeight, this.TitleHeight);
            }
        }

        private bool HitTest(MouseEventArgs e)
        {
            if (TitleHitTest != null)
            {
                return TitleHitTest(this, e);
            }
            else
            {
                return false;
            }
        }

        private void UpdateButtons(MouseEventArgs e, bool up = false)
        {
            this._customItem = null;
            ButtonState oldState = this._buttonState;
            bool showMin = this.MinimizeBox && this.MinButton.Enabled;
            bool showMax = this.MaximizeBox && this.MaxButton.Enabled;
            bool showClose = this.CloseButton.Visible && this.CloseButton.Enabled;
            SparkTitleBarItem item = this.items.FirstOrDefault(a => a.Visible && a.Bounds.Contains(e.Location));

            TitleItemAction hitTestResult = item != null ? item.TitleAction : TitleItemAction.None;
            if (hitTestResult != TitleItemAction.None)
            {
                if (e.Button == MouseButtons.Left && !up)
                {
                    if (showMin && !showMax && hitTestResult == TitleItemAction.Max)
                        this._buttonState = ButtonState.MinDown;
                    else if (showMin && hitTestResult == TitleItemAction.Min)
                        this._buttonState = ButtonState.MinDown;
                    else if (showMax && hitTestResult == TitleItemAction.Max)
                        this._buttonState = ButtonState.MaxDown;
                    else if (showClose && hitTestResult == TitleItemAction.Close)
                        this._buttonState = ButtonState.XDown;
                    else if (hitTestResult == TitleItemAction.Custom)
                    {
                        this._buttonState = ButtonState.Custom;
                        this._customItem = item;
                    }
                }
                else
                {
                    if (showMin && !showMax && hitTestResult == TitleItemAction.Max)
                    {
                        this._buttonState = ButtonState.MinOver;
                        if (oldState == ButtonState.MinDown && up)
                        {
                            //WindowState = FormWindowState.Minimized;
                            this.SysButtonDonw(ButtonState.MinDown);
                        }
                    }
                    else if (showMin && hitTestResult == TitleItemAction.Min)
                    {
                        this._buttonState = ButtonState.MinOver;
                        if (oldState == ButtonState.MinDown && up)
                        {
                            //WindowState = FormWindowState.Minimized;
                            this.SysButtonDonw(ButtonState.MinDown);
                        }
                    }
                    else if (this.MaximizeBox && hitTestResult == TitleItemAction.Max)
                    {
                        this._buttonState = ButtonState.MaxOver;

                        if (oldState == ButtonState.MaxDown && up)
                        {
                            //  MaximizeWindow(!_maximized);
                            this.SysButtonDonw(ButtonState.MaxDown);
                        }
                    }
                    else if (showClose && hitTestResult == TitleItemAction.Close)
                    {
                        this._buttonState = ButtonState.XOver;

                        if (oldState == ButtonState.XDown && up)
                        {
                            this.SysButtonDonw(ButtonState.XDown);
                            //Close();
                        }
                    }
                    else if (hitTestResult == TitleItemAction.Custom)
                    {
                        this._buttonState = ButtonState.Custom;
                        if (this._customItem != item)
                        {
                            oldState = ButtonState.None;
                            this._customItem = item;
                        }
                    }
                }
                if (up == true && this._buttonState != ButtonState.None && item.TitleAction == TitleItemAction.Custom)
                {
                    if (item.ContextMenu != null)
                    {
                        //弹出菜单
                        item.ContextMenu.Show(this.Parent, new Point(item.Bounds.X, item.Bounds.Height));
                    }
                    else
                    {
                        //触发单击事件
                        TitleButtonClick?.Invoke(this, new TitleButtonClickEventArgs(item.Key, hitTestResult, item));
                    }
                }
            }
            else
            {
                this._buttonState = ButtonState.None;
            }
            if (oldState != this._buttonState) this.Parent.Invalidate();
        }

        private void SysButtonDonw(ButtonState buttonState)
        {
            if (this.IsRegisteredSystemButtonClick)
            {
                switch (buttonState)
                {
                    case ButtonState.MaxDown:
                        this.OnMaximizeRestore(null, null);
                        break;
                    case ButtonState.MinDown:
                        this.OnMinimize(null, null);
                        break;
                    case ButtonState.XDown:
                        this.OnClose(null, null);
                        break;
                }
            }
            else
            {
                TitleItemAction titleItemAction;
                SparkTitleBarItem titleBarItem;
                switch (buttonState)
                {
                    case ButtonState.MaxDown:
                        titleItemAction = TitleItemAction.Max;
                        titleBarItem = this.MaxButton;
                        break;
                    case ButtonState.MinDown:
                        titleItemAction = TitleItemAction.Min;
                        titleBarItem = this.MinButton;
                        break;
                    case ButtonState.XDown:
                        titleItemAction = TitleItemAction.Close;
                        titleBarItem = this.CloseButton;
                        break;
                    default:
                        return;
                }
                //触发单击事件
                TitleButtonClick?.Invoke(this, new TitleButtonClickEventArgs(titleItemAction.ToString(), titleItemAction, titleBarItem));
            }
        }
        #endregion

        #region public
        /// <summary>
        /// 刷新标题栏
        /// </summary>
        public void RefreshTitle()
        {
            this.Parent?.Invalidate();
        }
        #endregion

        internal void Draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (!this.isDrawTitle)
            {
                //g.Clear(this.Parent.BackColor);
                return;
            }

            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //g.Clear(Color.FromArgb(255, 255, 255, 255));

            g.FillRectangle(new SolidBrush(this.Theme.BackColor), new RectangleF(0, 0, this.Parent.ClientRectangle.Width, this.titleHeight));
            //Draw Icon
            int iconWidth = 0;
            if (this.Icon != null)
            {
                if (this.TitleHeight >= 16)
                {
                    iconWidth = 16 + 4;
                    g.DrawIcon(this.Icon, new Rectangle(4, (this.TitleHeight - 16) / 2, 16, 16));
                }
                else
                {
                    iconWidth = this.TitleHeight + 4;
                    g.DrawIcon(this.Icon, new Rectangle(4, 0, this.TitleHeight, this.TitleHeight));
                }
            }


            //Draw border
            using (Pen borderPen = new Pen(this.Theme.BorderColor, 1))
            {
                //g.DrawLine(borderPen, new Point(0, _actionBarBounds.Bottom), new Point(0, Height - 2));
                //g.DrawLine(borderPen, new Point(Width - 1, _actionBarBounds.Bottom), new Point(Width - 1, Height - 2));
                g.DrawLine(borderPen, new Point(0, this.titleHeight - 1), new Point(this.Parent.ClientRectangle.Width - 1, this.titleHeight - 1));
            }

            // Determine whether or not we even should be drawing the buttons.
            SolidBrush hoverBrush = new SolidBrush(this.Theme.TitleMouseOverBackColor);
            SolidBrush downBrush = new SolidBrush(this.Theme.TitleMouseDownBackColor);
            SolidBrush currentFillSolidBrush = null;
            foreach (SparkTitleBarItem item in this.items.Where(a => a.Visible))
            {
                #region 填充背景色
                if (this._buttonState > 0 && (int)item.TitleAction == (int)this._buttonState)
                {//Down

                    if (item.TitleAction == TitleItemAction.Max)
                    {
                        currentFillSolidBrush = downBrush;
                    }
                    g.FillRectangle(downBrush, item.Bounds);
                }
                else if ((int)item.TitleAction == (int)this._buttonState - 100)
                {//Over
                    g.FillRectangle(hoverBrush, item.Bounds);
                }
                else if (this._buttonState == ButtonState.Custom
                        && item.TitleAction == TitleItemAction.Custom
                        && item == this._customItem)
                {
                    g.FillRectangle(hoverBrush, item.Bounds);
                }
                #endregion

                #region 绘制按钮
                Color itemColor = this.Theme.BackColor == item.ItemColor ? this.Theme.ForeColor : item.ItemColor;
                using (Pen formButtonsPen = new Pen(Color.FromArgb(153, itemColor), 2))
                {
                    item.Draw(formButtonsPen, g, this, currentFillSolidBrush);
                }
                #endregion
            }

            //Form title
            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap))
            {
                IEnumerable<SparkTitleBarItem> visibleItems = this.items.Where(a => a.Visible);
                int rectWidth = visibleItems.Any() ? this.items.Where(a => a.Visible).Min(a => a.Bounds.X) : this.Parent.Width - 1;
                Rectangle rect = new Rectangle(iconWidth + 5, 0, rectWidth - iconWidth, this.TitleHeight);
                sf.Trimming = StringTrimming.Character;
                sf.Alignment = StringAlignment.Near; //水平左对齐（后面有需要可以开放
                sf.LineAlignment = StringAlignment.Center; //垂直中间对齐
                g.DrawString(this.Text,
                    this.TitleFont,
                    new SolidBrush(this.Theme.ForeColor),
                    rect,
                    sf
                );
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="isDispose"></param>
        protected virtual void Dispose(bool isDispose)
        {
            if (isDispose)
            {
                this.titleFont = null;
                this.customItem?.Clear();
                this.items?.Clear();
                this.icon = null;
            }
        }

        /// <summary>
        /// 释放接口
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}