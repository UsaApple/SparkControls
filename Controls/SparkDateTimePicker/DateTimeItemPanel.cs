using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 显示当前日期时间的面板
    /// </summary>
    [ToolboxItem(false)]
    internal class DateTimeItemPanel : Panel
    {
        #region 常量
        //时间项目位置间隔
        private const int ITEM_SPACING = 5;
        #endregion

        #region 变量
        internal readonly List<DateTimeCellElement> _dtItems = new List<DateTimeCellElement>();
        private DateTimeCellElement _selectItem = null;
        #endregion

        #region 事件
        /// <summary>
        /// 日期时间项目点击事件
        /// </summary>
        public event EventHandler DtItemClick;
        #endregion

        #region 属性
        private DateTime _value;
        /// <summary>
        /// 显示的时间
        /// </summary>
        public DateTime Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.RefreshDtItems();
                this.Refresh();
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="eleType"></param>
        /// <returns></returns>
        public DateTimeCellElement this[ElementType eleType] => this._dtItems?.FirstOrDefault(p => p.EleType == eleType);

        /// <summary>
        /// 主题
        /// </summary>
        [Browsable(false)]
        public SparkDateTimePickerTheme Theme { get; set; }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public DateTimeItemPanel() : base()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |      //调整大小时重绘
               ControlStyles.DoubleBuffer |                 //双缓冲
               ControlStyles.OptimizedDoubleBuffer |        //双缓冲
               ControlStyles.AllPaintingInWmPaint |         //禁止擦除背景
               ControlStyles.SupportsTransparentBackColor | //透明
               ControlStyles.Selectable |
               ControlStyles.UserPaint, true
            );
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 初始化日期时间项目
        /// </summary>
        public void InitDateTimeItems()
        {
            this._dtItems.Clear();
            Dictionary<ElementType, Size> dicInfo = new Dictionary<ElementType, Size>()
            {
                {ElementType.Year, new Size(55, 30)}, {ElementType.Month, new Size(50, 30)},
                {ElementType.Day, new Size(50, 30)}, {ElementType.Hour, new Size(50, 30)},
                {ElementType.Minute, new Size(50, 30)}, {ElementType.Second, new Size(50, 30)}
            };

            int startX = 0;
            Font itemTxtFont = new Font("微软雅黑", 9);
            foreach (ElementType key in dicInfo.Keys)
            {
                DateTimeCellElement item = new DateTimeCellElement(key)
                {
                    Name = this.GetDtItemVal(key).ToString(),
                    TextFont = itemTxtFont,
                    Theme = this.Theme,
                    Size = dicInfo[key],
                    Location = new Point(startX, 0),
                    DrawBorder = true
                };
                item.Text = $"{item.Name.PadLeft(2, '0')}{key.GetDescription()}";
                startX += item.Size.Width + ITEM_SPACING;
                this._dtItems.Add(item);
            }
        }

        /// <summary>
        /// 设置日期时间项目
        /// </summary>
        /// <param name="format"></param>
        /// <param name="customFormat"></param>
        public void SetDtDispItems(DateTimePickerFormat format, string customFormat)
        {
            this._dtItems.ForEach(p => p.Visible = true);

            //根据时间日期格式来匹配
            switch (format)
            {
                case DateTimePickerFormat.Long:
                case DateTimePickerFormat.Short:
                    this.SetTimePartInvisible();
                    break;
                case DateTimePickerFormat.Time:
                    this.SetDatePartInvisible();
                    break;
                default:
                    try
                    {
                        DateTime stdDt = new DateTime(0001, 11, 11, 11, 11, 11);
                        if (DateTime.TryParseExact(stdDt.ToString(customFormat),
                            new string[] { customFormat }, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out DateTime dt))
                        {
                            if (dt.Year != 1) this[ElementType.Year].Visible = false;
                            if (dt.Month != 11) this[ElementType.Month].Visible = false;
                            if (dt.Day != 11) this[ElementType.Day].Visible = false;

                            if (dt.Hour == 0) this[ElementType.Hour].Visible = false;
                            if (dt.Minute == 0) this[ElementType.Minute].Visible = false;
                            if (dt.Second == 0) this[ElementType.Second].Visible = false;
                            ReSetInvisible();
                            //年月日的组合：
                            //yyyy,yyyy-MM,yyyy-MM-dd
                            //MM,MM-dd
                            //不能单独dd
                        }
                    }
                    catch (Exception) { }
                    break;
            }
        }

        /// <summary>
        /// 设置选中项目
        /// </summary>
        /// <param name="dispItem"></param>
        public void Select(DateTimeCellElement dispItem)
        {
            if (dispItem == null) return;
            if (this._selectItem != null)
            {
                this._selectItem.Select = false;
                this.Invalidate(this._selectItem.Bounds);
            }

            dispItem.Select = true;
            this.Invalidate(dispItem.Bounds);
            this._selectItem = dispItem;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 绘制方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this._dtItems == null || this._dtItems.Count == 0) return;
            foreach (DateTimeCellElement item in this._dtItems)
            {
                if (!item.Visible) continue;
                item.Paint(e.Graphics);
            }
        }

        /// <summary>
        /// 鼠标抬起事件处理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this._selectItem != null)
            {
                this._selectItem.Select = false;
                this.Invalidate(this._selectItem.Bounds);
            }

            this._selectItem = this._dtItems.FirstOrDefault(p => p.Bounds.Contains(e.Location) && p.Visible);
            if (this._selectItem == null) return;

            this._selectItem.Select = true;
            this.Invalidate(this._selectItem.Bounds);

            //触发点击事件
            DtItemClick?.Invoke(this._selectItem, e);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 刷新日期时间项目集合
        /// </summary>
        private void RefreshDtItems()
        {
            if (this._dtItems == null || this._dtItems.Count == 0) return;
            this._dtItems.ForEach(p =>
            {
                p.Name = this.Name = this.GetDtItemVal(p.EleType).ToString();
                p.Text = $"{p.Name.PadLeft(2, '0')}{p.EleType.GetDescription()}";
            });
        }

        /// <summary>
        /// 根据类型获取日期时间的各个部分的值
        /// </summary>
        /// <param name="eleType"></param>
        /// <returns></returns>
        private int GetDtItemVal(ElementType eleType)
        {
            switch (eleType)
            {
                case ElementType.Year:
                    return this.Value.Year;
                case ElementType.Month:
                    return this.Value.Month;
                case ElementType.Day:
                    return this.Value.Day;
                case ElementType.Hour:
                    return this.Value.Hour;
                case ElementType.Minute:
                    return this.Value.Minute;
                case ElementType.Second:
                    return this.Value.Second;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 设置时间部分不可见
        /// </summary>
        private void SetTimePartInvisible()
        {
            if (this._dtItems == null || this._dtItems.Count == 0) return;
            this._dtItems.Where(p => p.EleType == ElementType.Hour || p.EleType == ElementType.Minute ||
            p.EleType == ElementType.Second).ToList().ForEach(t => t.Visible = false);
        }

        /// <summary>
        /// 设置日期部分不可见
        /// </summary>
        private void SetDatePartInvisible()
        {
            if (this._dtItems == null || this._dtItems.Count == 0) return;

            this._dtItems.Where(p => p.EleType == ElementType.Year || p.EleType == ElementType.Month ||
            p.EleType == ElementType.Day).ToList().ForEach(t => t.Visible = false);

            //移动位置
            DateTimeCellElement eleHour = this[ElementType.Hour];
            DateTimeCellElement eleMinute = this[ElementType.Minute];
            DateTimeCellElement eleSecond = this[ElementType.Second];

            eleMinute.Location = new Point(eleMinute.Location.X - eleHour.Location.X, 0);
            eleSecond.Location = new Point(eleSecond.Location.X - eleHour.Location.X, 0);
            eleHour.Location = new Point(0, 0);
        }

        /// <summary>
        /// 重新设置位置
        /// </summary>
        private void ReSetInvisible()
        {
            int startX = 0;
            foreach (var item in this._dtItems.Where(a => a.Visible == true))
            {
                item.Location = new Point(startX, 0);
                startX += item.Size.Width + ITEM_SPACING;
            }
        }
        #endregion
    }
}