using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Foundation;
using SparkControls.Theme;

namespace SparkControls.Controls
{
    [ToolboxBitmap(typeof(DataGridView)), Description("表格")]
    public class SparkDataGridView : DataGridView, ISparkTheme<SparkDataGridViewTheme>
    {
        #region 字段

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backColor = default;

        /// <summary>
        /// 前景色
        /// </summary>
        private Color _foreColor = default;

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;

        /// <summary>
        /// 是否显示行号
        /// </summary>
        private bool _showLineNumber = true;

        /// <summary>
        /// 表格更新定时器
        /// </summary>
        private readonly Timer _timer = null;

        /// <summary>
        /// 合并列名集合
        /// </summary>
        private List<string> _mergeColumnNames = new List<string>();

        /// <summary>
        /// 需要2维表头的列
        /// </summary>
        private readonly Dictionary<int, SparkColumnSpanInfo> _spanRows = new Dictionary<int, SparkColumnSpanInfo>();

        /// <summary>
        /// 列的样式
        /// </summary>
        private List<DataGridViewColumnStyle> _listStyle = null;

        /// <summary>
        /// 默认的样式
        /// </summary>
        private List<DataGridViewColumnStyle> _templateStyle = null;

        /// <summary>
        /// 右击菜单
        /// </summary>
        private ContextMenuStrip cMenu = null;
        #endregion

        #region 属性

        private Font mFont = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件的字体。
        /// </summary>
        [Category("Spark"), Description("用于显示控件文本的字体")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get { return this.mFont; }
            set
            {
                this.ColumnHeadersDefaultCellStyle.Font = this.DefaultCellStyle.Font = this.mFont = value;
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
        /// 显示行号
        /// </summary>
        [Category("Spark"), Description("显示行号"), DefaultValue(true)]
        public bool ShowLineNumber
        {
            set
            {
                this._showLineNumber = value;
                this.RowHeadersVisible = value;
                this.Invalidate();
            }
            get => this._showLineNumber;
        }

        /// <summary>
        /// 禁用根据值类型绑定与之对应的编辑框样式
        /// </summary>
        [Category("Spark"), Description("禁用根据值类型绑定与之对应的编辑框样式"), DefaultValue(false)]
        public bool DisableValueTypeMappingColumnStyle { set; get; }

        /// <summary>
        /// 绑定时，时间显示格式
        /// </summary>
        [Category("Spark"), Description("时间显示格式"), DefaultValue(null)]
        public string DateTimeFormat { set; get; }

        /// <summary>
        /// 显示数字键盘
        /// </summary>
        [Category("Spark"), Description("显示数字键盘")]
        [DefaultValue(false)]
        public bool ShowNumberKeyBoard { set; get; }

        /// <summary>
        /// 表头填充
        /// </summary>
        [Category("Spark"), Description("表头填充")]
        [DefaultValue(false)]
        public bool HeaderFillDisplay { set; get; }

        /// <summary>
        /// 选择行索引
        /// </summary>
        [Category("Spark"), Description("选择行索引")]
        [DefaultValue(-1)]
        public int SelectRowIndex
        {
            get { return this.CurrentRow == null ? -1 : this.CurrentRow.Index; }
            set
            {
                DataGridViewSelectedRowCollection rows = this.SelectedRows;
                if (rows != null) foreach (DataGridViewRow r in rows) r.Selected = false;
                if (this.Rows.Count <= value || value < 0) return;
                if (this.SelectionMode == DataGridViewSelectionMode.FullRowSelect) this.Rows[value].Selected = true;
                else this.Rows[value].Cells[0].Selected = true;
            }
        }

        /// <summary>
        /// 设置或获取合并列的集合
        /// </summary>
        [MergableProperty(false)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Localizable(true)]
        [Description("设置或获取合并列的集合"), Browsable(true), Category("Spark")]
        public List<string> MergeColumnNames
        {
            get
            {
                return this._mergeColumnNames;
            }
            set
            {
                this._mergeColumnNames = value;
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            }
        }

        /// <summary>
        /// 设置下拉选项样式,目前支持列的索引,表头文本和列的名称定位一列
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<object, ComboBoxStyle> ColumnComboBoxStyles
        {
            get; set;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new DataGridViewCellStyle RowHeadersDefaultCellStyle
        {
            get { return base.RowHeadersDefaultCellStyle; }
            private set { base.RowHeadersDefaultCellStyle = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new DataGridViewHeaderBorderStyle RowHeadersBorderStyle
        {
            get { return base.RowHeadersBorderStyle; }
            private set { base.RowHeadersBorderStyle = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new DataGridViewCellStyle RowsDefaultCellStyle
        {
            get { return base.RowsDefaultCellStyle; }
            private set { base.RowsDefaultCellStyle = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool RowHeadersVisible
        {
            get { return base.RowHeadersVisible; }
            set { base.RowHeadersVisible = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackgroundColor
        {
            get { return base.BackgroundColor; }
            private set { base.BackgroundColor = value; }
        }

        /// <summary>
        /// 是否默认选中第一行数据
        /// </summary>
        [DefaultValue(true), Description("是否默认选中第一行数据,true：选中，false：不选中")]
        public bool DefaultSelectFirstRow { get; set; } = true;

        /// <summary>
        /// 设置或获取Guid,用于保存样式文件的名称（需要唯一值）
        /// </summary>
        [Category("Spark"), Description("设置或获取Guid,用于保存样式文件的名称（需要唯一值）")]
        [DefaultValue("")]
        public string Guid
        {
            get => guid;
            set
            {
                if (guid != value)
                {
                    if (!guid.IsNullOrEmpty())
                    {
                        _templateStyle?.Clear();
                        CreateColumnAndStyle(null, false);
                    }
                    guid = value;
                }
            }
        }

        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object DataSource
        {
            get { return base.DataSource; }
            set
            {
                //判断是否存在列，存在列，则直接复制数据源
                //不存在列，根据数据源来创建列，并设置样式
                //数据源可以是DataTable,DataView,实体
                if (!this.Guid.IsNullOrEmpty())
                {
                    if (value != null)
                    {
                        if (this.ColumnCount == 0)
                        {
                            CreateColumnAndStyle(value, false);
                        }
                        else if (this.ColumnCount > 0 && this._listStyle == null)
                        {//datagridview本身创建列了，直接根据列创建样式，然后把DataSource其他的列添加到样式中
                            CreateColumnAndStyle(this.Columns.Cast<DataGridViewColumn>().ToList());
                            CreateColumnAndStyle(value, true);
                        }
                    }
                    base.DataSource = value;
                    if (!this.DesignMode)
                    {
                        if (value == null)
                        {
                            //this.Columns.Clear();
                            //数据清空,不清除列
                            //this.Rows.Clear();
                        }
                    }
                }
                else
                {
                    //单机版，不设置样式
                    try
                    {
                        base.DataSource = value;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 获取样式模版的路径
        /// </summary>
        internal string TemplateFilePath
        {
            get
            {
                return Guid.IsNullOrEmpty() ? "" : "GridStyle/" + Guid + ".style";
            }
        }

        /// <summary>
        /// 模板文件列的样式集合
        /// </summary>
        internal List<DataGridViewColumnStyle> TemplateStyle
        {
            get
            {
                if (_templateStyle == null || _templateStyle.Count == 0)
                {
                    _templateStyle = LoadTemplate();
                }
                return _templateStyle;
            }
        }

        /// <summary>
        /// 列的样式
        /// </summary>
        internal List<DataGridViewColumnStyle> ListStyle => _listStyle;
        #endregion

        #region 自定义事件

        /// <summary>
        /// 表格按钮点击事件
        /// </summary>
        public event SparkGridButtonClickEventHandler GridButtonClick = null;

        /// <summary>
        /// 表格多选框状态改变事件
        /// </summary>
        public event SparkGridCheckBoxStateChangedEventHandler GridCheckBoxStateChanged = null;

        /// <summary>
        /// 表格下拉选项值改变事件
        /// </summary>
        public event SparkGridComboBoxSelectChangedEventHandler GridComboBoxSelectChanged = null;

        /// <summary>
        /// 表格下拉多选项值改变事件
        /// </summary>
        public event SparkGridMultiComboBoxSelectChangedEventHandler GridMultiComboBoxSelectChanged = null;

        /// <summary>
        /// 表格下拉多选项值改变事件
        /// </summary>
        public event SparkGridComboBoxDataGridSelectChangedEventHanlder GridComboBoxDataGridSelectChanged = null;

        /// <summary>
        /// 表格下拉多选项值改变事件
        /// </summary>
        public event SparkGridMultiComboBoxDataGridSelectChangedEventHandler GridMultiComboBoxDataGridSelectChanged = null;

        /// <summary>
        /// 表格文本框文本改变事件
        /// </summary>
        public event SparkGridTextBoxTextChangedEventHandler GridTextBoxTextChanged = null;

        /// <summary>
        /// 表格单元格颜色选择改变事件
        /// </summary>
        public event SparkGridColorSelectChangedEventHandler GridColorSelectChanged = null;

        /// <summary>
        /// 表格单元格事件选择改变事件
        /// </summary>
        public event SparkGridDateSelectChangedEventHandler GridDateSelectChanged = null;

        /// <summary>
        /// 变革单元格超链接点击事件
        /// </summary>
        public event SparkGridHyperlinkClickEventHandler GridHyperlinkClick = null;

        #endregion

        public SparkDataGridView()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
                ControlStyles.DoubleBuffer |                      // 双缓冲
                ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
                ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
            );
            this.Theme = new SparkDataGridViewTheme(this);
            this.Font = Consts.DEFAULT_FONT;
            this.ColumnHeadersHeight = 28;
            this.RowTemplate.Height = 28;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.InitBaseProperty();

            this._timer = new Timer() { Interval = 20 };
            this._timer.Tick += this.Timer_Tick;
            this._timer.Start();
        }

        #region 内部方法

        /// <summary>
        /// 初始化样式
        /// </summary>
        private void InitControlStyle()
        {
            switch (this._controlState)
            {
                case ControlState.Focused:
                    this._backColor = this.Theme.MouseDownBackColor;
                    this._foreColor = this.Theme.MouseDownForeColor;
                    break;
                case ControlState.Highlight:
                    this._backColor = this.Theme.MouseOverBackColor;
                    this._foreColor = this.Theme.MouseOverForeColor;
                    break;
                case ControlState.Selected:
                    this._backColor = this.Theme.SelectedBackColor;
                    this._foreColor = this.Theme.SelectedForeColor;
                    break;
                default:
                    this._backColor = this.Theme.BackColor;
                    this._foreColor = this.Theme.ForeColor;
                    break;
            }
            this.AlternatingRowsDefaultCellStyle.BackColor = this.Theme.AlternatingBackColor;

            this.ColumnHeadersDefaultCellStyle.BackColor = this.Theme.HeaderBackColor;
            this.ColumnHeadersDefaultCellStyle.ForeColor = this.Theme.HeaderForeColor;
            this.BackgroundColor = this.Theme.BackColor;
            this.GridColor = this.Theme.BorderColor;
            this.DefaultCellStyle.SelectionBackColor = this.Theme.SelectedBackColor;
            this.DefaultCellStyle.SelectionForeColor = this._foreColor;
            this.DefaultCellStyle.BackColor = this.Theme.BackColor;
            this.DefaultCellStyle.ForeColor = this.Theme.ForeColor;
        }

        /// <summary>
        /// 初始化基础属性
        /// </summary>
        private void InitBaseProperty()
        {
            this.InitControlStyle();
            this.EnableHeadersVisualStyles = false;
            this.DefaultCellStyle.Font = Consts.DEFAULT_FONT;
            this.ColumnHeadersDefaultCellStyle.Font = Consts.DEFAULT_FONT;

            //this.RowsDefaultCellStyle.Font = Constant.DEFAULT_FONT;
            //this.ScrollBars = ScrollBars.Both;
            //this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//一次选择一个单元格
            //this.MultiSelect = false;
            //this.BorderStyle = BorderStyle.FixedSingle;
            //this.AllowUserToResizeColumns = false;
            //this.AllowUserToResizeRows = false;
            //this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//这样就禁止拖动标题行
            //this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;//这样标题行内容在垂直水平方向均居中
            //this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;//这样所有单元格内容在垂直水平方向均居中
            //this.ColumnHeadersHeight = 30;//设置列标题行高为30
            //this.RowTemplate.Height = 23;//设置单元格行高为23            
            //this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            //this.AllowUserToAddRows = false;//这样默认就没有额外的空行   
        }

        /// <summary>
        /// 动态绘制界面效果
        /// </summary>
        /// <param name="g">绘制上下文</param>
        /// <param name="rowIndex">行索引</param>
        private void Draw(Graphics g = null, int rowIndex = -1)
        {
            this.InitControlStyle();
            if (rowIndex > -1 && rowIndex < this.RowCount - 1)
            {
                if (this.AlternatingRowsDefaultCellStyle.BackColor != default(Color) && rowIndex % 2 == 1)
                {
                    this._backColor = this.AlternatingRowsDefaultCellStyle.BackColor;
                }
                this.Rows[rowIndex].DefaultCellStyle.BackColor = this._backColor;
            }
            if (g != null)
            {
                GDIHelper.DrawRectangle(g, new Rectangle(0, 0, this.Width - 1, this.Height - 1), this.Theme.BorderColor);
            }
        }

        /// <summary>
        /// 绘制表头样式
        /// </summary>
        private void DrawHeader(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1) return;
            Graphics g = e.Graphics;
            Rectangle r = e.CellBounds;
            Rectangle rect = new Rectangle(r.X - 1, r.Y - 1, r.Width, r.Height);
            StringFormat sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            GDIHelper.FillRectangle(g, rect, this.Theme.HeaderBackColor);
            Rectangle textSize = rect;
            if (!this.HeaderFillDisplay)
            {
                Size size = GDIHelper.MeasureString(g, e.Value?.ToString(), this.DefaultCellStyle.Font).ToSize();
                textSize = new Rectangle(rect.X, (rect.Height - size.Height) / 2 + 2, rect.Width, size.Height);
            }
            else
            {
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                float height = g.MeasureString(e.Value?.ToString(), this.DefaultCellStyle.Font, r.Width).Height;
                this.ColumnHeadersHeight = Math.Max(this.ColumnHeadersHeight, (int)height);
            }
            g.DrawString(e.Value?.ToString(), this.DefaultCellStyle.Font, new SolidBrush(this.Theme.HeaderForeColor), textSize, sf);
            if (this._spanRows.ContainsKey(e.ColumnIndex)) //被合并的列
            {
                SparkColumnSpanInfo spanRow = this._spanRows[e.ColumnIndex];
                int left = r.Left, top = r.Top + 2, right = r.Right, bottom = r.Bottom;
                SolidBrush brush = new SolidBrush(this.Theme.HeaderBackColor);
                g.FillRectangle(brush, r);
                g.DrawLine(new Pen(this.Theme.BorderColor), left, (top + bottom) / 2, right, (top + bottom) / 2);//画中线
                int y = rect.Top;
                if (e.ColumnIndex != spanRow.Left) y = rect.Top + rect.Height / 2 + 2;
                g.DrawLine(new Pen(this.Theme.BorderColor), rect.Left, y, rect.Left, rect.Bottom);
                brush = new SolidBrush(this.Theme.HeaderForeColor);
                rect = new Rectangle(left, (top + bottom) / 2, right - left, (bottom - top) / 2);
                g.DrawString(e.Value + "", this.DefaultCellStyle.Font, brush, rect, sf);
                left = this.GetColumnDisplayRectangle(spanRow.Left, true).Left - 2;
                if (left < 0) left = this.GetCellDisplayRectangle(-1, -1, true).Width;
                right = this.GetColumnDisplayRectangle(spanRow.Right, true).Right - 2;
                if (right < 0) right = this.Width;
                rect = new Rectangle(left, top, right - left, (bottom - top) / 2);
                g.DrawString(spanRow.Text, this.DefaultCellStyle.Font, brush, rect, sf);
            }
            else GDIHelper.DrawRectangle(g, rect, this.Theme.BorderColor);
            e.Handled = true;
        }

        /// <summary>
        /// 绘制行号
        /// </summary>
        private void DrawLineNumber(DataGridViewRowPostPaintEventArgs e)
        {
            if (!this._showLineNumber) return;
            string txt = (e.RowIndex + 1).ToString(CultureInfo.CurrentUICulture);
            Font font = this.DefaultCellStyle.Font;
            Rectangle rect = new Rectangle(0, e.RowBounds.Top, this.RowHeadersWidth, e.RowBounds.Height);
            GDIHelper.DrawImageAndString(e.Graphics, rect, null, default(Size), txt, font, this.Theme.HeaderForeColor);
        }

        /// <summary>
        /// 画单元格
        /// </summary>
        /// <param name="e"></param>
        private void DrawCell(DataGridViewCellPaintingEventArgs e)
        {
            if (e.CellStyle.Alignment == DataGridViewContentAlignment.NotSet)
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            Brush gridBrush = new SolidBrush(this.GridColor);
            SolidBrush backBrush = new SolidBrush(e.CellStyle.BackColor);
            SolidBrush fontBrush = new SolidBrush(e.CellStyle.ForeColor);
            Rectangle rect = e.CellBounds;
            int cellwidth = rect.Width;
            int UpRows = 0;//上面相同的行数            
            int DownRows = 0;//下面相同的行数            
            int count = 0;//总行数            
            if (this.MergeColumnNames.Contains(this.Columns[e.ColumnIndex].Name) && e.RowIndex != -1)
            {
                Pen gridLinePen = new Pen(gridBrush);
                string curValue = e.Value == null ? "" : e.Value.ToString().Trim();
                string curSelected = this.CurrentRow.Cells[e.ColumnIndex].Value == null ? "" : this.CurrentRow.Cells[e.ColumnIndex].Value.ToString().Trim();
                if (!string.IsNullOrEmpty(curValue))
                {
                    #region 获取下面的行数
                    for (int i = e.RowIndex; i < this.Rows.Count; i++)
                    {
                        DataGridViewCell cell = this.Rows[i].Cells[e.ColumnIndex];
                        string str = cell.Value?.ToString();
                        if (str != null && str.Equals(curValue))
                        {
                            DownRows++;
                            if (e.RowIndex != i) cellwidth = cellwidth < cell.Size.Width ? cellwidth : cell.Size.Width;
                        }
                        else break;
                    }
                    #endregion
                    #region 获取上面的行数
                    for (int i = e.RowIndex; i >= 0; i--)
                    {
                        DataGridViewCell cell = this.Rows[i].Cells[e.ColumnIndex];
                        string str = cell.Value?.ToString();
                        if (str != null && str.Equals(curValue))
                        {
                            UpRows++;
                            if (e.RowIndex != i) cellwidth = cellwidth < cell.Size.Width ? cellwidth : cell.Size.Width;
                        }
                        else break;
                    }
                    #endregion
                    count = DownRows + UpRows - 1;
                    if (count < 2) return;
                }
                if (this.Rows[e.RowIndex].Selected)
                {
                    backBrush.Color = e.CellStyle.SelectionBackColor;
                    fontBrush.Color = e.CellStyle.SelectionForeColor;
                }
                e.Graphics.FillRectangle(backBrush, rect);//以背景色填充                
                this.PaintingFont(e, cellwidth, UpRows, DownRows, count);//画字符串
                if (DownRows == 1)
                {
                    e.Graphics.DrawLine(gridLinePen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
                    count = 0;
                }
                e.Graphics.DrawLine(gridLinePen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);// 画右边线
                e.Handled = true;
            }
        }

        /// <summary>
        /// 画字符串
        /// </summary>
        /// <param name="e"></param>
        /// <param name="cellwidth"></param>
        /// <param name="upRows"></param>
        /// <param name="downRows"></param>
        /// <param name="count"></param>
        private void PaintingFont(DataGridViewCellPaintingEventArgs e, int cellwidth, int upRows, int downRows, int count)
        {
            string text = e.Value?.ToString() ?? "";
            Font font = e.CellStyle.Font;
            Rectangle rect = e.CellBounds;
            Graphics g = e.Graphics;
            PointF point = new PointF();
            SolidBrush fontBrush = new SolidBrush(e.CellStyle.ForeColor);
            int fontheight = (int)g.MeasureString(text, font).Height;
            int fontwidth = (int)g.MeasureString(text, font).Width;
            int cellheight = rect.Height;
            switch (e.CellStyle.Alignment)
            {
                case DataGridViewContentAlignment.BottomCenter:
                    point = new PointF(rect.X + (cellwidth - fontwidth) / 2, rect.Y + cellheight * downRows - fontheight);
                    break;
                case DataGridViewContentAlignment.BottomLeft:
                    point = new PointF(rect.X, rect.Y + cellheight * downRows - fontheight);
                    break;
                case DataGridViewContentAlignment.BottomRight:
                    point = new PointF(rect.X + cellwidth - fontwidth, rect.Y + cellheight * downRows - fontheight);
                    break;
                case DataGridViewContentAlignment.MiddleCenter:
                    point = new PointF(rect.X + (cellwidth - fontwidth) / 2, rect.Y - cellheight * (upRows - 1) + (cellheight * count - fontheight) / 2);
                    break;
                case DataGridViewContentAlignment.MiddleLeft:
                    point = new PointF(rect.X, rect.Y - cellheight * (upRows - 1) + (cellheight * count - fontheight) / 2);
                    break;
                case DataGridViewContentAlignment.MiddleRight:
                    point = new PointF(rect.X + cellwidth - fontwidth, rect.Y - cellheight * (upRows - 1) + (cellheight * count - fontheight) / 2);
                    break;
                case DataGridViewContentAlignment.TopCenter:
                    point = new PointF(rect.X + (cellwidth - fontwidth) / 2, rect.Y - cellheight * (upRows - 1));
                    break;
                case DataGridViewContentAlignment.TopLeft:
                    point = new PointF(rect.X, rect.Y - cellheight * (upRows - 1));
                    break;
                case DataGridViewContentAlignment.TopRight:
                    point = new PointF(rect.X + cellwidth - fontwidth, rect.Y - cellheight * (upRows - 1));
                    break;
                default:
                    point = new PointF(rect.X + (cellwidth - fontwidth) / 2, rect.Y - cellheight * (upRows - 1) + (cellheight * count - fontheight) / 2);
                    break;
            }
            g.DrawString(text, font, fontBrush, point);
        }

        /// <summary>
        /// 定时器处理程序
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            this._timer.Enabled = false;
            this.RefreshHeader();
        }

        #endregion

        #region 对外方法

        /// <summary>
        /// 合并列
        /// </summary>
        /// <param name="ColIndex">列的索引</param>
        /// <param name="ColCount">需要合并的列数</param>
        /// <param name="Text">合并列后的文本</param>
        public void AppendMergeHeader(int ColIndex, int ColCount, string Text)
        {
            if (ColCount < 2)
            {
                throw new Exception("行宽应大于等于2，合并1列无意义。");
            }
            //将这些列加入列表
            int Right = ColIndex + ColCount - 1; //同一大标题下的最后一列的索引
            this._spanRows[ColIndex] = new SparkColumnSpanInfo(Text, 1, ColIndex, Right); //添加标题下的最左列
            this._spanRows[Right] = new SparkColumnSpanInfo(Text, 3, ColIndex, Right); //添加该标题下的最右列
            for (int i = ColIndex + 1; i < Right; i++) //中间的列
            {
                this._spanRows[i] = new SparkColumnSpanInfo(Text, 2, ColIndex, Right);
            }
        }

        /// <summary>
        /// 添加合并列集合
        /// </summary>
        /// <param name="names">列名集合</param>
        public void AppendMergeColumnNames(IEnumerable<string> names)
        {
            this.MergeColumnNames.AddRange(names);
        }

        /// <summary>
        /// 清除合并的列
        /// </summary>
        public void ClearMergeInfo()
        {
            this._spanRows.Clear();
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        /// <summary>
        /// 刷新表头
        /// </summary>
        public void RefreshHeader()
        {
            foreach (int si in this._spanRows.Keys)
            {
                this.Invalidate(this.GetCellDisplayRectangle(si, -1, true));
            }
        }

        /// <summary>
        /// 根据列索引，列名称或者列头获取下拉框类型
        /// </summary>
        /// <param name="index">列索引</param>
        /// <param name="name">列名称</param>
        /// <param name="headerText"列头></param>
        /// <returns></returns>
        public ComboBoxStyle QueryComboBoxStyle(int index, string name, string headerText)
        {
            if (this.ColumnComboBoxStyles?.ContainsKey(index) == true)
            {
                return this.ColumnComboBoxStyles[index];
            }
            else if (this.ColumnComboBoxStyles?.ContainsKey(name) == true)
            {
                return this.ColumnComboBoxStyles[name];
            }
            else if (this.ColumnComboBoxStyles?.ContainsKey(headerText) == true)
            {
                return this.ColumnComboBoxStyles[headerText];
            }
            return ComboBoxStyle.DropDownList;
        }

        #endregion

        #region 重写
        /// <summary>
        /// ProcessCmdKey事件
        /// </summary>
        public Func<Message, Keys, bool> ProcessCmdKeyAction;
        private string guid = "";

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ProcessCmdKeyAction != null)
            {
                var ret = ProcessCmdKeyAction(msg, keyData);
                if (ret)
                {
                    return true;
                }
                else
                {
                    return base.ProcessCmdKey(ref msg, keyData);
                }
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1) this.DrawCell(e);
            else this.DrawHeader(e);
            base.OnCellPainting(e);
        }

        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);
            this.DrawLineNumber(e);
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);

            if (!this.Enabled)
            {
                this.ClearSelection();
                this.DefaultCellStyle.BackColor = this.Theme.DisabledBackColor;
            }
            if (this.DefaultSelectFirstRow)
            {
                this.SelectRowIndex = 0;
            }
            else
            {
                this.ClearSelection();
                this.CurrentCell = null;
            }
            //var customScrollbar1 = new CustomScrollbar();
            //customScrollbar1.Minimum = VerticalScrollBar.Minimum;
            //customScrollbar1.Maximum = VerticalScrollBar.Maximum;
            //customScrollbar1.LargeChange = VerticalScrollBar.LargeChange;
            //customScrollbar1.SmallChange = VerticalScrollBar.SmallChange;
            //customScrollbar1.Value = VerticalScrollBar.Value;
            //customScrollbar1.Height = this.Height;
            //customScrollbar1.Scroll += (s, ee) =>
            //{
            //    this.FirstDisplayedScrollingRowIndex = (int)Math.Ceiling(customScrollbar1.Value * 1.0 / this.RowTemplate.Height);
            //};
            //customScrollbar1.Left = this.DisplayRectangle.Width - 10;
            //this.Controls.Add(customScrollbar1);
        }

        protected override void OnCellMouseEnter(DataGridViewCellEventArgs e)
        {
            base.OnCellMouseEnter(e);
            this._controlState = ControlState.Highlight;
            this.Draw(null, e.RowIndex);
        }

        protected override void OnCellMouseLeave(DataGridViewCellEventArgs e)
        {
            base.OnCellMouseLeave(e);
            this._controlState = ControlState.Default;
            this.Draw(null, e.RowIndex);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                this.Draw(e.Graphics);

            }
            catch (Exception ex) { MessageBox.Show(ex.StackTrace); }
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                this._timer.Enabled = false; this._timer.Enabled = true;
            }
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {

            e.ThrowException = false;
            //base.OnDataError(displayErrorDialogIfNoHandler, e);
        }

        protected override DataGridViewColumnCollection CreateColumnsInstance()
        {
            return new SparkDataGridColumnCollection(this);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right)
            {
                var hit = this.HitTest(e.X, e.Y);
                //点解列标题才显示设置菜单
                if (hit.Type != DataGridViewHitTestType.ColumnHeader && hit.Type != DataGridViewHitTestType.None)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(Guid) && (!this.DesignMode || (UserObject.LoginUser?.UserCode ?? "").ToLower() == "admin"))
                {
                    if (cMenu == null)
                    {
                        cMenu = new ContextMenuStrip();
                        ToolStripMenuItem menuEdit = new ToolStripMenuItem();
                        menuEdit.Click += (sender, args) =>
                        {
                            Style.FrmDataGridTemplateNew frm = new Style.FrmDataGridTemplateNew(this);
                            frm.ShowDialog(this);
                        };
                        menuEdit.Text = "设置列";
                        cMenu.Items.Add(menuEdit);
                    }
                    cMenu.Show(this, new System.Drawing.Point(e.X, e.Y));
                }
            }
        }
        #endregion

        #region 程序集内部触发事件

        internal void RaiseGridButtonClick(DataGridViewButtonCell sender, SparkGridButtonClickEventArgs args)
        {
            if (GridButtonClick == null) return;
            GridButtonClick(sender, args);
        }

        internal void RaiseGridCheckBoxStateChanged(DataGridViewCheckBoxCell sender, SparkGridCheckBoxStateChangedEventArgs args)
        {
            if (GridCheckBoxStateChanged == null) return;
            GridCheckBoxStateChanged(sender, args);
        }

        internal void RaiseGridComboBoxSelectChanged(DataGridViewComboBoxCell sender, SparkGridComboBoxSelectChangedEventArgs args)
        {
            if (GridComboBoxSelectChanged == null) return;
            GridComboBoxSelectChanged(sender, args);
        }

        internal void RaiseGridComboBoxDataGridSelectChanged(SparkDataGridComboBoxDataGridCell sender, SparkGridComboBoxDataGridSelectChangedEventArgs args)
        {
            if (GridComboBoxDataGridSelectChanged == null) return;
            GridComboBoxDataGridSelectChanged(sender, args);
        }

        internal void RaiseGridMultiComboBoxDataGridSelectChanged(SparkDataGridMultiComboBoxDataGridCell sender, SparkGridMultiComboBoxDataGridSelectChangedEventArgs args)
        {
            if (GridMultiComboBoxDataGridSelectChanged == null) return;
            GridMultiComboBoxDataGridSelectChanged(sender, args);
        }

        internal void RaiseGridMultiComboBoxSelectChanged(DataGridViewTextBoxCell sender, SparkGridMultiComboBoxSelectChangedEventArgs args)
        {
            if (GridMultiComboBoxSelectChanged == null) return;
            GridMultiComboBoxSelectChanged(sender, args);
        }

        internal void RaiseGridTextBoxTextChanged(DataGridViewTextBoxCell sender, SparkGridTextBoxTextChangedEventArgs args)
        {
            if (GridTextBoxTextChanged == null) return;
            GridTextBoxTextChanged(sender, args);
        }

        internal void RaiseGridColorSelectChanged(DataGridViewCell sender, SparkGridColorSelectChangedEventArgs args)
        {
            if (GridColorSelectChanged == null) return;
            GridColorSelectChanged(sender, args);
        }

        internal void RaiseGridDateSelectChanged(DataGridViewTextBoxCell sender, SparkGridDateSelectChangedEventArgs args)
        {
            if (GridDateSelectChanged == null) return;
            GridDateSelectChanged(sender, args);
        }

        internal void RaiseGridHyperlinkClick(DataGridViewLinkCell sender, SparkGridHyperlinkClickEventArgs args)
        {
            if (GridHyperlinkClick == null) return;
            GridHyperlinkClick(sender, args);
        }

        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkDataGridViewTheme Theme { get; private set; }

        #endregion

        #region 设置页眉
        /// <summary>
        /// 创建样式
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="continueToAdd">true=继续添加(不清空原来的值),false=清空,重新添加</param>
        private void CreateColumnAndStyle(object dataSource, bool continueToAdd)
        {
            if (!continueToAdd)
            {
                this.Columns.Clear();
                if (_listStyle != null) _listStyle.Clear();
            }
            if (_listStyle == null) _listStyle = new List<DataGridViewColumnStyle>();
            this.AutoGenerateColumns = false;
            if (dataSource is DataTable)
            {
                CreateColumnAndStyle(dataSource as DataTable);
            }
            else if (dataSource is DataSet)
            {
                if ((dataSource as DataSet).Tables.Count > 0)
                {
                    if (!this.DataMember.IsNullOrEmpty() && (dataSource as DataSet).Tables.Contains(this.DataMember))
                    {
                        CreateColumnAndStyle((dataSource as DataSet).Tables[this.DataMember]);
                    }
                    else
                    {
                        CreateColumnAndStyle((dataSource as DataSet).Tables[0]);
                    }
                }
            }
            else if (dataSource is DataView)
            {
                CreateColumnAndStyle((dataSource as DataView).Table);
            }
            else if (dataSource is System.Collections.IList || dataSource is System.Collections.IEnumerable)
            {

                Type type = dataSource.GetType();
                if (type.IsGenericType)//判断是否是泛型
                {
                    Type typeClass = type.GetGenericArguments().FirstOrDefault();
                    if (typeClass.IsClass && !typeClass.IsPrimitive)
                    {
                        CreateColumnAdnnStyle(typeClass);
                    }
                }
            }
        }

        /// <summary>
        /// 创建样式
        /// </summary>
        /// <param name="column"></param>
        private void CreateColumnAndStyle(List<DataGridViewColumn> column)
        {
            if (_listStyle == null) _listStyle = new List<DataGridViewColumnStyle>();
            for (int i = 0; i < column.Count; i++)
            {
                var style = new DataGridViewColumnStyle()
                {
                    CellAlignment = this.DefaultCellStyle.Alignment,//col.DefaultCellStyle.Alignment,
                    HeaderText = column[i].HeaderText,
                    ColFrozen = false,
                    DataPropertyName = column[i].DataPropertyName, //col.DataPropertyName
                    Hide = false,//!col.Visible,
                                 //Width = this.col//Math.Max(col.Width, col.MinimumWidth),
                    Sort = i,//dt.Columns.IndexOf(col),// col.DisplayIndex,
                };
                _listStyle.Add(style);
            }
            SynchronousStyle();
            //设置样式
            SetColsStyle();
        }

        /// <summary>
        /// 根据DataTable创建列及样式
        /// </summary>
        /// <param name="dt"></param>
        private void CreateColumnAndStyle(DataTable dt)
        {
            if (_listStyle == null) _listStyle = new List<DataGridViewColumnStyle>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (_listStyle.Any(a => a.DataPropertyName.Equals(dt.Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase))) continue;
                DataGridViewTextBoxColumn dc = new DataGridViewTextBoxColumn();
                dc.Name = dt.Columns[i].ColumnName;
                dc.HeaderText = dt.Columns[i].ColumnName;
                //dc.Width = dp[i].Width;
                //dc.Frozen = dp[i].Frozen;
                dc.DataPropertyName = dt.Columns[i].ColumnName;
                this.Columns.Add(dc);

                var style = new DataGridViewColumnStyle()
                {
                    CellAlignment = this.DefaultCellStyle.Alignment,//col.DefaultCellStyle.Alignment,
                    HeaderText = dt.Columns[i].ColumnName,
                    ColFrozen = false,
                    DataPropertyName = dt.Columns[i].ColumnName, //col.DataPropertyName
                    Hide = false,//!col.Visible,
                                 //Width = this.col//Math.Max(col.Width, col.MinimumWidth),
                    Sort = i,//dt.Columns.IndexOf(col),// col.DisplayIndex,
                };
                _listStyle.Add(style);
            }
            SynchronousStyle();
            //设置样式
            SetColsStyle();
        }

        /// <summary>
        /// 根据实体添加创建列及样式
        /// </summary>
        /// <param name="type"></param>
        private void CreateColumnAdnnStyle(Type type)
        {
            if (_listStyle == null) _listStyle = new List<DataGridViewColumnStyle>();
            var list = Foundation.Mapper.SparkMapper.GetTypeMap(type).GetMemberListByColumnNameAttribute(false);
            if (list?.Any() == true)
            {
                var items = list.Where(a => !_listStyle.Any(b => a.ColumnName.Equals(b.DataPropertyName, StringComparison.OrdinalIgnoreCase)));
                int index = 0;
                foreach (var item in items)
                {
                    DataGridViewTextBoxColumn dc = new DataGridViewTextBoxColumn();
                    dc.Name = item.Property.Name;
                    dc.HeaderText = item.Property.Name;
                    dc.DataPropertyName = item.Property.Name;
                    this.Columns.Add(dc);
                    var style = new DataGridViewColumnStyle()
                    {
                        CellAlignment = this.DefaultCellStyle.Alignment,
                        HeaderText = item.Property.Name,
                        ColFrozen = false,
                        DataPropertyName = item.Property.Name, //col.DataPropertyName
                        Hide = false,
                        Sort = index,
                    };
                    _listStyle.Add(style);
                    index++;
                }
            }
            SynchronousStyle();
            //设置样式
            SetColsStyle();
        }

        /// <summary>
        /// 同步模板样式到列的样式
        /// </summary>
        private void SynchronousStyle()
        {
            if (TemplateStyle != null && _listStyle != null && _listStyle.Any())
            {
                foreach (var item in TemplateStyle)
                {
                    var style = _listStyle.FirstOrDefault(a => a.DataPropertyName == item.DataPropertyName);
                    if (style != null)
                    {
                        style.CellAlignment = item.CellAlignment;
                        style.ColFrozen = item.ColFrozen;
                        style.DataPropertyName = item.DataPropertyName;
                        style.HeaderText = item.HeaderText;
                        style.Hide = item.Hide;
                        style.Sort = item.Sort;
                        style.Width = item.Width;
                    }
                }
                _listStyle = _listStyle.OrderBy(a => a.Sort).ToList();
            }
        }

        /// <summary>
        /// 设置列的显示样式
        /// </summary>
        private void SetColsStyle()
        {
            if (string.IsNullOrEmpty(Guid) || _listStyle == null || _listStyle.Count == 0
                || this.ColumnCount == 0 || TemplateStyle == null || TemplateStyle.Count == 0)
            {
                return;
            }
            foreach (var style in _listStyle)
            {
                SetColsStyle(style);
            }

            var frozen = _listStyle.Where(a => a.ColFrozen);
            if (frozen != null && frozen.Any())
            {
                var colName = frozen.FirstOrDefault(a => a.Sort == frozen.Max(b => b.Sort)).DataPropertyName;
                SetFrozen(colName);
            }
        }

        /// <summary>
        /// 设置列的样式
        /// </summary>
        /// <param name="style"></param>
        internal void SetColsStyle(DataGridViewColumnStyle style)
        {
            try
            {
                if (this.ColumnCount == 0)
                    return;
                var col = base.Columns.Cast<DataGridViewColumn>().FirstOrDefault(a => a.DataPropertyName.Equals(style.DataPropertyName, StringComparison.OrdinalIgnoreCase));
                if (col != null)
                {
                    if (col.HeaderText != style.HeaderText) col.HeaderText = style.HeaderText;
                    if (col.DefaultCellStyle.Alignment != style.CellAlignment) col.DefaultCellStyle.Alignment = style.CellAlignment;
                    if (col.Width != style.Width) col.Width = style.Width;
                    if (col.Visible == style.Hide) col.Visible = !style.Hide;
                    if (col.DisplayIndex != style.Sort) col.DisplayIndex = style.Sort;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 设置冻结列
        /// </summary>
        /// <param name="colName"></param>
        internal void SetFrozen(string colName)
        {
            ClearFrozen();
            if (colName.Length > 0)
            {
                var col = this.Columns.Cast<DataGridViewColumn>().FirstOrDefault(a => a.DataPropertyName == colName);
                if (col != null) col.Frozen = true;
            }
        }

        /// <summary>
        /// 清除冻结列
        /// </summary>
        internal void ClearFrozen()
        {
            var frozenCol = this.Columns.Cast<DataGridViewColumn>().Where(a => a.Frozen);
            if (frozenCol != null && frozenCol.Any())
            {
                foreach (var col in frozenCol)
                {
                    col.Frozen = false;
                }
            }
        }

        /// <summary>
        /// 加载样式模板文件
        /// </summary>
        /// <returns></returns>
        internal List<DataGridViewColumnStyle> LoadTemplate()
        {
            if (string.IsNullOrEmpty(TemplateFilePath)) return null;
            string templateValue = Lib.DAL.DbAccessFactory.Create().GetConfig(TemplateFilePath).Content;
            if (templateValue.IsNullOrEmpty()) return null;
            templateValue = Spark.Foundation.Comm.SparkDecrypt(templateValue);
            object obj = Serializer.XmlDeserializeObj(templateValue, typeof(List<DataGridViewColumnStyle>));
            return obj as List<DataGridViewColumnStyle>;
        }

        /// <summary>
        /// 保存样式模板
        /// </summary>
        internal void SaveTemplateFile()
        {
            if (Guid.IsNullOrEmpty()) return;
            string xml = Serializer.XmlSerializeObj(_listStyle);
            if (!xml.IsNullOrEmpty())
            {
                xml = Spark.Foundation.Comm.SparkEncrypt2Str(xml);
                Lib.DAL.DbAccessFactory.Create().UpdateConfig(TemplateFilePath, xml);
            }
        }
        #endregion
    }
}