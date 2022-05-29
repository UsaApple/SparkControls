using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SparkControls.Foundation;

namespace SparkControls.Controls
{
    /// <summary>
    /// <para>分页控件</para>
    /// <para>使用方法：方法一：注册PagerChanged事件，调用SetRowCount(总行数).SetPageIndex(页索引)；</para>
    /// <para>方法二：注册PagerChanged和QueryTotalRowCount事件，调用QueryRowCount().SetPageIndex(页索引)；</para>
    /// </summary>
    [ToolboxItem(true)]
    public partial class SparkPager : UserControl
    {
        /// <summary>
        /// {0}=RowCount,{1}=PageSize,{2}=PageCount
        /// </summary>
        private const string memo = "共 {0} 条记录，每页 {1} 条，共 {2} 页";
        private int currentPageIndex = 0;

        #region 事件定义
        /// <summary>
        /// 页码或者页大小改变事件
        /// </summary>
        public event SparkPagerChangedEventHandler PagerChanged;

        /// <summary>
        /// 查询总行数的事件
        /// </summary>
        public event SparkPagerQueryTotalRowCountEventHandler PagerQueryTotalRowCount;
        #endregion

        #region 属性
        private Font font = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
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
        /// 总共多少页
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex => currentPageIndex;

        /// <summary>
        /// 总共多少行
        /// </summary>
        public long RowCount { get; private set; }

        /// <summary>
        /// 页大小,每页多少行
        /// </summary>
        public int PageSize { get; private set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public SparkPager()
        {
            InitializeComponent();
            this.Font = this.font;
            Init();
        }

        private void Init()
        {
            this.cmbPageSize.SelectedIndex = 1;
            PageSize = this.cmbPageSize.Text.ToInt(500);
            txtNum.Value = 0;
            SetButtonEnabled();
            this.btnDown.Click += BtnDown_Click;
            this.btnFirst.Click += BtnFirst_Click;
            this.btnLast.Click += BtnLast_Click;
            this.btnUp.Click += BtnUp_Click;
            this.cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;

            this.txtNum.TextChanged += TxtNum_TextChanged;


            this.btnLast.Image = global::SparkControls.Properties.Resources.尾页;
            this.btnDown.Image = global::SparkControls.Properties.Resources.下一页;
            this.btnUp.Image = global::SparkControls.Properties.Resources.上一页;
            this.btnFirst.Image = global::SparkControls.Properties.Resources.第一页;

        }

        #region 事件
        private void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSizeBefore = this.PageSize;//之前每页多少行
            int indexPageBefore = this.CurrentPageIndex;

            //每页多少行改变,重写计算有多少页
            this.PageSize = cmbPageSize.Text.ToInt(500);
            CalaPageCount();
            if (this.RowCount <= 0) return;

            //重写计算当前页
            //根据当前的页数和行数,计算当前第一行的数据在现在的第几页上
            var rowIndex = (indexPageBefore - 1) * pageSizeBefore + 1;//在第几行

            var currentIndex2 = (int)(Math.Floor(1.0f * rowIndex / this.PageSize) + 1);//之前的第几行,在现在的第几页.
            if (this.txtNum.Value == currentIndex2)
            {
                currentPageIndex = currentIndex2;
                OnPagerChanged();
            }
            else
            {
                currentPageIndex = -1;
                this.txtNum.Value = currentIndex2;
            }
        }

        private void TxtNum_TextChanged(object sender, EventArgs e)
        {
            if (currentPageIndex != (int)this.txtNum.Value)
            {
                currentPageIndex = (int)this.txtNum.Value;
                OnPagerChanged();
            }
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex > 1 && this.PageCount > 0)
            {
                this.txtNum.Value = this.CurrentPageIndex - 1;
            }
        }

        private void BtnLast_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex != this.PageCount && this.PageCount > 0)
            {
                this.txtNum.Value = this.PageCount;
            }
        }

        private void BtnFirst_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex != 1 && this.PageCount > 0)
            {
                this.txtNum.Value = 1;
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex != this.PageCount && this.PageCount > 0)
            {
                this.txtNum.Value = this.CurrentPageIndex + 1;
            }
        }
        #endregion

        #region protected
        protected virtual void OnPagerChanged(SparkPagerChangedEventArgs e)
        {
            PagerChanged?.Invoke(this, e);
        }
        #endregion

        #region public
        /// <summary>
        /// 设置总共多少行
        /// </summary>
        /// <param name="rowCount">总共多少行</param>
        public SparkPager SetRowCount(long rowCount)
        {
            this.RowCount = rowCount;
            CalaPageCount();
            return this;
        }

        /// <summary>
        /// 跳转到指定页，从1开始
        /// </summary>
        /// <param name="index">页码索引，从1开始</param>
        public SparkPager SetPageIndex(int index)
        {
            if (index >= 1 && index <= this.PageCount)
            {
                if (this.txtNum.Value == index)
                {
                    currentPageIndex = index;
                    OnPagerChanged();
                }
                else
                {
                    this.currentPageIndex = -1;
                    this.txtNum.Value = index;
                }
            }
            else
            {
                this.currentPageIndex = -1;
                //this.label1.Text = string.Format(memo, RowCount, PageSize, PageCount);
                OnPagerChanged();
            }
            return this;
        }

        /// <summary>
        /// 查询总行数
        /// </summary>
        /// <returns></returns>
        public SparkPager QueryRowCount()
        {
            var ret = PagerQueryTotalRowCount?.Invoke(this, new EventArgs());
            if (ret != null && ret.Value > 0)
            {
                SetRowCount(ret.Value);
            }
            else
            {
                SetRowCount(0);
            }
            return this;
        }
        #endregion

        #region private
        private void SetButtonEnabled()
        {
            if ((this.CurrentPageIndex == 1 && this.PageCount == 1) || this.PageCount <= 0)
            {
                this.btnFirst.Enabled = false;
                this.btnUp.Enabled = false;
                this.btnDown.Enabled = false;
                this.btnLast.Enabled = false;
                this.txtNum.Enabled = false;
            }
            else if (this.CurrentPageIndex == 1)
            {
                this.btnFirst.Enabled = false;
                this.btnUp.Enabled = false;
                this.btnDown.Enabled = true;
                this.btnLast.Enabled = true;
                this.txtNum.Enabled = true;
            }
            else if (this.CurrentPageIndex == this.PageCount)
            {
                this.btnFirst.Enabled = true;
                this.btnUp.Enabled = true;
                this.btnDown.Enabled = false;
                this.btnLast.Enabled = false;
                this.txtNum.Enabled = true;
            }
            else
            {
                this.btnFirst.Enabled = true;
                this.btnUp.Enabled = true;
                this.btnDown.Enabled = true;
                this.btnLast.Enabled = true;
                this.txtNum.Enabled = true;
            }
        }

        /// <summary>
        /// 计算多少页
        /// </summary>
        private void CalaPageCount()
        {
            this.PageCount = (int)Math.Ceiling(1.0f * this.RowCount / this.PageSize);

            if (this.PageCount > 0)
            {
                this.txtNum.MaxValue = this.PageCount;
                this.txtNum.MinValue = 1;
            }
            else
            {
                this.txtNum.MaxValue = 0;
                this.txtNum.MinValue = 0;
                if (this.RowCount > 0) this.txtNum.Value = 0;
                this.currentPageIndex = 1;
            }
            SetButtonEnabled();
        }


        private void OnPagerChanged()
        {
            this.label1.Text = string.Format(memo, RowCount, PageSize, PageCount);
            SetButtonEnabled();
            var args = new SparkPagerChangedEventArgs(PageCount, CurrentPageIndex, RowCount, PageSize);
            OnPagerChanged(args);
        }
        #endregion
    }

    /// <summary>
    /// 分页控件改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SparkPagerChangedEventHandler(object sender, SparkPagerChangedEventArgs e);

    /// <summary>
    /// 分页控件改变事件的参数
    /// </summary>
    public class SparkPagerChangedEventArgs
    {
        /// <summary>
        /// 总共多少页
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex { get; private set; }

        /// <summary>
        /// 总共多少行
        /// </summary>
        public long RowCount { get; private set; }

        /// <summary>
        /// 页大小,每页多少行
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageCount">多少页</param>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="rowCount">多少行</param>
        /// <param name="pageSize">每页多少行</param>
        public SparkPagerChangedEventArgs(int pageCount, int currentPageIndex, long rowCount, int pageSize)
        {
            PageCount = pageCount;
            CurrentPageIndex = currentPageIndex;
            RowCount = rowCount;
            PageSize = pageSize;
        }

        public override string ToString()
        {
            return $"PageIndex={CurrentPageIndex} PageCount={PageCount} RowCount={RowCount} PageSize={PageSize}";
        }
    }

    /// <summary>
    /// 查询总行数事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate long SparkPagerQueryTotalRowCountEventHandler(object sender, EventArgs e);

}
