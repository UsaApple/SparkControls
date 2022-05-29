using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 自定义列表
    /// </summary>
    [ToolboxBitmap(typeof(ListBox))]
    [ToolboxItem(true)]
    [DefaultEvent("ItemMouseClick")]
    [DefaultProperty("Items")]
    [Docking(DockingBehavior.Ask)]
    public class SparkList : ScrollableControl, ISparkTheme<SparkListTheme>
    {
        #region 字段
        private StringFormat defaultStringFormat = null;
        private int indent = 0;
        private Font defaultFont = Consts.DEFAULT_FONT;
        private int _translateTransformX = 0;//X的偏移量
        private int _translateTransformY = 0;//Y的偏移量
        internal bool hasVerticalVBar = false;//是否有垂直滚动条
        private bool isCalcItem = true;
        private float fontHeight = 0;
        private int itemHeight = 50;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;
        private int verticalSpacing = 2;
        private int horizontalSpacing = 2;
        /// <summary>
        /// 鼠标所在的节点(不是选中的)
        /// </summary>
        private SparkListItem _hoverItem = null;

        private SparkListItem selectedItem = null;
        private int _beginUpdateCount = 0;

        private const int VERTICAL_BAR_WIDTH = 18;
        #endregion

        #region 事件
        /// <summary>
        /// Item单击事件
        /// </summary>
        public event SparkListMouseClickEventHandler ItemMouseClick;

        /// <summary>
        /// 选中前事件
        /// </summary>
        public event SparkListCancelEventHandler BeforeSelect;

        /// <summary>
        /// 选中后事件
        /// </summary>
        public event SparkListEventHandler AfterSelect;
        #endregion

        #region 属性
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
                CalcFontHeight();
                this.ResetCalcItem();
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
            get => this.Theme.BackColor;
            set
            {
                base.BackColor = value;
                this.Theme.BackColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Category("Spark"), Description("控件的前景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
        public override Color ForeColor
        {
            get => this.Theme.ForeColor;
            set
            {
                base.ForeColor = value;
                this.Theme.ForeColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        ///  获取或设置树视图控件的边框样式。
        /// </summary>
        [DefaultValue(BorderStyle.FixedSingle)]
        public BorderStyle BorderStyle { get; set; } = BorderStyle.FixedSingle;

        /// <summary>
        /// <see cref="SparkListItem"/>的集合
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [MergableProperty(false)]
        public SparkListItemCollection Items { get; }

        /// <summary>
        /// 水平左右显示时的缩进距离
        /// </summary>
        [DefaultValue(0)]
        public int Indent
        {
            get => indent;
            set
            {
                indent = value;
                this.ResetCalcItem();
                this.Invalidate();
            }
        }

        /// <summary>
        /// 项目高度
        /// </summary>
        [DefaultValue(50)]
        public int ItemHeight
        {
            get => itemHeight;
            set
            {
                this.ResetCalcItem();
                itemHeight = value;
            }
        }
        /// <summary>
        /// 文本的对齐方式
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get => textAlign;
            set
            {
                this.ResetCalcItem();
                this.CalcDefalutStringFormat();
                textAlign = value;
            }
        }
        /// <summary>
        /// 文本和图像相对于彼此的位置
        /// </summary>
        [DefaultValue(TextImageRelation.ImageAboveText)]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                this.ResetCalcItem();
                this.CalcDefalutStringFormat();
                textImageRelation = value;
            }
        }
        /// <summary>
        /// 垂直间距
        /// </summary>
        [DefaultValue(2)]
        public int VerticalSpacing
        {
            get => verticalSpacing;
            set
            {
                this.ResetCalcItem();
                verticalSpacing = value;
            }
        }
        /// <summary>
        /// 水平间距
        /// </summary>
        [DefaultValue(2)]
        public int HorizontalSpacing
        {
            get => horizontalSpacing;
            set
            {
                this.ResetCalcItem();
                horizontalSpacing = value;
            }
        }

        /// <summary>
        /// 当前选中的节点
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SparkListItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem == value) return;

                bool isInvaidate = false;

                if (value == null)
                {//如果设置当前节点为null，则不触发任何事件
                    //上一个选中的项取消选中状态
                    if (selectedItem != null)
                    {
                        selectedItem.State ^= DrawItemState.Selected; //移除之前的选择项
                    }
                    selectedItem = null;
                    isInvaidate = true;
                }
                else
                {
                    if (selectedItem != null)
                    {
                        var args = new SparkListCancelEventArgs(selectedItem, false);
                        OnBeforeSelect(args);
                        if (args.Cancel) return;
                    }

                    //上一个选中的项取消选中状态
                    if (selectedItem != null)
                    {
                        selectedItem.State ^= DrawItemState.Selected; //移除之前的选择项
                    }
                    selectedItem = value;

                    selectedItem.State |= DrawItemState.Selected;//添加当前的选中项
                    OnAfterSelect(new SparkListEventArgs(selectedItem));
                    isInvaidate = true;
                }
                if (isInvaidate) this.Invalidate();
            }
        }

        /// <summary>
        /// 图标的大小
        /// </summary>
        [DefaultValue(typeof(SizeF), "16,16")]
        public SizeF ImageSize { get; set; } = new SizeF(16, 16);
        #endregion


        #region 构造函数
        /// <summary>
        /// <see cref="SparkList"/>的新实例
        /// </summary>
        public SparkList()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ContainerControl, false);

            this.UpdateStyles();

            this.Items = new SparkListItemCollection(this);
            this.Font = defaultFont;
            this.Theme = new SparkListTheme(this);
            base.BackColor = this.Theme.BackColor;
            CalcDefalutStringFormat();
            CalcFontHeight();
        }
        #endregion

        #region 重写
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.Clear(this.Theme.BackColor);

            if (_beginUpdateCount > 0) return;
            GDIHelper.InitializeGraphics(g);
            CalcItemBounds();
            SetAutoScrollMinSize();
            g.TranslateTransform(-_translateTransformX, -_translateTransformY);

            foreach (var item in this.Items)
            {
                OnDrawItem(new DrawItemEventArgs(g, this.Font, item.Bounds, this.Items.IndexOf(item), item.State));
            }

            g.ResetTransform();//恢复矩陈
            if (BorderStyle != BorderStyle.None && this.Theme.BorderColor != Color.Transparent)
            {
                GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
            }
        }

        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            OnDrawBackground(e);
            OnDrawImage(e);
            OnDrawText(e);
        }

        protected void OnDrawBackground(DrawItemEventArgs e)
        {
            var item = this.Items[e.Index];
            if (item != null)
            {
                Color backColor = GetBackColor(item);
                e.Graphics.FillRectangle(new SolidBrush(backColor), item.Bounds);

            }
        }

        protected override void OnResize(EventArgs e)
        {
            ResetCalcItem();
            base.OnResize(e);
        }

        protected void OnDrawImage(DrawItemEventArgs e)
        {
            var item = this.Items[e.Index];
            if (item != null && item.Image != null)
            {
                if (item.IsSelected || item == _hoverItem)
                {
                    e.Graphics.DrawImage(item.SelectedImage, item.ImageRectangle, new RectangleF(0, 0, item.Image.Width, item.Image.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    e.Graphics.DrawImage(item.Image, item.ImageRectangle, new RectangleF(0, 0, item.Image.Width, item.Image.Height), GraphicsUnit.Pixel);
                }
            }
        }

        protected void OnDrawText(DrawItemEventArgs e)
        {
            var item = this.Items[e.Index];
            if (item != null && !item.Text.IsNullOrEmpty())
            {
                Font font = item.ItemFont;
                if (font == null)
                {
                    font = e.Font;
                }
                var foreColor = GetForeColor(item);
                GDIHelper.DrawString(e.Graphics, item.TextRectangle, item.Text, font, foreColor, item.StringFormat ?? this.defaultStringFormat);
            }
        }

        #endregion

        #region 鼠标事件
        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverItem != null && _hoverItem != this.SelectedItem)
            {
                _hoverItem.State ^= DrawItemState.HotLight;
                _hoverItem = null;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var item = GetNodeAt(e.Location);
            if (item == null) return;
            if (item != _hoverItem)
            {
                if (_hoverItem != null)
                {
                    _hoverItem.State ^= DrawItemState.HotLight;
                }
                _hoverItem = item;
                item.State |= DrawItemState.HotLight;
                this.Invalidate();
            }
        }


        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
            var item = GetNodeAt(e.Location);
            if (item == null) return;
            //var mousePoint = GetMousePointByOffset(e.Location);
            OnItemMouseClick(new SparkListMouseClickEventArgs(item, e.Button, 1, e.X, e.Y, e.Delta));
            this.SelectedItem = item;
        }

        /// <summary>
        ///  引发 NodeMouseClick 事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemMouseClick(SparkListMouseClickEventArgs e)
        {
            this.ItemMouseClick?.Invoke(this, e);
        }

        /// <summary>
        ///  引发 BeforeSelect 事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeforeSelect(SparkListCancelEventArgs e)
        {
            this.BeforeSelect?.Invoke(this, e);
        }

        /// <summary>
        ///  引发 AfterSelect 事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterSelect(SparkListEventArgs e)
        {
            this.AfterSelect?.Invoke(this, e);
        }
        #endregion


        #region public
        /// <summary>
        /// 检索位于指定点的子项。
        /// </summary>
        /// <param name="pt"> 要从其计算和检索节点的 System.Drawing.Point。</param>
        /// <returns>位于以树视图（客户端）坐标表示的指定点的 SparkListItem，或在该位置没有节点时为 null。</returns>
        public SparkListItem GetNodeAt(Point pt)
        {
            if (this.Items.Count == 0) return null;
            var pt2 = new Point(pt.X, pt.Y + _translateTransformY);
            var node = this.Items.FirstOrDefault(a => new Rectangle(0, a.Bounds.Y, this.Width, a.Bounds.Height).Contains(pt2));
            return node;
        }


        /// <summary>
        /// 禁用任何树视图重绘
        /// </summary>
        public void BeginUpdate()
        {
            if (!IsHandleCreated)
            {
                return;
            }
            _beginUpdateCount++;
        }

        /// <summary>
        /// 启用树视图的重绘
        /// </summary>
        public void EndUpdate()
        {
            if (_beginUpdateCount > 0)
            {
                _beginUpdateCount--;
                if (_beginUpdateCount == 0)
                {
                    this.Invalidate();
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取鼠标偏移的坐标
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Point GetMousePointByOffset(Point pt)
        {
            return new Point(pt.X, pt.Y + _translateTransformY);
        }

        private RectangleF GetTextRectangle(SparkListItem item)
        {
            if (item.Text.IsNullOrEmpty())
            {
                return RectangleF.Empty;
            }
            else if (item.Image == null)
            {
                switch (this.TextAlign)
                {
                    case ContentAlignment.TopLeft:
                        break;
                    case ContentAlignment.TopCenter:
                        break;
                    case ContentAlignment.TopRight:
                        break;
                    case ContentAlignment.MiddleLeft:
                        break;
                    case ContentAlignment.MiddleCenter:
                        //暂时只支持居中
                        //和默认不一样了
                        item.StringFormat = new StringFormat()
                        {
                            Trimming = StringTrimming.None,
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            FormatFlags = StringFormatFlags.NoWrap,
                        };
                        return item.Bounds;
                    case ContentAlignment.MiddleRight:
                        break;
                    case ContentAlignment.BottomLeft:
                        break;
                    case ContentAlignment.BottomCenter:
                        break;
                    case ContentAlignment.BottomRight:
                        break;
                }
                return RectangleF.Empty;
            }
            switch (this.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    break;
                case ContentAlignment.TopCenter:
                    break;
                case ContentAlignment.TopRight:
                    break;
                case ContentAlignment.MiddleLeft:
                    break;
                case ContentAlignment.MiddleCenter:
                    //暂时只支持居中方式
                    switch (this.TextImageRelation)
                    {
                        case TextImageRelation.Overlay:
                            break;
                        case TextImageRelation.ImageBeforeText:
                            break;
                        case TextImageRelation.TextBeforeImage:
                            break;
                        case TextImageRelation.ImageAboveText:
                            item.StringFormat = null;
                            //暂时只支持图片在文字的上方
                            //上边距 =（ Item高度-（图片高度+gay+文字高度））/2 + 图片高度+gay
                            //计算文字矩形的大小
                            var topGap = (this.ItemHeight - this.ImageSize.Height - VerticalSpacing - fontHeight) / 2 + this.ImageSize.Height + VerticalSpacing;
                            var rectangle = new RectangleF(0, item.Bounds.Y + topGap, item.Bounds.Width, this.ItemHeight - topGap);
                            return rectangle;
                        case TextImageRelation.TextAboveImage:
                            break;
                    }
                    break;
                case ContentAlignment.MiddleRight:
                    break;
                case ContentAlignment.BottomLeft:
                    break;
                case ContentAlignment.BottomCenter:
                    break;
                case ContentAlignment.BottomRight:
                    break;
            }
            return RectangleF.Empty;
        }

        private RectangleF GetImageRectangle(SparkListItem item)
        {
            if (item.Image == null)
            {
                return RectangleF.Empty;
            }
            else if (item.Text.IsNullOrEmpty())
            {
                switch (this.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        break;
                    case TextImageRelation.ImageBeforeText:
                        break;
                    case TextImageRelation.TextBeforeImage:
                        break;
                    case TextImageRelation.ImageAboveText:
                        //暂时只支持居中
                        var y = (1.0f * this.ItemHeight - this.ImageSize.Height) / 2;
                        var x = (1.0f * item.Bounds.Width - this.ImageSize.Width) / 2;
                        var rect = new RectangleF(x, item.Bounds.Y + y, this.ImageSize.Width, this.ImageSize.Height);
                        return rect;
                    case TextImageRelation.TextAboveImage:
                        break;
                }
                return RectangleF.Empty;
            }
            switch (this.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    break;
                case ContentAlignment.TopCenter:
                    break;
                case ContentAlignment.TopRight:
                    break;
                case ContentAlignment.MiddleLeft:
                    break;
                case ContentAlignment.MiddleCenter:
                    //暂时只支持居中方式
                    switch (this.TextImageRelation)
                    {
                        case TextImageRelation.Overlay:
                            break;
                        case TextImageRelation.ImageBeforeText:
                            break;
                        case TextImageRelation.TextBeforeImage:
                            break;
                        case TextImageRelation.ImageAboveText:
                            //暂时只支持图片在文字的上方
                            //上边距 =（ Item高度-（图片高度+gay+文字高度））/2x
                            var topGap = (1.0f * this.ItemHeight - this.ImageSize.Height - VerticalSpacing - fontHeight) / 2;
                            var x = (1.0f * item.Bounds.Width - this.ImageSize.Width) / 2;
                            var rect = new RectangleF(x, item.Bounds.Y + topGap, this.ImageSize.Width, this.ImageSize.Height);
                            return rect;
                        case TextImageRelation.TextAboveImage:
                            break;
                    }
                    break;
                case ContentAlignment.MiddleRight:
                    break;
                case ContentAlignment.BottomLeft:
                    break;
                case ContentAlignment.BottomCenter:
                    break;
                case ContentAlignment.BottomRight:
                    break;
            }
            return RectangleF.Empty;
        }

        private void CalcItemBounds()
        {
            _translateTransformY = this.VerticalScroll.Value; //垂直滚动的高度
            if (!isCalcItem) return;
            hasVerticalVBar = this.ItemHeight * this.Items.Count > this.Height;
            int y = 0;
            foreach (var item in this.Items)
            {
                item.Bounds = new Rectangle(0, y, this.Width - hasVerticalVBar.IIF(VERTICAL_BAR_WIDTH, 0), this.ItemHeight);
                y += this.ItemHeight;
                item.TextRectangle = GetTextRectangle(item);
                item.ImageRectangle = GetImageRectangle(item);
            }
            isCalcItem = false;
        }

        internal void ResetCalcItem()
        {
            isCalcItem = true;
        }

        private void CalcDefalutStringFormat()
        {
            switch (this.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    break;
                case ContentAlignment.TopCenter:
                    break;
                case ContentAlignment.TopRight:
                    break;
                case ContentAlignment.MiddleLeft:
                    break;
                case ContentAlignment.MiddleCenter:
                    //暂时只支持居中对齐
                    switch (this.TextImageRelation)
                    {
                        case TextImageRelation.Overlay:
                            break;
                        case TextImageRelation.ImageBeforeText:
                            break;
                        case TextImageRelation.TextBeforeImage:
                            break;
                        case TextImageRelation.ImageAboveText:
                            //暂时只支持图片在文字上方
                            this.defaultStringFormat = new StringFormat()
                            {
                                Trimming = StringTrimming.None,
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Near,
                                FormatFlags = StringFormatFlags.NoWrap,
                            };
                            break;
                        case TextImageRelation.TextAboveImage:
                            break;
                    }
                    break;
                case ContentAlignment.MiddleRight:
                    break;
                case ContentAlignment.BottomLeft:
                    break;
                case ContentAlignment.BottomCenter:
                    break;
                case ContentAlignment.BottomRight:
                    break;
            }

        }

        private void CalcFontHeight()
        {
            fontHeight = GDIHelper.MeasureString(this.CreateGraphics(), "你好", this.Font).Height;
        }

        /// <summary>
        /// 获取节点背景色
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Color GetBackColor(SparkListItem item)
        {
            Color backColor;
            if (item.IsSelected)
            {
                //选中，或者获取焦点
                backColor = this.Theme.SelectedBackColor;
            }
            else if (item.State.HasFlag(DrawItemState.HotLight))
            {
                //鼠标在节点上
                backColor = this.Theme.MouseOverBackColor;
            }
            else
            {
                backColor = item.BackColor.IsEmpty ? this.Theme.BackColor : item.BackColor;
            }
            return backColor;
        }

        private Color GetForeColor(SparkListItem item)
        {
            Color foreColor;
            if (item.IsSelected)
            {
                //选中，或者获取焦点
                foreColor = this.Theme.SelectedForeColor;
            }
            else if (item.State.HasFlag(DrawItemState.HotLight))
            {
                //鼠标在节点上
                foreColor = this.Theme.MouseOverForeColor;
            }
            else
            {
                foreColor = item.ForeColor.IsEmpty ? this.Theme.ForeColor : item.ForeColor;
            }
            return foreColor;
        }

        /// <summary>
        /// 设置滚动区域
        /// </summary>
        internal void SetAutoScrollMinSize()
        {
            //设置滚动条
            var lastNode = Items.LastOrDefault();
            if (lastNode != null && lastNode.Bounds.Bottom > this.Height)
            {
                hasVerticalVBar = true;
                //设置滚动条
                this.AutoScrollMinSize = new Size(this.Width - VERTICAL_BAR_WIDTH, lastNode.Bounds.Bottom + 10);
            }
            else
            {
                hasVerticalVBar = false;
                this.AutoScrollMinSize = new Size(this.Width, this.Height);
            }
        }
        #endregion

        #region ISparkTheme 接口
        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkListTheme Theme { get; private set; }
        #endregion
    }



}
