using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    [ToolboxItem(false)]
    [Docking(DockingBehavior.Never)]
    [System.ComponentModel.DesignerCategory("Code")]
    public class SparkNavigateBarPanel : Panel
    {
        #region 字段
        private SparkNavigationBarStripButton _button;
        private readonly bool _backColorAssigned;
        private Font titleFont = new Font("微软雅黑", 12);
        private int titleHeight = 32;
        private readonly SparkTitleBarItem _closeButton = null;
        //private SparkTitleBar _titleBar = null;
        #endregion

        #region 属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override AnchorStyles Anchor
        {
            get { return base.Anchor; }
            set { base.Anchor = value; }
        }

        [Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public override void ResetBackColor()
        //{
        //    var tabControl = Parent as SparkNavigationBar;

        //    if (tabControl != null)
        //    {
        //        base.BackColor = tabControl.BackColor;
        //    }
        //    _backColorAssigned = false;
        //}

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBackColor()
        {
            return this._backColorAssigned;
        }

        [Browsable(true)]
        [DefaultValue(null)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Image
        {
            get { return this.TabStripButton.Image; }
            set { this.TabStripButton.Image = value; }
        }

        [Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set { base.MaximumSize = value; }
        }

        [Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set { base.MinimumSize = value; }
        }

        /// <summary>
        /// 设置显示的文本，\r\n表示换行
        /// </summary>
        [Description("设置显示的文本，\\r\\n表示换行")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                //if (_titleBar != null)
                //{
                //    _titleBar.Text = value.Replace("\\r\\n", "");
                //}
            }
        }
        [Obsolete("",true)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SparkNavigationBar TabControl
        {
            get { return this.Parent as SparkNavigationBar; }
        }

        [Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }

        [Browsable(false)]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                //if (_titleBar != null)
                //{
                //    _titleBar.Theme.BackColor = value;
                //}
            }
        }

        #region 标题栏属性
        /// <summary>
        /// 标题字体
        /// </summary>
        [DefaultValue(typeof(Font), "微软雅黑,12pt")]
        [Description("标题字体")]
        public Font TitleFont
        {
            get => this.titleFont;
            set
            {
                if (this.titleFont != value)
                {
                    this.titleFont = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 标题栏高度
        /// </summary>
        [DefaultValue(32)]
        [Description("标题栏高度")]
        public int TitleHeight
        {
            get => this.titleHeight;
            set
            {
                if (this.titleHeight != value)
                {
                    if (value < 0)
                    {
                        return;
                    }
                    this.titleHeight = value;
                    this.Padding = new Padding(0, value, 0, 0);
                    this.CaclButtonBounds();
                    this.Invalidate();
                }
            }
        }
        #endregion
        #endregion

        #region 事件
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler TextChanged
        {
            add { base.TextChanged += value; }
            remove { base.TextChanged -= value; }
        }
        #endregion

        #region 构造
        public SparkNavigateBarPanel()
        {
            //_titleBar = new SparkTitleBar();
            //_titleBar.EnableTheming = false;
            //_titleBar.Dock = DockStyle.Top;
            //_titleBar.MaximizeBox = false;
            //_titleBar.MinimizeBox = false;
            //_titleBar.Height = _titleBar.TitleHeight;
            //_titleBar.Text = this.Text;
            //_titleBar.IsRegisteredSystemButtonClick = false;
            //_titleBar.Theme.BackColor = System.Drawing.Color.White;
            //_titleBar.Theme.ForeColor = Color.FromArgb(0, 186, 131);
            //_titleBar.TitleButtonClick += _titleBar_TitleButtonClick;
            //this.Controls.Add(_titleBar);
            this._closeButton = new SparkTitleBarItem(TitleItemAction.Close);
            this.CaclButtonBounds();
            this.Padding = new Padding(0, this.TitleHeight, 0, 0);
        }
        #endregion

        #region protected override
        protected override void OnParentChanged(EventArgs e)
        {
            if (!this._backColorAssigned)
            {
                this.ResetBackColor();
            }
            base.OnParentChanged(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this._button != null)
            {
                this._button.Text = this.Text;
            }

            base.OnTextChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //绘制标题和关闭按钮
            Graphics g = e.Graphics;
            GDIHelper.InitializeGraphics(g);

            string text = this.Text.Replace("\\r\\n", "");
            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap))
            {
                Rectangle rect = new Rectangle(5, 0, this.Width - this.TitleHeight - 5, this.TitleHeight);
                sf.Trimming = StringTrimming.Character;
                sf.Alignment = StringAlignment.Near;//水平左对齐（后面有需要可以开放
                sf.LineAlignment = StringAlignment.Center;//垂直中间对齐
                g.DrawString(text,
                this.TitleFont,
                new SolidBrush(this.TabControl.Theme.TitleFontColor),
                rect,
                sf);
            }
            this.CaclButtonBounds();
            //绘制关闭按钮
            this._closeButton.Draw(Pens.Black, g, null, null);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.HitTest(e.Location))
            {
                this.OnCloseButton(e);
            }
        }

        protected virtual void OnCloseButton(EventArgs e)
        {
            this.TabControl.Expanded = false;
        }
        #endregion

        #region internal方法
        internal void ButtonInit(SparkNavigationBar bar)
        {
            if (bar == null) return;
            switch (bar.Alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    this.TabStripButton.TextImageRelation = TextImageRelation.ImageBeforeText;
                    break;
                case TabAlignment.Left:
                case TabAlignment.Right:
                    this.TabStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
                    break;
            }

            switch (bar.DisplayStyle)
            {
                case NavigationBarDisplayStyle.Image:
                    this.TabStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    break;
                case NavigationBarDisplayStyle.Text:
                    this.TabStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    break;
                case NavigationBarDisplayStyle.ImageAndText:
                    this.TabStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    break;
            }
        }

        internal SparkNavigationBarStripButton TabStripButton
        {
            get
            {
                if (this._button == null)
                {
                    this._button = new NavigationPageButton(this)
                    {
                        Image = Properties.Resources.NavigateBarButton
                    };
                }

                return this._button;
            }
        }

        internal static SparkNavigateBarPanel GetButtonPage(ToolStripItem button)
        {
            NavigationPageButton pageButton = button as NavigationPageButton;

            if (pageButton != null)
            {
                return pageButton.TabPage;
            }

            return null;
        }

        #endregion

        #region 私有方法
        private void CaclButtonBounds()
        {
            this._closeButton.Bounds = new Rectangle(this.Width - this.TitleHeight, 0, this.TitleHeight, this.TitleHeight);
        }

        private bool HitTest(Point point)
        {
            if (this._closeButton.Bounds.Contains(point))
            {
                return true;
            }
            return false;
        }
        #endregion

        private sealed class NavigationPageButton : SparkNavigationBarStripButton
        {
            private readonly SparkNavigateBarPanel _page;

            public NavigationPageButton(SparkNavigateBarPanel page)
                : base(page.Text)
            {
                this._page = page;
                this.AutoToolTip = false;
            }


            public SparkNavigateBarPanel TabPage
            {
                get { return this._page; }
            }

           
            public SparkNavigationBar TabControl
            {
                get { return this._page.TabControl; }
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                this.TabControl.SelectedTab = this.TabPage;
            }
        }
    }
}