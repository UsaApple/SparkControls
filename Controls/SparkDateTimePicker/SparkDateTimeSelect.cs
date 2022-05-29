using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 日期时间控件选择面板
    /// </summary>
    public partial class SparkDateTimeSelect : UserControl, ISparkTheme<SparkDateTimePickerTheme>
    {
        #region 变量
        #endregion

        #region 事件
        /// <summary>
        /// DtEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DtEventHandler(object sender, DateTimeEventArgs e);
        /// <summary>
        /// 日期时间改变事件
        /// </summary>
        public event DtEventHandler DateTimeChanged = null;

        /// <summary>
        /// 执行确定按钮事件
        /// </summary>
        public event EventHandler ExecOKBtnClick = null;

        /// <summary>
        /// 执行取消按钮事件
        /// </summary>
        public event EventHandler ExecCancelBtnClick = null;
        #endregion

        #region 属性
        private BorderStyle _borderStyle = BorderStyle.FixedSingle;
        /// <summary>
        /// 边框样式
        /// </summary>
        [Category("Spark"), Description("边框样式")]
        [DefaultValue(BorderStyle.FixedSingle)]
        public new BorderStyle BorderStyle
        {
            get => this._borderStyle;
            set => this._borderStyle = value;
        }

        private DateTimePickerFormat _format = DateTimePickerFormat.Long;
        /// <summary>
        /// 获取或设置控件中显示的日期和时间格式
        /// </summary>
        [Category("Spark"), Description("显示的日期和时间格式")]
        [DefaultValue(typeof(DateTimePickerFormat), "Long")]
        public DateTimePickerFormat Format
        {
            get => this._format;
            set
            {
                this._format = value;
                this.dtItemPnl.SetDtDispItems(value, this.CustomFormat);
                this.dtItemPnl.Invalidate();
            }
        }

        private string _customFormat = string.Empty;
        /// <summary>
        /// 获取或设置自定义日期/时间格式字符串
        /// </summary>
        [Category("Spark"), Description("自定义日期/时间格式字符串")]
        [DefaultValue(typeof(string), "")]
        public string CustomFormat
        {
            get => this._customFormat;
            set
            {
                this._customFormat = value;
                this.dtItemPnl.SetDtDispItems(this.Format, value);
                this.dtItemPnl.Invalidate();
            }
        }

        /// <summary>
        /// 选择日期或时间后是否自动选择下一级
        /// </summary>
        [Category("Spark"), Description("是否自动选中下一级")]
        [DefaultValue(typeof(bool), "True")]
        public bool AutoSelectNext { get; set; } = true;

        /// <summary>
        /// 当前选择的时间
        /// </summary>
        [Browsable(false)]
        public DateTime SelectValue { get; private set; } = DateTime.Now;

        /// <summary>
        /// 是否显示执行按钮
        /// </summary>
        [Browsable(false)]
        public bool ShowExecBtn { get; set; } = false;

        /// <summary>
        /// 没有任何改变时(直接点击确定)重新构造时间
        /// </summary>
        [Browsable(false)]
        public bool ReCtorDtWhenNoChange { get; set; } = false;
        #endregion

        #region 构造方法
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public SparkDateTimeSelect()
        {
            this.InitializeComponent();
            this.Theme = new SparkDateTimePickerTheme(this);
            this.InitControls();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtFormat"></param>
        /// <param name="customFormat"></param>
        public SparkDateTimeSelect(DateTime? dt, DateTimePickerFormat dtFormat, string customFormat = "") : this()
        {
            this.SelectValue = dt == null ? DateTime.Now : dt.Value;
            this.Format = dtFormat;
            if (dtFormat == DateTimePickerFormat.Custom)
            {
                this.CustomFormat = customFormat;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 重置DateTimeSelect面板信息
        /// </summary>
        /// <param name="dt"></param>
        public void ResetDtSelectInfo(DateTime? dt)
        {
            this.SelectValue = dt == null ? DateTime.Now : dt.Value;
            this.dtItemPnl.Value = this.SelectValue;
            this.LocateDateTimePage();
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 重新绘制
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.BorderStyle != BorderStyle.None)
            {
                GDIHelper.DrawNonWorkAreaBorder(this, this.Theme.BorderColor);
            }
            base.OnPaint(e);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControls()
        {
            base.BorderStyle = BorderStyle.None;

            this.dtPnl.Theme = this.Theme;
            this.dtItemPnl.Theme = this.Theme;
            this.dtItemPnl.InitDateTimeItems();
            this.lblToday.ForeColor = this.Theme.ForeColor;
            this.lblToday.Text += DateTime.Now.ToString("yyyy/MM/dd");

            //注册事件
            this.dtItemPnl.DtItemClick += this.DateTimeItem_Click;
            this.dtPnl.DtCellEleClick += this.DtPnl_DtCellEleClick;
            this.dtPnl.DtCellEleDoubleClick += this.DtPnl_DtCellEleDoubleClick;
            this.btnOK.Click += this.ExecuteBtn_Click;
            this.btnCancel.Click += this.ExecuteBtn_Click;
            this.lblToday.Click += this.LblToday_Click;
            this.lblToday.MouseEnter += this.LblToday_MouseEnter;
            this.lblToday.MouseLeave += this.LblToday_MouseLeave;
            this.lblLeft.Click += this.LblCommon_Click;
            this.lblRight.Click += this.LblCommon_Click;
        }

        /// <summary>
        /// 刷新用户界面
        /// </summary>
        /// <param name="dtItem"></param>
        private void RefreshDtSelect(DateTimeCellElement dtItem)
        {
            if (dtItem == null || !dtItem.Visible) return;

            //设置左右移动标签是否显示
            this.lblLeft.Visible = this.lblRight.Visible = dtItem.EleType == ElementType.Year;

            //设置按钮选中
            this.dtItemPnl.Select(dtItem);

            //设置日期时间面板相关信息
            this.dtPnl.CellEleType = dtItem.EleType;
            this.dtPnl.DataSource = this.GetDataSource(this.dtPnl.CellEleType);
            this.dtPnl.SetSelectCell(dtItem.Name);
            this.dtPnl.Invalidate();
        }

        /// <summary>
        /// 根据key值获取绑定的数据源
        /// </summary>
        /// <param name="eleType"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetDataSource(ElementType eleType)
        {
            switch (eleType)
            {
                case ElementType.Year:
                    return this.GetYearSource();
                case ElementType.Month:
                    return this.GetMonthSource();
                case ElementType.Day:
                    return this.GetDaySource();
                case ElementType.Hour:
                    return this.GetHourSource();
                case ElementType.Minute:
                case ElementType.Second:
                    return this.GetMinuteOrSecSource();
                default:
                    return new List<KeyValuePair<string, string>>();
            }
        }

        /// <summary>
        /// 获取年份数据源
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetYearSource()
        {
            //设置表格行和列
            this.dtPnl.Row = 5;
            this.dtPnl.Column = 5;

            //设置数据源
            List<KeyValuePair<string, string>> dataSource = new List<KeyValuePair<string, string>>();
            int intYear = this.SelectValue.Year - this.SelectValue.Year % 25;
            for (int i = 0; i < 25; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>((intYear + i).ToString(), (intYear + i).ToString()));
            }
            return dataSource;
        }

        /// <summary>
        /// 根据基数年份获取年份数据源
        /// </summary>
        /// <param name="baseYear"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetYearSource(int baseYear, bool next = true)
        {
            //设置表格行和列
            this.dtPnl.Row = 5;
            this.dtPnl.Column = 5;

            //设置数据源
            List<KeyValuePair<string, string>> dataSource = new List<KeyValuePair<string, string>>();
            int intYear = baseYear - baseYear % 25 + (next ? 1 : -1) * 25;
            for (int i = 0; i < 25; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>((intYear + i).ToString(), (intYear + i).ToString()));
            }
            return dataSource;
        }

        /// <summary>
        /// 获取月份数据源
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetMonthSource()
        {
            this.dtPnl.Row = 3;
            this.dtPnl.Column = 4;

            //设置数据源
            List<KeyValuePair<string, string>> dataSource = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 12; i++)
            {
                string enMonth = $"2019-{i}-11".ToDateTime().Value.ToString("MMM",
                    CultureInfo.CreateSpecificCulture("en-GB"));
                dataSource.Add(new KeyValuePair<string, string>(i.ToString(),
                    $"{i}{ElementType.Month.GetDescription()}{Environment.NewLine}{enMonth}"));
            }
            return dataSource;
        }

        /// <summary>
        /// 获取日期的数据源
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetDaySource()
        {
            int monthDayCnt = DateTime.DaysInMonth(this.SelectValue.Year, this.SelectValue.Month);
            DayOfWeek dayOfWeek = new DateTime(this.SelectValue.Year, this.SelectValue.Month, 1).DayOfWeek;
            int offSetDay = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;

            this.dtPnl.Column = 7;
            this.dtPnl.Row = (monthDayCnt + offSetDay - 1) / 7 + 2;

            List<KeyValuePair<string, string>> dataSource = new List<KeyValuePair<string, string>>();
            //设置标题区域
            string[] tileRow = new string[] { "一", "二", "三", "四", "五", "六", "日" };
            for (int i = 0; i < tileRow.Length; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>(DateTimePanel.INVALID_KEY.ToString(), tileRow[i]));
            }

            //设置空白区域
            for (int i = 0; i < offSetDay; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>(DateTimePanel.INVALID_KEY.ToString(), string.Empty));
            }
            for (int i = 1; i <= monthDayCnt; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }

            return dataSource;
        }

        /// <summary>
        /// 获取小时数据源
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetHourSource()
        {
            this.dtPnl.Row = 4;
            this.dtPnl.Column = 6;

            //设置数据源
            List<KeyValuePair<string, string>> dataSource = new List<KeyValuePair<string, string>>();
            for (int i = 0; i <= 23; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>(i.ToString(),
                    $"{i}{ElementType.Hour.GetDescription()}"));
            }
            return dataSource;
        }

        /// <summary>
        /// 获取分钟或秒的数据源
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> GetMinuteOrSecSource()
        {
            this.dtPnl.Row = 6;
            this.dtPnl.Column = 10;

            //设置数据源
            List<KeyValuePair<string, string>> dataSource = new List<KeyValuePair<string, string>>();
            for (int i = 0; i <= 59; i++)
            {
                dataSource.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }
            return dataSource;
        }

        /// <summary>
        /// 设置当前时间
        /// </summary>
        /// <param name="eleType"></param>
        /// <param name="value"></param>
        private void SetCurrentDateTime(ElementType eleType, int value)
        {
            if (eleType == ElementType.None || value < 0) return;
            switch (eleType)
            {
                case ElementType.Year:
                    int monthDayCnt = DateTime.DaysInMonth(value, this.SelectValue.Month);
                    this.SelectValue = new DateTime(value, this.SelectValue.Month,
                        this.SelectValue.Day > monthDayCnt ? monthDayCnt : this.SelectValue.Day,
                        this.SelectValue.Hour, this.SelectValue.Minute, this.SelectValue.Second);
                    break;
                case ElementType.Month:
                    monthDayCnt = DateTime.DaysInMonth(this.SelectValue.Year, value);
                    this.SelectValue = new DateTime(this.SelectValue.Year, value,
                        this.SelectValue.Day > monthDayCnt ? monthDayCnt : this.SelectValue.Day,
                        this.SelectValue.Hour, this.SelectValue.Minute, this.SelectValue.Second);
                    break;
                case ElementType.Day:
                    this.SelectValue = new DateTime(this.SelectValue.Year, this.SelectValue.Month, value,
                        this.SelectValue.Hour, this.SelectValue.Minute, this.SelectValue.Second);
                    break;
                case ElementType.Hour:
                    this.SelectValue = new DateTime(this.SelectValue.Year, this.SelectValue.Month, this.SelectValue.Day,
                        value, this.SelectValue.Minute, this.SelectValue.Second);
                    break;
                case ElementType.Minute:
                    this.SelectValue = new DateTime(this.SelectValue.Year, this.SelectValue.Month, this.SelectValue.Day,
                        this.SelectValue.Hour, value, this.SelectValue.Second);
                    break;
                case ElementType.Second:
                    this.SelectValue = new DateTime(this.SelectValue.Year, this.SelectValue.Month, this.SelectValue.Day,
                        this.SelectValue.Hour, this.SelectValue.Minute, value);
                    break;
                default:
                    break;
            }

            this.dtItemPnl.Value = this.SelectValue;
            DateTimeChanged?.Invoke(eleType, new DateTimeEventArgs(this.SelectValue));
        }

        /// <summary>
        /// 定位起始显示页面
        /// </summary>
        private void LocateDateTimePage()
        {
            if (this.dtItemPnl[ElementType.Day].Visible)
            {
                this.RefreshDtSelect(this.dtItemPnl[ElementType.Day]);
            }
            else if (this.dtItemPnl[ElementType.Year].Visible)
            {
                this.RefreshDtSelect(this.dtItemPnl[ElementType.Year]);
            }
            else if (this.dtItemPnl[ElementType.Hour].Visible)
            {
                this.RefreshDtSelect(this.dtItemPnl[ElementType.Hour]);
            }
            else
            {
                var item = this.dtItemPnl._dtItems.LastOrDefault(a => a.Visible);
                if (item != null)
                {
                    this.RefreshDtSelect(item);
                }
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 控件加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimeSelect_Load(object sender, EventArgs e)
        {
            this.btnOK.Visible = this.btnCancel.Visible = this.ShowExecBtn;
            this.dtItemPnl.SetDtDispItems(this.Format, this.CustomFormat);
            this.dtItemPnl.Value = this.SelectValue;
            this.LocateDateTimePage();
        }

        /// <summary>
        /// 日期时间按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimeItem_Click(object sender, EventArgs e)
        {
            if (!(sender is DateTimeCellElement dtItem)) return;
            this.RefreshDtSelect(dtItem);
        }

        /// <summary>
        /// 日期时间单元格元素点击处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtPnl_DtCellEleClick(object sender, EventArgs e)
        {
            if (!(sender is DateTimeCellElement cellEle)) return;

            this.SetCurrentDateTime(cellEle.EleType, cellEle.Name.ToInt(DateTimePanel.INVALID_KEY));

            //自动跳转到下一级
            if (!this.AutoSelectNext) return;
            ElementType nextEleType = (cellEle.EleType + 1).ToString().ToEnum(ElementType.None);
            if (nextEleType == ElementType.None) return;

            this.RefreshDtSelect(this.dtItemPnl[nextEleType]);
        }

        /// <summary>
        /// 日期时间单元格元素双击处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtPnl_DtCellEleDoubleClick(object sender, EventArgs e)
        {
            if (!this.ShowExecBtn) return;
            ExecOKBtnClick?.Invoke(this, e);
        }

        /// <summary>
        /// 确定或取消操作按钮处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteBtn_Click(object sender, EventArgs e)
        {
            if ((SparkButton)sender == this.btnOK)
            {
                //重新构造时间
                if (ReCtorDtWhenNoChange)
                {
                    SelectValue = new DateTime(SelectValue.Year, SelectValue.Month,
                        SelectValue.Day, SelectValue.Hour, SelectValue.Minute, SelectValue.Second);
                }
                ExecOKBtnClick?.Invoke(this, e);
            }
            else
            {
                ExecCancelBtnClick?.Invoke(this, e);
            }
        }

        /// <summary>
        /// 今天标签单击处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblToday_Click(object sender, EventArgs e)
        {
            this.SelectValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                DateTime.Now.Day, this.SelectValue.Hour, this.SelectValue.Minute, this.SelectValue.Second);

            this.dtItemPnl.Value = this.SelectValue;
            this.RefreshDtSelect(this.dtItemPnl[ElementType.Day]);

            //触发事件
            DateTimeChanged?.Invoke(ElementType.Day, new DateTimeEventArgs(this.SelectValue));
        }

        /// <summary>
        /// 今天标签鼠标离开处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblToday_MouseLeave(object sender, EventArgs e)
        {
            this.lblToday.ForeColor = this.Theme.ForeColor;
        }

        /// <summary>
        /// 今天标签鼠标进入处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblToday_MouseEnter(object sender, EventArgs e)
        {
            this.lblToday.ForeColor = this.Theme.DateTimeSelectColor;
        }

        /// <summary>
        /// 年份上或下一页处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblCommon_Click(object sender, EventArgs e)
        {
            if (!(sender is Label clickLbl)) return;

            if (this.dtPnl.DataSource == null || this.dtPnl.DataSource.Count == 0) return;

            DateTimeCellElement dtItemYear = this.dtItemPnl[ElementType.Year];
            if (dtItemYear == null || !dtItemYear.Visible) return;

            this.dtPnl.DataSource = this.GetYearSource(this.dtPnl.DataSource[0].Key.ToInt(), clickLbl == this.lblRight);
            this.dtPnl.SetSelectCell(dtItemYear.Name);
            this.dtPnl.Invalidate();
        }
        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkDateTimePickerTheme Theme { get; private set; }

        #endregion
    }
}