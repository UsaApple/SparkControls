using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SparkControls.Controls
{
    [Description("卡片布局")]
    [Docking(DockingBehavior.Ask)]
    public partial class SparkCardList : ScrollableControl
    {
        [DllImport("user32", EntryPoint = "HideCaret")]
        private static extern bool HideCaret(IntPtr hWnd);

        #region 事件定义

        /// <summary>
        /// 当前卡片索引变更时发生。
        /// </summary>
        public event EventHandler SelectedIndexChanged;
        /// <summary>
        /// 鼠标双击时发生。
        /// </summary>
        public new event SparkCardEventHandler DoubleClick;
        /// <summary>
        /// 绘制卡片时发生。
        /// </summary>
        public event SparkCardDrawItemEventHandler DrawItem;

        #endregion

        #region 成员变量

        private readonly int _scrollBarWidth = 17;

        private int _hoverIndex = -1;

        private bool _paintForced = false;

        private SparkCardTemplate _template = null;
        private ContextMenuStrip _contextMenuStrip = null;

        private float _translateTransformValue = 0;//偏移值
        List<SparkCard> _drawSparkCardList = new List<SparkCard>();//当前绘制的卡片

        /// <summary>
        /// 当前点击选中的对象
        /// </summary>
        private SparkCard _selectedCard = null;

        /// <summary>
        /// 鼠标浮动的项
        /// </summary>
        private SparkCard _hoverCard = null;

        private bool _isInitDataSource = false;//是否在初始化，true在，false不在
        private bool _isResetCardSize = false; //重置卡片大小标识

        private DataView _dataSource = null;
        private Orientation orientation = Orientation.Horizontal;
        private CardLayout layoutMode = CardLayout.Wrap;
        private int horizontalSpacing = 10;
        private int verticalSpacing = 10;
        private int horizontalSpacingFill = 10;
        private int verticalSpacingFill = 10;
        #endregion

        #region 成员属性

        private int CardWidth => _template.Width + this.HorizontalSpacing;

        private int CardHeight => _template.Height + this.VerticalSpacing;

        /// <summary>
        /// 获取或设置卡片绘制间距的方式,只有在LayoutMode=CardLayout.Wrap生效
        /// </summary>
        [DefaultValue(CardDrawSpacingMode.Fill)]
        [Category("Spark"), Description("获取或设置卡片绘制间距的方式,只有在LayoutMode=CardLayout.Wrap生效。")]
        public CardDrawSpacingMode CardDrawSpacingMode { get; set; } = CardDrawSpacingMode.Fill;

        /// <summary>
        /// 获取或设置控件的数据源。
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public DataView DataSource
        {
            get => this._dataSource;
            set
            {
                this._dataSource = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// 获取卡片集合。
        /// </summary>
        [Browsable(false)]
        public List<SparkCard> Cards { get; } = new List<SparkCard>();

        /// <summary>
        /// 获取当前选中的卡片实例。
        /// </summary>
        [Browsable(false)]
        public SparkCard SelectedCard => _selectedCard;

        /// <summary>
        /// 获取或设置卡片之间的水平间隔，单位：像素
        /// </summary>
        [Category("Spark"), Description("获取或设置卡片之间的水平间隔，单位：像素。")]
        [DefaultValue(10)]
        public int HorizontalSpacing
        {
            get => this.LayoutMode == CardLayout.Wrap && CardDrawSpacingMode == CardDrawSpacingMode.Fill ? horizontalSpacingFill : horizontalSpacing;
            set => horizontalSpacing = value;
        }

        /// <summary>
        /// 获取或设置卡片之间的垂直间隔，单位：像素。
        /// </summary>
        [Category("Spark"), Description("获取或设置卡片之间的垂直间隔，单位：像素。")]
        [DefaultValue(10)]
        public int VerticalSpacing
        {
            get => this.LayoutMode == CardLayout.Wrap && CardDrawSpacingMode == CardDrawSpacingMode.Fill ? verticalSpacingFill : verticalSpacing;
            set => verticalSpacing = value;
        }

        /// <summary>
        /// 获取或设置卡片的布局方式。
        /// </summary>
        [Category("Spark"), Description("获取或设置卡片的布局方式。")]
        [DefaultValue(CardLayout.Wrap)]
        public CardLayout LayoutMode
        {
            get => layoutMode;
            set
            {
                if (layoutMode != value)
                {
                    SetResetCardSizeFlag();
                    layoutMode = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置卡片的布局方向。
        /// </summary>
        [Category("Spark"), Description("获取或设置卡片的布局方向。")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                if (orientation != value)
                {
                    SetResetCardSizeFlag();
                    orientation = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置是否可以选中Lable的文本内容，true可以，false不可以
        /// </summary>
        [Category("Spark"), Description("获取或设置是否可以选中Lable的文本内容，true可以，false不可以。")]
        [DefaultValue(false)]
        public bool SelectLableText { get; set; }
        #endregion

        #region 构造函数

        /// <summary>
        /// 初始类型 <see cref="SparkCardList"/> 的新实例。
        /// </summary>
        public SparkCardList()
        {
            this.InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();
            this.textBox.GotFocus += (sender, e) => HideCaret(this.textBox.Handle);
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 初始化控件。
        /// </summary>
        public void Init()
        {
            this.Init(null);
        }

        /// <summary>
        /// 初始化控件。
        /// </summary>
        /// <param name="contextMenuStrip">上下文菜单。</param>
        public void Init(ContextMenuStrip contextMenuStrip = null)
        {
            Assembly assembly = this.GetType().Assembly;
            XElement template = XElement.Load(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Controls.SparkCard.Default.xml"));
            this.Init(template, contextMenuStrip);
        }

        /// <summary>
        /// 初始化控件。
        /// </summary>
        /// <param name="template">卡片模板。</param>
        /// <param name="contextMenuStrip">上下文菜单。</param>
        public void Init(XElement template, ContextMenuStrip contextMenuStrip = null)
        {
            this._template = SparkCardTemplate.FromXml(template);
            this._contextMenuStrip = contextMenuStrip;
        }

        /// <summary>
        /// 根据主键值定位行并选中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="colName"></param>
        public void SelectCard(string key, string colName)
        {
            if (this.Cards?.Any() == true && this.DataSource != null && this.DataSource.Table.HasColumn(colName))
            {
                var item = this.Cards.FirstOrDefault(a => (a.DataSource[colName] ?? "").ToString() == key);
                SelectCard(item);
            }
        }

        /// <summary>
        /// 根据DataRow来定位行并选中
        /// </summary>
        /// <param name="dr"></param>
        public void SelectCard(DataRowView dr)
        {
            if (this.Cards?.Any() == true)
            {
                var item = this.Cards.FirstOrDefault(a => a.DataSource == dr);
                SelectCard(item);
            }
        }

        /// <summary>
        /// 根据索引号选中卡片
        /// </summary>
        /// <param name="index"></param>
        public void SelectCard(int index)
        {
            if (this.Cards?.Any() == true && this.Cards.Count > index && index >= 0)
            {
                var item = this.Cards[index];
                SelectCard(item);
            }
        }

        #endregion

        #region 私有方法

        private void SetScrollValue(int value)
        {
            if (value < this.VerticalScroll.Maximum)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.VerticalScroll.Value = value;
                }
                else
                {
                    this.HorizontalScroll.Value = value;
                }
                this.PerformLayout();
            }
        }

        private void PaintForced()
        {
            try
            {
                //重新创建SparkCard集合
                CreateSparkCards();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PaintSlient(Graphics g)
        {
            try
            {
                CalcSpacing();
                _drawSparkCardList = GetDrawCardList();
                if (_drawSparkCardList?.Any() != true) return;
                int maxWidth = 0, maxHeight = 0;
                int x = this.HorizontalSpacing, y = this.VerticalSpacing;

                for (int i = 0; i < _drawSparkCardList.Count; i++)
                {
                    SparkCard card = this._drawSparkCardList[i];
                    if (_isResetCardSize)
                    {
                        card.ResetSize();
                    }
                    if (this.LayoutMode == CardLayout.Stack)
                    {
                        card.Size = this.Orientation == Orientation.Horizontal ?
                            new Size(this.Width - this._scrollBarWidth - this.HorizontalSpacing * 2, card.Size.Height) :
                            new Size(card.Size.Width, this.Height - this._scrollBarWidth - this.VerticalSpacing * 2)
                            ;
                    }

                    if (i > 0)
                    {
                        SparkCard prev = this._drawSparkCardList[i - 1];
                        if (this.LayoutMode == CardLayout.Wrap)
                        {
                            if (this.Orientation == Orientation.Horizontal)
                            {
                                x = prev.Location.X + prev.Size.Width + this.HorizontalSpacing;
                                if (x + card.Size.Width > this.Width)
                                {
                                    x = this.HorizontalSpacing;
                                    y += maxHeight + this.VerticalSpacing;
                                    maxHeight = 0;
                                }
                            }
                            else
                            {
                                y = prev.Location.Y + prev.Size.Height + this.VerticalSpacing;
                                if (y + card.Size.Height > this.Height)
                                {
                                    x += maxWidth + this.HorizontalSpacing;
                                    y = this.VerticalSpacing;
                                    maxWidth = 0;
                                }
                            }
                        }
                        else if (this.LayoutMode == CardLayout.Stack)
                        {
                            if (this.Orientation == Orientation.Vertical)
                            {
                                x = prev.Location.X + prev.Size.Width + this.HorizontalSpacing;
                            }
                            else
                            {
                                y = prev.Location.Y + prev.Size.Height + this.VerticalSpacing;
                            }
                        }
                    }

                    if (this.LayoutMode == CardLayout.Wrap)
                    {
                        if (this.Orientation == Orientation.Horizontal)
                        {
                            if (card.Size.Height > maxHeight) { maxHeight = card.Size.Height; }
                        }
                        else
                        {
                            if (card.Size.Width > maxWidth) { maxWidth = card.Size.Width; }
                        }
                    }
                    card.Location = new Point(x, y);
                }

                //if (this._totalHeight > this.Height)
                //{
                //    // 这里 _scrollBarWidth 为滚动条的宽度
                //    this.AutoScrollMinSize = new Size(this.Width - this._scrollBarWidth, this._totalHeight);
                //}
                //else if (this._totalWidth > this.Width)
                //{
                //    // 这里 _scrollBarWidth 为滚动条的宽度
                //    this.AutoScrollMinSize = new Size(this._totalWidth, this.Height - this._scrollBarWidth);
                //}
                //else
                //{
                //    this.AutoScrollMinSize = new Size(this.Width, this.Height);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SelectCard(SparkCard item)
        {
            if (item != null)
            {
                int index = this.Cards.IndexOf(item);
                if (index >= 0)
                {
                    var value = 0d;
                    var cols = Math.Max(1, GetColumnNum());
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        //根据个数,算滚动的位置
                        value = this.VerticalSpacing + (Math.Floor(1.0F * index / cols)) * CardHeight;
                    }
                    else
                    {
                        value = this.HorizontalSpacing + (Math.Floor(1.0F * index / cols)) * CardWidth;
                    }
                    if (_selectedCard != null)
                    {
                        _selectedCard.IsActived = false;
                    }

                    _selectedCard = item;
                    _selectedCard.IsActived = true;
                    SetScrollValue((int)value);
                    this.Invalidate();
                }
            }
        }

        private void SelectDrawCard(int key)
        {
            if (_isInitDataSource) return;
            try
            {
                SparkCard tempCard = null;
                if (this._drawSparkCardList.Count > 0 && key >= 0 && this._drawSparkCardList.Count > key)
                {
                    tempCard = this._drawSparkCardList[key];
                }
                if (tempCard != _selectedCard)
                {
                    if (_selectedCard != null)
                    {
                        _selectedCard.IsActived = false;
                    }
                    _selectedCard = tempCard;
                    if (tempCard != null)
                    {
                        tempCard.IsActived = true;
                    }
                    this.Invalidate();
                    this.SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SetAutoScrollMinSize(Size size)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => { SetAutoScrollMinSize(size); }));
            }
            else
            {
                this.AutoScrollMinSize = size;
            }
        }

        private void SetAutoScrollMinSize()
        {
            Size size = Size.Empty;

            var count = this.Cards.Count();

            double totalWidth = 0;
            double totalHeight = 0;
            var cols = Math.Max(1, this.GetColumnNum());
            if (this.Orientation == Orientation.Horizontal)
            {
                totalHeight = Math.Ceiling(1.0f * count / cols) * (this.CardHeight) + this.VerticalSpacing;
                totalWidth = 0;
            }
            else
            {
                totalHeight = 0;
                totalWidth = Math.Ceiling(1.0f * count / cols) * (this.CardWidth) + this.HorizontalSpacing;
            }

            if (totalHeight > this.Height)
            {
                // 这里 _scrollBarWidth 为滚动条的宽度
                size = new Size(this.Width - this._scrollBarWidth, (int)totalHeight);
            }
            else if (totalWidth > this.Width)
            {
                // 这里 _scrollBarWidth 为滚动条的宽度
                size = new Size((int)totalWidth, this.Height - this._scrollBarWidth);
            }
            else
            {
                size = new Size(this.Width, this.Height);
            }
            SetAutoScrollMinSize(size);
        }

        /// <summary>
        /// 可以绘制的列数
        /// </summary>
        /// <returns></returns>
        private int GetColumnNum()
        {
            if (LayoutMode == CardLayout.Wrap)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    return (this.Width - this.HorizontalSpacing - _scrollBarWidth) / (CardWidth);
                }
                else
                {
                    return (this.Height - this.VerticalSpacing - _scrollBarWidth) / (CardHeight);
                }
            }
            return 1;
        }

        private int GetRowNum()
        {
            double count = 1;
            if (Orientation == Orientation.Horizontal)
            {
                count = Math.Ceiling(1.0f * (this.Height - this.VerticalSpacing - _scrollBarWidth) / (CardHeight));
            }
            else
            {
                count = Math.Ceiling(1.0f * (this.Width - this.HorizontalSpacing - _scrollBarWidth) / (CardWidth));
            }
            return (int)count;
        }

        private List<SparkCard> GetDrawCardList()
        {
            //动态算出当前画哪些
            List<SparkCard> items = null;
            var colNum = Math.Max(1, GetColumnNum()); //页面有几列
            var rowNum = Math.Max(1, GetRowNum()); //页面有几行
            if (Orientation == Orientation.Horizontal)
            {
                var scrollValue = this.VerticalScroll.Value; //滚动的高度

                var scrollRow = 1.0f * scrollValue / CardHeight; //在第几行开始==>5.1
                var scrollRowInt = (int)Math.Floor(scrollRow); //5.1=>5
                var scrllRowMax = (int)Math.Ceiling(scrollRow);//5.1=>6

                _translateTransformValue = (scrollRow - scrollRowInt) * CardHeight;
                items = this.Cards.Where((a, index) => index >= scrollRowInt * colNum && index < (scrllRowMax + rowNum) * colNum).ToList();
            }
            else
            {
                var scrollValue = this.HorizontalScroll.Value;
                var scrollRow = 1.0f * scrollValue / CardWidth;
                var scrollRowInt = (int)Math.Floor(scrollRow);
                var scrllRowMax = (int)Math.Ceiling(scrollRow);
                _translateTransformValue = (scrollRow - scrollRowInt) * CardHeight;
                items = this.Cards.Where((a, index) => index >= scrollRowInt * colNum && index < (scrllRowMax + rowNum) * colNum).ToList();
            }
            return items;
        }

        private void CreateSparkCards()
        {
            if (this.DataSource == null || this.DataSource.Count < 1) { return; }
            int maxWidth = 0, maxHeight = 0;
            CalcSpacing();
            int x = this.HorizontalSpacing, y = this.VerticalSpacing;
            for (int i = 0; i < this.DataSource.Count; i++)
            {
                SparkCard card = new SparkCard(this._template, this.DataSource[i]);
                if (card == null) { continue; }

                if (this.LayoutMode == CardLayout.Stack)
                {
                    card.Size = this.Orientation == Orientation.Horizontal ?
                        new Size(this.Width - this._scrollBarWidth - this.HorizontalSpacing * 2, card.Size.Height) :
                        new Size(card.Size.Width, this.Height - this._scrollBarWidth - this.VerticalSpacing * 2);
                }

                if (i > 0)
                {
                    SparkCard prev = this.Cards[i - 1];
                    if (this.LayoutMode == CardLayout.Wrap)
                    {
                        if (this.Orientation == Orientation.Horizontal)
                        {
                            x = prev.Location.X + prev.Size.Width + this.HorizontalSpacing;
                            if (x + card.Size.Width > this.Width)
                            {
                                x = this.HorizontalSpacing;
                                y += maxHeight + this.VerticalSpacing;
                                maxHeight = 0;
                            }
                        }
                        else
                        {
                            y = prev.Location.Y + prev.Size.Height + this.VerticalSpacing;
                            if (y + card.Size.Height > this.Height)
                            {
                                x += maxWidth + this.HorizontalSpacing;
                                y = this.VerticalSpacing;
                                maxWidth = 0;
                            }
                        }
                    }
                    else if (this.LayoutMode == CardLayout.Stack)
                    {
                        if (this.Orientation == Orientation.Horizontal)
                        {
                            x = prev.Location.X + prev.Size.Width + this.HorizontalSpacing;
                        }
                        else
                        {
                            y = prev.Location.Y + prev.Size.Height + this.VerticalSpacing;
                        }
                    }
                }

                if (this.LayoutMode == CardLayout.Wrap)
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        if (card.Size.Height > maxHeight)
                        {
                            maxHeight = card.Size.Height;
                        }
                    }
                    else
                    {
                        if (card.Size.Width > maxWidth)
                        {
                            maxWidth = card.Size.Width;
                        }
                    }
                }

                card.Location = new Point(x, y);
                //card.DataSource = this.DataSource[i];
                // 添加到缓存
                this.Cards.Add(card);
            }
        }

        /// <summary>
        /// 因为采取了异步刷新，在刷新的过程中,去设置LayoutMode,Orientation属性,导致卡片的大小是不正确的
        /// 所以需要设置标识,标识需要重新设置大小
        /// </summary>
        private void SetResetCardSizeFlag()
        {
            _isResetCardSize = true;
        }

        /// <summary>
        /// 计算间距
        /// </summary>
        private void CalcSpacing()
        {
            if (CardDrawSpacingMode == CardDrawSpacingMode.Fill && this.LayoutMode == CardLayout.Wrap)
            {
                horizontalSpacingFill = 0;
                verticalSpacingFill = 0;
                if (Orientation == Orientation.Horizontal)
                {
                    int index = (this.Width - _scrollBarWidth) / _template.Width;
                    if (index > 0)
                    {
                        var difference = (this.Width - index * _template.Width - _scrollBarWidth) / (index + 1);
                        if (difference < 0)
                        {
                            horizontalSpacingFill = (this.Width - (index - 1) * _template.Width - _scrollBarWidth) / index;
                        }
                        else
                        {
                            horizontalSpacingFill = difference;
                        }
                    }
                    else
                    {
                        horizontalSpacingFill = horizontalSpacing;
                    }
                    verticalSpacingFill = verticalSpacing;
                }
                else
                {
                    int index = (this.Height - _scrollBarWidth) / _template.Height;
                    if (index > 0)
                    {
                        var difference = (this.Height - index * _template.Height - _scrollBarWidth) / (index + 1);
                        if (difference < 0)
                        {
                            verticalSpacingFill = (this.Height - (index - 1) * _template.Height - _scrollBarWidth) / index;
                        }
                        else
                        {
                            verticalSpacingFill = difference;
                        }
                    }
                    else
                    {
                        verticalSpacingFill = verticalSpacing;
                    }
                    horizontalSpacingFill = horizontalSpacing;

                }
            }
        }
        #endregion

        #region 重写方法

        /// <summary>
        /// <para>强制控件使其工作区无效并立即重绘自己和任何子控件</para>
        /// <para>如果未改变数据源或数据源内部的值，请使用Invalidate方法进行重绘，可以提高性能</para>
        /// </summary>
        public override void Refresh()
        {
            if (_isInitDataSource) return;
            this.toolTip.Hide(this);
            this.AutoScrollPosition = new Point(0, 0);
            _selectedCard = null;
            _hoverCard = null;
            _drawSparkCardList.Clear();
            Cards.Clear();
            _translateTransformValue = 0;
            this._paintForced = true;
            this._hoverIndex = -1;
            _isInitDataSource = true;
            PaintForced();
            _isInitDataSource = false;
            base.Invalidate();
        }

        /// <summary>
        /// 触发双击事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (_isInitDataSource) return;
            if (DoubleClick != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var mouse = e.Location;

                    if (this.Orientation == Orientation.Horizontal) mouse.Y = e.Y + (int)_translateTransformValue;
                    else mouse.X = e.X + (int)_translateTransformValue;
                    try
                    {
                        int y = e.Y + this.VerticalScroll.Value;
                        SparkCard card = this._drawSparkCardList.FirstOrDefault(c => c.Rectangle.Contains(mouse));
                        if (card != null) { DoubleClick.Invoke(this, new SparkCardEventArgs(card)); }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isInitDataSource) return;
            if (this._hoverIndex < 0) { this.textBox.Visible = false; return; }

            // 选中卡片
            this.SelectDrawCard(this._hoverIndex);

            if (SelectLableText)
            {
                // 进入选择模式
                var ctrl = this.SelectedCard.SparkCardStyle.Children.FirstOrDefault(a => a.Rectangle.Contains(e.Location));
                if (ctrl is SparkCardLabelStyle label)
                {
                    string text = label.Value;
                    if (string.IsNullOrEmpty(text))
                    {
                        this.textBox.Visible = false;
                        return;
                    }

                    label.TextAlign.ToStringAlignment(out StringAlignment hsa, out StringAlignment vsa);
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF measureSize = g.MeasureString(text, label.Font, label.Size);
                        this.textBox.Size = new Size((int)Math.Ceiling(measureSize.Width), (int)Math.Ceiling(measureSize.Height));
                        switch (vsa)
                        {
                            case StringAlignment.Near:
                                this.textBox.Location = label.Location;
                                break;
                            case StringAlignment.Center:
                                this.textBox.Location = new Point(label.Rectangle.X, label.Rectangle.Y + (int)Math.Round((label.Rectangle.Height - measureSize.Height) / 2));
                                break;
                            case StringAlignment.Far:
                                this.textBox.Location = new Point(label.Rectangle.X, label.Rectangle.Y + (int)Math.Round((label.Rectangle.Height - measureSize.Height)));
                                break;
                        }
                    }
                    this.textBox.TextAlign = hsa == StringAlignment.Near ? HorizontalAlignment.Left : (hsa == StringAlignment.Center ? HorizontalAlignment.Center : HorizontalAlignment.Right);
                    this.textBox.BackColor = this.SelectedCard.ActivedBackColor;
                    this.textBox.BackColor = this.SelectedCard.ActivedBackColor;
                    this.textBox.ForeColor = label.ForeColor;
                    this.textBox.Font = label.Font;
                    this.textBox.Text = text;
                    this.textBox.Visible = true;
                }
            }

            // 显示右键菜单
            if (e.Button == MouseButtons.Right && this._contextMenuStrip != null)
            {
                this._contextMenuStrip.Tag = _selectedCard;
                this._contextMenuStrip.Show(this, e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isInitDataSource) return;
            if (!_drawSparkCardList.Any()) return;
            var mouse = e.Location;
            if (this.Orientation == Orientation.Horizontal) mouse.Y = e.Y + (int)_translateTransformValue;
            else mouse.X = e.X + (int)_translateTransformValue;
            // 定位卡片
            SparkCard card = this._drawSparkCardList.FirstOrDefault(c => c.Rectangle.Contains(mouse));
            // 显示提示
            void ShowTooltip()
            {
                if (card == null) { this.toolTip.Hide(this); return; }

                var ctrl = card.SparkCardStyle.Children.FirstOrDefault(a => a.Rectangle.Contains(mouse));
                if (ctrl != null && ctrl is ITooltipString tooltip)
                {
                    string tooltipText = tooltip.Tooltip;
                    if (tooltipText.IsNullOrEmpty())
                    {
                        tooltipText = ctrl.ToString();
                    }
                    if (!tooltipText.IsNullOrEmpty())
                    {
                        if (this.toolTip.Active && this.toolTip.GetToolTip(this).Equals(tooltipText)) { return; }
                        this.toolTip.SetToolTip(this, tooltipText); return;
                    }
                }
                this.toolTip.Hide(this);
            }

            try
            {
                // 获取索引
                int index = this._drawSparkCardList.IndexOf(card);
                if (card == _hoverCard && _hoverCard != null) { ShowTooltip(); return; }

                // 设置状态
                if (_hoverCard != null && _hoverCard != card && _hoverCard != _selectedCard)
                {
                    _hoverCard.IsActived = false;
                }
                if (index >= 0)
                {
                    _hoverCard = card;
                    _hoverCard.IsActived = true;
                    ShowTooltip();
                }
                else
                {
                    _hoverCard = null;
                }
                // 缓存索引
                this._hoverIndex = index;

                // 重绘控件
                this.Invalidate();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_isInitDataSource)
            {
                e.Graphics.DrawString("初始化数据中,请稍等......", new Font("微软雅黑", 20), Brushes.Blue, 0, 0);
                return;
            }
            // 重绘控件事件处理器
            if (this.DesignMode) return;
            if (this._template == null || _dataSource == null || _dataSource.Count <= 0) { return; }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            this.PaintSlient(e.Graphics);
            _drawSparkCardList = GetDrawCardList();
            if (this.Orientation == Orientation.Horizontal)
            {
                g.TranslateTransform(0f, -_translateTransformValue);

            }
            else
            {
                g.TranslateTransform(-_translateTransformValue, 0f);
            }

            if (_drawSparkCardList?.Any() == true)
            {
                foreach (var item in _drawSparkCardList)
                {
                    if (DrawItem != null)
                    {
                        var index = Cards.IndexOf(item);
                        SparkCardDrawItemEventArgs args = new SparkCardDrawItemEventArgs(g, index, item.Location, item.DataSource);
                        DrawItem.Invoke(this, args);
                        if (args.Template != null)
                        {
                            item.Reset(args.Template);
                        }
                    }
                    item.Paint(g);
                }
            }

            SetAutoScrollMinSize();
            g.ResetTransform();

            this._paintForced = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.toolTip.Hide(this);
            if (_hoverIndex >= 0 && _hoverCard != null && _hoverCard != _selectedCard)
            {
                _hoverCard.IsActived = false;
                this.Invalidate();
                _hoverIndex = -1;
                _hoverCard = null;
            }
        }

        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);
        //    //改不大小的时候，需要重新计算每个卡片的大小和位置
        //}
        #endregion

        /// <summary>
        /// 卡片布局方式枚举
        /// </summary>
        public enum CardLayout
        {
            /// <summary>
            /// 卡片按照一定方向罗列，当长度或高度不够时自动调整进行换行换列。
            /// </summary>
            Wrap,
            /// <summary>
            /// 卡片按照行或列来顺序排列，但不会换行。
            /// </summary>
            Stack
        }
    }

    /// <summary>
    /// 卡片绘制间件的方式,按垂直和水平间距数值的固定方式,按填满方式(垂直和水平间距数值无效)
    /// </summary>
    public enum CardDrawSpacingMode
    {
        /// <summary>
        /// 填充方式
        /// </summary>
        Fill = 0,
        /// <summary>
        /// 固定方式
        /// </summary>
        Fixed
    }
}